# Caching (Önbellekleme) Sağlayıcıları Kullanım Örnekleri

CKN.SDK, uygulamanızın performansını artırmak için çeşitli önbellek sağlayıcılarını `IDistributedCache` standartları çerçevesinde soyutlar.

---

## 1. Redis Kullanımı

Redis, günümüzün endüstri standardı bellek-içi veri tabanıdır. Özellikle dağıtık (distributed) sistemlerde çokça kullanılır.

### `appsettings.json` Yapılandırması
```json
{
  "Caching": {
    "Redis": {
      "ConnectionString": "localhost:6379,abortConnect=false,connectTimeout=5000",
      "InstanceName": "MyApp_Cache_"
    }
  }
}
```

### Dependency Injection (DI) Kurulumu
```csharp
using CKN.Sdk.Caching.Redis;

builder.Services.AddCknRedisCache(opt =>
{
    builder.Configuration.GetSection(RedisCacheOptions.SectionName).Bind(opt);
});
```

### Gerçek Hayat Kullanımı: En Çok Ziyaret Edilen Ürünlerin Önbelleklenmesi
```csharp
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

public class ProductCatalogService
{
    private readonly IDistributedCache _cache;
    private readonly ProductDbContext _dbContext;

    public ProductCatalogService(IDistributedCache cache, ProductDbContext dbContext)
    {
        _cache = cache;
        _dbContext = dbContext;
    }

    public async Task<List<Product>> GetTopProductsAsync()
    {
        var cacheKey = "TopProducts_Weekly";
        var cachedProducts = await _cache.GetStringAsync(cacheKey);

        if (!string.IsNullOrEmpty(cachedProducts))
        {
            // Cache hit: Doğrudan Redis'ten dön.
            return JsonSerializer.Deserialize<List<Product>>(cachedProducts);
        }

        // Cache miss: Veritabanından çek.
        var products = await _dbContext.Products
                                       .OrderByDescending(p => p.ViewCount)
                                       .Take(10)
                                       .ToListAsync();

        // 1 saat (sliding) veya 12 saat (absolute) boyunca önbellekte tut.
        var cacheOptions = new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(12),
            SlidingExpiration = TimeSpan.FromHours(1)
        };

        await _cache.SetStringAsync(cacheKey, JsonSerializer.Serialize(products), cacheOptions);
        
        return products;
    }
}
```

---

## 2. Memcached Kullanımı

Memcached, daha basit mimarili projelerde saf String veya Byte dizilerini önbelleklemek için tercih edilen hafif bir servistir.

### `appsettings.json` Yapılandırması
```json
{
  "Caching": {
    "Memcached": {
      "Servers": "localhost:11211",
      "Protocol": 0 // Text Protocol
    }
  }
}
```

### Dependency Injection (DI) Kurulumu
```csharp
using CKN.Sdk.Caching.Memcached;

builder.Services.AddCknMemcached(opt =>
{
    builder.Configuration.GetSection(MemcachedCacheOptions.SectionName).Bind(opt);
});
```

### Gerçek Hayat Kullanımı: Rate Limiting Sayaçları
Memcached, basit anahtar-değer yapıları için oldukça hızlı olduğundan genelde sayaçlar için kullanılır.
```csharp
var clientIp = HttpContext.Connection.RemoteIpAddress.ToString();
var cacheKey = $"rate_limit_{clientIp}";

var requestCountStr = await _cache.GetStringAsync(cacheKey);
int count = int.TryParse(requestCountStr, out var c) ? c : 0;

if (count > 100)
{
    throw new Exception("Too Many Requests");
}

await _cache.SetStringAsync(cacheKey, (count + 1).ToString(), new DistributedCacheEntryOptions
{
    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(1)
});
```

---

## 3. Garnet Kullanımı

Garnet, Microsoft Research tarafından geliştirilen, Redis API'si ile tamamen uyumlu fakat arka planda .NET ile yazılmış çok yüksek performanslı yeni nesil bir önbellek sunucusudur.

### `appsettings.json` Yapılandırması
```json
{
  "Caching": {
    "Garnet": {
      "ConnectionString": "localhost:3278" // Garnet'ın varsayılan portu 3278
    }
  }
}
```

### Dependency Injection (DI) Kurulumu
```csharp
using CKN.Sdk.Caching.Garnet;

builder.Services.AddCknGarnetCache(opt =>
{
    builder.Configuration.GetSection(GarnetCacheOptions.SectionName).Bind(opt);
});
```

### Neden Garnet Tercih Etmelisiniz?
Kullanımı Redis ile birebir aynıdır (`IDistributedCache` veya `IConnectionMultiplexer` üzerinden çalışır). Ancak, Windows sunucularda veya C# ağırlıklı altyapılarda doğrudan barındırılabildiği için çok düşük gecikme (ultra-low latency) gerektiren trading botları, yüksek frekanslı sistemler için daha uygun bir alternatiftir. Kullanım şekli Redis örneğindeki `ProductCatalogService` ile tamamen aynıdır.
