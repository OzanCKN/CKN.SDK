namespace CKN.Sdk.Search;

public interface ISearchService<T> where T : class
{
    Task IndexAsync(string indexName, T document, CancellationToken cancellationToken = default);
    Task IndexManyAsync(string indexName, IEnumerable<T> documents, CancellationToken cancellationToken = default);
    Task<IEnumerable<T>> SearchAsync(string indexName, string query, CancellationToken cancellationToken = default);
    Task DeleteAsync(string indexName, string id, CancellationToken cancellationToken = default);
}
