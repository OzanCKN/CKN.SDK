using Microsoft.Extensions.DependencyInjection;

namespace CKN.Sdk.Core.DependencyInjection;

/// <summary>
/// A centralized builder for configuring the CKN SDK.
/// </summary>
public interface ICknBuilder
{
    /// <summary>
    /// Gets the application service collection.
    /// </summary>
    IServiceCollection Services { get; }
}

internal sealed class CknBuilder(IServiceCollection services) : ICknBuilder
{
    public IServiceCollection Services { get; } = services;
}
