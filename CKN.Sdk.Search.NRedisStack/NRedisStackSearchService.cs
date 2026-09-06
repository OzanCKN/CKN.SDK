using System.Text.Json;
using NRedisStack;
using NRedisStack.RedisStackCommands;
using StackExchange.Redis;

namespace CKN.Sdk.Search.NRedisStack;

public class NRedisStackSearchService<T> : ISearchService<T> where T : class
{
    private readonly IConnectionMultiplexer _connectionMultiplexer;

    public NRedisStackSearchService(IConnectionMultiplexer connectionMultiplexer)
    {
        _connectionMultiplexer = connectionMultiplexer;
    }

    public async Task IndexAsync(string indexName, T document, CancellationToken cancellationToken = default)
    {
        var db = _connectionMultiplexer.GetDatabase();
        var ft = db.FT();
        
        var id = typeof(T).GetProperty("Id")?.GetValue(document)?.ToString() ?? Guid.NewGuid().ToString();
        var dict = JsonSerializer.Deserialize<Dictionary<string, string>>(JsonSerializer.Serialize(document));
        
        var hashFields = dict?.Select(kv => new HashEntry(kv.Key, kv.Value)).ToArray();
        
        if (hashFields != null && hashFields.Length > 0)
        {
            await db.HashSetAsync($"{indexName}:{id}", hashFields);
        }
    }

    public async Task IndexManyAsync(string indexName, IEnumerable<T> documents, CancellationToken cancellationToken = default)
    {
        foreach (var doc in documents)
        {
            await IndexAsync(indexName, doc, cancellationToken);
        }
    }

    public async Task<IEnumerable<T>> SearchAsync(string indexName, string query, CancellationToken cancellationToken = default)
    {
        var db = _connectionMultiplexer.GetDatabase();
        var ft = db.FT();

        var result = await ft.SearchAsync(indexName, new global::NRedisStack.Search.Query(query));
        var docs = new List<T>();

        foreach (var doc in result.Documents)
        {
            var dict = doc.GetProperties().ToDictionary(k => k.Key, v => v.Value.ToString());
            var json = JsonSerializer.Serialize(dict);
            var item = JsonSerializer.Deserialize<T>(json);
            if (item != null) docs.Add(item);
        }

        return docs;
    }

    public async Task DeleteAsync(string indexName, string id, CancellationToken cancellationToken = default)
    {
        var db = _connectionMultiplexer.GetDatabase();
        await db.KeyDeleteAsync($"{indexName}:{id}");
    }
}
