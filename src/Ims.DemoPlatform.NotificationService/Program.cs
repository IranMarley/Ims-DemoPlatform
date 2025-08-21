using EmailService.Consumers;
using EmailService.Options;
using EmailService.Services;

var builder = Host.CreateApplicationBuilder(args);

// Options
builder.Services.Configure<RabbitOptions>(builder.Configuration.GetSection("RabbitMQ"));
builder.Services.Configure<EmailOptions>(builder.Configuration.GetSection("Email"));
builder.Services.Configure<SmtpOptions>(builder.Configuration.GetSection("Smtp"));

// Email provider clean
var provider = builder.Configuration.GetValue<string>("Email:Provider") ?? "Console";
switch (provider.ToLowerInvariant())
{
    case "smtp":
        builder.Services.AddSingleton<IEmailSender, SmtpEmailSender>();
        break;
    default:
        builder.Services.AddSingleton<IEmailSender, ConsoleEmailSender>();
        break;
}

// Worker
builder.Services.AddHostedService<RabbitConsumerService>();

var app = builder.Build();
app.Run();