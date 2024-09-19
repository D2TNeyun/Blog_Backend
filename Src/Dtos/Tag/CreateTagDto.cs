using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Src.Dtos.Tag
{
    public class CreateTagDto
    {
        public int? CategoryID { get; set; }
        public string? TagName { get; set; }
    }
}