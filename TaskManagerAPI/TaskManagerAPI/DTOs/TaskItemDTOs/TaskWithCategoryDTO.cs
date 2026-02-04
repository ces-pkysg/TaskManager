namespace TaskManagerAPI.DTOs.TaskItemDTOs
{
    public class TaskWithCategoryDTO
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public bool IsCompleted { get; set; }
        public int Step { get; set; }
        public DateTime CreatedAt { get; set; }

        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
    }
}
