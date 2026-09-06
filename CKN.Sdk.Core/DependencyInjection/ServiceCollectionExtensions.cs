using Microsoft.Extensions.DependencyInjection;

namespace CKN.Sdk.Core.DependencyInjection;

/// <summary>
/// Extension methods for setting up CKN SDK services in an <see cref="IServiceCollection" />.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds the core CKN SDK services to the specified <see cref="IServiceCollection" />.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection" /> to add services to.</param>
    /// <returns>An <see cref="ICknBuilder"/> that can be used to further configure the CKN SDK.</returns>
    public static ICknBuilder AddCkn(this IServiceCollection services)
    {
        // Core registration logic goes here
        // E.g., registering default decorators, options, etc.

        return new CknBuilder(services);
    }
}
