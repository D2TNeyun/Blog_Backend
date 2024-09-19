using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace Src.Dtos.Auth
{
    public class RegistrationResultDto
    {
        public bool Success { get; set; }
        public string? Message { get; set; }
        public NewUserDto? NewUser { get; set; }
        public IEnumerable<IdentityError>? Errors { get; set; }
    }
}