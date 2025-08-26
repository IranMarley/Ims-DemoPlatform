using Ims.DemoPlatform.Core.MessageBus;
using Microsoft.Extensions.Options;

namespace Ims.DemoPlatform.Identity.API.Configuration;

public static class ServiceBusConfig
{
    public static IServiceCollection AddServiceBusConfiguration(this WebApplicationBuilder builder)
    {
        builder.Services.Configure<RabbitMqOptions>(builder.Configuration.GetSection(nameof(RabbitMqOptions)));
        builder.Services.AddSingleton(sp =>
        {
            var opt = sp.GetRequiredService<IOptions<RabbitMqOptions>>().Value;
            return new MessageBus(opt);
        });
        builder.Services.AddSingleton<IMessageBus>(sp => sp.GetRequiredService<MessageBus>());
        
        return builder.Services;
    }
}