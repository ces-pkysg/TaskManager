using Microsoft.AspNetCore.Mvc;
using TaskManagerAPI.DTOs;
using TaskManagerAPI.DTOs.TaskItemDTOs;

namespace TaskManagerAPI.Interfaces.Tasks
{
    public interface ITaskService
    {
        Task<PagedResultDTO<TaskWithCategoryDTO>> AdvancedSearchAsync(
        string? text,
        bool? completed,
        int? step,
        int? categoryId,
        string? categoryName,
        int page,
        int pageSize
    );
        Task<IEnumerable<TaskWithCategoryDTO>> GetWithCategoryAsync();
        Task<IEnumerable<TaskSearchResult>> GetPagedAsync(TaskSearchRequest request);
        Task<IEnumerable<TaskSearchResult>> SearchAsync(
        string? text,
        bool? completed,
        int? step,
        string? orderBy,
        TaskSearchRequest request
    );
        Task<bool> DeleteAsync(int id);
        Task<bool> UpdateAsync(
        int id,
        UpdateTaskRequest request
    );
        Task<TaskItemResponse> CreateAsync(CreateTaskRequest request);
        Task<TaskItemResponse?> GetByIdAsync(int id);
        Task<IEnumerable<TaskItemResponse>> GetAllAsync();


    }

}
