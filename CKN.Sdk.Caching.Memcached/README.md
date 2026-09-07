# CKN.Sdk.Caching.Memcached

Basit ve çok hızlı bir key-value store olan **Memcached** için entegrasyon kütüphanesidir. Genellikle dağıtık önbellekleme (Distributed Cache) mimarisinde, karmaşık veri yapısı (list, hash vb.) gerekmeyen basit durumlarda tercih edilir.

## Yapılandırma (`appsettings.json`)

```json
{
  "Caching": {
    "Memcached": {
      "Servers": [
        { "Host": "localhost", "Port": 11211 }
      ]
    }
  }
}
```

## Servis Kaydı (Dependency Injection)

```csharp
using CKN.Sdk.Caching.Memcached;

var builder = WebApplication.CreateBuilder(args);

// Memcached Provider'ı sisteme dahil etme
builder.Services.AddCknMemcached(builder.Configuration);

var app = builder.Build();
```

## Gerçek Hayat Kullanım Senaryosu

**Oturum (Session) Yönetimi**
Kullanıcı oturum verilerinin veya e-ticaret sepetlerinin veritabanı yerine Memcached üzerinde tutulması.

```csharp
using CKN.Sdk.Caching;

public class SessionManager
{
    private readonly ICacheProvider _cache;

    public SessionManager(ICacheProvider cache)
    {
        _cache = cache;
    }

    public async Task SaveSessionAsync(string sessionId, UserSession session)
    {
        // Oturumu 2 saatliğine Memcached'de tut
        await _cache.SetAsync($"session:{sessionId}", session, TimeSpan.FromHours(2));
    }

    public async Task<UserSession?> GetSessionAsync(string sessionId)
    {
        return await _cache.GetAsync<UserSession>($"session:{sessionId}");
    }
}

public class UserSession { public string UserId { get; set; } }
```
