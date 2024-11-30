using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Src.Data;
using Src.Dtos.User;
using Src.Models;


namespace Src.Services
{
    public class UserService(UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager, ApplicationDBContext Context, Cloudinary cloudinary)
    {
        private readonly UserManager<AppUser> _userManager = userManager;
        private readonly RoleManager<IdentityRole> _roleManager = roleManager;
        private readonly ApplicationDBContext _context = Context;
        private readonly Cloudinary _cloudinary = cloudinary;



        public async Task<IEnumerable<UserDto>> GetAllUsersAsync(UserQuery userQuery)
        {
            // Fetch users from database
            var users = await _userManager.Users.ToListAsync();

            // Filter users by UserName if provided
            if (!string.IsNullOrWhiteSpace(userQuery.UserName))
            {
                users = users
                    .Where(u => (u.UserName ?? string.Empty)
                    .ToLower()
                    .Contains(userQuery.UserName.ToLower()))
                    .ToList();
            }

            var userWithRolesList = new List<UserDto>();

            foreach (var user in users)
            {
                // Retrieve roles for each user
                var userRoles = await _userManager.GetRolesAsync(user);

                if (_context.Actives == null)
                {
                    throw new InvalidOperationException("Active statuses data source is unavailable.");
                }

                // Retrieve active status for the user
                var activeStatus = await _context.Actives
                    .Where(a => a.AppUserID == user.Id && a.StatusName != null)
                    .Select(a => a.StatusName!)
                    .ToListAsync();

                // Add user data to result list
                userWithRolesList.Add(new UserDto
                {
                    Id = user.Id,
                    Username = user.UserName ?? string.Empty,
                    Email = user.Email ?? string.Empty,
                    Roles = userRoles,
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
            if (_context.Actives == null)
            {
                throw new InvalidOperationException("Active statuses data source is unavailable.");
            }

            // Retrieve active status and avatar from the 'Actives' table
            var activeStatus = await _context.Actives
                .Where(a => a.AppUserID == user.Id)
                .Select(a => new { a.StatusName, a.Avata })
                .FirstOrDefaultAsync();

            if (activeStatus == null)
            {
                // Log thông tin để kiểm tra tại sao không tìm thấy Active
                throw new Exception($"Active status for user with ID {id} not found in database.");
            }

            return new UserDto
            {
                Id = user.Id,
                Username = user.UserName ?? string.Empty,
                Email = user.Email ?? string.Empty,
                Roles = roles,
                IsActives = new List<string> { activeStatus.StatusName ?? "Unknown" }, // Default status
                Avata = activeStatus.Avata // Avatar URL
            };
        }




        public async Task<(bool Success, string Message)> UpdateUserAsync(string userId, UpdateUserDto updateUser, IFormFile? avatarImage)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return (false, "User not found.");
            }

            // Check if username or email already exists only if they are being changed
            if (!string.IsNullOrEmpty(updateUser.UserName) && updateUser.UserName != user.UserName)
            {
                var existingUser = await _userManager.Users
                    .FirstOrDefaultAsync(u => u.UserName == updateUser.UserName);

                if (existingUser != null)
                {
                    return (false, "Username already exists.");
                }
                else
                {
                    user.UserName = updateUser.UserName;
                }
            }

            if (!string.IsNullOrEmpty(updateUser.Email) && updateUser.Email != user.Email)
            {
                var existingUser = await _userManager.Users
                    .FirstOrDefaultAsync(u => u.Email == updateUser.Email);

                if (existingUser != null)
                {
                    return (false, "Email already exists.");
                }
                else
                {
                    user.Email = updateUser.Email;
                }
            }



            // Update role if provided
            if (!string.IsNullOrEmpty(updateUser.Role))
            {
                var newRole = await _roleManager.FindByNameAsync(updateUser.Role);
                if (newRole != null) // Ensure newRole is found
                {
                    var currentRoles = await _userManager.GetRolesAsync(user);
                    if (!currentRoles.Contains(newRole.Name ?? string.Empty)) // Check if role is different
                    {
                        await _userManager.RemoveFromRolesAsync(user, currentRoles);
                        await _userManager.AddToRoleAsync(user, newRole.Name ?? string.Empty);
                    }
                }
                else
                {
                    return (false, "Role not found.");
                }
            }

            // Update active status if provided
            if (!string.IsNullOrEmpty(updateUser.StatusName))
                if (!string.IsNullOrEmpty(updateUser.StatusName))
                {
                    if (_context.Actives == null)
                    {
                        throw new InvalidOperationException("Active statuses data source is unavailable.");
                    }
                    // Check if an active entry exists
                    var activeEntry = await _context.Actives
                        .FirstOrDefaultAsync(a => a.AppUserID == user.Id);

                    if (activeEntry != null)
                    {
                        activeEntry.StatusName = updateUser.StatusName;
                    }
                    else
                    {
                        // Create a new active status if none exists
                        activeEntry = new Actives
                        {
                            AppUserID = user.Id,
                            StatusName = updateUser.StatusName,
                        };
                        await _context.Actives.AddAsync(activeEntry);
                    }
                    _context.Actives.Update(activeEntry);
                }

            // Upload and update avatar if an image is provided
            if (avatarImage != null)
            {
                if (_context.Actives == null)
                {
                    throw new InvalidOperationException("Active statuses data source is unavailable.");
                }
                var activeEntry = await _context.Actives.FirstOrDefaultAsync(a => a.AppUserID == user.Id);

                // Delete old avatar from Cloudinary if it exists
                if (activeEntry != null && !string.IsNullOrEmpty(activeEntry.Avata))
                {
                    await _cloudinary.DeleteResourcesAsync(activeEntry.Avata.Split('/').Last());
                }

                // Upload new avatar image to Cloudinary
                var uploadResult = new ImageUploadResult();
                using (var stream = avatarImage.OpenReadStream())
                {
                    var uploadParams = new ImageUploadParams()
                    {
                        File = new FileDescription(avatarImage.FileName, stream),
                        Folder = "BlogProject",
                        Transformation = new Transformation().Width(200).Height(200).Crop("fill").Gravity("face")
                    };

                    uploadResult = await _cloudinary.UploadAsync(uploadParams);
                }

                // Update or add avatar path in database
                if (activeEntry != null)
                {
                    activeEntry.Avata = uploadResult.SecureUrl.AbsoluteUri;
                    _context.Actives.Update(activeEntry);
                }
                else
                {
                    activeEntry = new Actives
                    {
                        AppUserID = user.Id,
                        Avata = uploadResult.SecureUrl.AbsoluteUri,
                        StatusName = updateUser.StatusName // Optional: Add any default status name if needed
                    };
                    await _context.Actives.AddAsync(activeEntry);
                }
            }

            // Attempt to update the user
            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                return (false, "Failed to update user.");
            }

            await _context.SaveChangesAsync();
            return (true, "User updated successfully.");
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


        public async Task<IdentityResult> AddUserAsync(AddUser addUserDto)
        {
            // Tạo đối tượng người dùng mới
            var user = new AppUser
            {
                UserName = addUserDto.Username,
                Email = addUserDto.Email,

            };

            // Tạo người dùng với mật khẩu
            var result = await _userManager.CreateAsync(user, addUserDto.Password);
            if (!result.Succeeded) return result;

            // Kiểm tra và gán vai trò cho người dùng nếu role hợp lệ
            var selectedRole = addUserDto.Role;
            if (!await _roleManager.RoleExistsAsync(selectedRole))
            {
                await _roleManager.CreateAsync(new IdentityRole(selectedRole));
            }
            await _userManager.AddToRoleAsync(user, selectedRole);
            if (_context.Actives == null)
            {
                throw new InvalidOperationException("Active statuses data source is unavailable.");
            }
            // Thêm trạng thái hoạt động vào bảng Actives
            var activeStatus = new Actives
            {
                AppUserID = user.Id,
                StatusName = "Y"  // Mặc định là "Y" cho trạng thái hoạt động
            };
            _context.Actives.Add(activeStatus);
            await _context.SaveChangesAsync();

            return result;
        }


    }
}