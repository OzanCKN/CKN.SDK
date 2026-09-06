using System;
using Azure.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace CKN.Sdk.Infrastructure.Configuration;

/// <summary>
/// Provides extension methods for configuring application settings, such as Azure KeyVault.
/// </summary>
public static class ConfigurationExtensions
{
    /// <summary>
    /// Integrates Azure KeyVault into the application configuration.
    /// </summary>
    /// <param name="builder">The host application builder.</param>
    /// <param name="vaultUri">The URI of the Azure KeyVault.</param>
    /// <returns>The updated host application builder.</returns>
    public static IHostApplicationBuilder AddCknAzureKeyVault(this IHostApplicationBuilder builder, string vaultUri)
    {
        if (string.IsNullOrWhiteSpace(vaultUri))
        {
            throw new ArgumentException("KeyVault URI cannot be null or empty.", nameof(vaultUri));
        }

        builder.Configuration.AddAzureKeyVault(
            new Uri(vaultUri),
            new DefaultAzureCredential());

        return builder;
    }
}
