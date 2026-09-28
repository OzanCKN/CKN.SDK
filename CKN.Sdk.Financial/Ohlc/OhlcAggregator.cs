namespace CKN.Sdk.Financial.Ohlc;

/// <summary>
/// Aggregates a sequence of finer-grained <see cref="OhlcBar"/> records into coarser ones.
/// </summary>
/// <remarks>
/// The input sequence must be sorted ascending by <see cref="OhlcBar.Timestamp"/>.
/// Both synchronous and async enumeration paths are provided to avoid buffering large streams.
/// </remarks>
public static class OhlcAggregator
{
    /// <summary>
    /// Aggregates a sorted <paramref name="source"/> sequence into <paramref name="timeframe"/>
    /// buckets.  Each output bar's <see cref="OhlcBar.Timestamp"/> is the UTC bucket start.
    /// </summary>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="source"/> is <see langword="null"/>.
    /// </exception>
    public static IEnumerable<OhlcBar> Aggregate(
        IEnumerable<OhlcBar> source,
        OhlcTimeframe timeframe)
    {
        ArgumentNullException.ThrowIfNull(source);

        OhlcBar? pending = null;
        DateTimeOffset pendingBucket = default;

        foreach (var bar in source)
        {
            var bucket = GetBucketStart(bar.Timestamp, timeframe);

            if (pending is null)
            {
                pending = bar with { Timestamp = bucket };
                pendingBucket = bucket;
                continue;
            }

            if (bucket == pendingBucket)
            {
                pending = Merge(pending.Value, bar, pendingBucket);
            }
            else
            {
                yield return pending.Value;
                pending = bar with { Timestamp = bucket };
                pendingBucket = bucket;
            }
        }

        if (pending is not null)
            yield return pending.Value;
    }

    /// <summary>
    /// Async variant of <see cref="Aggregate"/>.  Suitable for streaming data sources without
    /// buffering the full sequence in memory.
    /// </summary>
    public static async IAsyncEnumerable<OhlcBar> AggregateAsync(
        IAsyncEnumerable<OhlcBar> source,
        OhlcTimeframe timeframe,
        [System.Runtime.CompilerServices.EnumeratorCancellation]
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(source);

        OhlcBar? pending = null;
        DateTimeOffset pendingBucket = default;

        await foreach (var bar in source.WithCancellation(cancellationToken))
        {
            var bucket = GetBucketStart(bar.Timestamp, timeframe);

            if (pending is null)
            {
                pending = bar with { Timestamp = bucket };
                pendingBucket = bucket;
                continue;
            }

            if (bucket == pendingBucket)
            {
                pending = Merge(pending.Value, bar, pendingBucket);
            }
            else
            {
                yield return pending.Value;
                pending = bar with { Timestamp = bucket };
                pendingBucket = bucket;
            }
        }

        if (pending is not null)
            yield return pending.Value;
    }

    // --- helpers ---

    private static OhlcBar Merge(OhlcBar acc, OhlcBar next, DateTimeOffset bucket) =>
        new(
            Timestamp: bucket,
            Open:      acc.Open,
            High:      next.High > acc.High ? next.High : acc.High,
            Low:       next.Low  < acc.Low  ? next.Low  : acc.Low,
            Close:     next.Close,
            Volume:    acc.Volume + next.Volume);

    /// <summary>
    /// Returns the UTC bucket-start timestamp for a given <paramref name="ts"/> and
    /// <paramref name="timeframe"/>.
    /// </summary>
    internal static DateTimeOffset GetBucketStart(DateTimeOffset ts, OhlcTimeframe timeframe)
    {
        // Normalise to UTC for bucketing
        var utc = ts.ToUniversalTime();

        return timeframe switch
        {
            OhlcTimeframe.Minute1  => TruncateToMinutes(utc, 1),
            OhlcTimeframe.Minute5  => TruncateToMinutes(utc, 5),
            OhlcTimeframe.Minute15 => TruncateToMinutes(utc, 15),
            OhlcTimeframe.Minute30 => TruncateToMinutes(utc, 30),
            OhlcTimeframe.Hour1    => TruncateToHours(utc, 1),
            OhlcTimeframe.Hour4    => TruncateToHours(utc, 4),
            OhlcTimeframe.Day1     => new DateTimeOffset(utc.Year, utc.Month, utc.Day, 0, 0, 0, TimeSpan.Zero),
            OhlcTimeframe.Week1    => TruncateToWeek(utc),
            OhlcTimeframe.Month1   => new DateTimeOffset(utc.Year, utc.Month, 1, 0, 0, 0, TimeSpan.Zero),
            _                      => TruncateToMinutes(utc, 1),
        };
    }

    private static DateTimeOffset TruncateToMinutes(DateTimeOffset utc, int minutes)
    {
        long ticks = utc.UtcTicks;
        long ticksPerBucket = TimeSpan.FromMinutes(minutes).Ticks;
        return new DateTimeOffset(ticks - ticks % ticksPerBucket, TimeSpan.Zero);
    }

    private static DateTimeOffset TruncateToHours(DateTimeOffset utc, int hours)
    {
        long ticks = utc.UtcTicks;
        long ticksPerBucket = TimeSpan.FromHours(hours).Ticks;
        return new DateTimeOffset(ticks - ticks % ticksPerBucket, TimeSpan.Zero);
    }

    private static DateTimeOffset TruncateToWeek(DateTimeOffset utc)
    {
        // ISO week starts on Monday
        int daysFromMonday = ((int)utc.DayOfWeek + 6) % 7;
        return new DateTimeOffset(utc.Year, utc.Month, utc.Day, 0, 0, 0, TimeSpan.Zero)
            .AddDays(-daysFromMonday);
    }
}
