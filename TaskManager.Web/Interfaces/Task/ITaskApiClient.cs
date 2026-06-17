using TaskManager.Web.Models;

namespace TaskManager.Web.Services
{
    public interface ITaskApiClient
    {
        Task<PagedResultViewModel<TaskViewModel>> GetTasksAsync(int page = 1, int pageSize = 5);
        Task<PagedResultViewModel<TaskViewModel>> SearchTasksAsync(TaskSearchViewModel filters);
        Task CreateTaskAsync(CreateTaskViewModel model);
        Task<EditTaskViewModel> GetTaskByIdAsync(int id);
        Task UpdateTaskAsync(EditTaskViewModel model);
        Task<bool> DeleteTaskAsync(int id);
        Task<string> ImportTaskFromExcelAsync(IFormFile file);
        Task<TaskViewModel> GetTaskDetailAsync(int id);
        Task<PagedResultViewModel<TaskViewModel>> AdvancedSearchAsync(TaskSearchViewModel filters);

    }
}
