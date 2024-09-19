using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Src.Dtos.User;
using Src.Models;
using Src.Services;

namespace Src.Controllers
{
    [Route("api/user")]
    [ApiController]
    // [Authorize]

    public class UserController(UserService userService, UserManager<AppUser> userManager) : ControllerBase
    {
        private readonly UserService _userService = userService;
        private readonly UserManager<AppUser> _userManager = userManager;

        [HttpGet]
        public async Task<ActionResult<List<AppUser>>> GetUsers()
        {
            var users = await _userService.GetAllUsersAsync();
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
        public async Task<IActionResult> UpdateUser(string id, [FromForm] UpdateUserDto update)
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


                var (Success, message) = await _userService.UpdateUserAsync(id, update);
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

        // [HttpDelete("{id}")]
        // public async Task<IActionResult> DeleteUser(string id)
        // {
        //     var user = await _userManager.FindByIdAsync(id);
        //     if (user == null)
        //     {
        //         return NotFound("User not found");
        //     }

        //     await _userManager.DeleteAsync(user);
        //     await _userService.DeleteUserAsync(id);
        //     return Ok(new { message = "User deleted successfully" });
        // }

    }
}