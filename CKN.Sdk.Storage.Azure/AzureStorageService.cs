using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Azure;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;

namespace CKN.Sdk.Storage.Azure;

public class AzureStorageService : IStorageService
{
    private readonly BlobServiceClient _blobServiceClient;

    public AzureStorageService(BlobServiceClient blobServiceClient)
    {
        _blobServiceClient = blobServiceClient;
    }

    public async Task<string> UploadAsync(string bucketName, string objectName, Stream data, string contentType, CancellationToken cancellationToken = default)
    {
        var containerClient = _blobServiceClient.GetBlobContainerClient(bucketName);
        await containerClient.CreateIfNotExistsAsync(PublicAccessType.None, cancellationToken: cancellationToken).ConfigureAwait(false);

        var blobClient = containerClient.GetBlobClient(objectName);

        var options = new BlobUploadOptions
        {
            HttpHeaders = new BlobHttpHeaders { ContentType = contentType }
        };

        await blobClient.UploadAsync(data, options, cancellationToken).ConfigureAwait(false);

        return objectName;
    }

    public async Task<Stream> DownloadAsync(string bucketName, string objectName, CancellationToken cancellationToken = default)
    {
        var containerClient = _blobServiceClient.GetBlobContainerClient(bucketName);
        var blobClient = containerClient.GetBlobClient(objectName);

        var response = await blobClient.DownloadContentAsync(cancellationToken).ConfigureAwait(false);
        return response.Value.Content.ToStream();
    }

    public async Task DeleteAsync(string bucketName, string objectName, CancellationToken cancellationToken = default)
    {
        var containerClient = _blobServiceClient.GetBlobContainerClient(bucketName);
        var blobClient = containerClient.GetBlobClient(objectName);

        await blobClient.DeleteIfExistsAsync(cancellationToken: cancellationToken).ConfigureAwait(false);
    }

    public async Task<bool> ExistsAsync(string bucketName, string objectName, CancellationToken cancellationToken = default)
    {
        var containerClient = _blobServiceClient.GetBlobContainerClient(bucketName);
        var blobClient = containerClient.GetBlobClient(objectName);

        var response = await blobClient.ExistsAsync(cancellationToken).ConfigureAwait(false);
        return response.Value;
    }
}
