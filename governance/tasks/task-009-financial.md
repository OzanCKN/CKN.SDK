# Task 009 — CKN.Sdk.Financial İmplementasyonu

**Sprint:** Sprint 6 — Finance SDK Gap Closure  
**Durum:** ✅ Tamamlandı (2026-09-27)  
**Atanan:** AI Agent (Claude Sonnet)

---

## Hedef

`CKN.Sdk.Financial` paketini tasarla ve uygula. Hedef kalite: bir .NET yazılım mimarının tercih edebileceği olgunlukta — sıfır 3. parti bağımlılık, TDD, Zero-Warning, Native AOT uyumlu.

## Kapsam

### Domain Modeli
- `MarketExchange` enum: BIST, NYSE, NASDAQ, Crypto
- `MarketSession` record: Date, Open/Close (TimeOnly), TimeZone, IsHalfDay
- `OhlcBar` readonly record struct: Timestamp, OHLCV (decimal)
- `OhlcTimeframe` enum: Minute1..Month1
- `Trade` readonly record struct: Symbol, Quantity, Price, Side, ExecutedAt
- `TradeSide` enum: Buy, Sell
- `PortfolioPnl` readonly record struct: RealizedPnl, UnrealizedPnl, TotalPnl, RoiPercent, PositionQuantity, WAC, TotalCostBasis

### Servisler / Abstraction
- `IMarketCalendar`: IsOpen, GetNextOpen, GetSession, GetHolidays
- `IHolidayProvider`: GetHolidays — extensibility hook

### Implementasyon
- `MarketCalendarService`: IANA timezone (Europe/Istanbul, America/New_York), hafta sonu + tatil kontrolü
- `DefaultHolidayProvider`: built-in veri + FinancialOptions override desteği
- `BistHolidayData`: sabit milli bayramlar (algoritmik) + dini bayramlar (hardcoded 2024–2030)
- `UsMarketHolidayData`: NYSE/NASDAQ tatilleri tamamıyla algoritmik (Easter algoritması dahil)

### OHLC
- `OhlcAggregator`: sync + async IAsyncEnumerable, streaming, O(1) memory

### Portfolio
- `PortfolioMath`: WAC (Ağırlıklı Ortalama Maliyet), CalculatePnl, PercentageChange — saf fonksiyonlar

### DI
- `AddCknFinancial(Action<FinancialOptions>?)` — Microsoft.Extensions.DI namespace convention

## Dosyalar

```
CKN.Sdk.Financial/
  CKN.Sdk.Financial.csproj
  Calendar/
    MarketExchange.cs
    MarketSession.cs
    IMarketCalendar.cs
    IHolidayProvider.cs
    DefaultHolidayProvider.cs
    MarketCalendarService.cs
    BuiltInHolidays/
      BistHolidayData.cs
      UsMarketHolidayData.cs
  Ohlc/
    OhlcBar.cs
    OhlcTimeframe.cs
    OhlcAggregator.cs
  Portfolio/
    TradeSide.cs
    Trade.cs
    PortfolioPnl.cs
    PortfolioMath.cs
  Options/
    FinancialOptions.cs
  Extensions/
    FinancialServiceCollectionExtensions.cs
  README.md

CKN.Sdk.Tests/Financial/
  MarketCalendarTests.cs    (16 test)
  OhlcAggregatorTests.cs    (11 test)
  PortfolioMathTests.cs     (20 test)
```

## Kararlar

- Sıfır 3. parti bağımlılık — yalnızca `CKN.Sdk.Core` + `Microsoft.Extensions.*` (DI, Options)
- IANA timezone ID (.NET 6+ built-in) — NodaTime gerekmedi
- `OhlcAggregator` sync + async path: streaming, O(1) memory, IAsyncEnumerable
- WAC yöntemi: SPK mevzuatı standartı, FIFO değil
- İslami tatil verileri hardcoded 2024–2030 (Hicri takvim algoritması yok)
- NYSE tatilleri tamamıyla algoritmik (Easter anonymous Gregorian + nth-weekday helpers)
- `FinancialOptions.AdditionalHolidays` + `RemovedHolidays` ile override mekanizması
