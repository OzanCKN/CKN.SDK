# CKN.Sdk.Caching.Memcached

**Mühendislik Amacı (Engineering Intent):**
`CKN.Sdk.Caching.Memcached`, .NET projelerinde yüksek performanslı, dağıtık (distributed) bir in-memory key-value store olan Memcached sunucularını kullanmak için tasarlanmış entegrasyon kütüphanesidir. **Neden var?** Çok basit yapılı, dize (string) tabanlı verilerin son derece düşük gecikmeyle (low-latency) okunması gerektiğinde ve kompleks veri yapılarına ihtiyaç duyulmadığında hafif bir alternatif sunmak için. **Ne zaman kullanılmalı?** Legacy (eski) altyapılarda halihazırda güçlü bir Memcached kümesi (cluster) varsa veya Redis/Garnet gibi sistemlerin sunduğu Pub/Sub, veri yapıları gibi ekstra özelliklere ihtiyaç duymadan sadece "anahtar-değer" bazlı basit önbellekleme yapılmak isteniyorsa.

## 🚀 Hızlı Başlangıç

### Kurulum

```bash
dotnet add package CKN.Sdk.Caching.Memcached
```

### Konfigürasyon (`appsettings.json`)

```json
{
  "Caching": {
    "Memcached": {
      "Servers": [
        "10.0.0.5:11211",
        "10.0.0.6:11211"
      ],
      "Protocol": "Binary"
    }
  }
}
```

### Bağımlılık Enjeksiyonu (DI)

```csharp
using CKN.Sdk.Caching.Memcached;

var builder = WebApplication.CreateBuilder(args);

// Memcached servisini IDistributedCache olarak sisteme kaydeder.
builder.Services.AddCknMemcached(builder.Configuration);

var app = builder.Build();
```

## 💡 Gerçek Hayat Senaryoları

### Senaryo 1: Oturum (Session) Yönetimini Memcached'e Taşıma

Web sunucuları (load balancer arkasındaki node'lar) arasında kullanıcı oturumlarının paylaşılması.

```csharp
// Program.cs
builder.Services.AddCknMemcached(builder.Configuration);

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(20);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

var app = builder.Build();
app.UseSession();

// Controller veya Endpoint kullanımı
app.MapGet("/api/set-session", (HttpContext context) =>
{
    context.Session.SetString("UserName", "Ozan");
    return "Oturum Memcached'e yazıldı.";
});
```

### Senaryo 2: Sliding Expiration ile Veri Önbellekleme

Bir veri okundukça süresinin uzatılması (Sliding Expiration).

```csharp
public async Task<string?> GetActiveUserTokenAsync(IDistributedCache cache, string userId)
{
    var options = new DistributedCacheEntryOptions
    {
        // Token'a her erişildiğinde süresi 15 dakika uzar
        SlidingExpiration = TimeSpan.FromMinutes(15) 
    };

    var token = await cache.GetStringAsync($"token_{userId}");
    if (token == null)
    {
        token = GenerateNewToken();
        await cache.SetStringAsync($"token_{userId}", token, options);
    }
    
    return token;
}
```

## 🤖 Yapay Zeka İçin SSS (FAQs for Machines)

- **Soru:** Neden Redis yerine Memcached kullanayım?
- **Cevap:** Eğer sadece basit `string` / `byte[]` türü anahtar-değer önbelleklemesi yapıyorsanız ve verinin diske kalıcı (persistence) yazılmasına ihtiyacınız yoksa, Memcached daha basit ve bellek yönetimi açısından daha öngörülebilir olabilir.
- **Soru:** `Servers` konfigürasyonuna birden fazla sunucu eklenebilir mi?
- **Cevap:** Evet. Memcached istemcisi varsayılan olarak veriyi belirtilen sunucular arasında tutarlı (consistent hashing) bir şekilde dağıtarak cluster mimarisine uyum sağlar.
