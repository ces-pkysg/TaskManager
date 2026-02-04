using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using TaskManager.Web.Models;

namespace TaskManager.Web.Services
{
    public class TaskApiClient : ITaskApiClient
    {
        private readonly HttpClient _httpClient;

        public TaskApiClient(HttpClient httpClient, IConfiguration configuration)
        {
            var baseUrl = configuration["ApiSettings:BaseUrl"];
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri(baseUrl);
        }

        public async Task<PagedResultViewModel<TaskViewModel>> GetTasksAsync(int page = 1, int pageSize = 5)
        {
            //Se quita int pagSize=5 de los parametros GetTasksAsync(int page = 1) para que no se mande distinto 
            //const int pageSize = 5;
            var url = $"/api/tasks/advanced-search?page={page}&pageSize={pageSize}";

            var result = await _httpClient.GetFromJsonAsync<PagedResultViewModel<TaskViewModel>>(url);

            return result!;
        }

        //
        public async Task<PagedResultViewModel<TaskViewModel>> SearchTasksAsync(TaskSearchViewModel filters)
        {
            //const int pageSize = 5;

            var query = new List<string>();

            if (!string.IsNullOrWhiteSpace(filters.Text))
                query.Add($"text={filters.Text}");

            if (!string.IsNullOrWhiteSpace(filters.CategoryName))
                query.Add($"categoryName={filters.CategoryName}");

            if (filters.IsCompleted.HasValue)
                query.Add($"isCompleted={filters.IsCompleted.Value}");

            if (filters.Step.HasValue)
                query.Add($"step={filters.Step.Value}");

            query.Add($"page={filters.Page}");
            //Se cambia filters.pageSize y se deja solamente pageSize
            query.Add($"pageSize={filters.PageSize}");

            var finalQueryString = string.Join("&", query);

            var url = $"/api/tasks/advanced-search?{finalQueryString}";

            return await _httpClient.GetFromJsonAsync<PagedResultViewModel<TaskViewModel>>(url);
        }

        public async Task CreateTaskAsync(CreateTaskViewModel model)
        {
            var response = await _httpClient.PostAsJsonAsync(
                "/api/tasks",
                model
            );

            response.EnsureSuccessStatusCode();//si la respuesta no es un 200 lanza una excepcion 
        }

        //detail tareas
        public async Task<TaskViewModel> GetTaskDetailAsync(int id)
        {
            return await _httpClient.GetFromJsonAsync<TaskViewModel>($"/api/tasks/{id}");
        }


        //editar tareas
        public async Task<EditTaskViewModel> GetTaskByIdAsync(int id)
        {
            var response = await _httpClient.GetFromJsonAsync<TaskViewModel>($"/api/tasks/{id}");

            return new EditTaskViewModel
            {
                Id = response.Id,
                Title = response.Title,
                CategoryId = response.CategoryId,
                Step = response.Step,
                IsCompleted = response.IsCompleted
            };
        }

        public async Task UpdateTaskAsync(EditTaskViewModel model)
        {
            var response = await _httpClient.PutAsJsonAsync(
                $"/api/tasks/{model.Id}",
                model
            );

            if (!response.IsSuccessStatusCode)//se verifica
            {
                // Leer mensaje de la API
                var error = await response.Content.ReadAsStringAsync();
                throw new Exception(error);
            }
        }


        public async Task<bool> DeleteTaskAsync(int id)
        {
            var response = await _httpClient.DeleteAsync($"/api/tasks/{id}");

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                throw new Exception(error);
            }

            return true;
        }

        //importar excel
        public async Task<string> ImportTaskFromExcelAsync(IFormFile file)
        {
            using var content = new MultipartFormDataContent();

            // Convertimos el IFormFile en StreamContent
            using var stream = file.OpenReadStream();
            var fileContent = new StreamContent(stream);

            // Tipo MIME típico para Excel .xlsx (no es obligatorio pero está bien ponerlo)
            fileContent.Headers.ContentType =
                new MediaTypeHeaderValue("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");

            // El "file" aquí debe coincidir con el nombre del parámetro en el endpoint de la API
            content.Add(fileContent, "file", file.FileName);

            // Llamamos a la API
            var response = await _httpClient.PostAsync("/api/tasks/import-excel", content);

            if (!response.IsSuccessStatusCode)
            {
                var errorBody = await response.Content.ReadAsStringAsync();
                throw new Exception($"Error al importar tareas. Respuesta API: {errorBody}");
            }

            // Leemos el JSON que envía la API
            var result = await response.Content.ReadFromJsonAsync<ImportTasksResult>();

            // Si por alguna razón no se pudo deserializar
            if (result == null || string.IsNullOrWhiteSpace(result.Message))
            {
                return "Importación realizada correctamente.";
            }

            // Devolvemos el mensaje que vino de la API
            // Ej: "Se importaron 5 categorías nuevas."
            return result.Message +
                   (result.Duplicadas > 0
                        ? $" ({result.Duplicadas} filas duplicadas no se importaron.)"
                        : string.Empty);
        }

    }
}
