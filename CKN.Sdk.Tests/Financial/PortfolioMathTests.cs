using CKN.Sdk.Financial.Portfolio;
using FluentAssertions;

namespace CKN.Sdk.Tests.Financial;

public sealed class PortfolioMathTests
{
    private static readonly DateTimeOffset T0 = new(2026, 1, 1, 9, 0, 0, TimeSpan.Zero);

    // ---- WeightedAverageCost ----

    [Fact]
    public void WeightedAverageCost_EmptyTrades_ReturnsZero()
    {
        PortfolioMath.WeightedAverageCost([]).Should().Be(0m);
    }

    [Fact]
    public void WeightedAverageCost_SingleBuy_ReturnsPrice()
    {
        var trades = new[] { Buy("A", 100m, 50m) };
        PortfolioMath.WeightedAverageCost(trades).Should().Be(50m);
    }

    [Fact]
    public void WeightedAverageCost_MultipleBuysAtDifferentPrices_ReturnsWeightedAverage()
    {
        // 100 @ 50 + 100 @ 60 → WAC = (100*50 + 100*60) / 200 = 55
        var trades = new[]
        {
            Buy("A", 100m, 50m),
            Buy("A", 100m, 60m),
        };
        PortfolioMath.WeightedAverageCost(trades).Should().Be(55m);
    }

    [Fact]
    public void WeightedAverageCost_AfterPartialSell_WacIsUnchanged()
    {
        // WAC should not change after a sell — only quantity decreases
        var trades = new[]
        {
            Buy("A",  100m, 50m),
            Buy("A",  100m, 60m),
            Sell("A", 50m,  70m),  // sell 50 units at 70 — WAC stays 55
        };
        PortfolioMath.WeightedAverageCost(trades).Should().Be(55m);
    }

    [Fact]
    public void WeightedAverageCost_FullExit_ReturnsZero()
    {
        var trades = new[]
        {
            Buy("A",  100m, 50m),
            Sell("A", 100m, 60m),
        };
        PortfolioMath.WeightedAverageCost(trades).Should().Be(0m);
    }

    [Fact]
    public void WeightedAverageCost_BuyAfterFullExit_RecalculatesFromNewBuy()
    {
        var trades = new[]
        {
            Buy("A",  100m, 50m),
            Sell("A", 100m, 60m), // full exit
            Buy("A",   50m, 80m), // re-enter
        };
        PortfolioMath.WeightedAverageCost(trades).Should().Be(80m);
    }

    [Fact]
    public void WeightedAverageCost_ThrowsOnNonPositiveQuantity()
    {
        var badTrade = new Trade("A", 0m, 50m, TradeSide.Buy, T0);
        Assert.Throws<ArgumentException>(() =>
            PortfolioMath.WeightedAverageCost([badTrade]));
    }

    // ---- CalculatePnl ----

    [Fact]
    public void CalculatePnl_NoTrades_ReturnsAllZero()
    {
        var pnl = PortfolioMath.CalculatePnl([], 100m);
        pnl.RealizedPnl.Should().Be(0m);
        pnl.UnrealizedPnl.Should().Be(0m);
        pnl.TotalPnl.Should().Be(0m);
        pnl.RoiPercent.Should().Be(0m);
        pnl.PositionQuantity.Should().Be(0m);
    }

    [Fact]
    public void CalculatePnl_BuyOnly_UnrealizedPnlReflectsCurrentPrice()
    {
        // Buy 100 @ 50, current price 60 → unrealized = 100 * (60-50) = 1000
        var trades = new[] { Buy("A", 100m, 50m) };
        var pnl = PortfolioMath.CalculatePnl(trades, 60m);

        pnl.RealizedPnl.Should().Be(0m);
        pnl.UnrealizedPnl.Should().Be(1000m);
        pnl.TotalPnl.Should().Be(1000m);
        pnl.PositionQuantity.Should().Be(100m);
        pnl.WeightedAverageCost.Should().Be(50m);
    }

    [Fact]
    public void CalculatePnl_PartialSell_SplitsBetweenRealizedAndUnrealized()
    {
        // Buy 100 @ 50, sell 50 @ 70 (realize 50*(70-50)=1000),
        // current price 80 → unrealized = 50*(80-50) = 1500
        var trades = new[]
        {
            Buy("A",  100m, 50m),
            Sell("A", 50m,  70m),
        };
        var pnl = PortfolioMath.CalculatePnl(trades, 80m);

        pnl.RealizedPnl.Should().Be(1000m);
        pnl.UnrealizedPnl.Should().Be(1500m);
        pnl.TotalPnl.Should().Be(2500m);
        pnl.PositionQuantity.Should().Be(50m);
        pnl.WeightedAverageCost.Should().Be(50m);
    }

    [Fact]
    public void CalculatePnl_FullExit_OnlyRealizedRemainsUnrealizedIsZero()
    {
        var trades = new[]
        {
            Buy("A",  100m, 50m),
            Sell("A", 100m, 70m),
        };
        var pnl = PortfolioMath.CalculatePnl(trades, 999m); // current price doesn't matter

        pnl.RealizedPnl.Should().Be(2000m);
        pnl.UnrealizedPnl.Should().Be(0m);
        pnl.PositionQuantity.Should().Be(0m);
    }

    [Fact]
    public void CalculatePnl_Loss_ProducesNegativePnl()
    {
        // Buy 100 @ 50, current price 40 → unrealized = -1000
        var trades = new[] { Buy("A", 100m, 50m) };
        var pnl = PortfolioMath.CalculatePnl(trades, 40m);

        pnl.UnrealizedPnl.Should().Be(-1000m);
        pnl.RoiPercent.Should().BeNegative();
    }

    [Fact]
    public void CalculatePnl_RoiIsCorrect()
    {
        // Buy 100 @ 50 = 5000 cost, current price 60 = 6000 value → ROI = 20%
        var trades = new[] { Buy("A", 100m, 50m) };
        var pnl = PortfolioMath.CalculatePnl(trades, 60m);
        pnl.RoiPercent.Should().Be(20m);
    }

    [Fact]
    public void CalculatePnl_WacAveragedAcrossMultipleBuys()
    {
        // 100 @ 40 + 100 @ 60 → WAC = 50; current = 55 → unrealized = 200*(55-50) = 1000
        var trades = new[]
        {
            Buy("A", 100m, 40m),
            Buy("A", 100m, 60m),
        };
        var pnl = PortfolioMath.CalculatePnl(trades, 55m);

        pnl.WeightedAverageCost.Should().Be(50m);
        pnl.UnrealizedPnl.Should().Be(1000m);
    }

    [Fact]
    public void CalculatePnl_ThrowsOnNonPositiveCurrentPrice()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            PortfolioMath.CalculatePnl([], 0m));
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            PortfolioMath.CalculatePnl([], -1m));
    }

    [Fact]
    public void CalculatePnl_DecimalPlacesRoundsCorrectly()
    {
        // 1 unit @ 3 = 3, current price = 4 → unrealized = 1.0000
        // With decimalPlaces=2 it should be 1.00
        var trades = new[] { Buy("A", 1m, 3m) };
        var pnl = PortfolioMath.CalculatePnl(trades, 4m, decimalPlaces: 2);
        pnl.UnrealizedPnl.Should().Be(1.00m);
    }

    // ---- PercentageChange ----

    [Fact]
    public void PercentageChange_Gain_ReturnsPositive()
    {
        PortfolioMath.PercentageChange(100m, 110m).Should().Be(10m);
    }

    [Fact]
    public void PercentageChange_Loss_ReturnsNegative()
    {
        PortfolioMath.PercentageChange(100m, 90m).Should().Be(-10m);
    }

    [Fact]
    public void PercentageChange_NoChange_ReturnsZero()
    {
        PortfolioMath.PercentageChange(50m, 50m).Should().Be(0m);
    }

    [Fact]
    public void PercentageChange_ZeroCostBasis_Throws()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            PortfolioMath.PercentageChange(0m, 100m));
    }

    [Fact]
    public void PercentageChange_NegativeCostBasis_UsesAbsoluteValue()
    {
        // Going from -100 to -50 is a 50% gain (denominator is |−100| = 100)
        PortfolioMath.PercentageChange(-100m, -50m).Should().Be(50m);
    }

    // ---- Argument validation ----

    [Fact]
    public void WeightedAverageCost_NullTrades_Throws()
    {
        Assert.Throws<ArgumentNullException>(() =>
            PortfolioMath.WeightedAverageCost(null!));
    }

    [Fact]
    public void CalculatePnl_NullTrades_Throws()
    {
        Assert.Throws<ArgumentNullException>(() =>
            PortfolioMath.CalculatePnl(null!, 100m));
    }

    // ---- helpers ----

    private static Trade Buy(string symbol, decimal qty, decimal price) =>
        new(symbol, qty, price, TradeSide.Buy, T0);

    private static Trade Sell(string symbol, decimal qty, decimal price) =>
        new(symbol, qty, price, TradeSide.Sell, T0.AddHours(1));
}
