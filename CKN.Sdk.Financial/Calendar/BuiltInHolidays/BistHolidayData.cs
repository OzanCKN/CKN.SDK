namespace CKN.Sdk.Financial.Calendar.BuiltInHolidays;

/// <summary>
/// Built-in holiday data for Borsa İstanbul.
/// Fixed national holidays are calculated algorithmically; Islamic holidays (Eid al-Fitr,
/// Eid al-Adha) are hardcoded for 2024–2030 because they follow the Hijri calendar.
/// </summary>
internal static class BistHolidayData
{
    // Islamic holidays for BIST (Ramadan Bayramı + Kurban Bayramı).
    // BIST is closed on the arefe (eve) plus all bayram days.
    // Dates are approximate — the Turkish government may announce final dates 1–2 days earlier.
    private static readonly Dictionary<int, DateOnly[]> IslamicHolidays = new()
    {
        [2024] =
        [
            // Ramadan Bayramı (Arefe + 3 days): Apr 9–12
            new(2024, 4, 9), new(2024, 4, 10), new(2024, 4, 11), new(2024, 4, 12),
            // Kurban Bayramı (Arefe + 4 days): Jun 15–19
            new(2024, 6, 15), new(2024, 6, 16), new(2024, 6, 17), new(2024, 6, 18), new(2024, 6, 19),
        ],
        [2025] =
        [
            // Ramadan Bayramı: Mar 29 – Apr 2 (arefe Mar 29)
            new(2025, 3, 29), new(2025, 3, 30), new(2025, 3, 31), new(2025, 4, 1),
            // Kurban Bayramı: Jun 4–8
            new(2025, 6, 4), new(2025, 6, 5), new(2025, 6, 6), new(2025, 6, 7), new(2025, 6, 8),
        ],
        [2026] =
        [
            // Ramadan Bayramı: Mar 18–21
            new(2026, 3, 18), new(2026, 3, 19), new(2026, 3, 20), new(2026, 3, 21),
            // Kurban Bayramı: May 24–28
            new(2026, 5, 24), new(2026, 5, 25), new(2026, 5, 26), new(2026, 5, 27), new(2026, 5, 28),
        ],
        [2027] =
        [
            // Ramadan Bayramı: Mar 7–10
            new(2027, 3, 7), new(2027, 3, 8), new(2027, 3, 9), new(2027, 3, 10),
            // Kurban Bayramı: May 13–17
            new(2027, 5, 13), new(2027, 5, 14), new(2027, 5, 15), new(2027, 5, 16), new(2027, 5, 17),
        ],
        [2028] =
        [
            // Ramadan Bayramı: Feb 24–27
            new(2028, 2, 24), new(2028, 2, 25), new(2028, 2, 26), new(2028, 2, 27),
            // Kurban Bayramı: May 1–5
            new(2028, 5, 1), new(2028, 5, 2), new(2028, 5, 3), new(2028, 5, 4), new(2028, 5, 5),
        ],
        [2029] =
        [
            // Ramadan Bayramı: Feb 12–15
            new(2029, 2, 12), new(2029, 2, 13), new(2029, 2, 14), new(2029, 2, 15),
            // Kurban Bayramı: Apr 20–24
            new(2029, 4, 20), new(2029, 4, 21), new(2029, 4, 22), new(2029, 4, 23), new(2029, 4, 24),
        ],
        [2030] =
        [
            // Ramadan Bayramı: Feb 1–4
            new(2030, 2, 1), new(2030, 2, 2), new(2030, 2, 3), new(2030, 2, 4),
            // Kurban Bayramı: Apr 9–13
            new(2030, 4, 9), new(2030, 4, 10), new(2030, 4, 11), new(2030, 4, 12), new(2030, 4, 13),
        ],
    };

    /// <summary>
    /// Returns BIST holidays for <paramref name="year"/>.
    /// Fixed national holidays are derived algorithmically; Islamic holidays are returned only
    /// for years 2024–2030 (the built-in data range).
    /// </summary>
    internal static IReadOnlyList<DateOnly> GetHolidays(int year)
    {
        var holidays = new List<DateOnly>(16);

        // Fixed national holidays — observed on the date itself (BIST does not shift to Mon/Fri)
        AddIfWeekday(holidays, new DateOnly(year, 1, 1));   // Yılbaşı
        AddIfWeekday(holidays, new DateOnly(year, 4, 23));  // Ulusal Egemenlik ve Çocuk Bayramı
        AddIfWeekday(holidays, new DateOnly(year, 5, 1));   // Emek ve Dayanışma Günü
        AddIfWeekday(holidays, new DateOnly(year, 5, 19));  // Atatürk'ü Anma, Gençlik ve Spor Bayramı
        AddIfWeekday(holidays, new DateOnly(year, 7, 15));  // Demokrasi ve Millî Birlik Günü
        AddIfWeekday(holidays, new DateOnly(year, 8, 30));  // Zafer Bayramı
        AddIfWeekday(holidays, new DateOnly(year, 10, 29)); // Cumhuriyet Bayramı

        if (IslamicHolidays.TryGetValue(year, out var islamic))
        {
            foreach (var d in islamic)
                AddIfWeekday(holidays, d);
        }

        holidays.Sort();
        return holidays;
    }

    private static void AddIfWeekday(List<DateOnly> list, DateOnly date)
    {
        if (date.DayOfWeek is not DayOfWeek.Saturday and not DayOfWeek.Sunday)
            list.Add(date);
    }
}
