using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace TaskManager.Web.Services
{
    public interface ICategoryApiClient
    {
        Task<IActionResult> GetCategoriesAsync();
        Task<string> ImportCategoriesFromExcelAsync(IFormFile file);

    }
}
