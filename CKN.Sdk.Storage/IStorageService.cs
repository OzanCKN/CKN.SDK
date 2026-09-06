using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace CKN.Sdk.Storage;

/// <summary>
/// Defines a provider-agnostic abstraction for object storage operations.
/// </summary>
public interface IStorageService
{
    /// <summary>
    /// Uploads a file to the storage.
    /// </summary>
    Task<string> UploadAsync(string bucketName, string objectName, Stream data, string contentType, CancellationToken cancellationToken = default);

    /// <summary>
    /// Downloads a file from the storage.
    /// </summary>
    Task<Stream> DownloadAsync(string bucketName, string objectName, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes a file from the storage.
    /// </summary>
    Task DeleteAsync(string bucketName, string objectName, CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if a file exists in the storage.
    /// </summary>
    Task<bool> ExistsAsync(string bucketName, string objectName, CancellationToken cancellationToken = default);
}
