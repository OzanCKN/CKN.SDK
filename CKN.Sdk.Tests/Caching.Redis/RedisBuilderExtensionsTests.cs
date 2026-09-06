using System;
using CKN.Sdk.Caching.Redis;
using CKN.Sdk.Core.Caching;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace CKN.Sdk.Tests.Caching.Redis;

public class RedisBuilderExtensionsTests
{
    [Fact]
    public void AddCknRedisCache_ShouldRegisterCacheService()
    {
        // Arrange
        var services = new ServiceCollection();
        
        // Act
        services.AddCknRedisCache(options =>
        {
            options.ConnectionString = "localhost:6379";
        });
        
        var serviceProvider = services.BuildServiceProvider();
        var cacheService = serviceProvider.GetService<ICacheService>();

        // Assert
        Assert.NotNull(cacheService);
        Assert.IsType<RedisCacheService>(cacheService);
    }
}
