namespace CKN.Sdk.Financial.Calendar;

/// <summary>
/// Concrete implementation of <see cref="IMarketCalendar"/> using IANA timezone IDs
/// (.NET 6+ cross-platform) and the injected <see cref="IHolidayProvider"/>.
/// </summary>
internal sealed class MarketCalendarService : IMarketCalendar
{
    // IANA timezone IDs — supported on .NET 6+ across Linux/macOS/Windows.
    private static readonly TimeZoneInfo IstanbulTz  = TimeZoneInfo.FindSystemTimeZoneById("Europe/Istanbul");
    private static readonly TimeZoneInfo EasternTz   = TimeZoneInfo.FindSystemTimeZoneById("America/New_York");

    // Regular session windows per exchange (local times)
    private static readonly Dictionary<MarketExchange, (TimeOnly Open, TimeOnly Close)> Sessions = new()
    {
        [MarketExchange.BIST]   = (new TimeOnly(10, 0), new TimeOnly(18, 10)),
        [MarketExchange.NYSE]   = (new TimeOnly( 9, 30), new TimeOnly(16, 0)),
        [MarketExchange.NASDAQ] = (new TimeOnly( 9, 30), new TimeOnly(16, 0)),
    };

    private readonly IHolidayProvider _holidayProvider;

    public MarketCalendarService(IHolidayProvider holidayProvider)
    {
        _holidayProvider = holidayProvider;
    }

    /// <inheritdoc/>
    public bool IsOpen(MarketExchange exchange, DateTimeOffset utcNow)
    {
        if (exchange == MarketExchange.Crypto) return true;

        var tz = GetTimeZone(exchange);
        var local = TimeZoneInfo.ConvertTime(utcNow, tz);
        var date  = DateOnly.FromDateTime(local.DateTime);

        if (local.DayOfWeek is DayOfWeek.Saturday or DayOfWeek.Sunday) return false;

        var holidays = _holidayProvider.GetHolidays(exchange, date.Year);
        if (holidays.Contains(date)) return false;

        var (open, close) = Sessions[exchange];
        var time = TimeOnly.FromDateTime(local.DateTime);
        return time >= open && time < close;
    }

    /// <inheritdoc/>
    public DateTimeOffset GetNextOpen(MarketExchange exchange, DateTimeOffset utcNow)
    {
        if (exchange == MarketExchange.Crypto) return utcNow;
        if (IsOpen(exchange, utcNow)) return utcNow;

        var tz    = GetTimeZone(exchange);
        var local = TimeZoneInfo.ConvertTime(utcNow, tz);
        var (open, _) = Sessions[exchange];

        // Walk forward day-by-day (max 14 days to handle long holiday runs)
        for (int i = 1; i <= 14; i++)
        {
            var candidate = local.DateTime.Date.AddDays(i);
            var date = DateOnly.FromDateTime(candidate);

            if (candidate.DayOfWeek is DayOfWeek.Saturday or DayOfWeek.Sunday) continue;

            var holidays = _holidayProvider.GetHolidays(exchange, date.Year);
            if (holidays.Contains(date)) continue;

            var openLocal = date.ToDateTime(open, DateTimeKind.Unspecified);
            return TimeZoneInfo.ConvertTimeToUtc(openLocal, tz);
        }

        // Defensive fallback — should never happen for real exchanges
        throw new InvalidOperationException(
            $"Could not determine next open for {exchange} within 14 days of {utcNow:O}.");
    }

    /// <inheritdoc/>
    public MarketSession? GetSession(MarketExchange exchange, DateOnly date)
    {
        if (exchange == MarketExchange.Crypto)
        {
            return new MarketSession(
                date,
                TimeOnly.MinValue,
                new TimeOnly(23, 59, 59),
                TimeZoneInfo.Utc);
        }

        if (date.DayOfWeek is DayOfWeek.Saturday or DayOfWeek.Sunday) return null;

        var holidays = _holidayProvider.GetHolidays(exchange, date.Year);
        if (holidays.Contains(date)) return null;

        var tz = GetTimeZone(exchange);
        var (open, close) = Sessions[exchange];
        return new MarketSession(date, open, close, tz);
    }

    /// <inheritdoc/>
    public IReadOnlyList<DateOnly> GetHolidays(MarketExchange exchange, int year)
        => _holidayProvider.GetHolidays(exchange, year);

    // ---

    private static TimeZoneInfo GetTimeZone(MarketExchange exchange) => exchange switch
    {
        MarketExchange.BIST              => IstanbulTz,
        MarketExchange.NYSE or
        MarketExchange.NASDAQ            => EasternTz,
        _                               => TimeZoneInfo.Utc,
    };
}
