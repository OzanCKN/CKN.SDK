using Hangfire;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace CKN.Sdk.Scheduling.Hangfire;

public static class HangfireBuilderExtensions
{
    /// <summary>
    /// Registers Hangfire scheduling services.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configure">Configuration for Hangfire.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddCknHangfire(this IServiceCollection services, Action<IGlobalConfiguration> configure)
    {
        if (services == null) throw new ArgumentNullException(nameof(services));
        if (configure == null) throw new ArgumentNullException(nameof(configure));

        services.AddHangfire(configure);
        services.AddHangfireServer();

        return services;
    }
}
