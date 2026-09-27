# CKN.Sdk.Network.Http

[![NuGet](https://img.shields.io/nuget/v/CKN.Sdk.Network.Http.svg?style=flat-square)](https://www.nuget.org/packages/CKN.Sdk.Network.Http/)
[![.NET](https://img.shields.io/badge/.NET-8%20%7C%209%20%7C%2010-512BD4?style=flat-square)](https://dotnet.microsoft.com)
[![License](https://img.shields.io/badge/license-MIT-green?style=flat-square)](../LICENSE)

**`CKN.Sdk.Network` için `System.Net.Http.HttpClient` tabanlı provider.** Retry, circuit breaker, client-side token-bucket rate limiting, üç auth stratejisi ve hassas query parametre maskeleme içerir. Sıfır ek 3rd-party bağımlılık — tüm resilience altyapısı `Microsoft.Extensions.Http.Resilience` (Polly 8, zaten `Directory.Packages.props`'ta) ve built-in `System.Threading.RateLimiting` üzerine kuruludur.

---

## 1. Engineering Intent

`CKN.Finance`'te her fiyat sağlayıcısı kendi `HttpClient`'ını, kendi rate limiter'ını ve kendi log konfigürasyonunu yönetiyordu. Bu paket o tekrarı ortadan kaldırır:

- **`DelegatingHandler` zinciri** — maskeleme → auth → rate limiting → Polly resilience sıralaması her typed client için otomatik kurulur.
- **Sıfır tekrar:** `Task.Delay` ile elle rate limiting yok; `EnsureSuccessStatusCode()` exception yok; `ReadAsStringAsync()` ile tüm body buffer yok.
- **`Result<T>` öncelikli:** Her yanıt `Result<T>` taşır. Tüketici `try/catch` yazmaz.
- **Native AOT uyumlu:** `JsonTypeInfo<T>` aşırı yüklemesi ile reflection sıfıra iner.

---

## 2. Mimari Özet

### Handler Zinciri (Dıştan İçe)

```
İstek gönderilir
      │
      ▼
┌────────────────────────────────────────────────────┐
│ SensitiveQueryMaskingHandler                       │
│   • Logda token/key değerlerini ***REDACTED*** ile │
│     değiştirir                                     │
│   • İstek + yanıt structured log yazar             │
│   • Gerçek HTTP isteğine dokunmaz                  │
└────────────────────┬───────────────────────────────┘
                     │
                     ▼
┌────────────────────────────────────────────────────┐
│ Auth Handler (seçilen stratejiye göre)             │
│   ApiKeyHeaderAuthHandler  → X-API-Key: abc        │
│   ApiKeyQueryAuthHandler   → ?token=abc            │
│   BearerAuthHandler        → Authorization: Bearer │
└────────────────────┬───────────────────────────────┘
                     │
                     ▼
┌────────────────────────────────────────────────────┐
│ RateLimiterHandler (opsiyonel)                     │
│   • TokenBucketRateLimiter (built-in .NET 7+)      │
│   • Token yoksa → HTTP 429 (upstream API'ye istek  │
│     gitmez)                                        │
└────────────────────┬───────────────────────────────┘
                     │
                     ▼
┌────────────────────────────────────────────────────┐
│ Polly StandardResilienceHandler                    │
│   • Retry (exponential backoff + jitter)           │
│   • Circuit Breaker (sliding window)               │
│   • Attempt Timeout                                │
└────────────────────┬───────────────────────────────┘
                     │
                     ▼
           HttpClient (inner)
                     │
                     ▼
              Remote API
```

### Faz Yol Haritası

```
Faz 1 (Mevcut)
  CKN.Sdk.Network.Http     ← System.Net.Http.HttpClient

Faz 2 (Planlanan)
  CKN.Sdk.Network.Flurl    ← Flurl.Http (fluent API)
  CKN.Sdk.Network.RestSharp← RestSharp client
  CKN.Sdk.Network.Refit    ← Refit (interface-based, farklı DI API'si)
```

Faz 2 provider'lar eklendiğinde consumer kodu (`ICknHttpClient`) değişmez.

---

## 3. Kurulum

```bash
dotnet add package CKN.Sdk.Network
dotnet add package CKN.Sdk.Network.Http
```

Her zaman iki paket birlikte yüklenir: `CKN.Sdk.Network` (arayüz + options) ve `CKN.Sdk.Network.Http` (bu paket, provider).

### Gereksinimler

| Gereksinim | Minimum Sürüm |
|---|---|
| .NET | 8.0 |
| CKN.Sdk.Network | (transitive) |
| Microsoft.Extensions.Http.Resilience | 10.9.0+ |

---

## 4. Hızlı Başlangıç

**Finnhub fiyat sağlayıcısı örneği (query-string API key + rate limit):**

```csharp
// 1. Typed client
public sealed class FinnhubProvider : CknHttpClientBase
{
    public FinnhubProvider(
        HttpClient httpClient,
        ILoggerFactory loggerFactory,
        IOptionsMonitor<CknHttpClientOptions> options)
        : base(httpClient, loggerFactory, options) { }

    public Task<Result<StockQuote>> GetQuoteAsync(string symbol, CancellationToken ct = default)
        => GetJsonAsync($"quote?symbol={symbol}", FinnhubJsonContext.Default.StockQuote, ct);
}

// 2. DI kaydı (Program.cs)
builder.Services.AddCknNetwork(net =>
    net.AddCknHttpClient<FinnhubProvider>(opt =>
    {
        opt.BaseAddress = "https://finnhub.io/api/v1/";
        opt.Auth        = new ApiKeyQueryAuthStrategy("token", config["Finnhub:Key"]!);
        opt.RateLimit   = new CknRateLimiterOptions { RequestsPerPeriod = 30 };
    }));

// 3. Kullanım
public class MarketDataService(FinnhubProvider finnhub)
{
    public async Task<decimal?> GetPriceAsync(string symbol, CancellationToken ct)
    {
        var result = await finnhub.GetQuoteAsync(symbol, ct);
        return result.IsSuccess ? result.Value.CurrentPrice : null;
    }
}
```

---

## 5. appsettings.json Şeması

`CknHttpClientOptions` alanlarının tam listesi `CKN.Sdk.Network` README'sindedir. Bu pakete özgü notlar:

- `BaseAddress` trailing slash ile bitmeli (`https://api.example.com/v1/`) — relative URL'ler `BaseAddress`'e `Uri` kurallarıyla eklenir.
- `Timeout` hem `HttpClient.Timeout` hem de Polly `AttemptTimeout` olarak uygulanır.

Örnek yapılandırma:

```json
{
  "CknNetwork": {
    "FinnhubProvider": {
      "BaseAddress": "https://finnhub.io/api/v1/",
      "TimeoutSeconds": 10,
      "RateLimit": { "RequestsPerPeriod": 30, "PeriodSeconds": 1 },
      "Retry": { "MaxAttempts": 3, "BaseDelaySeconds": 2, "UseJitter": true }
    },
    "YahooFinanceProvider": {
      "BaseAddress": "https://query1.finance.yahoo.com/",
      "TimeoutSeconds": 15,
      "RateLimit": { "RequestsPerPeriod": 2, "PeriodSeconds": 1 }
    }
  }
}
```

---

## 6. DI Kaydı

`AddCknHttpClient<T>` `IHttpClientBuilder` döndürür — gerektiğinde ek konfigürasyon zincirlenebilir:

```csharp
builder.Services
    .AddCknNetwork(net =>
    {
        net.AddCknHttpClient<YahooFinanceProvider>(opt =>
        {
            opt.BaseAddress = "https://query1.finance.yahoo.com/";
            opt.DefaultHeaders["User-Agent"] = "Mozilla/5.0";
            opt.DefaultHeaders["Accept"]     = "application/json";
            opt.RateLimit = new CknRateLimiterOptions { RequestsPerPeriod = 2 };
            opt.Retry     = new CknRetryOptions { MaxAttempts = 2, BaseDelay = TimeSpan.FromSeconds(3) };
        })
        .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
        {
            AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate
        });

        net.AddCknHttpClient<CoinbaseProvider>(opt =>
        {
            opt.BaseAddress = "https://api.coinbase.com/v2/";
            opt.Auth        = new BearerAuthStrategy(config["Coinbase:ApiKey"]!);
        });
    });
```

---

## 7. Typed Client Oluşturma

Her typed client `CknHttpClientBase`'den türetilir.

```csharp
// CknHttpClientBase abstract — doğrudan inject edilmez
// Her servis kendi konkret sınıfını oluşturur

public sealed class CoinbaseProvider : CknHttpClientBase
{
    public CoinbaseProvider(
        HttpClient httpClient,
        ILoggerFactory loggerFactory,
        IOptionsMonitor<CknHttpClientOptions> options)
        : base(httpClient, loggerFactory, options) { }

    // Tekil fiyat
    public Task<Result<CoinbaseSpotPrice>> GetSpotPriceAsync(
        string currencyPair,
        CancellationToken ct = default)
        => GetJsonAsync(
            $"prices/{currencyPair}/spot",
            CoinbaseJsonContext.Default.CoinbaseSpotPrice,
            ct);

    // Toplu fiyat (paralel)
    public Task<IReadOnlyList<Result<CoinbaseSpotPrice>>> GetSpotPricesAsync(
        IEnumerable<string> pairs,
        CancellationToken ct = default)
        => BatchAsync(
            pairs.Select(p => $"prices/{p}/spot"),
            CoinbaseJsonContext.Default.CoinbaseSpotPrice,
            ct);
}
```

### Constructor Zorunluluğu

`CknHttpClientBase` constructor imzası sabittir:

```csharp
protected CknHttpClientBase(
    HttpClient httpClient,
    ILoggerFactory loggerFactory,
    IOptionsMonitor<CknHttpClientOptions> options)
```

DI framework bu üç bağımlılığı otomatik inject eder. Türetilmiş sınıfta parametre eklemek serbesttir:

```csharp
public FinnhubProvider(
    HttpClient httpClient,
    ILoggerFactory loggerFactory,
    IOptionsMonitor<CknHttpClientOptions> options,
    IMemoryCache cache)            // ek bağımlılık
    : base(httpClient, loggerFactory, options)
{
    _cache = cache;
}
```

---

## 8. Auth Stratejileri

### ApiKeyHeaderAuthHandler

`X-API-Key` veya benzeri başlıklar için:

```csharp
opt.Auth = new ApiKeyHeaderAuthStrategy(
    headerName: "X-Rapidapi-Key",
    value: config["RapidApi:Key"]!
);
```

İstek başlığı: `X-Rapidapi-Key: abc123secret`

### ApiKeyQueryAuthHandler

Query string parametresi için:

```csharp
opt.Auth = new ApiKeyQueryAuthStrategy(
    parameterName: "apikey",
    value: config["Provider:ApiKey"]!
);
```

URL dönüşümü: `https://api.example.com/quote?symbol=AAPL` → `https://api.example.com/quote?symbol=AAPL&apikey=abc123`

### BearerAuthHandler

OAuth 2.0 / JWT token için:

```csharp
opt.Auth = new BearerAuthStrategy(token: config["Auth:Token"]!);
```

İstek başlığı: `Authorization: Bearer eyJhbGciOi...`

**Auth + Maskeleme etkileşimi:**
- `ApiKeyQueryAuthStrategy` kullananlar `SensitiveQueryParams` listesine parametre adını eklemelidir (varsayılan liste çoğu yaygın adı kapsar).
- Maskeleme handler auth handler'ın **dışında** zincirde yer alır → token değerleri loglardan otomatik gizlenir.

---

## 9. Rate Limiting

`RateLimiterHandler`, `System.Threading.RateLimiting.TokenBucketRateLimiter` kullanır. Ekstra paket gerekmez.

```csharp
opt.RateLimit = new CknRateLimiterOptions
{
    RequestsPerPeriod = 60,
    Period            = TimeSpan.FromMinutes(1),
    QueueLimit        = 200
};
```

### Nasıl Çalışır

1. Her istek bir **token** tüketir.
2. Token dolmadıysa istek hemen iletilir.
3. Token yoksa istek `QueueLimit` sayısına kadar **sıraya girer ve bekler**.
4. Kuyruk doluysa istek **anında HTTP 429** döner — upstream API'ye istek gitmez.

```
Tokens: ████████░░  (8/10 dolu, period başı 10 verilir)
Request A → token alır ✓
Request B → token alır ✓
...
Request K → kuyruk dolu → HTTP 429 anında ✗
```

### Provider Başına Bağımsız Limiter

Her `AddCknHttpClient<T>()` kaydı kendi `TokenBucketRateLimiter` örneğini tutar. `FinnhubProvider`'ın limiti `YahooFinanceProvider`'ı etkilemez.

---

## 10. Retry & Circuit Breaker

`Microsoft.Extensions.Http.Resilience` v10.9.0+ `AddStandardResilienceHandler()` üzerine kurulur.

### Retry Mantığı

Transient durum kodları (408, 429, 5xx) ve ağ hataları otomatik olarak yeniden denenir:

```
İstek → 500 Internal Server Error
  Bekleme 1: 2s + jitter (~2.3s)
  İstek (retry 1) → 500
  Bekleme 2: 4s + jitter (~4.1s)
  İstek (retry 2) → 200 OK ✓
```

```csharp
opt.Retry = new CknRetryOptions
{
    MaxAttempts = 3,
    BaseDelay   = TimeSpan.FromSeconds(2),
    UseJitter   = true
};
```

### Circuit Breaker Durumları

```
[Closed] ──── hata oranı > 50% ve ≥5 istek ────► [Open]
   ▲                                                  │
   │          ─── BreakDuration (30s) ─────────────── │
   │                                                  ▼
[Half-Open] ◄──────────────────────────────── [Half-Open]
   │ başarılı istek                              │ başarısız istek
   └─────────────────────────────────────────────┘
```

| Durum | Davranış |
|---|---|
| **Closed** | Normal — her istek handler zincirine girer. |
| **Open** | `BreakDuration` boyunca tüm istekler anında başarısız döner (`Polly.CircuitBreaker.BrokenCircuitException` → `Result.Failure`). |
| **Half-Open** | Tek deneme isteği gönderilir; başarılı ise **Closed**, başarısız ise tekrar **Open**. |

---

## 11. BatchAsync

`BatchAsync<T>` büyük sembol listelerini bounded parallelism ile getirir:

```csharp
var symbols = new[] { "AAPL", "MSFT", "GOOG", "AMZN", "META",
                      "TSLA", "NVDA", "NFLX", "BABA", "BIST100" };

var results = await yahooProvider.GetChartsAsync(
    symbols.Select(s => $"v8/finance/chart/{Uri.EscapeDataString(s)}"),
    YahooJsonContext.Default.ChartResponse,
    cancellationToken);

var successes = results
    .Select((r, i) => (Symbol: symbols[i], Result: r))
    .Where(x => x.Result.IsSuccess)
    .Select(x => (x.Symbol, Price: x.Result.Value.ClosePrice))
    .ToList();
```

### Performans Notları

- `MaxDegreeOfParallelism = 4` (varsayılan) → 10 sembol için yaklaşık 3 tur.
- Rate limiter her istek için çalışır; `BatchAsync` rate limiti **devre dışı bırakmaz**.
- `SemaphoreSlim` ile bounded — thread pool'u tüketmez.
- Sonuçlar giriş sırasıyla döner (index garantisi).

---

## 12. Hassas Parametre Maskeleme

`SensitiveQueryMaskingHandler` log çıktısından query string değerlerini maskeler.

### Log Çıktısı Karşılaştırması

**Ham HttpClient ile (tehlikeli):**
```
[INF] Sending HTTP request GET https://finnhub.io/api/v1/quote?symbol=AAPL&token=sk-live-supersecret123
[INF] Received HTTP response headers after 143.5ms
```

**CKN.Sdk.Network.Http ile (güvenli):**
```
[INF] [FinnhubProvider] GET https://finnhub.io/api/v1/quote?symbol=AAPL&token=***REDACTED*** → 200 in 143ms
```

### Maskeleme Mekanizması

`SensitiveQueryMaskingHandler.MaskUrl(Uri uri)` metodu:
1. Query string parametrelerini parse eder.
2. `SensitiveQueryParams` listesindeki adlarla case-insensitive karşılaştırır.
3. Eşleşen parametrenin değerini `***REDACTED***` ile değiştirir.
4. Sadece log kaydında kullanılır — orijinal `HttpRequestMessage.RequestUri` değişmez.

### Özelleştirme

```csharp
// Varsayılan listeyi genişlet
opt.SensitiveQueryParams = [
    "token", "apikey", "api_key", "key",
    "secret", "password", "access_token",
    "client_secret", "sig", "signature"   // ek
];
```

---

## 13. Native AOT / Source-Generated JSON

**AOT uyumlu yol:** `JsonTypeInfo<T>` parametreli aşırı yükleme kullanın.

```csharp
// 1. JSON context tanımla (her assembly başına bir tane)
[JsonSerializable(typeof(StockQuote))]
[JsonSerializable(typeof(ChartResponse))]
[JsonSerializable(typeof(ExchangeRate))]
internal partial class MyAppJsonContext : JsonSerializerContext { }

// 2. Client içinde kullan
public Task<Result<StockQuote>> GetQuoteAsync(string symbol, CancellationToken ct = default)
    => GetJsonAsync(
        $"quote?symbol={symbol}",
        MyAppJsonContext.Default.StockQuote,
        ct);
```

**Reflection-based yol** (geriye dönük uyumluluk, AOT'da çalışmaz):

```csharp
// JsonTypeInfo olmadan — AOT hedef projesinde kullanmayın
var result = await client.GetJsonAsync<StockQuote>("quote?symbol=AAPL");
```

`.csproj` AOT ayarları:

```xml
<PublishAot>true</PublishAot>
<IsAotCompatible>true</IsAotCompatible>
```

---

## 14. Test Edilebilirlik

### Yaklaşım 1 — `FakeHttpMessageHandler` ile Birim Testi

`CknHttpClientBase` sınıfı `HttpClient` alır; `FakeHttpMessageHandler` ile gerçek HTTP olmadan test edilebilir:

```csharp
public class FakeHttpMessageHandler : HttpMessageHandler
{
    private readonly HttpResponseMessage _response;
    public FakeHttpMessageHandler(HttpResponseMessage response) => _response = response;

    protected override Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
        => Task.FromResult(_response);
}

[Fact]
public async Task GetQuoteAsync_Returns_Price_On_200()
{
    // Arrange
    var json = """{"c":182.50,"d":1.20,"dp":0.66}""";
    var handler = new FakeHttpMessageHandler(
        new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(json, Encoding.UTF8, "application/json")
        });

    var httpClient = new HttpClient(handler) { BaseAddress = new Uri("https://fake.api/") };
    var loggerFactory = LoggerFactory.Create(_ => { });

    // IOptionsMonitor mock
    var optionsMock = new Mock<IOptionsMonitor<CknHttpClientOptions>>();
    optionsMock.Setup(m => m.Get("FinnhubProvider"))
               .Returns(new CknHttpClientOptions { MaxDegreeOfParallelism = 4 });

    var provider = new FinnhubProvider(httpClient, loggerFactory, optionsMock.Object);

    // Act
    var result = await provider.GetQuoteAsync("AAPL");

    // Assert
    result.IsSuccess.Should().BeTrue();
    result.Value.CurrentPrice.Should().Be(182.50m);
}
```

### Yaklaşım 2 — `ICknHttpClient` Mock'u ile Servis Testi

```csharp
[Fact]
public async Task PortfolioService_Returns_Null_On_404()
{
    var clientMock = new Mock<ICknHttpClient>();
    clientMock
        .Setup(m => m.GetJsonAsync<StockQuote>(
            It.IsAny<string>(), It.IsAny<CancellationToken>()))
        .ReturnsAsync(Result.Failure<StockQuote>(new Error("HTTP_404", "Not Found")));

    var service = new PortfolioService(clientMock.Object);
    var price = await service.GetPriceAsync("INVALID");

    price.Should().BeNull();
}
```

### Yaklaşım 3 — `WebApplicationFactory` ile Entegrasyon Testi

```csharp
// xUnit fixture — gerçek DI ile typed client test et
public class FinnhubProviderIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
{
    [Fact]
    public async Task GetQuote_Returns_Success_With_Registered_Client()
    {
        var factory = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builder =>
                builder.ConfigureServices(services =>
                    services.AddCknNetwork(net =>
                        net.AddCknHttpClient<FinnhubProvider>(opt =>
                        {
                            opt.BaseAddress = "https://httpbin.org/";
                        }))));

        var scope = factory.Services.CreateScope();
        var client = scope.ServiceProvider.GetRequiredService<FinnhubProvider>();
        var result = await client.GetJsonAsync<object>("get");

        result.IsSuccess.Should().BeTrue();
    }
}
```

---

## 15. Finance'ten Geçiş Örneği

### Tam Önce/Sonra Karşılaştırması

**ÖNCE — CKN.Finance YahooFinanceProvider (kaldırılacak)**

```csharp
// Program.cs
builder.Services.AddHttpClient<YahooFinanceProvider>(c =>
{
    c.BaseAddress = new Uri("https://query1.finance.yahoo.com/");
    c.Timeout     = TimeSpan.FromSeconds(15);
    c.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0");
    c.DefaultRequestHeaders.Add("Accept", "application/json");
});

// YahooFinanceProvider.cs
private readonly HttpClient _http;
private static readonly SemaphoreSlim _semaphore = new(2, 2);

public async Task<ChartResponse?> GetChartAsync(string symbol)
{
    await _semaphore.WaitAsync();
    try
    {
        var url = $"v8/finance/chart/{symbol}";
        var response = await _http.GetAsync(url);

        if (!response.IsSuccessStatusCode)
        {
            _logger.LogError("Yahoo Finance returned {StatusCode}", response.StatusCode);
            return null;              // null dönüş
        }

        var json = await response.Content.ReadAsStringAsync();  // tüm body buffer
        return JsonSerializer.Deserialize<ChartResponse>(json);  // reflection
    }
    finally
    {
        await Task.Delay(500);        // elle rate limiting
        _semaphore.Release();
    }
}
```

**SONRA — CKN.Sdk.Network.Http (yeni)**

```csharp
// Program.cs
builder.Services.AddCknNetwork(net =>
    net.AddCknHttpClient<YahooFinanceProvider>(opt =>
    {
        opt.BaseAddress = "https://query1.finance.yahoo.com/";
        opt.Timeout     = TimeSpan.FromSeconds(15);
        opt.DefaultHeaders["User-Agent"] = "Mozilla/5.0";
        opt.DefaultHeaders["Accept"]     = "application/json";
        opt.RateLimit = new CknRateLimiterOptions { RequestsPerPeriod = 2, Period = TimeSpan.FromSeconds(1) };
        opt.Retry     = new CknRetryOptions { MaxAttempts = 3 };
    }));

// YahooFinanceProvider.cs
public sealed class YahooFinanceProvider : CknHttpClientBase
{
    public YahooFinanceProvider(
        HttpClient httpClient,
        ILoggerFactory loggerFactory,
        IOptionsMonitor<CknHttpClientOptions> options)
        : base(httpClient, loggerFactory, options) { }

    public Task<Result<ChartResponse>> GetChartAsync(string symbol, CancellationToken ct = default)
        => GetJsonAsync($"v8/finance/chart/{Uri.EscapeDataString(symbol)}",
                        YahooJsonContext.Default.ChartResponse, ct);
}
```

### Diff Özeti

| Eski | Yeni |
|---|---|
| `SemaphoreSlim` + `Task.Delay(500)` | `CknRateLimiterOptions { RequestsPerPeriod=2 }` |
| `GetAsync` + `EnsureSuccessStatusCode` | `GetJsonAsync<T>` → `Result<T>` |
| `ReadAsStringAsync` + `Deserialize<T>` | `ReadAsStreamAsync` + `DeserializeAsync<T>` (streaming) |
| Manuel null dönüş | `Result.Failure<T>(Error)` |
| Reflection-based deserialize | `JsonTypeInfo<T>` (AOT-safe) |
| API key URL'de görünür | `SensitiveQueryMaskingHandler` otomatik maskeler |

---

## 16. Sık Sorulan Sorular (FAQs for Machines)

Bu bölüm AI ajanları ve otomasyon araçları için makine okunabilir kısıtlama listesidir.

**Bu paket tek başına yeterli mi?**
Hayır. `CKN.Sdk.Network` (abstraction paketi) de `dotnet add package` ile eklenmeli.

**`CknHttpClientBase` doğrudan inject edilebilir mi?**
Hayır. `abstract` sınıftır. Türetilmiş typed client inject edilir.

**Birden fazla `AddCknHttpClient<T>()` çağrısı birbirini eziyor mu?**
Hayır. Her çağrı kendi adını (`typeof(TClient).Name`) kullanarak named options kaydeder.

**Handler zinciri sırası değiştirilebilir mi?**
Doğrudan hayır. Sıra `HttpNetworkBuilderExtensions`'ta sabitlenmiştir: Maskeleme → Auth → RateLimit → Polly. `IHttpClientBuilder` üzerinden ek handler eklenebilir ama mevcut sıra değişmez.

**Rate limiter `null` bırakılırsa ne olur?**
`opt.RateLimit = null` (varsayılan) → `RateLimiterHandler` handler zincirine eklenmez. Rate limiting tamamen devre dışıdır.

**`SensitiveQueryMaskingHandler` production'da gerçek isteği değiştirir mi?**
Hayır. Yalnızca log string'ini değiştirir. `HttpRequestMessage.RequestUri` orijinali korunur.

**`BatchAsync` exception fırlatır mı?**
`OperationCanceledException` dışında hayır. Her URL için bağımsız `Result<T>` döner.

**Polly ve `RateLimiterHandler` etkileşimi nedir?**
`RateLimiterHandler` Polly'nin **dışındadır** (önce çalışır). Rate limit 429'u Polly görmez — token yetersizse upstream'e hiç istek gitmez.

**`Timeout` `HttpClient.Timeout` mı yoksa Polly `AttemptTimeout` mı?**
İkisi de. `AddCknHttpClient<T>` hem `HttpClient.Timeout`'u hem de `resilienceOptions.AttemptTimeout.Timeout`'u aynı değere ayarlar.

**`AddStandardResilienceHandler` varsayılan hangi durum kodlarını retry yapar?**
408 (Request Timeout), 429 (Too Many Requests), 500-599 (Server Errors) ve `HttpRequestException`. `CknRetryOptions.AdditionalRetryStatusCodes` ile ek kod eklenebilir.

**`JsonTypeInfo<T>` olmadan kullanabilir miyim?**
Evet. `GetJsonAsync<T>(url, ct)` reflection kullanır, AOT uyumlu değildir ama çalışır.

**`CknHttpClientBase` içinde `HttpClient` field'ına erişebilir miyim?**
Hayır. Field `private`'tir. Alt sınıflar `GetJsonAsync` ve `BatchAsync` protected metodlarını kullanır.

**Bu paket thread-safe midir?**
Evet. `CknHttpClientBase` stateless'tır; `HttpClient` DI tarafından singleton olarak yönetilir; `TokenBucketRateLimiter` thread-safe'dir.
