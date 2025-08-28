using Ims.DemoPlatform.Core.MessageBus;
using Ims.DemoPlatform.NotificationService.Consumers;
using Ims.DemoPlatform.NotificationService.Options;
using Ims.DemoPlatform.NotificationService.Services;
using Microsoft.Extensions.Options;

var builder = Host.CreateApplicationBuilder(args);

// Options
builder.Services.Configure<RabbitMqOptions>(builder.Configuration.GetSection(nameof(RabbitMqOptions)));
builder.Services.AddSingleton(sp =>
{
    var opt = sp.GetRequiredService<IOptions<RabbitMqOptions>>().Value;
    return new MessageBus(opt);
});
builder.Services.AddSingleton<IMessageBus>(sp => sp.GetRequiredService<MessageBus>());


builder.Services.Configure<SmtpOptions>(builder.Configuration.GetSection("Smtp"));
builder.Services.AddSingleton<IEmailSender, SmtpEmailSender>();

// Worker
builder.Services.AddHostedService<AuthConsumer>();

var app = builder.Build();
app.Run();