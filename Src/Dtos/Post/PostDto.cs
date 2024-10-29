using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Src.Dtos.Category;
using Src.Dtos.Comment;
using Src.Dtos.Tag;
using Src.Dtos.User;

namespace Src.Dtos.Post
{
    public class PostDto
    {
        public int PostID { get; set; }
        public string AppUserID { get; set; } = string.Empty; 
        public int? CategoryID { get; set; }
        public int? TagID { get; set; }
        public string Title { get; set; } = string.Empty; 
        public string? Content { get; set; }
        public DateTime PublishedDate { get; set; }
        public string? Image { get; set; }
        public int Views { get; set; }
        
         public string? Description { get; set; }
        //relationship
        public CategoryDto? Category { get; set; }
        public TagDto? Tag { get; set; }
        public UserDto? AppUser { get; set; }
        public List<CommentsDto>? Comments { get; set; }
    }
}