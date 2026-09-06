using System;
using CKN.Sdk.Core.Events;
using CKN.Sdk.Messaging.Extensions;
using CKN.Sdk.Messaging.RabbitMQ;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using RabbitMQ.Client;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection;

public static class RabbitMQMessagingBuilderExtensions
{
    /// <summary>
    /// Configures the messaging bus to use RabbitMQ as the transport provider.
    /// </summary>
    public static ICknMessagingBuilder UseRabbitMQ(this ICknMessagingBuilder builder, Action<RabbitMQOptions>? configureOptions = null)
    {
        var options = new RabbitMQOptions();
        configureOptions?.Invoke(options);
        
        builder.Services.Configure<RabbitMQOptions>(opt => 
        {
            opt.HostName = options.HostName;
            opt.Port = options.Port;
            opt.UserName = options.UserName;
            opt.Password = options.Password;
            opt.QueueName = options.QueueName;
        });

        builder.Services.TryAddSingleton<IConnectionFactory>(sp => new ConnectionFactory
        {
            HostName = options.HostName,
            Port = options.Port,
            UserName = options.UserName,
            Password = options.Password,
            DispatchConsumersAsync = true
        });

        builder.Services.TryAddSingleton<IEventBus, RabbitMQEventBus>();

        return builder;
    }
}
