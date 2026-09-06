using Microsoft.Extensions.DependencyInjection;
using Quartz;
using System;

namespace CKN.Sdk.Scheduling.Quartz;

public static class QuartzBuilderExtensions
{
    /// <summary>
    /// Registers Quartz scheduling services.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configure">Configuration for Quartz.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddCknQuartz(this IServiceCollection services, Action<IServiceCollectionQuartzConfigurator>? configure = null)
    {
        if (services == null) throw new ArgumentNullException(nameof(services));

        services.AddQuartz(config => 
        {
            configure?.Invoke(config);
        });

        services.AddQuartzHostedService(options =>
        {
            options.WaitForJobsToComplete = true;
        });

        return services;
    }
}
