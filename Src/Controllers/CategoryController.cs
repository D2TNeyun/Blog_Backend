using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Src.Data;
using Src.Dtos.Category;
using Src.Models;
using Src.Services;

namespace Src.Controllers
{
    [Route("api/categories")]
    [ApiController]
    // [Authorize(Roles = "Admin")]

    public class CategoryController(CategoryService categoryService) : ControllerBase
    {
        public readonly CategoryService _categoryService = categoryService;

        [HttpGet]
        public async Task<ActionResult<List<Category>>> GetCategories()
        {
            var categories = await _categoryService.GetAllCategories();
            return Ok(new { categories });
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Category>> GetCategoryById(int id)
        {
            var category = await _categoryService.GetCategoryByIdAsync(id);
            if (category == null)
            {
                return NotFound("Category not found");
            }
            return Ok(new { category });
        }

        [HttpPost]
        public async Task<IActionResult> CreateCategory([FromForm] createCategoryDto createCategoryDto)
        {
            var category = await _categoryService.CreateCategoryAsync(createCategoryDto);
            return Ok(category);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCategory(int id, [FromForm] UpdateCategoryDto update)
        {
            var UpdateCategory = await _categoryService.UpdateCategoryAsync(id, update);
            if (UpdateCategory == null)
            {
                return NotFound();
            }

            return Ok(UpdateCategory);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            var category = await _categoryService.DeleteCategoryAsync(id);
            if (!category)
            {
                return NotFound();
            }

            return Ok(new { message = "Category deleted successfully." });
        }
    }
}