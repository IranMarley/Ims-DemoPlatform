using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace Ims.DemoPlatform.WebApi.Core.Middlewares;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly JsonSerializerOptions _json = new() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

    public ExceptionHandlingMiddleware(RequestDelegate next) => _next = next;

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            await HandleAsync(context, ex);
        }
    }

    private async Task HandleAsync(HttpContext ctx, Exception ex)
    {
        // Structured error log
        Log.ForContext("TraceId", ctx.TraceIdentifier)
           .ForContext("Path", ctx.Request.Path.Value)
           .Error(ex, "Unhandled exception");

        var env = ctx.RequestServices.GetRequiredService<IHostEnvironment>();
        var pd = BuildProblemDetails(ctx, ex, env.IsDevelopment(), out var status);

        ctx.Response.ContentType = "application/problem+json";
        ctx.Response.StatusCode = status;
        await ctx.Response.WriteAsync(JsonSerializer.Serialize(pd, _json));
    }

    private static ProblemDetails BuildProblemDetails(HttpContext ctx, Exception ex, bool isDev, out int status)
    {
        // Default → 500
        string title;
        string type = "about:blank";

        switch (ex)
        {
            // 400 Bad Request for argument issues
            case ArgumentNullException:
            case ArgumentException:
            case FormatException:
                status = StatusCodes.Status400BadRequest;
                title  = "Bad request";
                type   = "https://httpstatuses.com/400";
                break;

            // 401 Unauthorized for auth problems commonly represented by this exception
            case UnauthorizedAccessException:
                status = StatusCodes.Status401Unauthorized;
                title  = "Unauthorized";
                type   = "https://httpstatuses.com/401";
                break;

            // 404 Not Found for lookups that fail
            case KeyNotFoundException:
                status = StatusCodes.Status404NotFound;
                title  = "Resource not found";
                type   = "https://httpstatuses.com/404";
                break;

            // 409 Conflict for invalid state/operation conflicts
            case InvalidOperationException:
                status = StatusCodes.Status409Conflict;
                title  = "Conflict";
                type   = "https://httpstatuses.com/409";
                break;

            // 408 Request Timeout when request/operation was canceled (often client-aborted)
            case TaskCanceledException:
            case OperationCanceledException:
                status = StatusCodes.Status408RequestTimeout;
                title  = "Request timeout";
                type   = "https://httpstatuses.com/408";
                break;

            // Fallback → 500
            default:
                status = StatusCodes.Status500InternalServerError;
                title  = "Internal server error";
                type   = "https://httpstatuses.com/500";
                break;
        }

        return new ProblemDetails
        {
            Status = status,
            Title = title,
            Detail = isDev ? ex.Message : "An unexpected error occurred.",
            Instance = ctx.TraceIdentifier,
            Type = type
        };
    }
}
