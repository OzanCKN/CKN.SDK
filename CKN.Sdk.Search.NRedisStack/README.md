# CKN.Sdk.Search.NRedisStack

Redis Stack (RediSearch modülü) üzerinde Full-Text ve Vector (Semantic) Search yapabilmeyi sağlayan **NRedisStack** entegrasyon kütüphanesidir. Sunucunuzda zaten Redis (Redis Stack) varsa, ekstra bir arama motoru kurmaya gerek kalmadan Elasticsearch benzeri yetenekler sunar. `CKN.Sdk.Search` arayüzünü uygular.

## Yapılandırma (`appsettings.json`)

```json
{
  "Search": {
    "RedisStack": {
      "ConnectionString": "localhost:6379",
      "IndexPrefix": "idx:ckn:"
    }
  }
}
```

## Servis Kaydı (Dependency Injection)

```csharp
using CKN.Sdk.Search.NRedisStack;

var builder = WebApplication.CreateBuilder(args);

// Redis Stack Search'i sisteme dahil etme
builder.Services.AddCknRedisStackSearch(builder.Configuration);

var app = builder.Build();
```

## Gerçek Hayat Kullanım Senaryosu

**Kullanıcı Rehberi Araması (Memory İçi)**
İnsan kaynakları uygulamasında, şirket çalışanlarını saniyeden çok daha kısa bir sürede (Ram üzerinden) bulma.

```csharp
using CKN.Sdk.Search;

public class EmployeeSearchService
{
    private readonly ISearchClient<Employee> _searchClient;

    public EmployeeSearchService(ISearchClient<Employee> searchClient)
    {
        _searchClient = searchClient;
    }

    public async Task<List<Employee>> FindEmployeeAsync(string name)
    {
        // Veriler RAM üzerinde (Redis) olduğu için yanıt süresi olağanüstü düşüktür.
        var result = await _searchClient.SearchAsync(name);
        return result.Documents.ToList();
    }
}

public class Employee { public string Id { get; set; } public string FullName { get; set; } }
```
