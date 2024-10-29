using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Src.Data;
using Src.Dtos.Auth;
using Src.Dtos.User;
using Src.Models;

namespace Src.Services
{
    public class AuthService(UserManager<AppUser> userManager, IConfiguration configuration, ITokenService tokenService, ApplicationDBContext Context)
    {
        private readonly UserManager<AppUser> _userManager = userManager;
        private readonly IConfiguration _configuration = configuration;
        private readonly ITokenService _tokenService = tokenService;
        private readonly ApplicationDBContext _context = Context;

        // // REGISTERASYNC
        public async Task<RegistrationResultDto> RegisterAsync(RegisterDto registerDto)
        {
            // Kiểm tra sự tồn tại của tên người dùng và email
            var existingUser = await _userManager.FindByNameAsync(registerDto.Username)
                              ?? await _userManager.FindByEmailAsync(registerDto.Email);
            if (existingUser != null)
            {
                return new RegistrationResultDto { Success = false, Message = "Username or Email already exists." };
            }

            // Tạo người dùng mới
            var user = new AppUser
            {
                UserName = registerDto.Username,
                Email = registerDto.Email
            };
            var result = await _userManager.CreateAsync(user, registerDto.Password);
            if (!result.Succeeded)
            {
                return new RegistrationResultDto
                {
                    Success = false,
                    Message = $"Error creating user: {string.Join(", ", result.Errors.Select(e => e.Description))}"
                };
            }

            // Gán vai trò cho người dùng
            var roleResult = await _userManager.AddToRoleAsync(user, "User");
            if (!roleResult.Succeeded)
            {
                return new RegistrationResultDto
                {
                    Success = false,
                    Message = $"Failed to assign role to user: {string.Join(", ", roleResult.Errors.Select(e => e.Description))}"
                };
            }
            // Tạo trạng thái hoạt động mặc định cho người dùng mới
            var active = new Actives
            {
                AppUserID = user.Id,
                StatusName = "Y",  // Trạng thái mặc định là "Y"
            };

            _context.Actives.Add(active);
            await _context.SaveChangesAsync();
            // Tạo token xác nhận email
            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            return new RegistrationResultDto
            {
                Success = true,
                Message = "User registered successfully",
                NewUser = new NewUserDto
                {
                    UserName = user.UserName,
                    Email = user.Email,
                    Token = token
                }
            };
        }


        // LOGINASYNC
        public async Task<(bool Success, string Message, string? Token, UserDto User)> LoginAsync(LoginDto loginDto)
        {
            var user = await _userManager.FindByNameAsync(loginDto.Username);
            if (user == null || !await _userManager.CheckPasswordAsync(user, loginDto.Password))
            {
                return (false, "Invalid username or password.", null, new UserDto());
            }
            if (user != null && await _userManager.CheckPasswordAsync(user, loginDto.Password))
            {
                var token = _tokenService.GenerateToken(user, (await _userManager.GetRolesAsync(user)).FirstOrDefault() ?? string.Empty);
                // Lấy thông tin trạng thái active của người dùng
                var activeStatus = await _context.Actives
                    .Where(a => a.AppUserID == user.Id && a.StatusName != null)  
                    .Select(a => a.StatusName!)
                    .ToListAsync();

                var UserInfor = new UserDto
                {
                    Id = user.Id,
                    Username = user.UserName ?? string.Empty,
                    Email = user.Email ?? string.Empty,
                    Roles = await _userManager.GetRolesAsync(user),
                    IsActives = activeStatus
                };

                return (true, "Login successful.", token, UserInfor);
            }

            return (false, "Failed to generate token.", null, new UserDto());
        }

    }
}
