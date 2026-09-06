namespace CKN.Sdk.Caching.Garnet;

/// <summary>
/// Configuration options for Microsoft Garnet cache provider.
/// </summary>
public class GarnetCacheOptions
{
    /// <summary>
    /// Gets or sets the configuration string for connecting to the Garnet server.
    /// Example: "localhost:3278"
    /// </summary>
    public string Configuration { get; set; } = "localhost:3278";

    /// <summary>
    /// Gets or sets the instance name.
    /// </summary>
    public string InstanceName { get; set; } = "ckn_garnet_";
}
