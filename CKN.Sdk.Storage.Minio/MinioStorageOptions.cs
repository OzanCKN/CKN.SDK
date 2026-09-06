namespace CKN.Sdk.Storage.Minio;

/// <summary>
/// Configuration options for the Minio storage provider.
/// </summary>
public class MinioStorageOptions
{
    /// <summary>
    /// Gets or sets the endpoint URL (e.g., "play.min.io").
    /// </summary>
    public string Endpoint { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the access key.
    /// </summary>
    public string AccessKey { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the secret key.
    /// </summary>
    public string SecretKey { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets a value indicating whether to use HTTPS.
    /// </summary>
    public bool UseSSL { get; set; } = true;
}
