using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CKN.Sdk.Storage.Minio;
using Minio;
using Minio.DataModel.Args;
using Moq;
using Xunit;

namespace CKN.Sdk.Tests.Storage.Minio;

public class MinioStorageServiceTests
{
    [Fact]
    public async Task UploadAsync_ShouldPutObject()
    {
        // Arrange
        var mockMinioClient = new Mock<IMinioClient>();
        mockMinioClient.Setup(x => x.BucketExistsAsync(It.IsAny<BucketExistsArgs>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
            
        mockMinioClient.Setup(x => x.PutObjectAsync(It.IsAny<PutObjectArgs>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new global::Minio.DataModel.Response.PutObjectResponse(System.Net.HttpStatusCode.OK, "test", new System.Collections.Generic.Dictionary<string, string>(), 123, "test"));

        var service = new MinioStorageService(mockMinioClient.Object);
        var data = new MemoryStream(Encoding.UTF8.GetBytes("test data"));
        
        // Act
        var result = await service.UploadAsync("test-bucket", "test.txt", data, "text/plain");

        // Assert
        Assert.Equal("test.txt", result);
        mockMinioClient.Verify(x => x.PutObjectAsync(It.IsAny<PutObjectArgs>(), It.IsAny<CancellationToken>()), Times.Once);
    }
}
