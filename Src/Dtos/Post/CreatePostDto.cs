using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Src.Dtos.Post
{
    public class CreatePostDto
    {
        public string AppUserID { get; set; } = string.Empty;
        public int CategoryID { get; set; }
        public int TagID { get; set; } 
        public string? Title { get; set; }
        public string? Description { get; set; }
        public string? Content { get; set; }
        public required IFormFile Image { get; set; }
    }
}