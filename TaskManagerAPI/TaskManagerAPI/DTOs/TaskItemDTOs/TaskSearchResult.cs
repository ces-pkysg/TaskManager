using System.ComponentModel.DataAnnotations;

namespace TaskManagerAPI.DTOs.TaskItemDTOs
{
    public class TaskSearchResult
    {
        public int Identificador { get; set; }
        public string Titulo { get; set; }
        public bool Completado { get; set; }

        public int PasoActual { get; set; }
        public DateTime FechaCreacion { get; set; } = DateTime.Now; //Gestionar cuando se creó el registro con un valor por defecto


    }
}