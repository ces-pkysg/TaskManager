using System.ComponentModel.DataAnnotations;

namespace TaskManagerAPI.DTOs.TaskItemDTOs
{
    public class UpdateTaskRequest
    {
        [Required(ErrorMessage = "El título es obligatorio.")]
        [MaxLength(200)]
        public string Title { get; set; }
        public bool? IsCompleted { get; set; }

        [Required(ErrorMessage = "La categoría es obligatoria")]
        public int? CategoryId { get; set; }

        [Range(1, 5, ErrorMessage = "El step debe estar entre 1 y 5")]
        public int? Step { get; set; }
    }
}
