using Microsoft.AspNetCore.Builder;
using Serilog;
using Serilog.Exceptions;
using Serilog.Sinks.Grafana.Loki;

namespace Ims.DemoPlatform.WebApi.Core.Extensions;

/// <summary>
/// Extension methods for configuring Serilog logging in API projects
/// </summary>
public static class SerilogExtensions
{
    /// <summary>
    /// Adds Serilog configuration to the WebApplicationBuilder with bootstrap logger
    /// Call this at the very beginning of your Program.cs
    /// </summary>
    /// <param name="builder">The WebApplicationBuilder</param>
    /// <param name="configureLogger">Optional additional logger configuration</param>
    /// <returns>The WebApplicationBuilder for chaining</returns>
    public static WebApplicationBuilder AddSerilogLogging(
        this WebApplicationBuilder builder,
        Action<LoggerConfiguration>? configureLogger = null)
    {
        builder.Host.UseSerilog((context, services, configuration) =>
        {
            configuration
                .ReadFrom.Configuration(context.Configuration)
                .ReadFrom.Services(services)
                .Enrich.FromLogContext()
                .Enrich.WithExceptionDetails()
                .WriteTo.Console();

            var lokiUrl = context.Configuration["LokiOptions:Url"];
            if (!string.IsNullOrEmpty(lokiUrl))
            {
                var appName = context.Configuration["Serilog:Properties:Application"] ?? "Unknown";
                configuration.WriteTo.GrafanaLoki(lokiUrl, labels:
                [
                    new LokiLabel { Key = "app", Value = appName },
                    new LokiLabel { Key = "environment", Value = context.HostingEnvironment.EnvironmentName }
                ]);
            }

            configureLogger?.Invoke(configuration);
        });

        return builder;
    }

    /// <summary>
    /// Creates a bootstrap logger for early application startup logging
    /// Use this before the host is built to capture early errors
    /// </summary>
    /// <param name="applicationName">Name of the application for logging context</param>
    /// <returns>The bootstrap logger configuration</returns>
    public static ILogger CreateBootstrapLogger(string applicationName)
    {
        return new LoggerConfiguration()
            .WriteTo.Console()
            .Enrich.WithProperty("Application", applicationName)
            .CreateBootstrapLogger();
    }

    /// <summary>
    /// Wraps the application run in proper Serilog exception handling
    /// Ensures all logs are flushed even if the application crashes
    /// </summary>
    /// <param name="action">The action to execute (typically app building and running)</param>
    /// <param name="applicationName">Name of the application for logging</param>
    public static void RunWithSerilog(Action action, string applicationName)
    {
        Log.Logger = CreateBootstrapLogger(applicationName);

        try
        {
            Log.Information("Starting {ApplicationName}", applicationName);
            action();
            Log.Information("{ApplicationName} stopped cleanly", applicationName);
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "{ApplicationName} failed to start or crashed", applicationName);
            throw;
        }
        finally
        {
            Log.CloseAndFlush();
        }
    }

    /// <summary>
    /// Async version of RunWithSerilog for applications with async startup
    /// </summary>
    public static async Task RunWithSerilogAsync(Func<Task> action, string applicationName)
    {
        Log.Logger = CreateBootstrapLogger(applicationName);

        try
        {
            Log.Information("Starting {ApplicationName}", applicationName);
            await action();
            Log.Information("{ApplicationName} stopped cleanly", applicationName);
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "{ApplicationName} failed to start or crashed", applicationName);
            throw;
        }
        finally
        {
            await Log.CloseAndFlushAsync();
        }
    }
}

