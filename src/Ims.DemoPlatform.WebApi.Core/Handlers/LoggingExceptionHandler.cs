using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Serilog;

namespace Ims.DemoPlatform.WebApi.Core.Handlers;

public sealed class LoggingExceptionHandler : IExceptionHandler
{
    private readonly ILogger<LoggingExceptionHandler> _logger;
    public LoggingExceptionHandler(ILogger<LoggingExceptionHandler> logger) => _logger = logger;

    public ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        if (exception is OperationCanceledException or TaskCanceledException)
            _logger.LogInformation(exception, "Request canceled: {Path}", httpContext.Request.Path);
        else
        {
            Log.ForContext("TraceId", httpContext.TraceIdentifier)
                .ForContext("Path", httpContext.Request.Path.Value)
                .Error(httpContext.Request.Path, "Unhandled exception");
        }
        
        // false => let the default ProblemDetails pipeline produce the response
        return ValueTask.FromResult(false);
    }
}