using TaskManager.Web.Services;

namespace TaskManager.Web.Extensions
{
    //inyección de dependencias para servivio e interfaz de cliente y se agrega la de cartegorias
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddApiClients(this IServiceCollection services)
        {
            services.AddHttpClient<ITaskApiClient, TaskApiClient>();//1ra inyección 
            services.AddHttpClient<ICategoryApiClient, CategoryApiClient>();//2da inyección

            return services;
        }
    }
}