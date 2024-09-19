using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Src.Dtos.User;
using Src.Models;


namespace Src.Services
{
    public class UserService(UserManager<AppUser> userManager) 
    {
        private readonly UserManager<AppUser> _userManager = userManager;

        public async Task<IEnumerable<UserDto>> GetAllUsersAsync()
        {
            var users = await _userManager.Users.ToListAsync();
            var userWithRolesList = new List<UserDto>();
            foreach (var user in users)
            {
                var userRoles = await _userManager.GetRolesAsync(user);
                userWithRolesList.Add(new UserDto
                {
                    Id = user.Id,
                    Username = user.UserName ?? string.Empty,
                    Email = user.Email ?? string.Empty,
                    Roles = userRoles
                });
            }
            return userWithRolesList;
        }

        public async Task<UserDto?> GetUserByIdAsync(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return null;
            }

            var roles = await _userManager.GetRolesAsync(user);
            return new UserDto
            {
                Id = user.Id,
                Username = user.UserName ?? string.Empty,
                Email = user.Email ?? string.Empty,
                Roles = roles
            };
        }

       

        public async Task<(bool Success, string Message)> UpdateUserAsync(string userId, UpdateUserDto updateUser)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return (false, "User not found.");
            }

            var existingUsers = await _userManager.Users
                .Where(u => u.Id != userId && (u.UserName == updateUser.UserName || u.Email == updateUser.Email))
                .ToArrayAsync();
            
            foreach (var existingUser in existingUsers)
            {
                if(existingUser.UserName == updateUser.UserName)
                {
                    return (false, "Username already exists.");
                }
                if(existingUser.Email == updateUser.Email)
                {
                    return (false, "Email already exists.");
                }
            }
            user.UserName = updateUser.UserName ?? user.UserName;
            user.Email = updateUser.Email?? user.Email;

            var result = await _userManager.UpdateAsync(user);
            return result.Succeeded? (true, "User updated successfully.") : (false, "Failed to update user.");
        }


    }
}