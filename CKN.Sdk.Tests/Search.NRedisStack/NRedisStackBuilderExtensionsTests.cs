using CKN.Sdk.Search;
using CKN.Sdk.Search.NRedisStack;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using StackExchange.Redis;

namespace CKN.Sdk.Tests.Search.NRedisStack;

public class NRedisStackBuilderExtensionsTests
{
    private class DummyDocument
    {
        public string Id { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
    }

    [Fact]
    public void AddCknNRedisStack_ShouldRegisterConnectionMultiplexer_And_ISearchService()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddCknNRedisStack(options =>
        {
            options.Configuration = "localhost:6379";
        });
        
        var provider = services.BuildServiceProvider();

        // Assert
        var options = provider.GetRequiredService<IOptions<NRedisStackOptions>>().Value;
        options.Configuration.Should().Be("localhost:6379");

        // Note: we can't assert IConnectionMultiplexer easily without a real redis instance,
        // it throws if it can't connect synchronously. But we can assert ISearchService is registered.
        // Actually, NRedisStackBuilderExtensions uses ConnectionMultiplexer.Connect synchronously.
        // We'll just verify the service descriptor exists to avoid test failures without Redis running.
        
        var descriptor = services.FirstOrDefault(d => d.ServiceType == typeof(IConnectionMultiplexer));
        descriptor.Should().NotBeNull();
        
        var searchDescriptor = services.FirstOrDefault(d => d.ServiceType == typeof(ISearchService<>));
        searchDescriptor.Should().NotBeNull();
        searchDescriptor!.ImplementationType.Should().Be(typeof(NRedisStackSearchService<>));
    }
}
