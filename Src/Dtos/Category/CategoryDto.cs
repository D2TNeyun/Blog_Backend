using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Src.Dtos.Post;
using Src.Dtos.Tag;

namespace Src.Dtos.Category
{
    public class CategoryDto
    {
        public int CategoryID { get; set; }
        public string CategoryName { get; set; } = string.Empty;
      
        // public List<TagDto>? Tags { get; set; } // List of tag 
        public List<TagIdDto>? Tags { get; set; } // List of tag
        public List<PostDto>? Posts { get; set; } // List of post
    }
}