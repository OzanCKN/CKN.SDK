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
}
