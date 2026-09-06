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

// 1. Standart (Global) Redis Kurulumu
builder.Services.AddCknRedisCache(opt =>
{
    builder.Configuration.GetSection(RedisCacheOptions.SectionName).Bind(opt);
});

// 2. Keyed Services (Çoklu Sunucu) Kurulumu (.NET 8+)
// Farklı amaçlar için (Örn: Sepet işlemleri) tamamen farklı bir Redis sunucusuna/veritabanına bağlanmak:
builder.Services.AddCknKeyedRedisCache("basketCache", opt =>
{
    opt.ConnectionString = "redis-basket-cluster.local:6379,password=secure";
    opt.InstanceName = "BasketApp_";
});
```

### Gerçek Hayat Kullanımı: Hızlı Sepet (Basket) İşlemleri ve Çoklu Önbellek

Büyük (Enterprise) sistemlerde genellikle "Session/Basket" verileri ile "Product/Catalog" verileri aynı Redis üzerinde tutulmaz. CKN.SDK'nın Keyed Services mimarisi ile her iki sunucuya da birbirinden bağımsız olarak bağlanabilirsiniz.

```csharp
using CKN.Sdk.Core.Caching;
using Microsoft.Extensions.DependencyInjection;

public class BasketService
{
    private readonly ICacheService _globalCache;
    private readonly ICacheService _basketCache;

    public BasketService(
        ICacheService globalCache, // Standart AddCknRedisCache üzerinden gelir
        [FromKeyedServices("basketCache")] ICacheService basketCache) // Sadece Sepet sunucusuna gider
    {
        _globalCache = globalCache;
        _basketCache = basketCache;
    }

    public async Task AddToBasketAsync(string userId, BasketItem item)
    {
        var cacheKey = $"basket:{userId}";
        
        // Sepet Redis sunucusuna bağlanıp veriyi çeker/yazar
        var currentBasket = await _basketCache.GetAsync<List<BasketItem>>(cacheKey) ?? new List<BasketItem>();
        currentBasket.Add(item);
        
        await _basketCache.SetAsync(cacheKey, currentBasket, TimeSpan.FromDays(1));
    }
}
```

### En Çok Ziyaret Edilen Ürünlerin Önbelleklenmesi
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
