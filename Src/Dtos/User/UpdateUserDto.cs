using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CloudinaryDotNet.Actions;

namespace Src.Dtos.User
{
    public class UpdateUserDto
    {
        public string UserName { get; set; }  = string.Empty;
        public string Email { get; set; }  = string.Empty;
        public string Role  { get; set; }  = string.Empty;
        public string StatusName { get; set; }  = string.Empty;
        public string Avata { get; set; } = string.Empty;

    }
}