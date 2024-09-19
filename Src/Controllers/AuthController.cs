using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Src.Dtos.Auth;
using Src.Models;
using Src.Services;

namespace Src.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController(AuthService authService, SignInManager<AppUser> signInManager ) : ControllerBase
    {
        private readonly AuthService? _authService = authService;
        private readonly SignInManager<AppUser> _signInManager = signInManager;

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromForm] RegisterDto register)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                if (_authService is null)
                {
                    ModelState.AddModelError(string.Empty, "Auth service is not available.");
                    return BadRequest(ModelState);
                }
                var result = await _authService.RegisterAsync(register);
                if (result.Success)
                {
                    return Ok(new { message = "User registered successfully" });
                }
                else if (result.Errors != null)
                {
                    return StatusCode(500, result.Errors);
                }
                else
                {
                    return BadRequest(result.Message);
                }
            }
            catch (InvalidOperationException ex)
            {
                // Xử lý ngoại lệ cụ thể
                ModelState.AddModelError(string.Empty, ex.Message);
                return BadRequest(ModelState);
                throw;
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
                throw;
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromForm] LoginDto login)
        {
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                if (_authService is null)
                {
                    ModelState.AddModelError(string.Empty, "Auth service is not available.");
                    return BadRequest(ModelState);
                }
                var (Success, Message, Token) = await _authService.LoginAsync(login);

                if (Success)
                {
                    return Ok(new { Success = true, Message = "Login Successfully!!", Token });
                }
                else
                {
                    return Unauthorized(Message);
                }
            }
        }


        [HttpPost("logout")]
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            if (User.Identity?.IsAuthenticated == true)
            {
                await _signInManager.SignOutAsync();
                return Ok(new { message = "User logged out successfully" });
            }
            else
            {
                return Unauthorized("User is not authenticated");
            }
        }

    }
}
