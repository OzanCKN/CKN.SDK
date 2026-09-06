using System;
using Amazon;
using Amazon.S3;
using CKN.Sdk.Storage;
using CKN.Sdk.Storage.S3;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection;

public static class S3StorageServiceCollectionExtensions
{
    /// <summary>
    /// Configures the DI container to use AWS S3 as the default storage provider.
    /// </summary>
    public static IServiceCollection AddCknS3Storage(this IServiceCollection services, Action<S3StorageOptions> configureOptions)
    {
        var options = new S3StorageOptions();
        configureOptions(options);

        services.Configure(configureOptions);

        services.TryAddSingleton<IAmazonS3>(sp =>
        {
            var region = RegionEndpoint.GetBySystemName(options.Region);
            return new AmazonS3Client(options.AccessKey, options.SecretKey, region);
        });

        services.TryAddScoped<IStorageService, S3StorageService>();

        return services;
    }
}
