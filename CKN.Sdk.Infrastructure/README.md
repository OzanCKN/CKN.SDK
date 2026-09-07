# CKN.Sdk.Infrastructure

CKN.Sdk içerisinde, tüm projelerde ortak olarak kullanılabilecek **Rate Limiting (Kısıtlama), KeyVault (Gizli Veri Yönetimi), HttpClient Fallback (Dayanıklılık), Webhook Yönlendirmeleri** ve **OpenTelemetry (Gözlemlenebilirlik)** gibi altyapı bileşenlerini barındıran kütüphanedir.

## Servis Kayıtları ve Kullanım (Dependency Injection)

```csharp
using CKN.Sdk.Infrastructure.Configuration;
using CKN.Sdk.Infrastructure.Http;
using CKN.Sdk.Infrastructure.RateLimiting;
using CKN.Sdk.Infrastructure.Telemetry;

var builder = WebApplication.CreateBuilder(args);

// 1. Azure KeyVault'u konfigürasyona dahil etmek (Appsettings şifrelerini KeyVault'tan okur)
builder.AddCknAzureKeyVault(new Uri("https://ckn-vault.vault.azure.net/"));

// 2. Polly v8 destekli Fallback (Zarif Düşüş) mekanizmasına sahip HttpClient kaydı
builder.Services.AddHttpClient("CknExternalApi", client =>
{
    client.BaseAddress = new Uri("https://api.external.com");
})
.AddCknFallbackHandler(new HttpResponseMessage(System.Net.HttpStatusCode.OK)
{
    Content = new StringContent("{ \"status\": \"fallback\", \"message\": \"Geçici süreliğine varsayılan veri gösteriliyor.\" }")
});

// 3. Rate Limiting (Hız Sınırlandırması) ekleme
builder.Services.AddCknRateLimiting();

// 4. OpenTelemetry (Metrikler ve İzleme) ekleme
// Jaeger, Prometheus vb. platformlara veri gönderimini (OTLP) aktifleştirir.
builder.Services.AddCKNTelemetry(serviceName: "MyCknMicroservice", otlpEndpoint: "http://localhost:4317");

var app = builder.Build();

// Rate limiting middleware'i aktif et
app.UseRateLimiter();
```

## Gerçek Hayat Kullanım Senaryosu

**Dış API Çağrısında Fallback**
Üçüncü parti bir döviz kuru API'si çöktüğünde uygulamanın hataya düşmemesi için önceden tanımlanmış sabit bir kur verisinin döndürülmesi.

```csharp
public class CurrencyService
{
    private readonly HttpClient _client;

    public CurrencyService(IHttpClientFactory httpClientFactory)
    {
        // AddCknFallbackHandler ile kaydedilmiş istemci
        _client = httpClientFactory.CreateClient("CknExternalApi");
    }

    public async Task<string> GetExchangeRatesAsync()
    {
        // Eğer dış API 500 dönerse veya Timeout olursa, 
        // FallbackHandler devreye girip varsayılan JSON'ı dönecektir.
        var response = await _client.GetAsync("/v1/rates");
        return await response.Content.ReadAsStringAsync();
    }
}
```
