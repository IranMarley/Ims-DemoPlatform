using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Ims.DemoPlatform.WebApi.Core.Extensions;

/// <summary>
/// Extension methods for basic API configuration
/// </summary>
public static class ApiExtensions
{
    /// <summary>
    /// Adds basic API services (Controllers)
    /// </summary>
    /// <param name="builder">The WebApplicationBuilder</param>
    /// <returns>The WebApplicationBuilder for chaining</returns>
    public static WebApplicationBuilder AddApiServices(this WebApplicationBuilder builder)
    {
        builder.Services.AddControllers();
        return builder;
    }
}

