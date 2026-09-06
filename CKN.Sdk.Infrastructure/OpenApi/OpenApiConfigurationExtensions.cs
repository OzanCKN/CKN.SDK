using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Scalar.AspNetCore;

namespace CKN.Sdk.Infrastructure.OpenApi;

public static class OpenApiConfigurationExtensions
{
    /// <summary>
    /// Adds standard OpenAPI support to the service collection.
    /// </summary>
    public static IServiceCollection AddCknOpenApi(this IServiceCollection services)
    {
#if NET9_0_OR_GREATER
        services.AddOpenApi();
#endif
        return services;
    }

    /// <summary>
    /// Configures the application endpoints to use Scalar API documentation.
    /// </summary>
    public static Microsoft.AspNetCore.Routing.IEndpointRouteBuilder MapCknScalar(this Microsoft.AspNetCore.Routing.IEndpointRouteBuilder endpoints, string title = "CKN SDK API Reference")
    {
#if NET9_0_OR_GREATER
        endpoints.MapOpenApi();
#endif
        endpoints.MapScalarApiReference(options =>
        {
            options.WithTitle(title)
                   .WithTheme(ScalarTheme.DeepSpace)
                   .WithDefaultHttpClient(ScalarTarget.CSharp, ScalarClient.HttpClient);
        });
        return endpoints;
    }
}
