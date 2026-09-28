namespace CKN.Sdk.Financial.Portfolio;

/// <summary>
/// Represents a single executed trade for a financial instrument.
/// </summary>
/// <param name="Symbol">Ticker symbol (e.g. "THYAO", "AAPL").</param>
/// <param name="Quantity">Number of units traded.  Must be positive.</param>
/// <param name="Price">Execution price per unit.  Must be positive.</param>
/// <param name="Side">Whether the trade was a buy or a sell.</param>
/// <param name="ExecutedAt">UTC timestamp at which the trade was filled.</param>
public readonly record struct Trade(
    string Symbol,
    decimal Quantity,
    decimal Price,
    TradeSide Side,
    DateTimeOffset ExecutedAt);
