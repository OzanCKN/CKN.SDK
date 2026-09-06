using Microsoft.Extensions.DependencyInjection;
using CKN.Sdk.Core.DependencyInjection;

namespace CKN.Sdk.Core.Configuration;

/// <summary>
/// Extension methods for configuring fail-fast options in the CKN SDK.
/// </summary>
public static class CknBuilderExtensions
{
    /// <summary>
    /// Configures an options class with data annotations validation that fails fast on application startup.
    /// </summary>
    /// <typeparam name="TOptions">The type of the options class.</typeparam>
    /// <param name="builder">The CKN builder.</param>
    /// <param name="sectionName">The configuration section name to bind to.</param>
    /// <returns>The <see cref="ICknBuilder"/> instance.</returns>
    public static ICknBuilder AddValidatedOptions<[System.Diagnostics.CodeAnalysis.DynamicallyAccessedMembers(System.Diagnostics.CodeAnalysis.DynamicallyAccessedMemberTypes.All)] TOptions>(
        this ICknBuilder builder, 
        string sectionName) where TOptions : class
    {
        builder.Services.AddOptions<TOptions>()
            .BindConfiguration(sectionName)
            .ValidateDataAnnotations()
            .ValidateOnStart(); // Fail-fast on application startup

        return builder;
    }
}
