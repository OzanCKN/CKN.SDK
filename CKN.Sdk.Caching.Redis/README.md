# CKN.Sdk.Caching.Redis

Endüstri standardı olan **Redis** için entegrasyon kütüphanesidir. Dağıtık önbellekleme (Distributed Cache), Pub/Sub mesajlaşma ve gelişmiş veri yapıları (Hashes, Sets, Sorted Sets) için kullanılır.

## Yapılandırma (`appsettings.json`)

```json
{
  "Caching": {
    "Redis": {
      "ConnectionString": "localhost:6379,abortConnect=false",
      "InstanceName": "CknApp_"
    }
  }
}
```

## Servis Kaydı (Dependency Injection)

```csharp
using CKN.Sdk.Caching.Redis;

var builder = WebApplication.CreateBuilder(args);

// Redis Cache Provider'ı sisteme dahil etme
builder.Services.AddCknRedisCache(builder.Configuration);

var app = builder.Build();
```

## Gerçek Hayat Kullanım Senaryosu

**Ürün Kataloğu Önbellekleme**
Sık erişilen ürün kategorilerinin veya ürün detaylarının veritabanı yorgunluğunu almak için Redis'te tutulması.

```csharp
using CKN.Sdk.Caching;

public class ProductCatalogService
{
    private readonly ICacheProvider _cache;

    public ProductCatalogService(ICacheProvider cache)
    {
        _cache = cache;
    }

    public async Task<List<Product>> GetTopSellingProductsAsync()
    {
        // En çok satanları getir. Yoksa DB'den al ve 10 dk boyunca Redis'te sakla.
        return await _cache.GetOrSetAsync("catalog:topselling", async () =>
        {
            return await FetchFromDbAsync();
        }, TimeSpan.FromMinutes(10));
    }
    
    private Task<List<Product>> FetchFromDbAsync() => Task.FromResult(new List<Product>());
}
```
