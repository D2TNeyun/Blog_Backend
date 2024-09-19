using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Src.Models
{
    public class Comment
    {
        public int CommentId { get; set; }
        public string? Content { get; set; }
        public string? AppUserID { get; set;}
        public int? PostId ; 

        //relationShip
        public Post? Post { get; set; }
        public AppUser? AppUser { get; set; }

    }
}