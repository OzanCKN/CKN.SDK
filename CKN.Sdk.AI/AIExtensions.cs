using System;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.DependencyInjection;

namespace CKN.Sdk.AI;

/// <summary>
/// Extension methods for configuring AI services.
/// </summary>
public static class AIExtensions
{
    /// <summary>
    /// Adds a generic IChatClient to the service collection.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="chatClient">The chat client instance.</param>
    /// <returns>The service collection.</returns>
    public static IServiceCollection AddCknChatClient(this IServiceCollection services, IChatClient chatClient)
    {
        services.AddSingleton<IChatClient>(chatClient);
        return services;
    }
}
