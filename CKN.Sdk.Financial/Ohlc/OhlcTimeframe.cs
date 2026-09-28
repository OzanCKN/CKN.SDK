namespace CKN.Sdk.Financial.Ohlc;

/// <summary>
/// Represents standard OHLC aggregation time frames.
/// </summary>
public enum OhlcTimeframe
{
    /// <summary>One-minute bars.</summary>
    Minute1,
    /// <summary>Five-minute bars.</summary>
    Minute5,
    /// <summary>Fifteen-minute bars.</summary>
    Minute15,
    /// <summary>Thirty-minute bars.</summary>
    Minute30,
    /// <summary>One-hour bars.</summary>
    Hour1,
    /// <summary>Four-hour bars.</summary>
    Hour4,
    /// <summary>Daily bars (UTC midnight bucket).</summary>
    Day1,
    /// <summary>Weekly bars (Monday UTC midnight bucket).</summary>
    Week1,
    /// <summary>Monthly bars (first-of-month UTC midnight bucket).</summary>
    Month1,
}
