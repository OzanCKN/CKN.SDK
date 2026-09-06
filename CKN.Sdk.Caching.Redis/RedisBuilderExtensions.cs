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
        });

        services.TryAddScoped<ICacheService, RedisCacheService>();

        return services;
    }
}
