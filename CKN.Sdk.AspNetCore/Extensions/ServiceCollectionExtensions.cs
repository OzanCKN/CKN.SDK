using Microsoft.Extensions.DependencyInjection;
using CKN.Sdk.AspNetCore.Middlewares;

namespace CKN.Sdk.AspNetCore.Extensions;

/// <summary>
/// Extension methods for configuring ASP.NET Core services.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds CKN AspNetCore specific services, such as the global exception handler.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <returns>The modified service collection.</returns>
    public static IServiceCollection AddCknAspNetCore(this IServiceCollection services)
    {
        services.AddExceptionHandler<GlobalExceptionHandler>();
        services.AddProblemDetails();
        
        return services;
    }
}
