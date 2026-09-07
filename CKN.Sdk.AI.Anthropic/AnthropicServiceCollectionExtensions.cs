using System;
using Anthropic.SDK;
using CKN.Sdk.AI.Anthropic;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection;

public static class AnthropicServiceCollectionExtensions
{
    /// <summary>
    /// Configures the DI container to use Anthropic's native client.
    /// </summary>
    public static IServiceCollection AddCknAnthropic(this IServiceCollection services, Action<AnthropicOptions> configureOptions)
    {
        var options = new AnthropicOptions();
        configureOptions(options);

        services.Configure(configureOptions);

        services.TryAddSingleton<AnthropicClient>(sp =>
        {
            return new AnthropicClient(new APIAuthentication(options.ApiKey));
        });

        services.TryAddSingleton<Microsoft.Extensions.AI.IChatClient>(sp =>
        {
            var anthropicClient = sp.GetRequiredService<AnthropicClient>();
            return new AnthropicChatClient(anthropicClient, options.DefaultModel);
        });

        return services;
    }
}
