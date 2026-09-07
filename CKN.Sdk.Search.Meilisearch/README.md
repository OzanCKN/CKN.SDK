# CKN.Sdk.Search.Meilisearch

Elasticsearch'e kıyasla çok daha hafif (lightweight), Rust ile yazılmış ve anında "typo-tolerance" (yazım hatası toleransı) sunan **Meilisearch** entegrasyon kütüphanesidir. `CKN.Sdk.Search` arayüzünü uygular. Genellikle e-ticaret sitelerindeki arama çubuğu (Search As You Type) gibi çok hızlı sonuç istenen yerlerde kullanılır.

## Yapılandırma (`appsettings.json`)

```json
{
  "Search": {
    "Meilisearch": {
      "Endpoint": "http://localhost:7700",
      "ApiKey": "masterKey"
    }
  }
}
```

## Servis Kaydı (Dependency Injection)

```csharp
using CKN.Sdk.Search.Meilisearch;

var builder = WebApplication.CreateBuilder(args);

// Meilisearch altyapısını sisteme dahil etme
builder.Services.AddCknMeilisearch(builder.Configuration);

var app = builder.Build();
```

## Gerçek Hayat Kullanım Senaryosu

**E-Ticaret Hızlı Arama Çubuğu**
Müşteriler arama kutusuna harf girdikçe anında (ms altında) sonuç dönmek.

```csharp
using CKN.Sdk.Search;

public class FastSearchService
{
    private readonly ISearchClient<Product> _searchClient;

    public FastSearchService(ISearchClient<Product> searchClient)
    {
        _searchClient = searchClient;
    }

    public async Task<List<Product>> TypeaheadSearchAsync(string prefix)
    {
        // Meilisearch, yazım hatalarına ve eksik harflere doğrudan toleranslıdır.
        var result = await _searchClient.SearchAsync(prefix, new SearchOptions { Limit = 5 });
        return result.Documents.ToList();
    }
}

public class Product { public string Id { get; set; } public string Name { get; set; } }
```
