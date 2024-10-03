using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Src.Dtos.Post;
using Src.Dtos.User;

namespace Src.Dtos.Comment
{
    public class CreateCmtDto
    {
        public string Content { get; set; } = string.Empty;
        public string AppUserID { get; set;} = string.Empty;
        public int? PostId { get; set; }  

        // //relationShip
        // public PostDto? Post { get; set; }
        // public UserDto? AppUser { get; set; }
    }
}