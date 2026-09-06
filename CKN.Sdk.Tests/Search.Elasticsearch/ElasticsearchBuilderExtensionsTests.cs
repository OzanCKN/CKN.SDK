using CKN.Sdk.Search;
using CKN.Sdk.Search.Elasticsearch;
using Elastic.Clients.Elasticsearch;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace CKN.Sdk.Tests.Search.Elasticsearch;

public class ElasticsearchBuilderExtensionsTests
{
    private class DummyDocument
    {
        public string Id { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
    }

    [Fact]
    public void AddCknElasticsearch_ShouldRegisterElasticsearchClient_And_ISearchService()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddCknElasticsearch(options =>
        {
            options.Url = "http://localhost:9200";
            options.ApiKey = "test-api-key";
        });
        
        var provider = services.BuildServiceProvider();

        // Assert
        var options = provider.GetRequiredService<IOptions<ElasticsearchOptions>>().Value;
        options.Url.Should().Be("http://localhost:9200");
        options.ApiKey.Should().Be("test-api-key");

        var client = provider.GetRequiredService<ElasticsearchClient>();
        client.Should().NotBeNull();

        var searchService = provider.GetRequiredService<ISearchService<DummyDocument>>();
        searchService.Should().NotBeNull();
        searchService.Should().BeOfType<ElasticsearchSearchService<DummyDocument>>();
    }
}
