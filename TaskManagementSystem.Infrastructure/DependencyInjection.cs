using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TaskManagementSystem.Application.Validators;
using TaskManagementSystem.Domain.Interfaces;
using TaskManagementSystem.Infrastructure.Data;
using TaskManagementSystem.Infrastructure.Repositories;

namespace TaskManagementSystem.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            // Register database context
            services.AddScoped<IDatabaseContext, SqliteDatabaseContext>();
            
            // Register repositories
            services.AddScoped<IWorkItemRepository, WorkItemRepository>();
            services.AddScoped<IUserRepository, UserRepository>();
            
            // Register validators
            services.AddScoped<CreateWorkItemValidator>();
            services.AddScoped<UpdateWorkItemValidator>();
            
            return services;
        }
    }
} 