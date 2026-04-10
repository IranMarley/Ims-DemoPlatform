using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Ims.DemoPlatform.WebApi.Core.Extensions;

/// <summary>
/// Extension methods for configuring CORS in API projects
/// </summary>
public static class CorsExtensions
{
    /// <summary>
    /// Adds CORS configuration reading from appsettings.json "Cors:AllowedOrigins" section
    /// </summary>
    /// <param name="builder">The WebApplicationBuilder</param>
    /// <param name="policyName">Optional policy name (default is "DefaultPolicy")</param>
    /// <returns>The WebApplicationBuilder for chaining</returns>
    public static WebApplicationBuilder AddCorsConfiguration(
        this WebApplicationBuilder builder,
        string policyName = "DefaultPolicy")
    {
        var allowedOrigins = builder.Configuration
            .GetSection("Cors:AllowedOrigins")
            .Get<string[]>() ?? Array.Empty<string>();

        builder.Services.AddCors(options =>
        {
            options.AddPolicy(policyName, policy =>
            {
                if (allowedOrigins.Length > 0)
                {
                    policy.WithOrigins(allowedOrigins)
                          .AllowAnyHeader()
                          .AllowAnyMethod()
                          .AllowCredentials();
                }
                else
                {
                    throw new InvalidOperationException(
                        "CORS origins not configured. Set 'Cors:AllowedOrigins' in appsettings.json or environment variables.");
                }
            });
        });

        return builder;
    }

    /// <summary>
    /// Adds CORS with custom configuration
    /// </summary>
    /// <param name="builder">The WebApplicationBuilder</param>
    /// <param name="configure">Action to configure CORS policies</param>
    /// <returns>The WebApplicationBuilder for chaining</returns>
    public static WebApplicationBuilder AddCorsConfiguration(
        this WebApplicationBuilder builder,
        Action<Microsoft.AspNetCore.Cors.Infrastructure.CorsOptions> configure)
    {
        builder.Services.AddCors(configure);
        return builder;
    }

    /// <summary>
    /// Uses the CORS policy configured with AddCorsConfiguration
    /// </summary>
    /// <param name="app">The WebApplication</param>
    /// <param name="policyName">Optional policy name (default is "DefaultPolicy")</param>
    /// <returns>The WebApplication for chaining</returns>
    public static WebApplication UseCorsPolicy(
        this WebApplication app,
        string policyName = "DefaultPolicy")
    {
        app.UseCors(policyName);
        return app;
    }
}

