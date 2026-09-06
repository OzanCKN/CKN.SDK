using Coravel;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace CKN.Sdk.Scheduling.Coravel;

public static class CoravelBuilderExtensions
{
    /// <summary>
    /// Registers Coravel scheduling and queuing services.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddCknCoravel(this IServiceCollection services)
    {
        if (services == null) throw new ArgumentNullException(nameof(services));

        services.AddScheduler();
        services.AddQueue();

        return services;
    }
}
