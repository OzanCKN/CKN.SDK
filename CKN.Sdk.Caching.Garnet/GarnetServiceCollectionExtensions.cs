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

        // Garnet için özel bir CKN wrapper servisimiz varsa onu da ekleyebiliriz.
        // Ancak şu an için sadece Microsoft'un IDistributedCache'ini ekliyor.
        return services;
    }

    /// <summary>
    /// Configures the DI container to use Garnet as a keyed service.
    /// Allows connecting to multiple distinct Garnet servers in the same application.
    /// </summary>
    public static IServiceCollection AddCknKeyedGarnetCache(this IServiceCollection services, object serviceKey, Action<GarnetCacheOptions>? configureOptions = null)
    {
        var options = new GarnetCacheOptions();
        configureOptions?.Invoke(options);

        // Register the standard Microsoft IDistributedCache as a keyed singleton for Garnet
        services.AddKeyedSingleton<Microsoft.Extensions.Caching.Distributed.IDistributedCache>(serviceKey, (sp, key) =>
        {
            var msOptions = new Microsoft.Extensions.Caching.StackExchangeRedis.RedisCacheOptions
            {
                Configuration = options.Configuration,
                InstanceName = options.InstanceName
            };
            return new Microsoft.Extensions.Caching.StackExchangeRedis.RedisCache(Microsoft.Extensions.Options.Options.Create(msOptions));
        });

        return services;
    }
}
