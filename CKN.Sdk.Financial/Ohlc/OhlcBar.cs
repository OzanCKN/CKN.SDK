namespace CKN.Sdk.Financial.Ohlc;

/// <summary>
/// Represents a single OHLC (Open/High/Low/Close) price bar.
/// </summary>
/// <param name="Timestamp">
/// The UTC opening timestamp of the bar's time bucket.
/// </param>
/// <param name="Open">First trade price within the period.</param>
/// <param name="High">Highest trade price within the period.</param>
/// <param name="Low">Lowest trade price within the period.</param>
/// <param name="Close">Last trade price within the period.</param>
/// <param name="Volume">Total traded quantity within the period.</param>
public readonly record struct OhlcBar(
    DateTimeOffset Timestamp,
    decimal Open,
    decimal High,
    decimal Low,
    decimal Close,
    decimal Volume);
