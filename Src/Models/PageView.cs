using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Src.Models
{
    public class PageView
    {
        public int Id { get; set; }
        public string? PageName { get; set; }
        public DateTime? Date { get; set; }
        public int Views { get; set; }
    }
}