using MassTransit;
using Microsoft.Extensions.DependencyInjection;

namespace CKN.Sdk.MassTransit.Messaging;

/// <summary>
/// Provides extension methods for configuring MassTransit across CKN products.
/// </summary>
public static class MassTransitConfigurationExtensions
{
    /// <summary>
    /// Configures MassTransit with RabbitMQ and the Entity Framework Core Transactional Outbox.
    /// Ensures reliable event delivery even if the message broker is temporarily down.
    /// </summary>
    /// <typeparam name="TDbContext">The type of the Application DbContext.</typeparam>
    /// <param name="services">The service collection.</param>
    /// <param name="rabbitMqConnectionString">The connection string for RabbitMQ.</param>
    /// <param name="configureConsumers">Optional action to configure consumers.</param>
    public static IServiceCollection AddCKNMessaging<TDbContext>(
        this IServiceCollection services,
        string rabbitMqConnectionString,
        System.Action<IBusRegistrationConfigurator>? configureConsumers = null)
        where TDbContext : Microsoft.EntityFrameworkCore.DbContext
    {
        services.AddMassTransit(x =>
        {
            // Register consumers if provided
            configureConsumers?.Invoke(x);

            // 1. Transactional Outbox Configuration
            x.AddEntityFrameworkOutbox<TDbContext>(o =>
            {
                o.UsePostgres();
                o.UseBusOutbox();
            });



            // 3. RabbitMQ Configuration
            x.UsingRabbitMq((context, cfg) =>
            {
                cfg.Host(rabbitMqConnectionString);
                cfg.ConfigureEndpoints(context);
            });
        });

        return services;
    }
}
