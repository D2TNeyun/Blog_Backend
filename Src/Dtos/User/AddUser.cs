using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Src.Dtos.User
{
    public class AddUser
    {
        public string Username { get; set; }= string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public string StatusName { get; set; }  = string.Empty;
    }
}