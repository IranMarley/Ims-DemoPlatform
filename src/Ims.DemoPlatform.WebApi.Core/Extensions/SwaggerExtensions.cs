using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;

namespace Ims.DemoPlatform.WebApi.Core.Extensions;

/// <summary>
/// Extension methods for configuring Swagger/OpenAPI in API projects
/// </summary>
public static class SwaggerExtensions
{
    /// <summary>
    /// Adds Swagger configuration with JWT Bearer authentication support
    /// </summary>
    public static WebApplicationBuilder AddSwaggerConfiguration(
        this WebApplicationBuilder builder,
        string apiTitle,
        string apiDescription,
        string apiVersion = "v1")
    {
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc(apiVersion, new OpenApiInfo
            {
                Title = apiTitle,
                Description = apiDescription,
                Version = apiVersion
            });

            var securityScheme = new OpenApiSecurityScheme
            {
                Description = "Enter JWT Bearer token in the format: Bearer {your token}",                Name = "Authorization",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.Http,
                Scheme = "bearer",
                BearerFormat = "JWT"
            };

            options.AddSecurityDefinition("Bearer", securityScheme);

            options.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    Array.Empty<string>()
                }
            });
        });

        return builder;
    }

    /// <summary>
    /// Uses Swagger UI middleware
    /// </summary>
    public static WebApplication UseSwaggerConfiguration(
        this WebApplication app,
        IHostEnvironment? environment = null,
        string apiVersion = "v1")
    {
        app.UseSwagger();
        app.UseSwaggerUI(options =>
        {
            options.SwaggerEndpoint($"/swagger/{apiVersion}/swagger.json", apiVersion);
            options.DisplayRequestDuration();
            options.EnablePersistAuthorization();
        });

        return app;
    }
}
