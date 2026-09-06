namespace CKN.Sdk.Storage.Azure;

/// <summary>
/// Configuration options for the Azure Blob Storage provider.
/// </summary>
public class AzureStorageOptions
{
    /// <summary>
    /// Gets or sets the connection string.
    /// </summary>
    public string ConnectionString { get; set; } = string.Empty;
}
