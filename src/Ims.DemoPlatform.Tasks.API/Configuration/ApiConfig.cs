using Ims.DemoPlatform.Tasks.API.Data;
using Ims.DemoPlatform.Tasks.API.Data.Repositories;
using Ims.DemoPlatform.Tasks.API.Services;
using Microsoft.EntityFrameworkCore;

namespace Ims.DemoPlatform.Tasks.API.Configuration;

public static class ApiConfig
{
    public static void AddApiConfiguration(this WebApplicationBuilder builder)
    {
        // Database
        builder.Services.AddDbContext<TaskDbContext>(options =>
            options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

        // Repositories
        builder.Services.AddScoped<ITaskRepository, TaskRepository>();

        // Services
        builder.Services.AddScoped<ITaskService, TaskService>();
    }
}

