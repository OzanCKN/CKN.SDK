using System;
using Azure.Messaging.ServiceBus;
using CKN.Sdk.Core.Events;
using CKN.Sdk.Messaging.ServiceBus;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceBusMessagingBuilderExtensions
{
    /// <summary>
    /// Configures the DI container to use Azure Service Bus as the messaging provider.
    /// </summary>
    public static IServiceCollection AddCknServiceBus(this IServiceCollection services, Action<ServiceBusOptions> configureOptions)
    {
        var options = new ServiceBusOptions();
        configureOptions(options);

        services.Configure(configureOptions);

        services.TryAddSingleton<ServiceBusClient>(sp =>
        {
            return new ServiceBusClient(options.ConnectionString);
        });

        services.TryAddSingleton<IEventBus, ServiceBusEventBus>();

        return services;
    }
}
