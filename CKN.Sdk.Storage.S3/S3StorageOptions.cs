namespace CKN.Sdk.Storage.S3;

/// <summary>
/// Configuration options for the AWS S3 storage provider.
/// </summary>
public class S3StorageOptions
{
    /// <summary>
    /// Gets or sets the AWS access key ID.
    /// </summary>
    public string AccessKey { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the AWS secret access key.
    /// </summary>
    public string SecretKey { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the AWS region (e.g., "us-east-1").
    /// </summary>
    public string Region { get; set; } = string.Empty;
}
