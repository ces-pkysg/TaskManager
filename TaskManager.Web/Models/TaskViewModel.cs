using System.Text.Json.Serialization;

namespace TaskManager.Web.Models
{
    public class TaskViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public bool IsCompleted { get; set; }

        //Se agrega mapeo para "step"
        [JsonPropertyName("step")]
        public int Step { get; set; }

        public DateTime CreatedAt { get; set; }

        //Se agrega mapeo para CategoryId
        [JsonPropertyName("categoryId")]
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
    }
}

