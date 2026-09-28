namespace CKN.Sdk.Financial.Calendar;

/// <summary>
/// Describes the trading session for a specific exchange on a specific calendar date.
/// </summary>
/// <param name="Date">The calendar date of the session.</param>
/// <param name="Open">Local opening time for the exchange.</param>
/// <param name="Close">Local closing time for the exchange.</param>
/// <param name="TimeZone">The exchange's local time zone.</param>
/// <param name="IsHalfDay">
/// <see langword="true"/> when the exchange closes early (e.g. day before a holiday).
/// </param>
public sealed record MarketSession(
    DateOnly Date,
    TimeOnly Open,
    TimeOnly Close,
    TimeZoneInfo TimeZone,
    bool IsHalfDay = false)
{
    /// <summary>Returns the UTC <see cref="DateTimeOffset"/> when this session opens.</summary>
    public DateTimeOffset OpenUtc =>
        TimeZoneInfo.ConvertTimeToUtc(
            Date.ToDateTime(Open, DateTimeKind.Unspecified),
            TimeZone);

    /// <summary>Returns the UTC <see cref="DateTimeOffset"/> when this session closes.</summary>
    public DateTimeOffset CloseUtc =>
        TimeZoneInfo.ConvertTimeToUtc(
            Date.ToDateTime(Close, DateTimeKind.Unspecified),
            TimeZone);
}
