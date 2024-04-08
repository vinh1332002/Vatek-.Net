using BookProject.Data;
using BookProject.Data.Models;
using BookProject.Service.DTOs.Category;
using BookProject.Service.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BookProject.Service.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly BookProjectContext _context;

        public CategoryService(BookProjectContext context)
        {
            _context = context;
        }

        public async Task<Category> CreateCategory(CreateCategoryRequest name)
        {
            if (string.IsNullOrEmpty(name.Name))
            {
                throw new ArgumentException("Category name cannot be empty.");
            }

            var category = new Category
            {
                CategoryName = name.Name,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            };

            _context.Categories.Add(category);
            await _context.SaveChangesAsync();

            return category;
        }

        public async Task<Category?> GetCategoryById(int categoryId)
        {
            return await _context.Categories.FindAsync(categoryId);
        }

        public async Task<Category> UpdateCategory(UpdateCategoryRequest updateCategory, int id)
        {
            var category = await _context.Categories.FindAsync(id);

            if (category == null) return null;

            category.CategoryName = updateCategory.Name;
            category.UpdatedAt = DateTime.Now;

            var update = _context.Categories.Update(category);

            if (update == null) return null;

            _context.SaveChanges();

            return category;
        }

        public async Task DeleteCategory(int categoryId)
        {
            var category = await GetCategoryById(categoryId);

            if (category != null)
            {
                _context.Categories.Remove(category);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<Category>> GetAllCategories()
        {
            return await _context.Categories.ToListAsync();
        }
    }
}
