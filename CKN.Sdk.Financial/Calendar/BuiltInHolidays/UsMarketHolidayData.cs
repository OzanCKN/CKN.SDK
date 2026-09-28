namespace CKN.Sdk.Financial.Calendar.BuiltInHolidays;

/// <summary>
/// Built-in holiday data for NYSE and NASDAQ.
/// All rules are algorithmic — no hardcoded year-specific tables required.
/// </summary>
internal static class UsMarketHolidayData
{
    /// <summary>
    /// Returns US equity market holidays for <paramref name="year"/>.
    /// When a fixed holiday falls on Saturday it is observed the preceding Friday;
    /// when it falls on Sunday it is observed the following Monday.
    /// </summary>
    internal static IReadOnlyList<DateOnly> GetHolidays(int year)
    {
        var holidays = new List<DateOnly>(12)
        {
            Observed(new DateOnly(year, 1, 1)),                        // New Year's Day
            NthWeekdayOfMonth(year, 1, DayOfWeek.Monday, 3),          // MLK Day (3rd Mon Jan)
            NthWeekdayOfMonth(year, 2, DayOfWeek.Monday, 3),          // Presidents' Day (3rd Mon Feb)
            GoodFriday(year),                                           // Good Friday
            LastWeekdayOfMonth(year, 5, DayOfWeek.Monday),            // Memorial Day (last Mon May)
            Observed(new DateOnly(year, 6, 19)),                       // Juneteenth
            Observed(new DateOnly(year, 7, 4)),                        // Independence Day
            NthWeekdayOfMonth(year, 9, DayOfWeek.Monday, 1),          // Labor Day (1st Mon Sep)
            NthWeekdayOfMonth(year, 11, DayOfWeek.Thursday, 4),       // Thanksgiving (4th Thu Nov)
            Observed(new DateOnly(year, 12, 25)),                      // Christmas Day
        };

        // Remove duplicates (rare edge-case: two rules fall on the same observed date)
        holidays.Sort();
        return holidays.Distinct().ToList();
    }

    // --- helpers ---

    /// <summary>Applies the Saturday→Friday / Sunday→Monday observation rule.</summary>
    private static DateOnly Observed(DateOnly date) => date.DayOfWeek switch
    {
        DayOfWeek.Saturday => date.AddDays(-1),
        DayOfWeek.Sunday   => date.AddDays(1),
        _                  => date,
    };

    /// <summary>Returns the Nth occurrence of <paramref name="dow"/> in the given month.</summary>
    private static DateOnly NthWeekdayOfMonth(int year, int month, DayOfWeek dow, int n)
    {
        var first = new DateOnly(year, month, 1);
        int daysUntil = ((int)dow - (int)first.DayOfWeek + 7) % 7;
        return first.AddDays(daysUntil + (n - 1) * 7);
    }

    /// <summary>Returns the last occurrence of <paramref name="dow"/> in the given month.</summary>
    private static DateOnly LastWeekdayOfMonth(int year, int month, DayOfWeek dow)
    {
        var last = new DateOnly(year, month, DateTime.DaysInMonth(year, month));
        int daysBack = ((int)last.DayOfWeek - (int)dow + 7) % 7;
        return last.AddDays(-daysBack);
    }

    /// <summary>
    /// Computes Good Friday (Friday before Easter) using the Anonymous Gregorian algorithm.
    /// </summary>
    private static DateOnly GoodFriday(int year)
    {
        int a = year % 19;
        int b = year / 100;
        int c = year % 100;
        int d = b / 4;
        int e = b % 4;
        int f = (b + 8) / 25;
        int g = (b - f + 1) / 3;
        int h = (19 * a + b - d - g + 15) % 30;
        int i = c / 4;
        int k = c % 4;
        int l = (32 + 2 * e + 2 * i - h - k) % 7;
        int m = (a + 11 * h + 22 * l) / 451;
        int month = (h + l - 7 * m + 114) / 31;
        int day = (h + l - 7 * m + 114) % 31 + 1;

        // Easter Sunday minus 2 = Good Friday
        return new DateOnly(year, month, day).AddDays(-2);
    }
}
