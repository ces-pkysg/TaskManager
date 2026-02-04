using System.ComponentModel.DataAnnotations;

namespace TaskManagerAPI.DTOs.CategoryDTOs
{
    public class CreateCategoryRequest
    {
        //se ubican sobre los que quiero que se apliquen
        [Required(ErrorMessage = "El título es obligatorio.")] //estos son unos data annotations
        [MaxLength(100, ErrorMessage = "El título no puede superar los 200 caracteres.")]
        public string Name { get; set; }
    }
}
