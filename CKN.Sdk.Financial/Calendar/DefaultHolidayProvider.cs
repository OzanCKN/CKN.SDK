using CKN.Sdk.Financial.Calendar.BuiltInHolidays;
using CKN.Sdk.Financial.Options;
using Microsoft.Extensions.Options;

namespace CKN.Sdk.Financial.Calendar;

/// <summary>
/// Default <see cref="IHolidayProvider"/> backed by built-in algorithmic rules.
/// Custom additions and removals can be supplied through <see cref="FinancialOptions"/>.
/// </summary>
internal sealed class DefaultHolidayProvider : IHolidayProvider
{
    private readonly FinancialOptions _options;

    public DefaultHolidayProvider(IOptions<FinancialOptions> options)
    {
        _options = options.Value;
    }

    /// <inheritdoc/>
    public IReadOnlyList<DateOnly> GetHolidays(MarketExchange exchange, int year)
    {
        var base_ = exchange switch
        {
            MarketExchange.BIST              => BistHolidayData.GetHolidays(year),
            MarketExchange.NYSE or
            MarketExchange.NASDAQ            => UsMarketHolidayData.GetHolidays(year),
            MarketExchange.Crypto            => Array.Empty<DateOnly>(),
            _                               => Array.Empty<DateOnly>(),
        };

        var set = new HashSet<DateOnly>(base_);

        if (_options.AdditionalHolidays.TryGetValue(exchange, out var additions))
        {
            foreach (var d in additions)
                if (d.Year == year) set.Add(d);
        }

        if (_options.RemovedHolidays.TryGetValue(exchange, out var removals))
        {
            foreach (var d in removals)
                if (d.Year == year) set.Remove(d);
        }

        var result = new List<DateOnly>(set);
        result.Sort();
        return result;
    }
}
