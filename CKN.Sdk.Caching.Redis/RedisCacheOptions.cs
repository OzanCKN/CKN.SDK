namespace CKN.Sdk.Caching.Redis;

/// <summary>
/// Configuration options for the Redis cache provider.
/// </summary>
public class RedisCacheOptions
{
    /// <summary>
    /// Gets or sets the Redis connection string.
    /// </summary>
    public string ConnectionString { get; set; } = "localhost:6379";

    /// <summary>
    /// Gets or sets the Redis instance name (prefix for all keys).
    /// </summary>
    public string? InstanceName { get; set; }
}
