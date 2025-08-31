using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Exceptions;

namespace Ims.DemoPlatform.Core.Configurations;

public static class LoggingConfiguration
{
    public static Action<HostBuilderContext, LoggerConfiguration> Configure =>
        (context, config) =>
        {
            config
                .Enrich.FromLogContext()
                .Enrich.WithExceptionDetails()
                .ReadFrom.Configuration(context.Configuration);
        };
}