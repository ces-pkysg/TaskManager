namespace TaskManager.Web.Models
{
    public class EditTaskModal
    {
        public int CategoryId { get; set; }
        public string Title { get; set; }
        public bool IsCompleted { get; set; }
    }
}
