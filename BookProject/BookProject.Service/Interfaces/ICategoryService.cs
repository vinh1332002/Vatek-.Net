using BookProject.Data.Models;
using BookProject.Service.DTOs.Category;

namespace BookProject.Service.Interfaces
{
    public interface ICategoryService
    {
        Task<Category> CreateCategory(CreateCategoryRequest name);
        Task<Category?> GetCategoryById(int categoryId);
        Task<Category> UpdateCategory(UpdateCategoryRequest updateCategory, int id);
        Task DeleteCategory(int categoryId);
        Task<IEnumerable<Category>> GetAllCategories();
    }
}
