using System;
using CKN.Sdk.Messaging.Extensions;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection;

public static class CknMessagingServiceCollectionExtensions
{
    /// <summary>
    /// Adds CKN Messaging support to the service collection.
    /// Call specific provider extensions like .UseRabbitMQ() on the builder.
    /// </summary>
    public static IServiceCollection AddCknMessaging(this IServiceCollection services, Action<ICknMessagingBuilder> configure)
    {
        var builder = new CknMessagingBuilder(services);
        configure(builder);
        return services;
    }
}
