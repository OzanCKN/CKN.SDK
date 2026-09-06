using Elastic.Clients.Elasticsearch;

namespace CKN.Sdk.Search.Elasticsearch;

public class ElasticsearchSearchService<T> : ISearchService<T> where T : class
{
    private readonly ElasticsearchClient _client;

    public ElasticsearchSearchService(ElasticsearchClient client)
    {
        _client = client;
    }

    public async Task IndexAsync(string indexName, T document, CancellationToken cancellationToken = default)
    {
        var response = await _client.IndexAsync(document, d => d.Index((IndexName)indexName), cancellationToken);
        if (!response.IsSuccess())
        {
            throw new Exception($"Failed to index document: {response.DebugInformation}");
        }
    }

    public async Task IndexManyAsync(string indexName, IEnumerable<T> documents, CancellationToken cancellationToken = default)
    {
        var response = await _client.BulkAsync(b => b
            .Index(indexName)
            .IndexMany(documents), cancellationToken);

        if (!response.IsSuccess())
        {
            throw new Exception($"Failed to index multiple documents: {response.DebugInformation}");
        }
    }

    public async Task<IEnumerable<T>> SearchAsync(string indexName, string query, CancellationToken cancellationToken = default)
    {
        var response = await _client.SearchAsync<T>(s => s
            .Indices(indexName)
            .Query(q => q
                .QueryString(qs => qs
                    .Query(query)
                )
            ), cancellationToken);

        if (!response.IsSuccess())
        {
            throw new Exception($"Search failed: {response.DebugInformation}");
        }

        return response.Documents;
    }

    public async Task DeleteAsync(string indexName, string id, CancellationToken cancellationToken = default)
    {
        var response = await _client.DeleteAsync<T>(id, d => d.Index((IndexName)indexName), cancellationToken);
        if (!response.IsSuccess())
        {
            throw new Exception($"Failed to delete document {id}: {response.DebugInformation}");
        }
    }
}
