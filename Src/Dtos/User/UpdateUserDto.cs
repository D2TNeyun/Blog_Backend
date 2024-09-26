using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CloudinaryDotNet.Actions;

namespace Src.Dtos.User
{
    public class UpdateUserDto
    {
        public string? UserName { get; set; } 
        public string? Email { get; set; } 
        public string? RoleID { get; set; }

        //

    }
}