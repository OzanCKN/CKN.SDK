using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Amazon.S3;
using Amazon.S3.Model;

namespace CKN.Sdk.Storage.S3;

public class S3StorageService : IStorageService
{
    private readonly IAmazonS3 _s3Client;

    public S3StorageService(IAmazonS3 s3Client)
    {
        _s3Client = s3Client;
    }

    public async Task<string> UploadAsync(string bucketName, string objectName, Stream data, string contentType, CancellationToken cancellationToken = default)
    {
        // AWS S3 buckets are usually created beforehand, but you could check/create here if needed.
        
        var putRequest = new PutObjectRequest
        {
            BucketName = bucketName,
            Key = objectName,
            InputStream = data,
            ContentType = contentType
        };

        await _s3Client.PutObjectAsync(putRequest, cancellationToken).ConfigureAwait(false);

        return objectName;
    }

    public async Task<Stream> DownloadAsync(string bucketName, string objectName, CancellationToken cancellationToken = default)
    {
        var getRequest = new GetObjectRequest
        {
            BucketName = bucketName,
            Key = objectName
        };

        var response = await _s3Client.GetObjectAsync(getRequest, cancellationToken).ConfigureAwait(false);
        return response.ResponseStream;
    }

    public async Task DeleteAsync(string bucketName, string objectName, CancellationToken cancellationToken = default)
    {
        var deleteRequest = new DeleteObjectRequest
        {
            BucketName = bucketName,
            Key = objectName
        };

        await _s3Client.DeleteObjectAsync(deleteRequest, cancellationToken).ConfigureAwait(false);
    }

    public async Task<bool> ExistsAsync(string bucketName, string objectName, CancellationToken cancellationToken = default)
    {
        try
        {
            var request = new GetObjectMetadataRequest
            {
                BucketName = bucketName,
                Key = objectName
            };

            await _s3Client.GetObjectMetadataAsync(request, cancellationToken).ConfigureAwait(false);
            return true;
        }
        catch (AmazonS3Exception ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            return false;
        }
    }
}
