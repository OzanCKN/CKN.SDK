using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using CKN.Sdk.Core.Caching;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace CKN.Sdk.Infrastructure.Caching;

/// <summary>
/// A hybrid cache service that attempts to use IDistributedCache (e.g. Redis), 
/// but gracefully falls back to IMemoryCache (RAM) if the distributed cache is unavailable.
/// </summary>
public class HybridCacheService(
    IDistributedCache distributedCache,
    IMemoryCache memoryCache,
    ILogger<HybridCacheService> logger) : ICacheService
{
    public async Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default)
    {
        try
        {
            var json = await distributedCache.GetStringAsync(key, cancellationToken);
            if (!string.IsNullOrEmpty(json))
            {
                return JsonSerializer.Deserialize<T>(json);
            }
        }
        catch (Exception ex)
        {
            // Graceful Degradation: Fallback to in-memory if Redis is down
            logger.LogWarning(ex, "Distributed cache failed. Falling back to MemoryCache for key: {Key}", key);
        }

        return memoryCache.TryGetValue(key, out T? memoryValue) ? memoryValue : default;
    }

    public async Task SetAsync<T>(string key, T value, TimeSpan? expirationTime = null, CancellationToken cancellationToken = default)
    {
        try
        {
            var options = new DistributedCacheEntryOptions();
            if (expirationTime.HasValue)
            {
                options.AbsoluteExpirationRelativeToNow = expirationTime.Value;
            }
            
            var json = JsonSerializer.Serialize(value);
            await distributedCache.SetStringAsync(key, json, options, cancellationToken);
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Distributed cache failed. Storing in MemoryCache for key: {Key}", key);
            
            var options = new MemoryCacheEntryOptions();
            if (expirationTime.HasValue) options.SetAbsoluteExpiration(expirationTime.Value);
            memoryCache.Set(key, value, options);
        }
    }

    public async Task RemoveAsync(string key, CancellationToken cancellationToken = default)
    {
        try
        {
            await distributedCache.RemoveAsync(key, cancellationToken);
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Distributed cache failed on Remove. Removing from MemoryCache for key: {Key}", key);
        }
        
        memoryCache.Remove(key);
    }
}
