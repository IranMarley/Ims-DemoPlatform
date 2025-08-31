using Ims.DemoPlatform.Core.MessageBus;
using Ims.DemoPlatform.NotificationService.Consumers;
using Ims.DemoPlatform.NotificationService.Options;
using Ims.DemoPlatform.NotificationService.Services;
using Microsoft.Extensions.Options;
using Serilog;
using Serilog.Exceptions;

var builder = Host.CreateApplicationBuilder(args);

Log.Logger = new LoggerConfiguration()
    .Enrich.FromLogContext()
    .Enrich.WithExceptionDetails()
    .ReadFrom.Configuration(builder.Configuration)
    .WriteTo.Console()
    .CreateLogger();

builder.Logging.ClearProviders();
builder.Logging.AddSerilog(Log.Logger, dispose: true);

// Options
builder.Services.Configure<ClientOptions>(builder.Configuration.GetSection(nameof(ClientOptions)));
builder.Services.Configure<SmtpOptions>(builder.Configuration.GetSection(nameof(SmtpOptions)));
builder.Services.Configure<RabbitMqOptions>(builder.Configuration.GetSection(nameof(RabbitMqOptions)));
builder.Services.AddSingleton(sp =>
{
    var opt = sp.GetRequiredService<IOptions<RabbitMqOptions>>().Value;
    return new MessageBus(opt);
});

builder.Services.AddSingleton<IMessageBus>(sp => sp.GetRequiredService<MessageBus>());
builder.Services.AddSingleton<IEmailSender, SmtpEmailSender>();

// Worker
builder.Services.AddHostedService<AuthConsumer>();

var app = builder.Build();
app.Run();