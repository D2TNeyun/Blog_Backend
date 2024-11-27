using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Src.Dtos.Auth;
using Src.Models;
using Src.Services;

namespace Src.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController(AuthService authService, SignInManager<AppUser> signInManager, UserManager<AppUser> userManager) : ControllerBase
    {
        private readonly AuthService? _authService = authService;
        private readonly SignInManager<AppUser> _signInManager = signInManager;

        private readonly UserManager<AppUser> _userManager = userManager;

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
                var (Success, Message, Token, User) = await _authService.LoginAsync(login);

                if (Success)
                {
                    return Ok(new { Success = true, Message = "Login Successfully!!", Token, User });
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

        [HttpGet("signin-google")]
        [EnableCors("AllowReactApp")]
        public IActionResult LoginWithGoogle() => Challenge(new AuthenticationProperties
        {
            RedirectUri = "http://localhost:5273/api/auth/signin-google"// URL callback khi đăng nhập thành công
        }, GoogleDefaults.AuthenticationScheme);

        [HttpGet("google/callback")]
        public async Task<IActionResult> GoogleCallbackAsync()
        {
            var authenticateResult = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            if (!authenticateResult.Succeeded)
            {
                return BadRequest("Authentication failed.");
            }

            // Extract user information from Google claims
            var externalClaims = authenticateResult.Principal.Claims;
            var googleId = externalClaims.FirstOrDefault(c => c.Type == "sub")?.Value;
            var email = externalClaims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
            var name = externalClaims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;

            // Check if the user already exists, otherwise create a new user
            if (string.IsNullOrEmpty(email))
            {
                return BadRequest("Email is not provided.");
            }
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                user = new AppUser { UserName = name, Email = email };
                var createResult = await _userManager.CreateAsync(user);
                if (!createResult.Succeeded)
                {
                    return BadRequest("Failed to create a new user.");
                }
            }

            // Sign in the user after authentication
            await _signInManager.SignInAsync(user, isPersistent: false);

            // Redirect the user to the frontend home page or any specific page
            return Redirect("http://localhost:5173"); // Adjust the redirect URI as needed
        }


    }
}
