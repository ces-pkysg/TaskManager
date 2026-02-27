namespace TaskManager.Models
{
    public class TaskItem
    {
        public int Id { get; set; } //Clave primaria de la tabla.
        public string Title { get; set; } //Nombre de la tarea.
        public bool IsCompleted { get; set; } //Indica si la tarea está terminada.

        public int Step { get; set; } //Paso actual de la tarea.
        public DateTime CreatedAt { get; set; } = DateTime.Now; //Fecha de creación. sino manda valor se le asigna atomaticamente fecha actual
        public int? CategoryId { get; set; } //Clave foránea (FK) hacia la tabla Categories. (FK y campo de categorias ?(nullable) )
        public Category? Category { get; set; } //Propiedad de navegación. esta linea hace que el campo apunte a una tabla  Entity Framework hace el JOIN automático.
        public bool IsDeleted { get; set; } = false; //Soft delete.


    }
}
