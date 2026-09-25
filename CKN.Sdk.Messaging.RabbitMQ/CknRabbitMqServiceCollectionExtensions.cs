using System;
using CKN.Sdk.Messaging.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Client;

namespace CKN.Sdk.Messaging.RabbitMQ;

public static class CknRabbitMqServiceCollectionExtensions
{
    public static IServiceCollection AddCknRabbitMqMessaging(this IServiceCollection services, Action<RabbitMQOptions> configureOptions)
    {
        var options = new RabbitMQOptions();
        configureOptions(options);

        services.AddSingleton<IConnection>(sp =>
        {
            var factory = new ConnectionFactory
            {
                HostName = options.HostName,
                UserName = options.UserName,
                Password = options.Password,
                VirtualHost = options.VirtualHost
            };

            return factory.CreateConnectionAsync().GetAwaiter().GetResult();
        });

        services.AddSingleton<ICknMessagePublisher, RabbitMqPublisher>();

        return services;
    }

    public static IServiceCollection AddCknRabbitMqConsumer<THandler, TMessage>(this IServiceCollection services, string queueName)
        where THandler : class, ICknMessageConsumer<TMessage>
    {
        services.AddScoped<THandler>();
        services.AddHostedService(sp => 
            ActivatorUtilities.CreateInstance<RabbitMqConsumerService<THandler, TMessage>>(sp, queueName));

        return services;
    }
}
