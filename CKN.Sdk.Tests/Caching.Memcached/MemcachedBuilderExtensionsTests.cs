using System;
using CKN.Sdk.Caching.Memcached;
using CKN.Sdk.Core.Caching;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace CKN.Sdk.Tests.Caching.Memcached;

public class MemcachedBuilderExtensionsTests
{
    [Fact]
    public void AddCknMemcached_ShouldRegisterCacheService()
    {
        // Arrange
        var services = new ServiceCollection();
        
        // Enyim requires ILoggerFactory and other basics, let's provide logging
        services.AddLogging();
        
        // Act
        services.AddCknMemcached(options =>
        {
            options.Server = "localhost";
            options.Port = 11211;
        });
        
        var serviceProvider = services.BuildServiceProvider();
        var cacheService = serviceProvider.GetService<ICacheService>();

        // Assert
        Assert.NotNull(cacheService);
        Assert.IsType<MemcachedCacheService>(cacheService);
    }
}
