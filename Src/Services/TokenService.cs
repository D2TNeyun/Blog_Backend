using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Src.Models;

public interface ITokenService
{
    string GenerateToken(AppUser user, string role);
}

namespace Src.Services
{
    public class TokenService : ITokenService
    {
        private readonly IConfiguration _configuration;

        public TokenService(IConfiguration configuration)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        public string GenerateToken(AppUser user, string role)
        {
            // Create the claims for the JWT token
            var claims = new List<Claim>
            {
                new(JwtRegisteredClaimNames.Email, user?.Email ?? ""),
                new(JwtRegisteredClaimNames.GivenName, user?.UserName ?? ""),
                new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new(ClaimTypes.Role, role)
            };


            // Retrieve the signing key from configuration
            var keyString = _configuration["Jwt:SigningKey"];
            if (string.IsNullOrEmpty(keyString))
            {
                throw new InvalidOperationException("Jwt:SigningKey is null or empty.");
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(keyString));

            // Generate the JWT token
            var tokenDescriptor = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(1),
                signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256)
            );

            return new JwtSecurityTokenHandler().WriteToken(tokenDescriptor);
        }

    }
}
