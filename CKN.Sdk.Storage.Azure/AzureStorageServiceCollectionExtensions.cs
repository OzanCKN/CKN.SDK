using System;
using Azure.Storage.Blobs;
using CKN.Sdk.Storage;
using CKN.Sdk.Storage.Azure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection;

public static class AzureStorageServiceCollectionExtensions
{
    /// <summary>
    /// Configures the DI container to use Azure Blob Storage as the default storage provider.
    /// </summary>
    public static IServiceCollection AddCknAzureStorage(this IServiceCollection services, Action<AzureStorageOptions> configureOptions)
    {
        var options = new AzureStorageOptions();
        configureOptions(options);

        services.Configure(configureOptions);

        services.TryAddSingleton<BlobServiceClient>(sp =>
        {
            return new BlobServiceClient(options.ConnectionString);
        });

        services.TryAddScoped<IStorageService, AzureStorageService>();

        return services;
    }
}
