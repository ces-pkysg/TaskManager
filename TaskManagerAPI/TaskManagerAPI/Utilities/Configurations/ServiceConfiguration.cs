using TaskManagerAPI.Interfaces.Categories;
using TaskManagerAPI.Interfaces.Tasks;
using TaskManagerAPI.Services.Category;


namespace TaskManager.Utilities.Configurations
{
    public static class ServiceConfiguration
    {

        public static void AddServices(this IServiceCollection services)
        {
            services.AddScoped<ITaskService, TaskService>();//AddScoped esta es la interface --> ITaskService a la que voy a acceder a este servicio (El de la izquierda es la puerta de acceso al de la derecha)
            services.AddScoped<ICategoryService, CategoryService>();

        }
    }
}
