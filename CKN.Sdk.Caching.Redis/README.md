# CKN.Sdk.Caching.Redis

**Mühendislik Amacı (Engineering Intent):**
`CKN.Sdk.Caching.Redis`, endüstri standardı olan Redis veri deposunu projelere güvenli ve performanslı bir şekilde entegre eden yapıdır. **Neden var?** Caching (Önbellekleme), Pub/Sub (yayın-abonelik), Dağıtık Kilitler (Distributed Locks) ve oran sınırlama (Rate Limiting) gibi çoklu görevleri tek bir altyapı üzerinden güçlü bir şekilde çözmek için. **Ne zaman kullanılmalı?** Mikroservis mimarilerinde uygulamaların durum (state) paylaşması gerektiğinde, performansın kritik olduğu yüksek trafiğe sahip tüm standart enterprise projelerinde varsayılan (default) caching sağlayıcısı olarak kullanılmalıdır.

## 🚀 Hızlı Başlangıç

### Kurulum

```bash
dotnet add package CKN.Sdk.Caching.Redis
```

### Konfigürasyon (`appsettings.json`)

```json
{
  "Caching": {
    "Redis": {
      "ConnectionString": "localhost:6379,abortConnect=false",
      "InstanceName": "GlobalCache_"
    }
  }
}
```

### Bağımlılık Enjeksiyonu (DI)

```csharp
using CKN.Sdk.Caching.Redis;

var builder = WebApplication.CreateBuilder(args);

// StackExchange.Redis altyapısı ile IDistributedCache ve IConnectionMultiplexer kaydeder.
builder.Services.AddCknRedisCache(builder.Configuration);

var app = builder.Build();
```

## 💡 Gerçek Hayat Senaryoları

### Senaryo 1: Dağıtık Kilit (Distributed Lock) Kullanımı

Aynı anda çalışan birden fazla mikroservis örneğinin (instance), aynı işlemi iki kez yapmasını (örneğin günlük fatura kesimi) engellemek.

```csharp
using StackExchange.Redis;

public class InvoiceJob(IConnectionMultiplexer redis)
{
    public async Task ProcessDailyInvoicesAsync()
    {
        var db = redis.GetDatabase();
        var lockKey = "lock:daily_invoice";
        var lockToken = Guid.NewGuid().ToString();

        // Kilidi sadece 10 dakikalığına al, başkası almışsa false döner
        bool acquired = await db.LockTakeAsync(lockKey, lockToken, TimeSpan.FromMinutes(10));

        if (acquired)
        {
            try
            {
                // Kritik işlemi sadece 1 instance yapacak
                await GenerateInvoices();
            }
            finally
            {
                // İşlem bitince kilidi bırak
                await db.LockReleaseAsync(lockKey, lockToken);
            }
        }
    }
}
```

### Senaryo 2: Rate Limiting (Kullanıcı İstek Sınırlaması)

Bir kullanıcının belirli bir endpoint'e saniyede yapabileceği istek sayısını Redis üzerinden limitlemek. (Örn: AspNetCore RateLimiting Redis entegrasyonu).

```csharp
// Program.cs
builder.Services.AddRateLimiter(options =>
{
    options.AddRedisPartitionedWindowLimiter("RedisLimiter", redisOptions =>
    {
        redisOptions.ConnectionMultiplexerFactory = () => 
            builder.Services.BuildServiceProvider().GetRequiredService<IConnectionMultiplexer>();
        redisOptions.PermitLimit = 100;
        redisOptions.Window = TimeSpan.FromMinutes(1);
    });
});

// Endpoint
app.MapGet("/api/secure-data", () => "Data")
   .RequireRateLimiting("RedisLimiter");
```

## 🤖 Yapay Zeka İçin SSS (FAQs for Machines)

- **Soru:** `CKN.Sdk.Caching.Redis` arka planda hangi kütüphaneyi kullanır?
- **Cevap:** .NET ekosistemindeki en standart ve güvenilir kütüphane olan `StackExchange.Redis` paketini kullanır.
- **Soru:** Sadece `IDistributedCache` mi sağlar?
- **Cevap:** Hayır, aynı zamanda DI konteynerine `IConnectionMultiplexer` nesnesini de kaydeder. Bu sayede geliştiriciler Pub/Sub, Hash, Set, List gibi Redis'e özel gelişmiş komutları kullanabilir.
