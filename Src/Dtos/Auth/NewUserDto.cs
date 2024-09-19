using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Src.Dtos.Auth
{
    public class NewUserDto
    {   
        [Required]
         public string? UserName { get; set; }
         
        [Required]
        public string? Email { get; set; }
        public string? Token { get; set; }
    }
}