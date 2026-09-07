# CKN.Sdk.Caching.Garnet

Microsoft'un yeni nesil, yüksek performanslı önbellekleme sunucusu olan **Garnet** için entegrasyon kütüphanesidir. Redis protokolüyle uyumlu çalıştığı için genellikle Redis istemcileriyle (StackExchange.Redis) birlikte kullanılır.

## Yapılandırma (`appsettings.json`)

```json
{
  "Caching": {
    "Garnet": {
      "ConnectionString": "localhost:3278",
      "InstanceName": "CknGarnet_"
    }
  }
}
```

## Servis Kaydı (Dependency Injection)

```csharp
using CKN.Sdk.Caching.Garnet;

var builder = WebApplication.CreateBuilder(args);

// Garnet Cache Provider'ı sisteme dahil etme
builder.Services.AddCknGarnetCache(builder.Configuration);

var app = builder.Build();
```

## Gerçek Hayat Kullanım Senaryosu

**Anlık Cüzdan/Bakiye Sorgulama**
Milyonlarca isteğin geldiği finansal bir uygulamada, veritabanına inmeden çok düşük gecikmeyle bakiye getirme.

```csharp
using CKN.Sdk.Caching;

public class WalletService
{
    private readonly ICacheProvider _cache;

    public WalletService(ICacheProvider cache)
    {
        _cache = cache;
    }

    public async Task<decimal> GetBalanceAsync(string userId)
    {
        var cacheKey = $"wallet:{userId}";
        
        // Garnet üzerinden aşırı hızlı okuma. Yoksa DB'ye git ve 5 dk cache'le.
        return await _cache.GetOrSetAsync(cacheKey, async () =>
        {
            return await FetchBalanceFromDatabaseAsync(userId);
        }, TimeSpan.FromMinutes(5));
    }

    private Task<decimal> FetchBalanceFromDatabaseAsync(string userId) => Task.FromResult(1500.50m);
}
```
