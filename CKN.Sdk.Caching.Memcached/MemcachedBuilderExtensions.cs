using System;
using CKN.Sdk.Caching.Memcached;
using CKN.Sdk.Core.Caching;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection;

public static class MemcachedBuilderExtensions
{
    /// <summary>
    /// Configures the DI container to use Memcached for caching.
    /// </summary>
    public static IServiceCollection AddCknMemcached(this IServiceCollection services, Action<MemcachedCacheOptions> configureOptions)
    {
        var options = new MemcachedCacheOptions();
        configureOptions(options);

        services.AddEnyimMemcached(o =>
        {
            o.Servers.Add(new Enyim.Caching.Configuration.Server { Address = options.Server, Port = options.Port });
        });

        services.TryAddScoped<ICacheService, MemcachedCacheService>();

        return services;
    }
}
