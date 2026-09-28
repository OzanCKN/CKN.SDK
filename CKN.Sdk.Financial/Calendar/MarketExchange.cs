namespace CKN.Sdk.Financial.Calendar;

/// <summary>
/// Identifies a financial exchange or asset class with its own trading schedule.
/// </summary>
public enum MarketExchange
{
    /// <summary>Borsa İstanbul — trades 10:00–18:10 Istanbul time (UTC+3, no DST).</summary>
    BIST,

    /// <summary>New York Stock Exchange — trades 09:30–16:00 Eastern time (EST/EDT).</summary>
    NYSE,

    /// <summary>NASDAQ — trades 09:30–16:00 Eastern time, same calendar as NYSE.</summary>
    NASDAQ,

    /// <summary>Cryptocurrency markets — trade 24/7/365, never closed.</summary>
    Crypto,
}
