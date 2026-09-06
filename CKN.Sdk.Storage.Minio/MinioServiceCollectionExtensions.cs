using System;
using CKN.Sdk.Storage;
using CKN.Sdk.Storage.Minio;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Minio;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection;

public static class MinioServiceCollectionExtensions
{
    /// <summary>
    /// Configures the DI container to use Minio as the default storage provider.
    /// </summary>
    public static IServiceCollection AddCknMinioStorage(this IServiceCollection services, Action<MinioStorageOptions> configureOptions)
    {
        var options = new MinioStorageOptions();
        configureOptions(options);

        services.Configure(configureOptions);

        services.TryAddSingleton<IMinioClient>(sp =>
        {
            var client = new MinioClient()
                .WithEndpoint(options.Endpoint)
                .WithCredentials(options.AccessKey, options.SecretKey);

            if (options.UseSSL)
            {
                client = client.WithSSL();
            }

            return client.Build();
        });

        services.TryAddScoped<IStorageService, MinioStorageService>();

        return services;
    }
}
