using System;
using Amazon.S3;
using CKN.Sdk.Storage;
using CKN.Sdk.Storage.S3;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace CKN.Sdk.Tests.Storage.S3;

public class S3StorageServiceCollectionExtensionsTests
{
    [Fact]
    public void AddCknS3Storage_ShouldRegisterAmazonS3Client()
    {
        // Arrange
        var services = new ServiceCollection();
        
        // Act
        services.AddCknS3Storage(options =>
        {
            options.AccessKey = "test";
            options.SecretKey = "test";
            options.Region = "us-east-1";
        });
        
        var serviceProvider = services.BuildServiceProvider();
        var client = serviceProvider.GetService<IAmazonS3>();
        var storageService = serviceProvider.GetService<IStorageService>();

        // Assert
        Assert.NotNull(client);
        Assert.NotNull(storageService);
        Assert.IsType<S3StorageService>(storageService);
    }
}
