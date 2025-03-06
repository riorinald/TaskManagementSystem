using Microsoft.OpenApi.Models;
using System.Reflection;
using TaskManagementSystem.API.Extensions;
using TaskManagementSystem.Infrastructure;
using TaskManagementSystem.Infrastructure.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// Configure Swagger with enhanced documentation
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "Task Management API",
        Description = "REST API for managing tasks"
    });

    // Add XML comments to Swagger
    var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFilename);
    options.IncludeXmlComments(xmlPath);
});

// Add application services
builder.Services.AddApplicationServices();

// Add infrastructure services
builder.Services.AddInfrastructure(builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline
app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "Task Management API v1");
    options.RoutePrefix = string.Empty; // Serve Swagger at the root
    options.DocumentTitle = "Task Management API Documentation";
    options.DefaultModelsExpandDepth(2);
});

app.UseAuthorization();
app.MapControllers();

// Initialize SQLite
await using (var scope = app.Services.CreateAsyncScope())
{
    var databaseContext = scope.ServiceProvider.GetRequiredService<IDatabaseContext>();
    await databaseContext.InitializeDatabaseAsync();
}

app.Run(); 