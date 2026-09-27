# CKN.Sdk.Network

[![NuGet](https://img.shields.io/nuget/v/CKN.Sdk.Network.svg?style=flat-square)](https://www.nuget.org/packages/CKN.Sdk.Network/)
[![.NET](https://img.shields.io/badge/.NET-8%20%7C%209%20%7C%2010-512BD4?style=flat-square)](https://dotnet.microsoft.com)
[![License](https://img.shields.io/badge/license-MIT-green?style=flat-square)](../LICENSE)

**Provider-agnostic HTTP istemci soyutlama katmanı.** Auth stratejilerini, resilience options'ları ve `ICknHttpClient` arayüzünü tanımlar. Provider paketi değiştirilerek test, staging ve production ortamları farklı transport kütüphanelerine bağlanabilir.

---

## 1. Engineering Intent

CKN.Finance projesinde üç fiyat sağlayıcısı (Yahoo Finance, Finnhub, Coinbase) ham `HttpClient` üzerinden yazılmış; her biri `Task.Delay` ile kendi rate limiting'ini yapıyor, retry mekanizması yok, API anahtarları loglardan maskelenmiyor. Bu paket o problemi kökünden çözüyor:

- **SDK sarmalayıcı kuralını zorunlu kılar:** Tüketici proje doğrudan `HttpClient` yerine `ICknHttpClient` kullanır.
- **Sıfır tekrarlı kod:** Rate limiting, retry, circuit breaker ve log maskeleme tek bir `AddCknHttpClient<T>()` çağrısıyla devreye girer.
- **Transport bağımsızlığı:** Faz 1 `CKN.Sdk.Network.Http` (vanilla HttpClient), Faz 2 `CKN.Sdk.Network.Flurl` ve `CKN.Sdk.Network.RestSharp` provider'ları gelmeden aynı consumer kodu değişmez.
- **Exception-free:** Her yanıt `Result<T>` döner; tüketen kod `try/catch` yazmaz.

---

## 2. Mimari Özet

```
┌──────────────────────────────────────────────────────────────────┐
│                    Consumer (CKN.Finance vb.)                    │
│          ICknHttpClient  •  CknHttpClientBase<T>                 │
└─────────────────────────────┬────────────────────────────────────┘
                              │ AddCknNetwork(net => ...)
          ┌───────────────────┼───────────────────────────┐
          │                   │                           │
  ┌───────▼──────┐  ┌─────────▼──────────┐  ┌────────────▼──────┐
  │ CKN.Sdk.Net  │  │ CKN.Sdk.Net.Http   │  │ CKN.Sdk.Net.Flurl │
  │  (this pkg)  │  │  [Faz 1 - mevcut]  │  │   [Faz 2 - TODO]  │
  │              │  │                    │  │                   │
  │ ICknHttpCli  │  │ CknHttpClientBase  │  │  FlUrlClient impl │
  │ CknAuthStrat │  │ DelegatingHandlers │  │                   │
  │ *Options     │  │ Polly Resilience   │  └───────────────────┘
  └──────────────┘  │ TokenBucket RL     │  ┌────────────────────┐
                    │ SensitiveMasking   │  │CKN.Sdk.Net.Refit   │
                    └────────────────────┘  │  [Faz 2 - TODO]    │
                                            └────────────────────┘
```

### DelegatingHandler Zinciri (Faz 1)

```
Outbound Request
      │
      ▼
[SensitiveQueryMaskingHandler]  ← Logda token/key değerlerini maskeler
      │
      ▼
[Auth Handler]                  ← ApiKeyHeader / ApiKeyQuery / Bearer
      │
      ▼
[RateLimiterHandler]            ← TokenBucketRateLimiter (built-in .NET)
      │
      ▼
[Polly StandardResilienceHandler] ← Retry + CircuitBreaker + Timeout
      │
      ▼
[HttpClient / Inner Handler]
      │
      ▼
Remote API
```

---

## 3. Kurulum

Bu paket tek başına çalışmaz — bir provider paketi ile birlikte yüklenir.

```bash
# Abstraction + HttpClient provider (Faz 1)
dotnet add package CKN.Sdk.Network
dotnet add package CKN.Sdk.Network.Http
```

### Gereksinimler

| Gereksinim | Minimum Sürüm |
|---|---|
| .NET | 8.0 |
| CKN.Sdk.Core | (geçişli bağımlılık) |

---

## 4. Hızlı Başlangıç

**3 adımda çalışan minimal kurulum:**

### Adım 1 — Typed Client sınıfı oluşturun

```csharp
using CKN.Sdk.Network.Http.Services;
using CKN.Sdk.Network.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

public class FinnhubClient : CknHttpClientBase
{
    public FinnhubClient(
        HttpClient httpClient,
        ILoggerFactory loggerFactory,
        IOptionsMonitor<CknHttpClientOptions> options)
        : base(httpClient, loggerFactory, options) { }

    public Task<Result<StockQuote>> GetQuoteAsync(string symbol, CancellationToken ct = default)
        => GetJsonAsync<StockQuote>($"quote?symbol={symbol}", ct);
}
```

### Adım 2 — DI kaydı

```csharp
// Program.cs
builder.Services.AddCknNetwork(net =>
    net.AddCknHttpClient<FinnhubClient>(opt =>
    {
        opt.BaseAddress = "https://finnhub.io/api/v1/";
        opt.Auth = new ApiKeyQueryAuthStrategy("token", builder.Configuration["Finnhub:Key"]!);
        opt.RateLimit = new CknRateLimiterOptions { RequestsPerPeriod = 30, Period = TimeSpan.FromSeconds(1) };
    }));
```

### Adım 3 — Kullanım

```csharp
public class PortfolioService(FinnhubClient finnhub)
{
    public async Task<decimal?> GetPriceAsync(string symbol)
    {
        var result = await finnhub.GetQuoteAsync(symbol);
        return result.IsSuccess ? result.Value.CurrentPrice : null;
    }
}
```

---

## 5. appsettings.json Şeması

`AddCknHttpClient<T>` doğrudan `Action<CknHttpClientOptions>` alır; `appsettings.json` üzerinden bağlamak için standart .NET `BindConfiguration` yaklaşımı kullanılabilir:

```json
{
  "CknNetwork": {
    "FinnhubClient": {
      "BaseAddress": "https://finnhub.io/api/v1/",
      "TimeoutSeconds": 15,
      "RateLimit": {
        "RequestsPerPeriod": 30,
        "PeriodSeconds": 1,
        "QueueLimit": 200
      },
      "Retry": {
        "MaxAttempts": 3,
        "BaseDelaySeconds": 2,
        "UseJitter": true
      },
      "CircuitBreaker": {
        "FailureRatio": 0.5,
        "MinimumThroughput": 5,
        "SamplingDurationSeconds": 30,
        "BreakDurationSeconds": 30
      }
    }
  }
}
```

### CknHttpClientOptions Alanları

| Alan | Tip | Varsayılan | Açıklama |
|---|---|---|---|
| `BaseAddress` | `string?` | — | Tüm relative URL'lerin çözüldüğü kök adres. |
| `Timeout` | `TimeSpan` | `10s` | İstek başına zaman aşımı. |
| `DefaultHeaders` | `Dictionary<string,string>` | `{}` | Her istekte gönderilecek başlıklar (`User-Agent`, `Accept` vb.). |
| `Auth` | `CknAuthStrategy?` | `null` | Kimlik doğrulama stratejisi. `null` = kimlik doğrulama yok. |
| `Retry.MaxAttempts` | `int` | `3` | İlk deneme hariç maksimum yeniden deneme sayısı. |
| `Retry.BaseDelay` | `TimeSpan` | `2s` | Üstel geri çekilmenin başlangıç gecikmesi. |
| `Retry.UseJitter` | `bool` | `true` | Thundering herd önlemek için rastgele gecikme ekler. |
| `CircuitBreaker.FailureRatio` | `double` | `0.5` | Devreyi açacak hata oranı (0–1). |
| `CircuitBreaker.MinimumThroughput` | `int` | `5` | Oran hesabı için gereken minimum istek sayısı. |
| `CircuitBreaker.SamplingDuration` | `TimeSpan` | `30s` | Hata oranının ölçüldüğü pencere. |
| `CircuitBreaker.BreakDuration` | `TimeSpan` | `30s` | Devre açık kaldıktan sonra half-open'a geçiş süresi. |
| `RateLimit.RequestsPerPeriod` | `int` | `10` | Period boyunca izin verilen istek sayısı. |
| `RateLimit.Period` | `TimeSpan` | `1s` | Token doldurma periyodu. |
| `RateLimit.QueueLimit` | `int` | `100` | Kuyruklanabilecek maksimum istek sayısı. |
| `MaxDegreeOfParallelism` | `int` | `4` | `BatchAsync` içinde eş zamanlı çalışacak istek sayısı. |
| `SensitiveQueryParams` | `IReadOnlyList<string>` | `["token","apikey","api_key","key","secret","password","access_token"]` | Loglardan değeri maskelenecek query param adları. |

---

## 6. DI Kaydı

```csharp
// Program.cs — builder.Services üzerinden tam örnek
builder.Services.AddCknNetwork(net =>
{
    // Yahoo Finance provider
    net.AddCknHttpClient<YahooFinanceProvider>(opt =>
    {
        opt.BaseAddress = "https://query1.finance.yahoo.com/";
        opt.Timeout = TimeSpan.FromSeconds(15);
        opt.DefaultHeaders["User-Agent"] = "Mozilla/5.0 (compatible; CKN-Finance/1.0)";
        opt.RateLimit = new CknRateLimiterOptions { RequestsPerPeriod = 2, Period = TimeSpan.FromSeconds(1) };
    });

    // Finnhub provider
    net.AddCknHttpClient<FinnhubProvider>(opt =>
    {
        opt.BaseAddress = "https://finnhub.io/api/v1/";
        opt.Auth = new ApiKeyQueryAuthStrategy("token", config["Finnhub:Key"]!);
        opt.RateLimit = new CknRateLimiterOptions { RequestsPerPeriod = 30 };
    });

    // Coinbase provider
    net.AddCknHttpClient<CoinbaseProvider>(opt =>
    {
        opt.BaseAddress = "https://api.coinbase.com/v2/";
        opt.Auth = new BearerAuthStrategy(config["Coinbase:ApiKey"]!);
    });
});
```

`AddCknNetwork` — `ICknNetworkBuilder` döndürür. Provider paketi (`CKN.Sdk.Network.Http`) bu builder üzerine `AddCknHttpClient<T>` extension metodunu ekler.

---

## 7. Typed Client Oluşturma

Tüm typed client'lar `CknHttpClientBase`'den türetilir (provider: `CKN.Sdk.Network.Http`):

```csharp
// YahooFinanceProvider.cs
public sealed class YahooFinanceProvider : CknHttpClientBase
{
    public YahooFinanceProvider(
        HttpClient httpClient,
        ILoggerFactory loggerFactory,
        IOptionsMonitor<CknHttpClientOptions> options)
        : base(httpClient, loggerFactory, options) { }

    public Task<Result<ChartResponse>> GetChartAsync(
        string symbol,
        CancellationToken ct = default)
        => GetJsonAsync(
            $"v8/finance/chart/{Uri.EscapeDataString(symbol)}",
            YahooJsonContext.Default.ChartResponse,   // Source-generated JSON
            ct);

    public Task<IReadOnlyList<Result<ChartResponse>>> GetChartsAsync(
        IEnumerable<string> symbols,
        CancellationToken ct = default)
        => BatchAsync(
            symbols.Select(s => $"v8/finance/chart/{Uri.EscapeDataString(s)}"),
            YahooJsonContext.Default.ChartResponse,
            ct);
}
```

**Kurallar:**
- Constructor imzası değişmez — DI framework otomatik inject eder.
- `CknHttpClientBase` abstract; doğrudan `new()` yapılmaz.
- Her typed client için ayrı `AddCknHttpClient<T>()` çağrısı zorunludur.

---

## 8. Auth Stratejileri

`CknHttpClientOptions.Auth` alanına üç strateji atanabilir:

### ApiKeyHeaderAuthStrategy — Header üzerinden kimlik doğrulama

```csharp
opt.Auth = new ApiKeyHeaderAuthStrategy(
    headerName: "X-API-Key",
    value: config["Provider:ApiKey"]!
);
// Her istekte: X-API-Key: abc123
```

### ApiKeyQueryAuthStrategy — Query string üzerinden kimlik doğrulama

```csharp
opt.Auth = new ApiKeyQueryAuthStrategy(
    parameterName: "token",
    value: config["Finnhub:Key"]!
);
// Her istekte: https://finnhub.io/api/v1/quote?symbol=AAPL&token=abc123
```

### BearerAuthStrategy — Bearer token kimlik doğrulama

```csharp
opt.Auth = new BearerAuthStrategy(
    token: config["Provider:Token"]!
);
// Her istekte: Authorization: Bearer abc123
```

**Not:** Strateji `DelegatingHandler` zincirinde ikinci halkadır — maskeleme handler'ı her zaman dıştadır, bu nedenle token değerleri loglardan otomatik maskelenir.

---

## 9. Rate Limiting

`CknRateLimiterOptions` ile `TokenBucketRateLimiter` (built-in .NET 7+, ekstra paket gerektirmez) konfigüre edilir:

```csharp
opt.RateLimit = new CknRateLimiterOptions
{
    RequestsPerPeriod = 30,               // 30 istek / saniye
    Period             = TimeSpan.FromSeconds(1),
    QueueLimit         = 100              // 100'ün üstündeyse anında 429 döner
};
```

### Gerçek Dünya Senaryoları

| Sağlayıcı | Kota | Önerilen Ayar |
|---|---|---|
| Yahoo Finance (halka açık) | ~2 req/s | `RequestsPerPeriod=2, Period=1s` |
| Finnhub Free | 30 req/s | `RequestsPerPeriod=30, Period=1s` |
| Coinbase REST | 10 req/s | `RequestsPerPeriod=10, Period=1s` |
| BIST Veri Servisi | 60 req/min | `RequestsPerPeriod=60, Period=60s` |

**Kota aşılınca:** Handler kuyruğun dolduğunu tespit eder ve HTTP 429 döndürür — upstream API'ye istek gitmez.

---

## 10. Retry & Circuit Breaker

`Microsoft.Extensions.Http.Resilience` (Polly 8 tabanlı) `AddStandardResilienceHandler` üzerinden çalışır.

### Varsayılan Retry Davranışı

```
İstek başarısız (5xx / 408 / 429)
  → 1. retry: 2s + jitter
  → 2. retry: 4s + jitter
  → 3. retry: 8s + jitter
  → Hata döner
```

```csharp
opt.Retry = new CknRetryOptions
{
    MaxAttempts = 3,
    BaseDelay   = TimeSpan.FromSeconds(2),
    UseJitter   = true,
    AdditionalRetryStatusCodes = [503]    // varsayılana ek
};
```

### Circuit Breaker Davranışı

| Durum | Açıklama |
|---|---|
| **Closed** | Normal çalışma. |
| **Open** | Hata oranı eşiği aşıldı. Tüm istekler anında `HTTP_CIRCUIT_OPEN` döner. `BreakDuration` sonunda **Half-Open**'a geçer. |
| **Half-Open** | Bir deneme isteği gönderilir. Başarılı → **Closed**, başarısız → tekrar **Open**. |

```csharp
opt.CircuitBreaker = new CknCircuitBreakerOptions
{
    FailureRatio       = 0.5,   // %50 hata oranında aç
    MinimumThroughput  = 5,     // en az 5 istek sonra değerlendir
    SamplingDuration   = TimeSpan.FromSeconds(30),
    BreakDuration      = TimeSpan.FromSeconds(30)
};
```

---

## 11. BatchAsync

`BatchAsync<T>` sembol listesi gibi toplu sorgular için tasarlanmıştır:

```csharp
var symbols = new[] { "AAPL", "GOOG", "MSFT", "TSLA", "AMZN" };

IReadOnlyList<Result<ChartResponse>> results = await yahooProvider.GetChartsAsync(
    symbols.Select(s => $"v8/finance/chart/{s}"),
    YahooJsonContext.Default.ChartResponse,
    ct);

foreach (var (symbol, result) in symbols.Zip(results))
{
    if (result.IsSuccess)
        Console.WriteLine($"{symbol}: {result.Value.ClosePrice}");
    else
        Console.WriteLine($"{symbol}: ERROR {result.Error.Code}");
}
```

**`MaxDegreeOfParallelism`:** Eş zamanlı çalışacak istek sayısını sınırlar. `4` ile 100 sembol için yaklaşık 25 tur gerekir; rate limiter bu turları otomatik frenler.

```csharp
opt.MaxDegreeOfParallelism = 4;  // varsayılan
```

- Sonuçlar **giriş sırasıyla** döner — `symbols[i]` her zaman `results[i]` ile eşleşir.
- Her eleman bağımsız `Result<T>` taşır; bir başarısızlık diğerlerini durdurmaz.

---

## 12. Hassas Parametre Maskeleme

`SensitiveQueryMaskingHandler` log çıktısından API key değerlerini otomatik siler.

### Öncesi (ham HttpClient)

```
[INF] GET https://finnhub.io/api/v1/quote?symbol=AAPL&token=sk-live-abc123secret
```

### Sonrası (CKN.Sdk.Network.Http)

```
[INF] [FinnhubProvider] GET https://finnhub.io/api/v1/quote?symbol=AAPL&token=***REDACTED*** → 200 in 143ms
```

**Önemli:** Maskeleme yalnızca log kaydında uygulanır. Gerçek HTTP isteğine dokunulmaz — kimlik doğrulama verisi API'ye eksiksiz ulaşır.

Varsayılan maskeli parametre adları: `token`, `apikey`, `api_key`, `key`, `secret`, `password`, `access_token`

Özelleştirme:

```csharp
opt.SensitiveQueryParams = ["token", "apiKey", "sig", "client_secret"];
```

---

## 13. Native AOT / Source-Generated JSON

`GetJsonAsync<T>(url, JsonTypeInfo<T>, ct)` aşırı yüklemesi Reflection kullanmaz:

```csharp
// 1. JSON serializer context tanımla
[JsonSerializable(typeof(StockQuote))]
[JsonSerializable(typeof(ChartResponse))]
internal partial class YahooJsonContext : JsonSerializerContext { }

// 2. Typed client içinde kullan
public Task<Result<StockQuote>> GetQuoteAsync(string symbol, CancellationToken ct = default)
    => GetJsonAsync(
        $"v8/finance/quote?symbols={symbol}",
        YahooJsonContext.Default.StockQuote,   // ← kayıt anında oluşturulan TypeInfo
        ct);
```

`.csproj`'ta AOT etkinleştirmek için:

```xml
<PublishAot>true</PublishAot>
<IsAotCompatible>true</IsAotCompatible>
```

Reflection-based (`GetJsonAsync<T>(url, ct)`) aşırı yükleme geriye dönük uyumluluk için korunmuştur; Native AOT hedeflerinde kullanılmamalıdır.

---

## 14. Test Edilebilirlik

`ICknHttpClient` arayüzü sayesinde mock veya fake ile tam birim testi mümkündür:

```csharp
public class FakeCknHttpClient : ICknHttpClient
{
    private readonly Dictionary<string, object> _responses = new();

    public void Setup<T>(string relativeUrl, T response)
        => _responses[relativeUrl] = response!;

    public Task<Result<T>> GetJsonAsync<T>(string relativeUrl, CancellationToken ct = default)
        => Task.FromResult(
            _responses.TryGetValue(relativeUrl, out var val)
                ? Result.Success((T)val)
                : Result.Failure<T>(new Error("FAKE_404", "Not found")));

    public Task<Result<T>> GetJsonAsync<T>(string relativeUrl, JsonTypeInfo<T> _, CancellationToken ct = default)
        => GetJsonAsync<T>(relativeUrl, ct);

    public Task<IReadOnlyList<Result<T>>> BatchAsync<T>(
        IEnumerable<string> relativeUrls,
        JsonTypeInfo<T> jsonTypeInfo,
        CancellationToken ct = default)
        => Task.FromResult<IReadOnlyList<Result<T>>>(
            relativeUrls.Select(url => GetJsonAsync<T>(url, ct).GetAwaiter().GetResult()).ToList());
}

// Test
[Fact]
public async Task GetPriceAsync_ReturnsPrice_WhenQuoteSucceeds()
{
    var fake = new FakeCknHttpClient();
    fake.Setup("quote?symbol=AAPL", new StockQuote { CurrentPrice = 182.50m });

    var service = new PortfolioService(fake);
    var price = await service.GetPriceAsync("AAPL");

    price.Should().Be(182.50m);
}
```

Tümleşik test için `HttpMessageHandler` tabanlı `FakeHttpMessageHandler` kullanımı: bkz. `CKN.Sdk.Network.Http` README.

---

## 15. Finance'ten Geçiş Örneği

### Öncesi (CKN.Finance — ham HttpClient)

```csharp
// Program.cs
builder.Services.AddHttpClient<FinnhubProvider>(c =>
{
    c.BaseAddress = new Uri("https://finnhub.io/api/v1/");
    c.Timeout = TimeSpan.FromSeconds(15);
});

// FinnhubProvider.cs
private readonly HttpClient _http;
private static readonly SemaphoreSlim _rateLimiter = new(30, 30);

public async Task<StockQuote?> GetQuoteAsync(string symbol)
{
    await _rateLimiter.WaitAsync();
    try
    {
        var url = $"quote?symbol={symbol}&token={_apiKey}";  // token logda görünür!
        var response = await _http.GetAsync(url);
        response.EnsureSuccessStatusCode();                  // exception fırlatır
        var json = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<StockQuote>(json); // tüm body string'e dönüşür
    }
    finally
    {
        await Task.Delay(34);       // elle rate limiting
        _rateLimiter.Release();
    }
}
```

### Sonrası (CKN.Sdk.Network + CKN.Sdk.Network.Http)

```csharp
// Program.cs
builder.Services.AddCknNetwork(net =>
    net.AddCknHttpClient<FinnhubProvider>(opt =>
    {
        opt.BaseAddress = "https://finnhub.io/api/v1/";
        opt.Timeout     = TimeSpan.FromSeconds(15);
        opt.Auth        = new ApiKeyQueryAuthStrategy("token", config["Finnhub:Key"]!);
        opt.RateLimit   = new CknRateLimiterOptions { RequestsPerPeriod = 30 };
        opt.Retry       = new CknRetryOptions { MaxAttempts = 3 };
    }));

// FinnhubProvider.cs
public sealed class FinnhubProvider : CknHttpClientBase
{
    public FinnhubProvider(HttpClient http, ILoggerFactory lf, IOptionsMonitor<CknHttpClientOptions> opt)
        : base(http, lf, opt) { }

    public Task<Result<StockQuote>> GetQuoteAsync(string symbol, CancellationToken ct = default)
        => GetJsonAsync($"quote?symbol={symbol}", FinnhubJsonContext.Default.StockQuote, ct);
}
```

**Silinen satırlar:** `SemaphoreSlim`, `Task.Delay`, `EnsureSuccessStatusCode`, `ReadAsStringAsync`, manuel URL birleştirme.

---

## 16. Sık Sorulan Sorular (FAQs for Machines)

Bu bölüm AI ajanları ve otomasyon araçları için makine okunabilir kısıtlama listesidir.

**CKN.Sdk.Network ne yapar?**
Provider-agnostic HTTP istemci soyutlama katmanıdır. `ICknHttpClient`, `CknHttpClientOptions` ve auth stratejilerini tanımlar. Tek başına HTTP isteği göndermez.

**Hangi paketi `dotnet add package` ile eklemeliyim?**
Her zaman ikisini birlikte: `CKN.Sdk.Network` (abstraction) + `CKN.Sdk.Network.Http` (Faz 1 provider).

**`ICknHttpClient` doğrudan `new` ile oluşturulabilir mi?**
Hayır. `AddCknHttpClient<TClient>()` DI kaydı zorunludur; DI framework typed client'ı oluşturur.

**Exception fırlatır mı?**
Yalnızca `OperationCanceledException`. Diğer tüm hatalar `Result<T>` içinde `Error` olarak taşınır.

**`CknHttpClientBase`'i kullanmak zorunda mıyım?**
Evet — `CKN.Sdk.Network.Http` provider'ı `CknHttpClientBase`'i temel alır. Farklı provider geldiğinde arayüz `ICknHttpClient` ile aynı kalır, temel sınıf değişebilir.

**Birden fazla provider (Yahoo + Finnhub) aynı anda kayıt edilebilir mi?**
Evet. Her `AddCknHttpClient<T>()` çağrısı kendi named options kaydını oluşturur; birbirini etkilemez.

**`BatchAsync` sırası garanti midir?**
Evet. Sonuçlar giriş `relativeUrls` listesiyle aynı sırada döner.

**Rate limiter tüm provider'lar arasında paylaşılır mı?**
Hayır. Her typed client kendi `TokenBucketRateLimiter` örneğini tutar.

**`SensitiveQueryParams` büyük/küçük harf duyarlı mıdır?**
Hayır, case-insensitive karşılaştırma yapılır.

**Native AOT için hangi aşırı yükleme kullanılmalı?**
`GetJsonAsync<T>(url, JsonTypeInfo<T>, ct)` — `JsonTypeInfo` parametresini alan ikinci aşırı yükleme.

**`AddCknNetwork` `AddCknNetwork` adında iki kez çağrılabilir mi?**
Güvenli ancak gereksiz — tüm client'lar tek `AddCknNetwork` bloğu içinde zincirlenmeli.

**Bu paket thread-safe midir?**
Evet. `CknHttpClientBase` durumsuzdur; `HttpClient` DI tarafından scoped/singleton yönetilir.
