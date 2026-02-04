namespace TaskManager.Web.Models
{
    public class ImportTasksResult
    {
        public string Message { get; set; }//recibe mensaje
        public int Duplicadas { get; set; }//recibe duplicados
    }
}