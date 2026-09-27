using Microsoft.Extensions.DependencyInjection;

namespace CKN.Sdk.Network.Abstractions;

/// <summary>
/// A builder for configuring CKN Network services. Call provider extensions
/// such as <c>UseHttpClient()</c> to select a concrete HTTP provider.
/// </summary>
public interface ICknNetworkBuilder
{
    /// <summary>Gets the application service collection.</summary>
    IServiceCollection Services { get; }
}

internal sealed class CknNetworkBuilder : ICknNetworkBuilder
{
    public IServiceCollection Services { get; }

    internal CknNetworkBuilder(IServiceCollection services)
    {
        Services = services;
    }
}
