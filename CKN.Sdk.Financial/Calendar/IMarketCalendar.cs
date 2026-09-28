namespace CKN.Sdk.Financial.Calendar;

/// <summary>
/// Determines whether a financial exchange is open at a given point in time
/// and provides session information.
/// </summary>
public interface IMarketCalendar
{
    /// <summary>
    /// Returns <see langword="true"/> if the specified <paramref name="exchange"/> is currently
    /// accepting orders at <paramref name="utcNow"/>.
    /// </summary>
    bool IsOpen(MarketExchange exchange, DateTimeOffset utcNow);

    /// <summary>
    /// Returns the UTC timestamp at which the specified <paramref name="exchange"/> will next
    /// open for trading, starting the search from <paramref name="utcNow"/>.
    /// If the exchange is already open at <paramref name="utcNow"/> this returns
    /// <paramref name="utcNow"/> unchanged.
    /// </summary>
    DateTimeOffset GetNextOpen(MarketExchange exchange, DateTimeOffset utcNow);

    /// <summary>
    /// Returns the <see cref="MarketSession"/> for the given <paramref name="exchange"/> on
    /// <paramref name="date"/>, or <see langword="null"/> if the exchange is closed that day
    /// (weekend or holiday).
    /// </summary>
    MarketSession? GetSession(MarketExchange exchange, DateOnly date);

    /// <summary>
    /// Returns all market holidays for the specified <paramref name="exchange"/> and
    /// <paramref name="year"/>.
    /// </summary>
    IReadOnlyList<DateOnly> GetHolidays(MarketExchange exchange, int year);
}
