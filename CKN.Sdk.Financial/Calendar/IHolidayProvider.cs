namespace CKN.Sdk.Financial.Calendar;

/// <summary>
/// Provides holiday data for a given exchange and year.
/// Implement this interface to supply custom or external holiday calendars.
/// </summary>
public interface IHolidayProvider
{
    /// <summary>
    /// Returns all market holidays for the specified <paramref name="exchange"/> and
    /// <paramref name="year"/>.  The list must not include weekends — only actual trading-day
    /// exceptions are returned.
    /// </summary>
    IReadOnlyList<DateOnly> GetHolidays(MarketExchange exchange, int year);
}
