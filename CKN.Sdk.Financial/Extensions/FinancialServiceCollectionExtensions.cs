using CKN.Sdk.Financial.Calendar;
using CKN.Sdk.Financial.Options;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Extension methods for registering <c>CKN.Sdk.Financial</c> services.
/// </summary>
public static class FinancialServiceCollectionExtensions
{
    /// <summary>
    /// Registers the financial services — <see cref="IMarketCalendar"/> and
    /// <see cref="IHolidayProvider"/> — as singletons in the service collection.
    /// </summary>
    /// <param name="services">The service collection to add services to.</param>
    /// <param name="configure">
    /// Optional delegate to customise <see cref="FinancialOptions"/> (e.g. add extra holidays).
    /// </param>
    /// <returns>The original <paramref name="services"/> for chaining.</returns>
    public static IServiceCollection AddCknFinancial(
        this IServiceCollection services,
        Action<FinancialOptions>? configure = null)
    {
        ArgumentNullException.ThrowIfNull(services);

        services.AddOptions<FinancialOptions>();

        if (configure is not null)
            services.Configure(configure);

        services.TryAddSingleton<IHolidayProvider, DefaultHolidayProvider>();
        services.TryAddSingleton<IMarketCalendar, MarketCalendarService>();

        return services;
    }
}
