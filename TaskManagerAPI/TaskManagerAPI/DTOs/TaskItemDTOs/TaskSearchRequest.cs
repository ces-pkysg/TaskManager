using System.ComponentModel.DataAnnotations;

namespace TaskManagerAPI.DTOs.TaskItemDTOs
{
    public class TaskSearchRequest
    {
        public string? text { get; set; }
        public bool? completed { get; set; }
        public int? step { get; set; }
        public string? orderBy { get; set; }



        // PAGINACIÓN
        [Range(1, int.MaxValue, ErrorMessage = "La página debe ser mayor o igual a 1.")]
        public int Page { get; set; } = 1;

        [Range(0, 50, ErrorMessage = "El tamaño de página no puede ser negativo.")]
        public int PageSize { get; set; } = 10;
    }
}
