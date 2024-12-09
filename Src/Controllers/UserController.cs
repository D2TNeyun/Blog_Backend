using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Src.Data;
using Src.Dtos.User;
using Src.Models;
using Src.Services;

namespace Src.Controllers
{
    [Route("api/user")]
    [ApiController]
    //  [Authorize(Roles = "Admin")]

    public class UserController(UserService userService, UserManager<AppUser> userManager, ApplicationDBContext context) : ControllerBase
    {
        private readonly UserService _userService = userService;
        private readonly UserManager<AppUser> _userManager = userManager;
        private readonly ApplicationDBContext _context = context;


        [HttpGet]
        public async Task<ActionResult<List<AppUser>>> GetUsers([FromQuery] UserQuery userQuery)
        {
            var users = await _userService.GetAllUsersAsync(userQuery);
            return Ok(new { users });
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<AppUser>> GetUserById(string id)
        {
            var user = await _userService.GetUserByIdAsync(id);
            if (user == null)
            {
                return NotFound("User not found");
            }
            return Ok(new { user });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(string id, [FromForm] UpdateUserDto update, IFormFile? Image)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(id);
                if (user == null)
                {
                    return NotFound();
                }

                // Kiểm tra username đã tồn tại hay chưa
                if (!string.IsNullOrEmpty(update.UserName) && update.UserName != user.UserName)
                {
                    var existingUserByName = await _userManager.FindByNameAsync(update.UserName);
                    if (existingUserByName != null)
                    {
                        return StatusCode(400, "Username is already taken.");
                    }
                }

                // Kiểm tra email đã tồn tại hay chưa
                if (!string.IsNullOrEmpty(update.Email) && update.Email != user.Email)
                {
                    var existingUserEmail = await _userManager.FindByEmailAsync(update.Email);
                    if (existingUserEmail != null)
                    {
                        return StatusCode(400, "Email is already taken.");
                    }
                }
                // tìm kiếm bản ghi liên quan trong bảng Actives trước
                if (_context.Actives == null)
                {
                    throw new InvalidOperationException("comments statuses data source is unavailable.");
                }
                var active = await _context.Actives.FirstOrDefaultAsync(a => a.AppUserID == id);
                if (active != null)
                {
                    _context.Actives.Update(active);
                    await _context.SaveChangesAsync(); // Lưu thay đổi
                }

                var (Success, message) = await _userService.UpdateUserAsync(id, update, Image);
                if (!Success)
                {
                    return BadRequest(new { message });
                }
                return Ok(new { message = "User updated successfully" });
            }
            catch (InvalidOperationException ex)
            {
                // Xử lý ngoại lệ cụ thể
                ModelState.AddModelError(string.Empty, ex.Message);
                return BadRequest(ModelState);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(string id)
        {
             if (_context.Actives == null || _context.Posts == null)
                {
                    throw new InvalidOperationException("comments statuses data source is unavailable.");
                }
            // Tìm người dùng theo ID
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound(new { message = "User not found" });
            }

            // Xóa bản ghi liên quan trong bảng Actives
            var active = await _context.Actives.FirstOrDefaultAsync(a => a.AppUserID == id);
            if (active != null)
            {
                _context.Actives.Remove(active);
                await _context.SaveChangesAsync();
            }

            // Tìm tài khoản "DeletedUser" (hoặc tạo nếu chưa tồn tại)
            var deletedUser = await _userManager.FindByNameAsync("DeletedUser");
            if (deletedUser == null)
            {
                deletedUser = new AppUser
                {
                    UserName = "DeletedUser",
                    Email = "deleteduser@example.com",
                    EmailConfirmed = true
                };

                var createResult = await _userManager.CreateAsync(deletedUser, "DefaultPassword@123");
                if (!createResult.Succeeded)
                {
                    return BadRequest(new { message = "Failed to create 'DeletedUser' account" });
                }
            }

            // Cập nhật các bài viết của người dùng bị xóa
            var userPosts = _context.Posts.Where(p => p.AppUserID == id);
            if (userPosts.Any())
            {
                foreach (var post in userPosts)
                {
                    post.AppUserID = deletedUser.Id; // Gán bài viết cho tài khoản DeletedUser
                }
                await _context.SaveChangesAsync(); // Lưu thay đổi trong cơ sở dữ liệu
            }

            // Xóa người dùng
            var result = await _userManager.DeleteAsync(user);
            if (result.Succeeded)
            {
                return Ok(new { message = "User deleted successfully, posts reassigned to 'DeletedUser'" });
            }

            return BadRequest(new { message = "Failed to delete user" });
        }


        [HttpPost("addUser")]
        public async Task<IActionResult> AddUser([FromForm] AddUser addUserDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _userService.AddUserAsync(addUserDto);

            if (result.Succeeded)
            {
                return Ok(new { Message = "User created successfully!" });
            }
            else
            {
                return BadRequest(result.Errors);
            }
        }


    }
}