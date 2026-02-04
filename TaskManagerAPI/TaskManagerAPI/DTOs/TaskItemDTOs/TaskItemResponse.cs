namespace TaskManagerAPI.DTOs.TaskItemDTOs
{
    public class TaskItemResponse
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public bool IsComplete { get; set; }
    }
}
