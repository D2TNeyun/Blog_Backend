using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Src.Dtos.Category;
using Src.Dtos.Tag;

namespace Src.Dtos.Post
{
    public class AddPostDto
    {
        public int CategoryID { get; set; }
        public int TagID { get; set; }
        public string? Title { get; set; }
        public string? Content { get; set; }

    }
}