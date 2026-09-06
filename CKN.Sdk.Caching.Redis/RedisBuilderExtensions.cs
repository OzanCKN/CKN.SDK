using System;
using CKN.Sdk.Caching.Redis;
using CKN.Sdk.Core.Caching;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection;

public static class RedisBuilderExtensions
{
    /// <summary>
    /// Configures the DI container to use Redis for caching.
    /// </summary>
    public static IServiceCollection AddCknRedisCache(this IServiceCollection services, Action<RedisCacheOptions> configureOptions)
    {
        var options = new RedisCacheOptions();
        configureOptions(options);

        services.AddStackExchangeRedisCache(redisOptions =>
        {
            redisOptions.Configuration = options.ConnectionString;
            redisOptions.InstanceName = options.InstanceName;
        });

        services.TryAddScoped<ICacheService, RedisCacheService>();

        return services;
    }

    /// <summary>
    /// Configures the DI container to use Redis for caching as a keyed service.
    /// Allows connecting to multiple distinct Redis servers in the same application.
    /// </summary>
    public static IServiceCollection AddCknKeyedRedisCache(this IServiceCollection services, object serviceKey, Action<RedisCacheOptions> configureOptions)
    {
        var options = new RedisCacheOptions();
        configureOptions(options);

        // Register the standard Microsoft IDistributedCache as a keyed singleton
        services.AddKeyedSingleton<Microsoft.Extensions.Caching.Distributed.IDistributedCache>(serviceKey, (sp, key) =>
        {
            var msOptions = new Microsoft.Extensions.Caching.StackExchangeRedis.RedisCacheOptions
            {
                Configuration = options.ConnectionString,
                InstanceName = options.InstanceName
            };
            return new Microsoft.Extensions.Caching.StackExchangeRedis.RedisCache(Microsoft.Extensions.Options.Options.Create(msOptions));
        });

        // Register our custom ICacheService wrapper as a keyed scoped service
        services.AddKeyedScoped<ICacheService>(serviceKey, (sp, key) =>
        {
            var distributedCache = sp.GetRequiredKeyedService<Microsoft.Extensions.Caching.Distributed.IDistributedCache>(key);
            return new RedisCacheService(distributedCache);
        });

        return services;
    }
}
