using CKN.Sdk.Financial.Ohlc;
using FluentAssertions;

namespace CKN.Sdk.Tests.Financial;

public sealed class OhlcAggregatorTests
{
    private static readonly DateTimeOffset BaseTime =
        new(2026, 9, 1, 9, 0, 0, TimeSpan.Zero);

    // ---- Edge cases ----

    [Fact]
    public void Aggregate_EmptySource_ReturnsEmpty()
    {
        var result = OhlcAggregator.Aggregate([], OhlcTimeframe.Hour1).ToList();
        result.Should().BeEmpty();
    }

    [Fact]
    public void Aggregate_SingleBar_ReturnsSameBar()
    {
        var bar = new OhlcBar(BaseTime, 100m, 110m, 90m, 105m, 1000m);
        var result = OhlcAggregator.Aggregate([bar], OhlcTimeframe.Hour1).ToList();

        result.Should().HaveCount(1);
        result[0].Open.Should().Be(100m);
        result[0].High.Should().Be(110m);
        result[0].Low.Should().Be(90m);
        result[0].Close.Should().Be(105m);
        result[0].Volume.Should().Be(1000m);
    }

    // ---- Minute aggregation ----

    [Fact]
    public void Aggregate_Minute5_GroupsCorrectly()
    {
        // Three 1-minute bars within the same 5-min bucket, then one in the next
        var bars = new[]
        {
            new OhlcBar(BaseTime.AddMinutes(0), 100m, 102m, 99m,  101m, 100m),
            new OhlcBar(BaseTime.AddMinutes(1), 101m, 103m, 100m, 102m, 200m),
            new OhlcBar(BaseTime.AddMinutes(2), 102m, 104m, 101m, 103m, 150m),
            new OhlcBar(BaseTime.AddMinutes(5), 103m, 105m, 102m, 104m, 300m), // new bucket
        };

        var result = OhlcAggregator.Aggregate(bars, OhlcTimeframe.Minute5).ToList();

        result.Should().HaveCount(2);

        var first = result[0];
        first.Open.Should().Be(100m);
        first.High.Should().Be(104m);
        first.Low.Should().Be(99m);
        first.Close.Should().Be(103m);
        first.Volume.Should().Be(450m);

        result[1].Open.Should().Be(103m);
        result[1].Volume.Should().Be(300m);
    }

    // ---- Hour aggregation ----

    [Fact]
    public void Aggregate_Hour1_GroupsByHourBucket()
    {
        var bars = new[]
        {
            new OhlcBar(BaseTime.AddMinutes(0),  100m, 102m, 98m, 101m, 500m),
            new OhlcBar(BaseTime.AddMinutes(30), 101m, 105m, 99m, 104m, 600m),
            new OhlcBar(BaseTime.AddMinutes(60), 104m, 108m, 103m, 107m, 700m), // next hour
        };

        var result = OhlcAggregator.Aggregate(bars, OhlcTimeframe.Hour1).ToList();

        result.Should().HaveCount(2);
        result[0].High.Should().Be(105m);
        result[0].Low.Should().Be(98m);
        result[0].Close.Should().Be(104m);
        result[0].Volume.Should().Be(1100m);
    }

    // ---- Day aggregation ----

    [Fact]
    public void Aggregate_Day1_GroupsByDate()
    {
        var day1 = new DateTimeOffset(2026, 9, 1, 9, 0, 0, TimeSpan.Zero);
        var day2 = new DateTimeOffset(2026, 9, 2, 9, 0, 0, TimeSpan.Zero);

        var bars = new[]
        {
            new OhlcBar(day1.AddHours(0), 100m, 110m, 95m, 108m, 1000m),
            new OhlcBar(day1.AddHours(4), 108m, 115m, 106m, 112m, 1500m),
            new OhlcBar(day2.AddHours(0), 112m, 120m, 110m, 118m, 2000m),
        };

        var result = OhlcAggregator.Aggregate(bars, OhlcTimeframe.Day1).ToList();

        result.Should().HaveCount(2);
        result[0].Open.Should().Be(100m);
        result[0].High.Should().Be(115m);
        result[0].Low.Should().Be(95m);
        result[0].Close.Should().Be(112m);
        result[0].Volume.Should().Be(2500m);
    }

    // ---- Week aggregation ----

    [Fact]
    public void Aggregate_Week1_GroupsByMondayBucket()
    {
        // 2026-08-31 is Monday week 1; 2026-09-07 is Monday week 2
        var week1a = new DateTimeOffset(2026, 8, 31, 10, 0, 0, TimeSpan.Zero);
        var week1b = new DateTimeOffset(2026, 9,  3, 10, 0, 0, TimeSpan.Zero); // Thursday, same week
        var week2  = new DateTimeOffset(2026, 9,  7, 10, 0, 0, TimeSpan.Zero); // Next Monday

        var bars = new[]
        {
            new OhlcBar(week1a, 100m, 105m, 98m,  102m, 1000m),
            new OhlcBar(week1b, 102m, 108m, 100m, 107m, 1200m),
            new OhlcBar(week2,  107m, 112m, 105m, 110m, 800m),
        };

        var result = OhlcAggregator.Aggregate(bars, OhlcTimeframe.Week1).ToList();
        result.Should().HaveCount(2);
        result[0].Volume.Should().Be(2200m);
    }

    // ---- Output ordering ----

    [Fact]
    public void Aggregate_OutputTimestamps_AreMonotonicallyIncreasing()
    {
        var bars = Enumerable.Range(0, 20)
            .Select(i => new OhlcBar(
                BaseTime.AddMinutes(i),
                100m + i, 105m + i, 95m + i, 102m + i, 100m))
            .ToList();

        var result = OhlcAggregator.Aggregate(bars, OhlcTimeframe.Minute5).ToList();

        for (int i = 1; i < result.Count; i++)
            result[i].Timestamp.Should().BeAfter(result[i - 1].Timestamp);
    }

    // ---- Async path ----

    [Fact]
    public async Task AggregateAsync_ProducesSameResultAsSync()
    {
        var bars = new[]
        {
            new OhlcBar(BaseTime.AddMinutes(0), 100m, 102m, 99m, 101m, 100m),
            new OhlcBar(BaseTime.AddMinutes(1), 101m, 103m, 100m, 102m, 200m),
            new OhlcBar(BaseTime.AddMinutes(5), 103m, 105m, 102m, 104m, 300m),
        };

        var syncResult  = OhlcAggregator.Aggregate(bars, OhlcTimeframe.Minute5).ToList();
        var asyncResult = await OhlcAggregator.AggregateAsync(ToAsyncEnumerable(bars), OhlcTimeframe.Minute5)
            .ToListAsync();

        asyncResult.Should().BeEquivalentTo(syncResult);
    }

    // ---- GetBucketStart internal contract ----

    [Theory]
    [InlineData(0,  OhlcTimeframe.Minute5,  0)]
    [InlineData(3,  OhlcTimeframe.Minute5,  0)]   // 3 min → bucket at 0
    [InlineData(5,  OhlcTimeframe.Minute5,  5)]   // 5 min → new bucket
    [InlineData(59, OhlcTimeframe.Hour1,    0)]   // minute 59 → same hour
    [InlineData(60, OhlcTimeframe.Hour1,    60)]  // minute 60 → new hour
    public void GetBucketStart_ReturnsExpectedMinuteOffset(
        int minuteOffset, OhlcTimeframe tf, int expectedMinute)
    {
        var ts = BaseTime.AddMinutes(minuteOffset);
        var bucket = OhlcAggregator.GetBucketStart(ts, tf);
        Assert.Equal(BaseTime.AddMinutes(expectedMinute), bucket);
    }

    // ---- helpers ----

    private static async IAsyncEnumerable<OhlcBar> ToAsyncEnumerable(IEnumerable<OhlcBar> bars)
    {
        foreach (var b in bars)
        {
            yield return b;
            await Task.CompletedTask;
        }
    }
}

internal static class AsyncEnumerableExtensions
{
    internal static async Task<List<T>> ToListAsync<T>(this IAsyncEnumerable<T> source)
    {
        var list = new List<T>();
        await foreach (var item in source) list.Add(item);
        return list;
    }
}
