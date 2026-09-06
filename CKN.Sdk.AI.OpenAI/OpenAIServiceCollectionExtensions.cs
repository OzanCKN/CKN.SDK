using System;
using CKN.Sdk.AI.OpenAI;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using OpenAI;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection;

public static class OpenAIServiceCollectionExtensions
{
    /// <summary>
    /// Configures the DI container to use OpenAI as the default IChatClient provider.
    /// </summary>
    public static IServiceCollection AddCknOpenAI(this IServiceCollection services, Action<OpenAIOptions> configureOptions)
    {
        var options = new OpenAIOptions();
        configureOptions(options);

        services.Configure(configureOptions);

        services.TryAddSingleton<IChatClient>(sp =>
        {
            var openAiChatClient = new OpenAI.Chat.ChatClient(options.DefaultModel, options.ApiKey);
            return openAiChatClient.AsIChatClient();
        });

        return services;
    }
}
