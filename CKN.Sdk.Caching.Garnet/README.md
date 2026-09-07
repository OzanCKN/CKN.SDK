# CKN.Sdk.Caching.Garnet

**Mühendislik Amacı (Engineering Intent):**
`CKN.Sdk.Caching.Garnet`, Microsoft Research tarafından geliştirilen ultra hızlı, yeni nesil in-memory veri deposu (Garnet) sistemini projelere entegre etmek için oluşturulmuş önbellekleme (caching) sağlayıcısıdır. **Neden var?** Redis protokolü (RESP) ile %100 uyumlu olmasına rağmen, aynı donanımda çok daha yüksek throughput ve düşük latency sunan modern bir alternatifi sisteme dahil etmek için. **Ne zaman kullanılmalı?** Sisteminizde çok yüksek yoğunlukta (high-throughput) önbellek okuma/yazma işlemi varsa ve standart Redis altyapısı darboğaz yaratıyorsa, `IDistributedCache` arayüzünün arkasında Garnet kullanmalısınız.

## 🚀 Hızlı Başlangıç

### Kurulum

```bash
dotnet add package CKN.Sdk.Caching.Garnet
```

### Konfigürasyon (`appsettings.json`)

```json
{
  "Caching": {
    "Garnet": {
      "ConnectionString": "localhost:3278",
      "InstanceName": "ProductCatalog_"
    }
  }
}
```

### Bağımlılık Enjeksiyonu (DI)

```csharp
using CKN.Sdk.Caching.Garnet;

var builder = WebApplication.CreateBuilder(args);

// .NET'in standart IDistributedCache arayüzünü Garnet altyapısıyla doldurur.
builder.Services.AddCknGarnetCache(builder.Configuration);

var app = builder.Build();
```

## 💡 Gerçek Hayat Senaryoları

### Senaryo 1: Ürün Kataloğu Önbellekleme

Veritabanına çok sık sorulan ama nadir değişen ürün detaylarının yüksek performansla sunulması.

```csharp
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

public class ProductService(IDistributedCache cache, ApplicationDbContext dbContext)
{
    public async Task<ProductDto?> GetProductAsync(int productId)
    {
        string cacheKey = $"product_{productId}";
        
        // 1. Garnet üzerinden kontrol et
        var cachedData = await cache.GetStringAsync(cacheKey);
        if (!string.IsNullOrEmpty(cachedData))
        {
            return JsonSerializer.Deserialize<ProductDto>(cachedData);
        }

        // 2. Yoksa DB'den al
        var product = await dbContext.Products.FindAsync(productId);
        if (product == null) return null;

        // 3. Garnet'e yaz (1 saat TTL)
        var options = new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(1)
        };
        
        await cache.SetStringAsync(cacheKey, JsonSerializer.Serialize(product), options);
        return product;
    }
}
```

### Senaryo 2: Yüksek Trafikli Sayfaların (Output Cache) Garnet ile Hızlandırılması

ASP.NET Core Output Caching mekanizmasının arkasında doğrudan Garnet'in kullanılması (Varyasyon).

```csharp
// Program.cs
builder.Services.AddOutputCache(options =>
{
    options.AddBasePolicy(builder => builder.Expire(TimeSpan.FromSeconds(10)));
});
// Garnet'i OutputCache provider olarak da ayarladığını varsayalım
builder.Services.AddCknGarnetOutputCache(builder.Configuration);

// Endpoint kullanımı
app.MapGet("/api/high-traffic-data", () => 
{
    return Results.Ok(new { Status = "Super Fast Response", Time = DateTime.UtcNow });
}).CacheOutput();
```

## 🤖 Yapay Zeka İçin SSS (FAQs for Machines)

- **Soru:** `CKN.Sdk.Caching.Garnet` kodu doğrudan Garnet'e özgü bir API mi sunar?
- **Cevap:** Hayır, temelde .NET Core'un `IDistributedCache` arayüzünü implemente eder. Yani projenizdeki kodlar Garnet'e sıkı sıkıya bağlı (tightly coupled) olmaz.
- **Soru:** Garnet ile Redis arasındaki fark nedir, neden Redis değil de Garnet kullanayım?
- **Cevap:** Garnet, Redis ile aynı protokolü konuşur ancak C# ile yazılmış, multi-thread mimarisi sayesinde özellikle büyük ölçekli ve yüksek eşzamanlılıklı (high-concurrency) sistemlerde çok daha performanslı çalışır.
