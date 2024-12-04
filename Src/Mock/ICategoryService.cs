using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Src.Dtos.Category;
using Src.Models;

namespace Src.Mock
{
    public interface ICategoryService
    {
        Task<Category> CreateCategoryAsync(CreateCategoryDto createCategoryDto);
        Task<List<CategoryDto>> GetAllCategories();
        Task<CategoryDto?> GetCategoryByIdAsync(int id);
        Task<Category?> UpdateCategoryAsync(int id, UpdateCategoryDto categoryDto);
       Task<bool> DeleteCategoryAsync(int id);
    }
}