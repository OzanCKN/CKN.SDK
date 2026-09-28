namespace CKN.Sdk.Financial.Portfolio;

/// <summary>
/// A snapshot of profit-and-loss metrics for a single symbol's position.
/// </summary>
/// <param name="RealizedPnl">
/// Profit or loss locked in by completed sell orders, calculated using the
/// Weighted Average Cost (WAC) method.
/// </param>
/// <param name="UnrealizedPnl">
/// Profit or loss on the still-open position, computed as
/// <c>PositionQuantity × (CurrentPrice − WeightedAverageCost)</c>.
/// </param>
/// <param name="TotalPnl"><c>RealizedPnl + UnrealizedPnl</c>.</param>
/// <param name="RoiPercent">
/// Return on invested capital as a percentage:
/// <c>(TotalPnl / TotalCostBasis) × 100</c>.
/// Returns 0 when no capital was deployed.
/// </param>
/// <param name="PositionQuantity">Remaining open quantity after all sells.</param>
/// <param name="WeightedAverageCost">
/// Current WAC per unit of the open position.  Zero when the position is fully closed.
/// </param>
/// <param name="TotalCostBasis">
/// Total capital deployed across all buy orders.  Used as the denominator for ROI.
/// </param>
public readonly record struct PortfolioPnl(
    decimal RealizedPnl,
    decimal UnrealizedPnl,
    decimal TotalPnl,
    decimal RoiPercent,
    decimal PositionQuantity,
    decimal WeightedAverageCost,
    decimal TotalCostBasis);
