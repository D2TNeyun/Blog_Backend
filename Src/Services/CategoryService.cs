using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Src.Data;
using Src.Dtos.Category;
using Src.Dtos.Post;
using Src.Dtos.Tag;
using Src.Models;

namespace Src.Services
{
    public class CategoryService(ApplicationDBContext context, IMapper mapper)
    {
        public readonly ApplicationDBContext _context = context;
        private readonly IMapper _mapper = mapper;

        public async Task<Category> CreateCategoryAsync(createCategoryDto createCategoryDto)
        {
            // Kiểm tra xem danh mục đã tồn tại hay chưa
            var existingCategory = await _context.Categories
                .FirstOrDefaultAsync(c => c.CategoryName == createCategoryDto.CategoryName);

            if (existingCategory != null)
            {
                throw new InvalidOperationException("Danh mục với tên này đã tồn tại.");
            }
            
            var category = new Category
            {
                CategoryName = createCategoryDto.CategoryName
            };

            _context.Categories.Add(category);
            await _context.SaveChangesAsync();
            return category;
        }

        public async Task<List<CategoryDto>> GetAllCategories()
        {
            var categories = await _context.Categories
                .Include(c => c.Tags) // Bao gồm các Tag liên quan
                                      // .Include(c => c.Posts)  // Bao gồm bảng Post
                .ToListAsync();
            return _mapper.Map<List<CategoryDto>>(categories);
        }

        public async Task<CategoryDto?> GetCategoryByIdAsync(int id)
        {
            var category = await _context.Categories
                    .Include(c => c.Tags)
                    .Include(c => c.Posts)
                    .FirstOrDefaultAsync(c => c.CategoryID == id);
            // .ToListAsync();
            if (category == null)
            {
                return null;
            }

            var categoryDto = _mapper.Map<CategoryDto>(category);

            return categoryDto;
        }

        public async Task<Category?> UpdateCategoryAsync(int id, UpdateCategoryDto categoryDto)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category == null)
            {
                return null;
            }
            category.CategoryName = categoryDto.CategoryName;

            _context.Categories.Update(category);
            await _context.SaveChangesAsync();
            return category;

        }

        public async Task<bool> DeleteCategoryAsync(int id)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category == null)
            {
                return false;
            }
            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}