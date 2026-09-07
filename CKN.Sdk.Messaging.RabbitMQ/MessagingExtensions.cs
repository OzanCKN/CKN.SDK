using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using RabbitMQ.Client;
using System;
using CKN.Sdk.Core.Events;

namespace CKN.Sdk.Messaging.RabbitMQ;

/// <summary>
/// Extension methods for configuring RabbitMQ messaging.
/// </summary>
public static class MessagingExtensions
{
    /// <summary>
    /// Configures the application to use RabbitMQ for messaging.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configure">The action to configure the connection factory.</param>
    /// <param name="exchangeName">The default exchange name to use.</param>
    /// <returns>The service collection.</returns>
    public static IServiceCollection AddCknRabbitMQ(
        this IServiceCollection services,
        Action<ConnectionFactory> configure,
        string exchangeName = "ckn_events")
    {
        var factory = new ConnectionFactory();
        configure(factory);

        services.AddSingleton<IConnectionFactory>(factory);
        services.AddSingleton<IEventBus>(sp => new RabbitMQEventBus(
            sp.GetRequiredService<IConnectionFactory>(),
            sp.GetRequiredService<Microsoft.Extensions.Options.IOptions<RabbitMQOptions>>(),
            sp));

        return services;
    }
}
