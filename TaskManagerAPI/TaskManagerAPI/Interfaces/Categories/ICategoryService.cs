using TaskManagerAPI.DTOs.CategoryDTOs;

namespace TaskManagerAPI.Interfaces.Categories
{
    public interface ICategoryService
    {
        Task<IEnumerable<CategoryResponse>> GetAllAsync();

        Task<CategoryResponse?> GetByIdAsync(int id);

        Task<CategoryResponse> CreateAsync(CreateCategoryRequest request);

        Task<bool> DeleteAsync(int id);
    }
}

