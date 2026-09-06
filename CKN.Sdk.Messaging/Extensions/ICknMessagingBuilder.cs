using Microsoft.Extensions.DependencyInjection;

namespace CKN.Sdk.Messaging.Extensions;

/// <summary>
/// A builder for configuring CKN Messaging options.
/// </summary>
public interface ICknMessagingBuilder
{
    /// <summary>
    /// Gets the application service collection.
    /// </summary>
    IServiceCollection Services { get; }
}

internal class CknMessagingBuilder : ICknMessagingBuilder
{
    public IServiceCollection Services { get; }

    public CknMessagingBuilder(IServiceCollection services)
    {
        Services = services;
    }
}
