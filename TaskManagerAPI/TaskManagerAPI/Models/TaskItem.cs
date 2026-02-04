namespace TaskManager.Models
{
    public class TaskItem
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public bool IsComplete { get; set; }

        public int Step { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now; //Gestionar cuando se creó el registro con un valor por defecto
        public int? CategoryId { get; set; } //FK y campo de categorias ?(nullable)
        public Category? Category { get; set; } //esta linea hace que el campo apunte a una tabla
        public bool IsDeleted { get; set; } = false;
    }
}
