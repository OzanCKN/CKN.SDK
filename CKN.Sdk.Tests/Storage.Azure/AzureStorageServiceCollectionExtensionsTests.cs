using System;
using Azure.Storage.Blobs;
using CKN.Sdk.Storage;
using CKN.Sdk.Storage.Azure;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace CKN.Sdk.Tests.Storage.Azure;

public class AzureStorageServiceCollectionExtensionsTests
{
    [Fact]
    public void AddCknAzureStorage_ShouldRegisterBlobServiceClient()
    {
        // Arrange
        var services = new ServiceCollection();
        
        // Act
        services.AddCknAzureStorage(options =>
        {
            options.ConnectionString = "DefaultEndpointsProtocol=https;AccountName=test;AccountKey=test;EndpointSuffix=core.windows.net";
        });
        
        var serviceProvider = services.BuildServiceProvider();
        var client = serviceProvider.GetService<BlobServiceClient>();
        var storageService = serviceProvider.GetService<IStorageService>();

        // Assert
        Assert.NotNull(client);
        Assert.NotNull(storageService);
        Assert.IsType<AzureStorageService>(storageService);
    }
}
