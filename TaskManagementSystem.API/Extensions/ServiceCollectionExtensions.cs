using TaskManagementSystem.Application.Interfaces;
using TaskManagementSystem.Application.Services;

namespace TaskManagementSystem.API.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services.AddScoped<IWorkItemService, WorkItemService>();
            
            return services;
        }
    }
} 