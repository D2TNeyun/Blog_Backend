using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Src.Dtos.Comment
{
    public class CmtById
    {
        public int CommentId { get; set; }
        public string Content { get; set; } = string.Empty;
        public string AppUserID { get; set; } = string.Empty;
        public int? PostId { get; set; }
    }
}