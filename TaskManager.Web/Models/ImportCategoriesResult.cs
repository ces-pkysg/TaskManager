namespace TaskManager.Web.Models
{
    public class ImportCategoriesResult
    {
        public string Message { get; set; }//recibe mensaje (no importa si esta mayus o minusculas)
        public int Duplicadas { get; set; }//recibe duplicados (no importa si esta mayus o minusculas)
    }
}