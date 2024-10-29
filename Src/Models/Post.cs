using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Src.Models
{
    public class Post
    {
        public int PostID { get; set; }
        public string? AppUserID  { get; set; }
        public int? CategoryID { get; set; }
        public int? TagID { get; set; }
        
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? Content { get; set; } 
        public DateTime PublishedDate { get; set; }
        public string? Image { get; set; }
        public int Views { get; set; }

        //relationship
        public Category? Category { get; set; }
        public AppUser? AppUser { get; set; }
        public Tag? Tag { get; set; }
        public List<Comment>? Comments { get; set; }
    }
}