using Ims.DemoPlatform.Core.MessageBus;
using Ims.DemoPlatform.NotificationsService.Consumers;
using Ims.DemoPlatform.NotificationsService.Options;
using Ims.DemoPlatform.NotificationsService.Services;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Serilog;
using Serilog.Exceptions;
using Serilog.Sinks.Grafana.Loki;

var builder = Host.CreateApplicationBuilder(args);

var loggerConfig = new LoggerConfiguration()
    .Enrich.FromLogContext()
    .Enrich.WithExceptionDetails()
    .ReadFrom.Configuration(builder.Configuration)
    .WriteTo.Console();

var lokiUrl = builder.Configuration["LokiOptions:Url"];
if (!string.IsNullOrEmpty(lokiUrl))
{
    var appName = builder.Configuration["Serilog:Properties:Application"] ?? "NotificationService";
    var env = builder.Environment.EnvironmentName;
    loggerConfig.WriteTo.GrafanaLoki(lokiUrl, labels:
    [
        new LokiLabel { Key = "app", Value = appName },
        new LokiLabel { Key = "environment", Value = env }
    ]);
}

Log.Logger = loggerConfig.CreateLogger();

builder.Logging.ClearProviders();
builder.Logging.AddSerilog(Log.Logger, dispose: true);

// Options
builder.Services.Configure<ClientOptions>(builder.Configuration.GetSection(nameof(ClientOptions)));
builder.Services.Configure<SmtpOptions>(builder.Configuration.GetSection(nameof(SmtpOptions)));
builder.Services.Configure<RabbitMqOptions>(builder.Configuration.GetSection(nameof(RabbitMqOptions)));
builder.Services.AddSingleton(sp =>
{
    var opt = sp.GetRequiredService<IOptions<RabbitMqOptions>>().Value;
    var logger = sp.GetRequiredService<ILogger<MessageBus>>();
    return new MessageBus(opt, logger);
});

builder.Services.AddSingleton<IMessageBus>(sp => sp.GetRequiredService<MessageBus>());
builder.Services.AddSingleton<IEmailSender, SmtpEmailSender>();

// Worker
builder.Services.AddHostedService<AuthConsumer>();

var app = builder.Build();
app.Run();