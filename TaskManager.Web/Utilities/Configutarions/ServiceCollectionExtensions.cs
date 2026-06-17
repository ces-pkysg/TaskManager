using TaskManager.Web.Http;
using TaskManager.Web.Services;

namespace TaskManager.Web.Extensions
{
    //inyección de dependencias para servivio e interfaz de cliente y se agrega la de cartegorias
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddApiClients(this IServiceCollection services)
        {
            //trackeo con handler
            services.AddTransient<ApiExceptionHandler>();
            //trackeo con handler
            services.AddHttpClient<ITaskApiClient, TaskApiClient>()
                    .AddHttpMessageHandler<ApiExceptionHandler>();
            //trackeo con handler
            services.AddHttpClient<ICategoryApiClient, CategoryApiClient>()
                    .AddHttpMessageHandler<ApiExceptionHandler>();



            services.AddHttpClient<ITaskApiClient, TaskApiClient>();//1ra inyección 
            services.AddHttpClient<ICategoryApiClient, CategoryApiClient>();//2da inyección

            return services;
        }
    }
}