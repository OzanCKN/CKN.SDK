using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using CKN.Sdk.Core.Caching;
using Microsoft.Extensions.Caching.Distributed;

namespace CKN.Sdk.Caching.Redis;

public class RedisCacheService : ICacheService
{
    private readonly IDistributedCache _cache;

    public RedisCacheService(IDistributedCache cache)
    {
        _cache = cache;
    }

    public async Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default)
    {
        var cached = await _cache.GetStringAsync(key, cancellationToken).ConfigureAwait(false);
        if (cached == null) return default;
        return JsonSerializer.Deserialize<T>(cached);
    }

    public async Task SetAsync<T>(string key, T value, TimeSpan? expirationTime = null, CancellationToken cancellationToken = default)
    {
        var options = new DistributedCacheEntryOptions();
        if (expirationTime.HasValue)
        {
            options.AbsoluteExpirationRelativeToNow = expirationTime.Value;
        }

        var serialized = JsonSerializer.Serialize(value);
        await _cache.SetStringAsync(key, serialized, options, cancellationToken).ConfigureAwait(false);
    }

    public Task RemoveAsync(string key, CancellationToken cancellationToken = default)
    {
        return _cache.RemoveAsync(key, cancellationToken);
    }
}
