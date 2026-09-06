using CKN.Sdk.Search;
using CKN.Sdk.Search.Meilisearch;
using FluentAssertions;
using Meilisearch;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace CKN.Sdk.Tests.Search.Meilisearch;

public class MeilisearchBuilderExtensionsTests
{
    private class DummyDocument
    {
        public string Id { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
    }

    [Fact]
    public void AddCknMeilisearch_ShouldRegisterMeilisearchClient_And_ISearchService()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddCknMeilisearch(options =>
        {
            options.Url = "http://localhost:7700";
            options.ApiKey = "masterKey";
        });
        
        var provider = services.BuildServiceProvider();

        // Assert
        var options = provider.GetRequiredService<IOptions<MeilisearchOptions>>().Value;
        options.Url.Should().Be("http://localhost:7700");
        options.ApiKey.Should().Be("masterKey");

        var client = provider.GetRequiredService<MeilisearchClient>();
        client.Should().NotBeNull();

        var searchService = provider.GetRequiredService<ISearchService<DummyDocument>>();
        searchService.Should().NotBeNull();
        searchService.Should().BeOfType<MeilisearchSearchService<DummyDocument>>();
    }
}
