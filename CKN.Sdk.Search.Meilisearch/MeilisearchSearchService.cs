using Meilisearch;

namespace CKN.Sdk.Search.Meilisearch;

public class MeilisearchSearchService<T> : ISearchService<T> where T : class
{
    private readonly MeilisearchClient _client;

    public MeilisearchSearchService(MeilisearchClient client)
    {
        _client = client;
    }

    public async Task IndexAsync(string indexName, T document, CancellationToken cancellationToken = default)
    {
        var index = _client.Index(indexName);
        var task = await index.AddDocumentsAsync(new[] { document }, cancellationToken: cancellationToken);
        // We could await the task completion if needed, but usually we just return the task id
    }

    public async Task IndexManyAsync(string indexName, IEnumerable<T> documents, CancellationToken cancellationToken = default)
    {
        var index = _client.Index(indexName);
        var task = await index.AddDocumentsAsync(documents, cancellationToken: cancellationToken);
    }

    public async Task<IEnumerable<T>> SearchAsync(string indexName, string query, CancellationToken cancellationToken = default)
    {
        var index = _client.Index(indexName);
        var result = await index.SearchAsync<T>(query, cancellationToken: cancellationToken);
        return result.Hits;
    }

    public async Task DeleteAsync(string indexName, string id, CancellationToken cancellationToken = default)
    {
        var index = _client.Index(indexName);
        await index.DeleteOneDocumentAsync(id, cancellationToken: cancellationToken);
    }
}
