using System;
using CKN.Sdk.Network.Abstractions;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Extension methods for registering CKN Network services.
/// </summary>
public static class CknNetworkServiceCollectionExtensions
{
    /// <summary>
    /// Adds the CKN Network infrastructure to the service collection.
    /// Chain provider extensions such as <c>.UseHttpClient()</c> on the returned builder.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configure">Optional delegate to configure providers on the builder inline.</param>
    /// <returns>An <see cref="ICknNetworkBuilder"/> for further provider configuration.</returns>
    /// <example>
    /// <code>
    /// services.AddCknNetwork(net =>
    ///     net.UseHttpClient()
    ///        .AddCknHttpClient&lt;MyApiClient&gt;(opt =>
    ///        {
    ///            opt.BaseAddress = "https://api.example.com/";
    ///            opt.Auth = new ApiKeyQueryAuthStrategy("token", config["Api:Key"]!);
    ///            opt.RateLimit = new CknRateLimiterOptions { RequestsPerPeriod = 5 };
    ///        }));
    /// </code>
    /// </example>
    public static ICknNetworkBuilder AddCknNetwork(
        this IServiceCollection services,
        Action<ICknNetworkBuilder>? configure = null)
    {
        var builder = new CknNetworkBuilder(services);
        configure?.Invoke(builder);
        return builder;
    }
}
