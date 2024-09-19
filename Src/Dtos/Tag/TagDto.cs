using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Src.Dtos.Category;
using Src.Dtos.Post;

namespace Src.Dtos.Tag
{
    public class TagDto
    {
        public int TagID { get; set; }
        public int? CategoryID { get; set; }
        public string? TagName { get; set; }

        // Navigation properties
        public CategoryDto? Category { get; set; }
        public ICollection<PostDto>? Posts { get; set; }

    }
}