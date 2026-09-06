using System;
using CKN.Sdk.AI.Ollama;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using OllamaSharp;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection;

public static class OllamaBuilderExtensions
{
    /// <summary>
    /// Configures the DI container to use Ollama as the default IChatClient provider.
    /// </summary>
    public static IServiceCollection AddCknOllama(this IServiceCollection services, Action<OllamaOptions> configureOptions)
    {
        var options = new OllamaOptions();
        configureOptions(options);

        services.Configure(configureOptions);

        services.TryAddSingleton<IChatClient>(sp =>
        {
            var apiClient = new OllamaApiClient(options.Uri) { SelectedModel = options.DefaultModel };
            return apiClient;
        });

        return services;
    }
}
