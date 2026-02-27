using System.ComponentModel.DataAnnotations;

namespace TaskManagerAPI.DTOs.TaskItemDTOs
{
    public class CreateTaskRequest
    {
        [Required(ErrorMessage = "El título es obligatorio.")] //estos son unos data annotations
        [MaxLength(200, ErrorMessage = "El título no puede superar los 200 caracteres.")]
        public string Title { get; set; }
        public bool IsCompleted { get; set; }

        [Required(ErrorMessage = "CategoryId es requerido.")]
        public int CategoryId { get; set; }

    }
}
