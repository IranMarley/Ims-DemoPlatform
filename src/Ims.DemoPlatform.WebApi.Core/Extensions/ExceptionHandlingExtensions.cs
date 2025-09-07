using Ims.DemoPlatform.WebApi.Core.Handlers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Ims.DemoPlatform.WebApi.Core.Extensions;

public static class ExceptionHandlingExtensions
{
    public static void AddGlobalExceptionHandling(this WebApplicationBuilder builder)
    {
        builder.Services.AddProblemDetails(options =>
        {
            options.CustomizeProblemDetails = ctx =>
            {
                var env = ctx.HttpContext.RequestServices.GetRequiredService<IHostEnvironment>();
                var ex  = ctx.Exception;

                if (ex is not null)
                {
                    var (status, title, type) = ex switch
                    {
                        ArgumentNullException or ArgumentException or FormatException or BadHttpRequestException
                            => (StatusCodes.Status400BadRequest, "Bad request", "https://httpstatuses.com/400"),

                        UnauthorizedAccessException
                            => (StatusCodes.Status401Unauthorized, "Unauthorized", "https://httpstatuses.com/401"),

                        KeyNotFoundException or FileNotFoundException
                            => (StatusCodes.Status404NotFound, "Resource not found", "https://httpstatuses.com/404"),

                        InvalidOperationException
                            => (StatusCodes.Status409Conflict, "Conflict", "https://httpstatuses.com/409"),

                        NotSupportedException
                            => (StatusCodes.Status415UnsupportedMediaType, "Unsupported media type", "https://httpstatuses.com/415"),

                        OperationCanceledException or TaskCanceledException
                            => (StatusCodes.Status408RequestTimeout, "Request timeout", "https://httpstatuses.com/408"),

                        _ => (StatusCodes.Status500InternalServerError, "Internal server error", "https://httpstatuses.com/500")
                    };

                    ctx.ProblemDetails.Status = status;
                    ctx.ProblemDetails.Title  = title;
                    ctx.ProblemDetails.Type   = type;
                    ctx.ProblemDetails.Detail = env.IsDevelopment()
                        ? (ctx.ProblemDetails.Detail ?? ex.Message)
                        : "An unexpected error occurred.";
                }

                ctx.ProblemDetails.Instance = ctx.HttpContext.Request.Path;
                ctx.ProblemDetails.Extensions ??= new Dictionary<string, object?>();
                ctx.ProblemDetails.Extensions["traceId"] = ctx.HttpContext.TraceIdentifier;
            };
        });

        builder.Services.AddExceptionHandler<LoggingExceptionHandler>();
    }
}
