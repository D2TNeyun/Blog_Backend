using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Src.Dtos.Category
{
    public class CreateCategoryDto
    {
        [Required]
        public string CategoryName { get; set; } = string.Empty;
    }
}