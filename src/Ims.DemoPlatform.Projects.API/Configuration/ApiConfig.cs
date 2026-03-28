using Ims.DemoPlatform.Projects.API.Data;
using Ims.DemoPlatform.Projects.API.Data.Repositories;
using Ims.DemoPlatform.Projects.API.Services;
using Microsoft.EntityFrameworkCore;
namespace Ims.DemoPlatform.Projects.API.Configuration;
public static class ApiConfig
{
    public static void AddApiConfiguration(this WebApplicationBuilder builder)
    {
        // Database
        builder.Services.AddDbContext<ProjectDbContext>(options =>
            options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
       
        // Repositories
        builder.Services.AddScoped<IProjectRepository, ProjectRepository>();
        
        // Services
        builder.Services.AddScoped<IProjectService, ProjectService>();
    }
}
