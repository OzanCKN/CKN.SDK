# CKN.Sdk.Financial

**CKN.Sdk.Financial**, CKN.SDK ekosisteminin sıfır 3. taraf bağımlılık (Zero-Dependency) felsefesiyle yazılmış finansal araçlar ve hesaplamalar kütüphanesidir.
Native AOT uyumlu, yüksek performanslı ve %100 Unit Test kapsamına sahip bir araç setidir.

## Özellikler

- **Piyasa Takvimi (Market Calendar):** BIST, NYSE, NASDAQ ve Kripto piyasaları için tatil ve seans saatleri hesaplaması.
- **OHLC Agregatörü:** O(1) bellek kullanarak finansal tick verilerinden Open-High-Low-Close barları üretme.
- **Portföy Matematiği:** WAC (Ağırlıklı Ortalama Maliyet), Gerçekleşen/Gerçekleşmeyen PnL ve ROI hesaplamaları.

## Hızlı Başlangıç

### 1. DI Konfigürasyonu
Projenize dahil etmek için `AddCknFinancial` metodunu kullanın:

```csharp
builder.Services.AddCknFinancial(opt =>
{
    // BIST için özel resmi tatil ekleme (Örn: Ekstra ilan edilen resmi tatiller)
    opt.AdditionalHolidays[MarketExchange.BIST] = new[] { new DateOnly(2027, 3, 15) };
});
```

### 2. Piyasa Takvimi Kullanımı (Market Calendar)
Seansın açık olup olmadığını kontrol edebilir, bir sonraki açılış zamanını alabilirsiniz:

```csharp
public class TradingService(IMarketCalendar _calendar)
{
    public void ExecuteTrade()
    {
        var now = DateTimeOffset.UtcNow;
        
        if (_calendar.IsOpen(MarketExchange.BIST, now))
        {
            Console.WriteLine("BIST şu an açık, işlem yapılabilir.");
        }
        else
        {
            var nextOpen = _calendar.GetNextOpen(MarketExchange.BIST, now);
            Console.WriteLine($"BIST kapalı. Bir sonraki açılış: {nextOpen.ToLocalTime()}");
        }
    }
}
```

### 3. Ağırlıklı Ortalama Maliyet (WAC) ve PnL Hesaplama
Gerçek portföy verileriyle maliyet analizi:

```csharp
var trades = new List<Trade>
{
    new Trade("THYAO", 100, 250.5m, TradeSide.Buy, DateTimeOffset.UtcNow.AddDays(-5)),
    new Trade("THYAO", 50, 260.0m, TradeSide.Buy, DateTimeOffset.UtcNow.AddDays(-2)),
    new Trade("THYAO", 75, 275.0m, TradeSide.Sell, DateTimeOffset.UtcNow.AddDays(-1))
};

// Kalan Miktar: 75 lot. 
// SPK Uyumlu Ağırlıklı Ortalama Maliyet hesabı:
var currentPrice = 280.0m;
PortfolioPnl pnl = PortfolioMath.CalculatePnl(trades, currentPrice);

Console.WriteLine($"Kalan Miktar: {pnl.PositionQuantity}");
Console.WriteLine($"WAC (Ort Maliyet): {pnl.WeightedAverageCost:C2}");
Console.WriteLine($"Gerçekleşen Kar/Zarar: {pnl.RealizedPnl:C2}");
Console.WriteLine($"ROI: % {pnl.RoiPercent:F2}");
```

### 4. OHLC Bar Agregasyonu
Gelen tick verilerinden mum (bar) üretimi. Milyonlarca tick verisini O(1) bellek ile işler:

```csharp
IEnumerable<OhlcBar> ticks = FetchMarketTicks(); 
// Dakikalık barlara çevirme
var minuteBars = OhlcAggregator.Aggregate(ticks, OhlcTimeframe.Minute1);

foreach (var bar in minuteBars)
{
    Console.WriteLine($"Time: {bar.Timestamp} | O: {bar.Open} | H: {bar.High} | L: {bar.Low} | C: {bar.Close} | Vol: {bar.Volume}");
}
```
