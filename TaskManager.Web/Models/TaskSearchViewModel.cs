namespace TaskManager.Web.Models
{
    public class TaskSearchViewModel
    {
        // Filtros del usuario
        public string? Text { get; set; }
        public string? CategoryName { get; set; }
        public bool? IsCompleted { get; set; }
        public int? Step { get; set; }

        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 5;

        public int? CategoryId { get; set; }//representa lo que el usuario pone en pantalla (filtros) y muestra resultado

        // Resultados devueltos por la API
        public PagedResultViewModel<TaskViewModel>? Result { get; set; }
    }
}