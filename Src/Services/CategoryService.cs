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
using Src.Mock;
using Src.Models;

namespace Src.Services
{
    public class CategoryService(ApplicationDBContext context, IMapper mapper) : ICategoryService
    {
        public readonly ApplicationDBContext _context = context;
        private readonly IMapper _mapper = mapper;

        public async Task<Category> CreateCategoryAsync(CreateCategoryDto createCategoryDto)
        {
            // Kiểm tra xem danh mục đã tồn tại hay chưa
            if (_context.Categories == null)
            {
                throw new InvalidOperationException("Categories statuses data source is unavailable.");
            }
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
            if (_context.Categories == null)
            {
                throw new InvalidOperationException("Categories statuses data source is unavailable.");
            }
            var categories = await _context.Categories
                .Include(c => c.Tags)
                .ToListAsync();
            return _mapper.Map<List<CategoryDto>>(categories);
        }

        public async Task<CategoryDto?> GetCategoryByIdAsync(int id)
        {
            if (_context.Categories == null)
            {
                throw new InvalidOperationException("Categories statuses data source is unavailable.");
            }
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
            if (_context.Categories == null)
            {
                throw new InvalidOperationException("Categories statuses data source is unavailable.");
            }
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
            if (_context.Categories == null)
            {
                throw new InvalidOperationException("Categories statuses data source is unavailable.");
            }
            // Tìm danh mục theo ID
            var category = await _context.Categories.FindAsync(id);

            if (category == null)
            {
                return false;
            }

            if (_context.Tags == null)
            {
                throw new InvalidOperationException("Tags data source is unavailable.");
            }
            var tags = await _context.Tags.Where(t => t.CategoryID == id).ToListAsync();

            _context.Tags.RemoveRange(tags);
            _context.Categories.Remove(category);

            // Lưu thay đổi
            return await _context.SaveChangesAsync() > 0;
        }


    }
}