using CKN.Sdk.Financial.Calendar;
using CKN.Sdk.Financial.Options;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace CKN.Sdk.Tests.Financial;

public sealed class MarketCalendarTests
{
    // ---- helpers ----

    private static IMarketCalendar BuildCalendar(Action<FinancialOptions>? configure = null)
    {
        var services = new ServiceCollection();
        services.AddCknFinancial(configure);
        return services.BuildServiceProvider().GetRequiredService<IMarketCalendar>();
    }

    // A known midweek trading date/time: Tuesday 2026-09-01 (no holiday for any exchange)
    private static readonly DateTimeOffset BistOpen =
        new(2026, 9, 1, 7, 30, 0, TimeSpan.Zero);   // 10:30 Istanbul = 07:30 UTC

    private static readonly DateTimeOffset NyseOpen =
        new(2026, 9, 1, 14, 0, 0, TimeSpan.Zero);    // 10:00 Eastern = 14:00 UTC (EDT = UTC-4)

    private static readonly DateTimeOffset BistClosed =
        new(2026, 9, 1, 8, 0, 0, TimeSpan.Zero);     // 11:00 Istanbul — still open, but let's use after close

    private static readonly DateTimeOffset BistAfterClose =
        new(2026, 9, 1, 15, 30, 0, TimeSpan.Zero);   // 18:30 Istanbul = after 18:10 close

    // ---- IsOpen ----

    [Fact]
    public void IsOpen_BIST_DuringHours_ReturnsTrue()
    {
        var cal = BuildCalendar();
        Assert.True(cal.IsOpen(MarketExchange.BIST, BistOpen));
    }

    [Fact]
    public void IsOpen_BIST_AfterClose_ReturnsFalse()
    {
        var cal = BuildCalendar();
        Assert.False(cal.IsOpen(MarketExchange.BIST, BistAfterClose));
    }

    [Fact]
    public void IsOpen_BIST_BeforeOpen_ReturnsFalse()
    {
        // 06:00 UTC = 09:00 Istanbul, before 10:00 open
        var beforeOpen = new DateTimeOffset(2026, 9, 1, 6, 0, 0, TimeSpan.Zero);
        var cal = BuildCalendar();
        Assert.False(cal.IsOpen(MarketExchange.BIST, beforeOpen));
    }

    [Fact]
    public void IsOpen_ReturnsFalse_OnSaturday()
    {
        // 2026-09-05 is Saturday
        var saturday = new DateTimeOffset(2026, 9, 5, 10, 0, 0, TimeSpan.Zero);
        var cal = BuildCalendar();
        Assert.False(cal.IsOpen(MarketExchange.BIST, saturday));
        Assert.False(cal.IsOpen(MarketExchange.NYSE, saturday));
    }

    [Fact]
    public void IsOpen_ReturnsFalse_OnSunday()
    {
        // 2026-09-06 is Sunday
        var sunday = new DateTimeOffset(2026, 9, 6, 14, 0, 0, TimeSpan.Zero);
        var cal = BuildCalendar();
        Assert.False(cal.IsOpen(MarketExchange.BIST, sunday));
        Assert.False(cal.IsOpen(MarketExchange.NASDAQ, sunday));
    }

    [Fact]
    public void IsOpen_Crypto_AlwaysReturnsTrue()
    {
        var cal = BuildCalendar();
        // Weekend, midnight, holiday — crypto never sleeps
        var saturday = new DateTimeOffset(2026, 9, 5, 3, 0, 0, TimeSpan.Zero);
        Assert.True(cal.IsOpen(MarketExchange.Crypto, saturday));
    }

    [Fact]
    public void IsOpen_BIST_OnRepublicDay_ReturnsFalse()
    {
        // Oct 29 is Republic Day — BIST holiday
        // 10:30 local time = 07:30 UTC (Istanbul is UTC+3)
        var republicDay = new DateTimeOffset(2026, 10, 29, 7, 30, 0, TimeSpan.Zero);
        var cal = BuildCalendar();
        Assert.False(cal.IsOpen(MarketExchange.BIST, republicDay));
    }

    [Fact]
    public void IsOpen_NYSE_OnChristmas_ReturnsFalse()
    {
        // Dec 25 2026 is Friday — NYSE holiday
        // 14:00 UTC = 10:00 EST
        var christmas = new DateTimeOffset(2026, 12, 25, 15, 0, 0, TimeSpan.Zero);
        var cal = BuildCalendar();
        Assert.False(cal.IsOpen(MarketExchange.NYSE, christmas));
    }

    [Fact]
    public void IsOpen_NYSE_DuringHours_ReturnsTrue()
    {
        var cal = BuildCalendar();
        Assert.True(cal.IsOpen(MarketExchange.NYSE, NyseOpen));
    }

    // ---- GetHolidays ----

    [Fact]
    public void GetHolidays_BIST_ContainsRepublicDay()
    {
        var cal = BuildCalendar();
        var holidays = cal.GetHolidays(MarketExchange.BIST, 2026);
        Assert.Contains(new DateOnly(2026, 10, 29), holidays);
    }

    [Fact]
    public void GetHolidays_BIST_ContainsNewYear()
    {
        var cal = BuildCalendar();
        var holidays = cal.GetHolidays(MarketExchange.BIST, 2026);
        Assert.Contains(new DateOnly(2026, 1, 1), holidays);
    }

    [Fact]
    public void GetHolidays_NYSE_ContainsChristmas()
    {
        var cal = BuildCalendar();
        var holidays = cal.GetHolidays(MarketExchange.NYSE, 2026);
        Assert.Contains(new DateOnly(2026, 12, 25), holidays);
    }

    [Fact]
    public void GetHolidays_NYSE_ContainsThanksgiving()
    {
        // 4th Thursday of November 2026 = Nov 26
        var cal = BuildCalendar();
        var holidays = cal.GetHolidays(MarketExchange.NYSE, 2026);
        Assert.Contains(new DateOnly(2026, 11, 26), holidays);
    }

    [Fact]
    public void GetHolidays_Crypto_ReturnsEmpty()
    {
        var cal = BuildCalendar();
        Assert.Empty(cal.GetHolidays(MarketExchange.Crypto, 2026));
    }

    [Fact]
    public void GetHolidays_HolidaysAreNotOnWeekends()
    {
        var cal = BuildCalendar();
        foreach (var exchange in new[] { MarketExchange.BIST, MarketExchange.NYSE, MarketExchange.NASDAQ })
        {
            var holidays = cal.GetHolidays(exchange, 2026);
            foreach (var h in holidays)
            {
                Assert.True(
                    h.DayOfWeek is not DayOfWeek.Saturday and not DayOfWeek.Sunday,
                    $"{exchange} holiday {h} falls on a weekend.");
            }
        }
    }

    // ---- GetSession ----

    [Fact]
    public void GetSession_Weekday_ReturnsSession()
    {
        var cal = BuildCalendar();
        var session = cal.GetSession(MarketExchange.BIST, new DateOnly(2026, 9, 1));
        Assert.NotNull(session);
        Assert.Equal(new TimeOnly(10, 0),  session.Open);
        Assert.Equal(new TimeOnly(18, 10), session.Close);
    }

    [Fact]
    public void GetSession_Weekend_ReturnsNull()
    {
        var cal = BuildCalendar();
        Assert.Null(cal.GetSession(MarketExchange.NYSE, new DateOnly(2026, 9, 5)));
    }

    [Fact]
    public void GetSession_Holiday_ReturnsNull()
    {
        var cal = BuildCalendar();
        Assert.Null(cal.GetSession(MarketExchange.BIST, new DateOnly(2026, 10, 29)));
    }

    [Fact]
    public void GetSession_Crypto_AlwaysReturnsSession()
    {
        var cal = BuildCalendar();
        // Saturday
        Assert.NotNull(cal.GetSession(MarketExchange.Crypto, new DateOnly(2026, 9, 5)));
    }

    // ---- GetNextOpen ----

    [Fact]
    public void GetNextOpen_WhenAlreadyOpen_ReturnsSameTime()
    {
        var cal = BuildCalendar();
        var next = cal.GetNextOpen(MarketExchange.BIST, BistOpen);
        Assert.Equal(BistOpen, next);
    }

    [Fact]
    public void GetNextOpen_WhenClosed_ReturnsNextMorning()
    {
        var cal = BuildCalendar();
        // After BIST close on a Tuesday → should return Wednesday 10:00 Istanbul
        var next = cal.GetNextOpen(MarketExchange.BIST, BistAfterClose);
        Assert.Equal(DayOfWeek.Wednesday, TimeZoneInfo.ConvertTime(next,
            TimeZoneInfo.FindSystemTimeZoneById("Europe/Istanbul")).DayOfWeek);
    }

    [Fact]
    public void GetNextOpen_OnFriday_SkipsWeekendToMonday()
    {
        // Friday after NYSE close
        // 2026-09-11 is Friday; after 16:00 Eastern
        var fridayClose = new DateTimeOffset(2026, 9, 11, 20, 30, 0, TimeSpan.Zero); // 16:30 EDT
        var cal = BuildCalendar();
        var next = cal.GetNextOpen(MarketExchange.NYSE, fridayClose);
        Assert.Equal(DayOfWeek.Monday,
            TimeZoneInfo.ConvertTime(next,
                TimeZoneInfo.FindSystemTimeZoneById("America/New_York")).DayOfWeek);
    }

    // ---- Custom holidays ----

    [Fact]
    public void AdditionalHolidays_BlocksOpenOnCustomDate()
    {
        var customDate = new DateOnly(2026, 9, 1); // Tuesday, normally open
        var cal = BuildCalendar(opt =>
        {
            opt.AdditionalHolidays[MarketExchange.BIST] = [customDate];
        });

        var duringHours = new DateTimeOffset(2026, 9, 1, 7, 30, 0, TimeSpan.Zero); // 10:30 Istanbul
        Assert.False(cal.IsOpen(MarketExchange.BIST, duringHours));
    }

    [Fact]
    public void RemovedHolidays_AllowsOpenOnBuiltInHoliday()
    {
        // Remove Republic Day 2026 (Oct 29) from BIST holidays
        var cal = BuildCalendar(opt =>
        {
            opt.RemovedHolidays[MarketExchange.BIST] = [new DateOnly(2026, 10, 29)];
        });

        var duringHours = new DateTimeOffset(2026, 10, 29, 7, 30, 0, TimeSpan.Zero); // 10:30 Istanbul
        Assert.True(cal.IsOpen(MarketExchange.BIST, duringHours));
    }
}
