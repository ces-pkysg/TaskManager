using Microsoft.EntityFrameworkCore;
using TaskManager.Context;
using TaskManager.Models;
using TaskManagerAPI.DTOs.CategoryDTOs;
using TaskManagerAPI.Interfaces.Categories;



namespace TaskManagerAPI.Services.Category
{
    public class CategoryService : ICategoryService
    {
        private readonly AppDbContext _context;

        public CategoryService(AppDbContext context)
        {
            _context = context;
        }

        // GET ALL
        public async Task<IEnumerable<CategoryResponse>> GetAllAsync()
        {
            return await _context.Categories
                .OrderBy(c => c.Id)
                .Select(c => new CategoryResponse
                {
                    Id = c.Id,
                    Name = c.Name
                })
                .ToListAsync();
        }

        // GET BY ID
        public async Task<CategoryResponse?> GetByIdAsync(int id)
        {
            return await _context.Categories
                .Where(c => c.Id == id)
                .Select(c => new CategoryResponse
                {
                    Id = c.Id,
                    Name = c.Name
                })
                .FirstOrDefaultAsync();
        }

        // CREATE
        public async Task<CategoryResponse> CreateAsync(CreateCategoryRequest request)
        {
            var entity = new TaskManager.Models.Category 
            {
                Name = request.Name.Trim()
            };

            _context.Categories.Add(entity);
            await _context.SaveChangesAsync();

            return new CategoryResponse
            {
                Id = entity.Id,
                Name = entity.Name
            };
        }

        // DELETE
        public async Task<bool> DeleteAsync(int id)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category == null) return false;

            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}

