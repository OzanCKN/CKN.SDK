using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;
using CKN.Sdk.Infrastructure.Caching;

namespace CKN.Sdk.Tests.Infrastructure;

public class HybridCacheServiceTests
{
    private readonly MemoryDistributedCache _redisCache;
    private readonly IMemoryCache _memoryCache;
    private readonly HybridCacheService _sut;

    public HybridCacheServiceTests()
    {
        _redisCache = new MemoryDistributedCache(Options.Create(new MemoryDistributedCacheOptions()));
        _memoryCache = new MemoryCache(new MemoryCacheOptions());
        var loggerMock = new Mock<ILogger<HybridCacheService>>();
        _sut = new HybridCacheService(_redisCache, _memoryCache, loggerMock.Object);
    }

    [Fact]
    public async Task GetAsync_WhenValueExistsInRedis_ShouldReturnFromRedis()
    {
        // Arrange
        var key = "test-key";
        var expectedValue = new TestData { Id = 2, Name = "RedisData" };
        var json = JsonSerializer.Serialize(expectedValue);
        await _redisCache.SetStringAsync(key, json);

        // Act
        var result = await _sut.GetAsync<TestData>(key);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(expectedValue.Id);
        result.Name.Should().Be("RedisData");
    }

    [Fact]
    public async Task GetAsync_WhenRedisFailsOrIsEmpty_AndValueInMemory_ShouldReturnFromMemory()
    {
        // Arrange
        var key = "test-key-mem";
        var expectedValue = new TestData { Id = 1, Name = "CKN" };
        
        // Value only in memory, not in Redis
        _memoryCache.Set(key, expectedValue);

        // Act
        var result = await _sut.GetAsync<TestData>(key);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(expectedValue.Id);
        result.Name.Should().Be("CKN");
    }
}

public class TestData
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
}
