using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Src.Models
{
    public class Tag
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int TagID { get; set; }
        public int? CategoryID { get; set; }
        public string? TagName { get; set; }

        //relationship
        public ICollection<Post>? Posts { get; set; }
        public Category? Category { get; set; }
    }
}