using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Events;

namespace Ims.DemoPlatform.Core.Extensions;

public static class LoggingExtensions
{
   public static IHostBuilder UseGlobalSerilog(this IHostBuilder hostBuilder)
        => hostBuilder.UseSerilog((ctx, services, loggerConfig) =>
        {
            ConfigureSerilog(loggerConfig, ctx.Configuration);
        }, writeToProviders: false);

    public static IApplicationBuilder UseGlobalSerilogRequestLogging(this IApplicationBuilder app)
    {
        return app.UseSerilogRequestLogging(opts =>
        {
            opts.MessageTemplate = "HTTP {RequestMethod} {RequestPath} responded {StatusCode} in {Elapsed:0.0000} ms";
        });
    }

    private static void ConfigureSerilog(LoggerConfiguration loggerConfig, IConfiguration configuration)
    {
        // Default configuration
        loggerConfig
            .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
            .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
            .MinimumLevel.Information()
            .Enrich.FromLogContext()
            .Enrich.WithEnvironmentName()
            .Enrich.WithProcessId()
            .Enrich.WithThreadId()
            .Enrich.WithProperty("Application", configuration["ApplicationName"] ?? AppDomain.CurrentDomain.FriendlyName)
            .WriteTo.Console()
            .WriteTo.File(
                path: "logs/log-.txt",
                rollingInterval: RollingInterval.Day,
                retainedFileCountLimit: 14,
                outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss} [{Level:u3}] {Message:lj} {Properties:j}{NewLine}{Exception}"
            );

        var section = configuration.GetSection("Serilog");
        if (section.Exists())
        {
            loggerConfig.ReadFrom.Configuration(configuration);
        }
    }
}
