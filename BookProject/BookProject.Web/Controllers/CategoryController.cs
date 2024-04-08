using BookProject.Data.Models;
using BookProject.Service.DTOs.Category;
using BookProject.Service.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace BookProject.Web.Controllers
{
    [Route("api")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryService _categoryService;

        public CategoryController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        [HttpPost]
        [Route("category/create")]
        public async Task<IActionResult> CreateCategory([FromBody] CreateCategoryRequest name)
        {
            if (name == null) return BadRequest();

            try
            {
                var category = await _categoryService.CreateCategory(name);
                return Ok(category);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating category: {ex.Message}");
                return StatusCode(500, "Internal server error.");
            }
        }

        [HttpPut]
        [Route("category/update/{id}")]
        public async Task<IActionResult> UpdateCategory([FromBody] UpdateCategoryRequest updateRequest, int id)
        {
            if (updateRequest == null) return BadRequest();

            try
            {
                var updatedDocument = await _categoryService.UpdateCategory(updateRequest, id);

                if (updatedDocument == null)
                {
                    return NotFound("Category not found");
                }

                return Ok(updatedDocument);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while update category with ID {id}: {ex.Message}");
            }
        }

        [HttpGet]
        [Route("category/getall")]
        public async Task<ActionResult<IEnumerable<Category>>> GetCategories()
        {
            try
            {
                var categories = await _categoryService.GetAllCategories();
                if (categories == null)
                {
                    return NotFound();
                }
                return Ok(categories.ToList());
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while retrieving categories: {ex.Message}");
            }
        }

        [HttpDelete]
        [Route("category/delete/{id}")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            try
            {
                await _categoryService.DeleteCategory(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while deleting category with ID {id}: {ex.Message}");
            }
        }
    }
}
