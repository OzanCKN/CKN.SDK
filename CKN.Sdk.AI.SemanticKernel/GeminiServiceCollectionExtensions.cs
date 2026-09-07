using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.AI;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection;

public class GeminiOptions
{
    public string ApiKey { get; set; } = string.Empty;
    public string DefaultModel { get; set; } = "gemini-1.5-flash";
}

public static class GeminiServiceCollectionExtensions
{
    /// <summary>
    /// Configures the DI container to use Google Gemini via Semantic Kernel as the default IChatClient provider.
    /// </summary>
    public static IServiceCollection AddCknGemini(this IServiceCollection services, Action<GeminiOptions> configureOptions)
    {
        var options = new GeminiOptions();
        configureOptions(options);

        services.Configure(configureOptions);

        // Register Semantic Kernel IChatCompletionService
        services.AddSingleton<IChatCompletionService>(sp =>
        {
            var builder = Kernel.CreateBuilder();
            builder.AddGoogleAIGeminiChatCompletion(options.DefaultModel, options.ApiKey);
            var kernel = builder.Build();
            return kernel.GetRequiredService<IChatCompletionService>();
        });

        // Register IChatClient bridging from Semantic Kernel to Microsoft.Extensions.AI
        services.TryAddSingleton<IChatClient>(sp =>
        {
            var chatCompletionService = sp.GetRequiredService<IChatCompletionService>();
            
            // Microsoft.Extensions.AI provides an AsChatClient extension method for Semantic Kernel's IChatCompletionService
            return chatCompletionService.AsChatClient();
        });

        return services;
    }
}
