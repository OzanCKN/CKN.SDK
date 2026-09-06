namespace CKN.Sdk.Caching.Memcached;

/// <summary>
/// Configuration options for the Memcached provider.
/// </summary>
public class MemcachedCacheOptions
{
    /// <summary>
    /// Gets or sets the Memcached server address.
    /// </summary>
    public string Server { get; set; } = "localhost";

    /// <summary>
    /// Gets or sets the Memcached server port.
    /// </summary>
    public int Port { get; set; } = 11211;
}
