using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Src.Models
{
    public class PostViewHistory
    {
        public int Id { get; set; }
        public int PostId { get; set; }
        public string IPAddress { get; set; } = string.Empty;
        public string UserId { get; set; } = string.Empty;
        public DateTime ViewedAt { get; set; }

        // Relationship
        public Post? Post { get; set; }
        public AppUser? AppUser { get; set; }
    }
}