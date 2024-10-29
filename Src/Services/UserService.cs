using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Src.Data;
using Src.Dtos.User;
using Src.Models;


namespace Src.Services
{
    public class UserService(UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager, ApplicationDBContext Context)
    {
        private readonly UserManager<AppUser> _userManager = userManager;
        private readonly RoleManager<IdentityRole> _roleManager = roleManager;
        private readonly ApplicationDBContext _context = Context;


        public async Task<IEnumerable<UserDto>> GetAllUsersAsync()
        {
            var users = await _userManager.Users.ToListAsync();
            var userWithRolesList = new List<UserDto>();
            // var IsActives = new List<
            foreach (var user in users)
            {
                var userRoles = await _userManager.GetRolesAsync(user);

                var activeStatus = await _context.Actives
                    .Where(a => a.AppUserID == user.Id && a.StatusName != null)  // Thêm điều kiện để loại bỏ giá trị null
                    .Select(a => a.StatusName!)
                    .ToListAsync();

                userWithRolesList.Add(new UserDto
                {
                    Id = user.Id,
                    Username = user.UserName ?? string.Empty,
                    Email = user.Email ?? string.Empty,
                    Roles = userRoles ?? throw new ArgumentNullException(nameof(userWithRolesList)),
                    IsActives = activeStatus
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

            // Check if username or email already exists
            var existingUsers = await _userManager.Users
                .Where(u => u.Id != userId && (u.UserName == updateUser.UserName || u.Email == updateUser.Email))
                .ToArrayAsync();

            foreach (var existingUser in existingUsers)
            {
                if (existingUser.UserName == updateUser.UserName)
                {
                    return (false, "Username already exists.");
                }
                if (existingUser.Email == updateUser.Email)
                {
                    return (false, "Email already exists.");
                }
            }

            // Update username and email if provided
            user.UserName = updateUser.UserName ?? user.UserName;
            user.Email = updateUser.Email ?? user.Email;

            // Update role if provided
            if (!string.IsNullOrEmpty(updateUser.RoleID))
            {
                var newRole = await _roleManager.FindByIdAsync(updateUser.RoleID);
                if (newRole != null) // Ensure newRole is found
                {
                    if (string.IsNullOrEmpty(newRole.Name)) // Check if role name is null or empty
                    {
                        return (false, "Role name is missing.");
                    }

                    var currentRoles = await _userManager.GetRolesAsync(user);
                    await _userManager.RemoveFromRolesAsync(user, currentRoles); // Remove all current roles
                    await _userManager.AddToRoleAsync(user, newRole.Name);        // Add the new role by name
                }
                else
                {
                    return (false, "Role not found.");
                }
            }
            // Update active status if provided
            if (!string.IsNullOrEmpty(updateUser.StatusName))
            {
                // Tìm trạng thái hoạt động hiện tại
                var activeEntry = await _context.Actives
                    .FirstOrDefaultAsync(a => a.AppUserID == user.Id);

                if (activeEntry != null)
                {
                    activeEntry.StatusName = updateUser.StatusName;
                }
                else
                {
                    // Nếu không có trạng thái hoạt động, có thể tạo mới
                    activeEntry = new Actives
                    {
                        AppUserID = user.Id,
                        StatusName = updateUser.StatusName,
                        // Avata = "default_avatar.png" // Giá trị mặc định cho Avatar (nếu cần)
                    };
                    await _context.Actives.AddAsync(activeEntry);
                }
            }

            // Attempt to update the user
            var result = await _userManager.UpdateAsync(user);
            return result.Succeeded ? (true, "User updated successfully.") : (false, "Failed to update user.");
        }

        public async Task<(bool Success, string Message)> DeleteUserAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return (false, "User not found.");
            }
            // // Check if the user has any posts
            // var posts = await _userManager.GetUsersWithPostsAsync().Where(u => u.Id == userId).ToListAsync();
            // if (posts.Count > 0)
            // {
            //     return (false, "User has posts. Cannot delete user.");
            // }
            // Attempt to delete the user
            var result = await _userManager.DeleteAsync(user);
            return result.Succeeded ? (true, "User deleted successfully.") : (false, "Failed to delete user.");
        }

    }
}