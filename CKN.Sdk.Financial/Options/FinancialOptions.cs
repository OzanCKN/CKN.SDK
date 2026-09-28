using CKN.Sdk.Financial.Calendar;

namespace CKN.Sdk.Financial.Options;

/// <summary>
/// Configuration options for <c>CKN.Sdk.Financial</c>.
/// </summary>
public sealed class FinancialOptions
{
    /// <summary>
    /// Additional trading-day exceptions that the built-in holiday data does not cover.
    /// Keyed by <see cref="MarketExchange"/>; values are lists of dates on which the exchange
    /// will be closed (weekdays only — weekend entries are silently ignored).
    /// </summary>
    public Dictionary<MarketExchange, IReadOnlyList<DateOnly>> AdditionalHolidays { get; set; } = [];

    /// <summary>
    /// Dates to remove from the built-in holiday list.
    /// Useful when a government reschedules a holiday to a different day.
    /// </summary>
    public Dictionary<MarketExchange, IReadOnlyList<DateOnly>> RemovedHolidays { get; set; } = [];
}
