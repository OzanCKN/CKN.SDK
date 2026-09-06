using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.SemanticKernel;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection;

public static class SemanticKernelBuilderExtensions
{
    /// <summary>
    /// Configures the DI container to use Semantic Kernel.
    /// </summary>
    public static IServiceCollection AddCknSemanticKernel(this IServiceCollection services, Action<IKernelBuilder> configureBuilder)
    {
        var builder = Kernel.CreateBuilder();
        
        // Allow consumers to add their models, plugins, etc.
        configureBuilder(builder);
        
        // Provide the same services container to the kernel if possible, 
        // though typically SemanticKernel creates its own internal provider if not careful.
        // For simplicity, we just build a singleton Kernel here.
        var kernel = builder.Build();
        services.AddSingleton(kernel);

        return services;
    }
}
