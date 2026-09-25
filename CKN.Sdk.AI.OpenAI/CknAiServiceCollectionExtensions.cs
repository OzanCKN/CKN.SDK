using System;
using CKN.Sdk.AI;
using CKN.Sdk.AI.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace CKN.Sdk.AI.OpenAI;

public static class CknAiServiceCollectionExtensions
{
    public static IServiceCollection AddCknAi(this IServiceCollection services, Action<CknAiOptions> configureOptions)
    {
        var options = new CknAiOptions();
        configureOptions(options);

        if (options.Provider == AiProviderType.OpenAi)
        {
            services.AddSingleton<ICknAiChatService>(sp => new OpenAiChatProvider(options.ApiKey, options.DefaultModel));
        }
        else
        {
            throw new NotSupportedException($"AI Provider '{options.Provider}' is not supported yet.");
        }

        return services;
    }
}
