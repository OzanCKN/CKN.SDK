using System;
using CKN.Sdk.Caching.Garnet;
using Microsoft.Extensions.DependencyInjection;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection;

public static class GarnetServiceCollectionExtensions
{
    /// <summary>
    /// Configures the DI container to use Microsoft Garnet as the distributed cache provider.
    /// Note: Garnet is fully compatible with the Redis RESP protocol, thus we utilize StackExchange.Redis under the hood.
    /// </summary>
    public static IServiceCollection AddCknGarnetCache(this IServiceCollection services, Action<GarnetCacheOptions>? configureOptions = null)
    {
        var options = new GarnetCacheOptions();
        configureOptions?.Invoke(options);

        services.AddStackExchangeRedisCache(redisOptions =>
        {
            redisOptions.Configuration = options.Configuration;
            redisOptions.InstanceName = options.InstanceName;
        });

        return services;
    }
}
