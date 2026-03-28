using Ims.DemoPlatform.Core.MessageBus;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Ims.DemoPlatform.WebApi.Core.Extensions;

/// <summary>
/// Extension methods for configuring RabbitMQ Service Bus in API projects
/// </summary>
public static class ServiceBusExtensions
{
    /// <summary>
    /// Adds RabbitMQ Service Bus configuration reading from appsettings.json "RabbitMqOptions" section
    /// </summary>
    /// <param name="builder">The WebApplicationBuilder</param>
    /// <returns>The WebApplicationBuilder for chaining</returns>
    public static WebApplicationBuilder AddServiceBusConfiguration(this WebApplicationBuilder builder)
    {
        builder.Services.Configure<RabbitMqOptions>(
            builder.Configuration.GetSection(nameof(RabbitMqOptions)));

        builder.Services.AddSingleton<IMessageBus>(sp =>
        {
            var options = sp.GetRequiredService<IOptions<RabbitMqOptions>>().Value;
            return new MessageBus(options);
        });

        return builder;
    }
}
