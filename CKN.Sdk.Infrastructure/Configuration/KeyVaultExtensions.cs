using System;
using Azure.Identity;
using Microsoft.Extensions.Configuration;

namespace CKN.Sdk.Infrastructure.Configuration;

/// <summary>
/// Extension methods for configuring Azure Key Vault in the CKN SDK ecosystem.
/// </summary>
public static class KeyVaultExtensions
{
    /// <summary>
    /// Adds Azure Key Vault to the configuration builder using DefaultAzureCredential.
    /// </summary>
    /// <param name="builder">The configuration builder.</param>
    /// <param name="keyVaultUri">The URI of the Azure Key Vault.</param>
    /// <returns>The modified configuration builder.</returns>
    public static IConfigurationBuilder AddCknAzureKeyVault(this IConfigurationBuilder builder, string keyVaultUri)
    {
        if (string.IsNullOrWhiteSpace(keyVaultUri))
        {
            throw new ArgumentException("KeyVault URI cannot be null or empty.", nameof(keyVaultUri));
        }

        builder.AddAzureKeyVault(
            new Uri(keyVaultUri),
            new DefaultAzureCredential());

        return builder;
    }
}
