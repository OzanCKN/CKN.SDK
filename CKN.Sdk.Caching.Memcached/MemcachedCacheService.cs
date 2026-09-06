using System;
using System.Threading;
using System.Threading.Tasks;
using CKN.Sdk.Core.Caching;
using Enyim.Caching;

namespace CKN.Sdk.Caching.Memcached;

public class MemcachedCacheService : ICacheService
{
    private readonly IMemcachedClient _client;

    public MemcachedCacheService(IMemcachedClient client)
    {
        _client = client;
    }

    public async Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default)
    {
        var result = await _client.GetAsync<T>(key).ConfigureAwait(false);
        return result.Value;
    }

    public async Task SetAsync<T>(string key, T value, TimeSpan? expirationTime = null, CancellationToken cancellationToken = default)
    {
        if (expirationTime.HasValue)
        {
            await _client.SetAsync(key, value, (int)expirationTime.Value.TotalSeconds).ConfigureAwait(false);
        }
        else
        {
            await _client.SetAsync(key, value, 3600).ConfigureAwait(false);
        }
    }

    public Task RemoveAsync(string key, CancellationToken cancellationToken = default)
    {
        return _client.RemoveAsync(key);
    }
}
