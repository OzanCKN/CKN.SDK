namespace CKN.Sdk.Financial.Portfolio;

/// <summary>
/// Pure, stateless financial math functions for portfolio analytics.
/// All methods use the <b>Weighted Average Cost (WAC)</b> method — the standard applied by
/// Turkish Capital Markets Board (SPK) regulations and most brokerage platforms.
/// </summary>
/// <remarks>
/// Methods in this class throw <see cref="ArgumentException"/> for structurally invalid input
/// (e.g. negative quantities) but never for empty trade lists — an empty list represents a
/// position with no trades, which is a valid (zero) state.
/// </remarks>
public static class PortfolioMath
{
    /// <summary>
    /// Computes the Weighted Average Cost per unit of the <b>current open position</b>
    /// after all trades have been applied.
    /// </summary>
    /// <param name="trades">
    /// Trade history for a single symbol, ordered chronologically.
    /// </param>
    /// <returns>
    /// The WAC per unit, or <c>0</c> when there is no remaining open position.
    /// </returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="trades"/> is null.</exception>
    /// <exception cref="ArgumentException">
    /// Thrown when any trade has a non-positive <see cref="Trade.Quantity"/> or
    /// <see cref="Trade.Price"/>.
    /// </exception>
    public static decimal WeightedAverageCost(IEnumerable<Trade> trades)
    {
        ArgumentNullException.ThrowIfNull(trades);

        decimal qty = 0m;
        decimal wac = 0m;

        foreach (var t in trades)
        {
            ValidateTrade(t);

            if (t.Side == TradeSide.Buy)
            {
                decimal newQty = qty + t.Quantity;
                wac = (qty * wac + t.Quantity * t.Price) / newQty;
                qty = newQty;
            }
            else
            {
                qty -= t.Quantity;
                if (qty <= 0m)
                {
                    qty = 0m;
                    wac = 0m;
                }
            }
        }

        return wac;
    }

    /// <summary>
    /// Calculates a full PnL snapshot for a single symbol using the WAC method.
    /// </summary>
    /// <param name="trades">
    /// Trade history for a single symbol, ordered chronologically.
    /// </param>
    /// <param name="currentPrice">Current market price per unit.  Must be positive.</param>
    /// <param name="decimalPlaces">Number of decimal places for rounding intermediate values.</param>
    /// <param name="rounding">Midpoint rounding strategy (defaults to banker's rounding).</param>
    /// <returns>A <see cref="PortfolioPnl"/> snapshot.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="trades"/> is null.</exception>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown when <paramref name="currentPrice"/> is not positive, or
    /// <paramref name="decimalPlaces"/> is negative.
    /// </exception>
    public static PortfolioPnl CalculatePnl(
        IEnumerable<Trade> trades,
        decimal currentPrice,
        int decimalPlaces = 4,
        MidpointRounding rounding = MidpointRounding.ToEven)
    {
        ArgumentNullException.ThrowIfNull(trades);
        if (currentPrice <= 0m)
            throw new ArgumentOutOfRangeException(nameof(currentPrice), "Current price must be positive.");
        if (decimalPlaces < 0)
            throw new ArgumentOutOfRangeException(nameof(decimalPlaces), "Decimal places cannot be negative.");

        decimal positionQty   = 0m;
        decimal wac           = 0m;
        decimal realizedPnl   = 0m;
        decimal totalCostBasis = 0m;

        foreach (var t in trades)
        {
            ValidateTrade(t);

            if (t.Side == TradeSide.Buy)
            {
                decimal newQty = positionQty + t.Quantity;
                wac = (positionQty * wac + t.Quantity * t.Price) / newQty;
                positionQty = newQty;
                totalCostBasis += t.Quantity * t.Price;
            }
            else
            {
                decimal sellableQty = Math.Min(t.Quantity, positionQty);
                realizedPnl  += sellableQty * (t.Price - wac);
                positionQty  -= sellableQty;
                if (positionQty <= 0m)
                {
                    positionQty = 0m;
                    wac = 0m;
                }
            }
        }

        decimal unrealizedPnl = positionQty > 0m
            ? positionQty * (currentPrice - wac)
            : 0m;

        decimal totalPnl = realizedPnl + unrealizedPnl;
        decimal roi = totalCostBasis != 0m
            ? totalPnl / totalCostBasis * 100m
            : 0m;

        return new PortfolioPnl(
            RealizedPnl:         Math.Round(realizedPnl,   decimalPlaces, rounding),
            UnrealizedPnl:       Math.Round(unrealizedPnl, decimalPlaces, rounding),
            TotalPnl:            Math.Round(totalPnl,      decimalPlaces, rounding),
            RoiPercent:          Math.Round(roi,           decimalPlaces, rounding),
            PositionQuantity:    positionQty,
            WeightedAverageCost: Math.Round(wac,           decimalPlaces, rounding),
            TotalCostBasis:      Math.Round(totalCostBasis, decimalPlaces, rounding));
    }

    /// <summary>
    /// Returns the percentage change between two prices.
    /// Useful for ROI display, momentum indicators, and stop-loss calculations.
    /// </summary>
    /// <param name="costBasis">Starting (reference) price.  Must not be zero.</param>
    /// <param name="currentPrice">Ending price.</param>
    /// <returns>Percentage change, e.g. <c>10.0m</c> for a 10% gain.</returns>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown when <paramref name="costBasis"/> is zero.
    /// </exception>
    public static decimal PercentageChange(decimal costBasis, decimal currentPrice)
    {
        if (costBasis == 0m)
            throw new ArgumentOutOfRangeException(nameof(costBasis), "Cost basis must not be zero.");

        return (currentPrice - costBasis) / Math.Abs(costBasis) * 100m;
    }

    // ---

    private static void ValidateTrade(Trade t)
    {
        if (t.Quantity <= 0m)
            throw new ArgumentException(
                $"Trade for '{t.Symbol}' has non-positive quantity {t.Quantity}.", nameof(t));
        if (t.Price <= 0m)
            throw new ArgumentException(
                $"Trade for '{t.Symbol}' has non-positive price {t.Price}.", nameof(t));
    }
}
