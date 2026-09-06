# Search (Arama Motoru) Sağlayıcıları Kullanım Örnekleri

CKN.SDK, Full-Text Search (Tam Metin Arama) ve Fuzzy Search (Hatalı Yazım Toleranslı Arama) ihtiyaçlarınız için `ISearchService<T>` arayüzünü sunar.

---

## 1. Elasticsearch Kullanımı

Büyük veri setleri ve gelişmiş metin analitiği gerektiren kurumsal projeler için endüstri standardıdır.

### `appsettings.json` Yapılandırması
```json
{
  "Search": {
    "Elasticsearch": {
      "Url": "https://localhost:9200",
      "ApiKey": "el_api_key_123",
      "IndexPrefix": "ckn_dev_"
    }
  }
}
```

### Dependency Injection (DI) Kurulumu
```csharp
using CKN.Sdk.Search.Elasticsearch;

builder.Services.AddCknElasticsearch(opt =>
{
    builder.Configuration.GetSection(ElasticsearchOptions.SectionName).Bind(opt);
});
```

### Gerçek Hayat Kullanımı: Ürün Kataloğu Araması
Bir e-ticaret sitesinde "Iphne 15" diye yazılsa bile (Fuzzy Tolerance) doğru ürünü getirme senaryosu.

```csharp
using CKN.Sdk.Search;

public class ProductDocument
{
    public string Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public decimal Price { get; set; }
}

public class ProductSearchService
{
    private readonly ISearchService<ProductDocument> _searchService;

    public ProductSearchService(ISearchService<ProductDocument> searchService)
    {
        _searchService = searchService;
    }

    public async Task IndexNewProductAsync(ProductDocument product)
    {
        // Yeni ürünü arama motoruna ekle (Endeksle)
        await _searchService.IndexAsync("products", product);
    }

    public async Task<List<ProductDocument>> SearchProductAsync(string searchTerm)
    {
        // "Iphne 15" kelimesini ürün adında veya açıklamasında ara
        var results = await _searchService.SearchAsync("products", searchTerm);
        return results.ToList();
    }
}
```

---

## 2. Meilisearch Kullanımı

Elasticsearch'e kıyasla kurulumu çok daha basit olan, özellikle "Typo Tolerance" (Hatalı yazım düzeltme) ve "Ultra Fast Response (50ms altı)" sunmak üzere Rust ile yazılmış yeni nesil bir arama motorudur.

### `appsettings.json` Yapılandırması
```json
{
  "Search": {
    "Meilisearch": {
      "Url": "http://localhost:7700",
      "ApiKey": "masterKey"
    }
  }
}
```

### Dependency Injection (DI) Kurulumu
```csharp
using CKN.Sdk.Search.Meilisearch;

builder.Services.AddCknMeilisearch(opt =>
{
    builder.Configuration.GetSection(MeilisearchOptions.SectionName).Bind(opt);
});
```

### Neden Meilisearch Tercih Etmelisiniz?
Eğer kompleks Analitik veya Loglama Dashboardlarına (Kibana gibi) ihtiyacınız yoksa; sadece sitenizdeki "Arama Çubuğunun" mükemmel çalışmasını istiyorsanız Elasticsearch yerine Meilisearch kullanmak sunucu maliyetlerinizi %80 oranında düşürecektir. Kullanım kodu `ProductSearchService` örneği ile tamamen aynıdır.

---

## 3. NRedisStack (RediSearch) Kullanımı

Zaten hali hazırda Redis kullanıyorsanız, sisteme ayrı bir Elasticsearch kurmak yerine Redis üzerine RediSearch modülünü ekleyerek onu mükemmel bir arama motoruna dönüştürebilirsiniz.

### `appsettings.json` Yapılandırması
```json
{
  "Search": {
    "NRedisStack": {
      "Configuration": "localhost:6379" // RediSearch eklentili Redis sunucunuz
    }
  }
}
```

### Dependency Injection (DI) Kurulumu
```csharp
using CKN.Sdk.Search.NRedisStack;

builder.Services.AddCknNRedisStack(opt =>
{
    builder.Configuration.GetSection(NRedisStackOptions.SectionName).Bind(opt);
});
```

### Gerçek Hayat Kullanımı: Redis FT.SEARCH
Kullanımı yine `ISearchService<T>` üzerinden yapılır ancak arka planda Redis'in yeni `FT.SEARCH` (RedisJSON) komutları işletilerek aşırı hızlı, In-Memory veri taraması sağlanır. Mikrosaniye (µs) mertebesinde hız gerektiren "Autocomplete / Suggestion" (Arama çubuğuna yazarken sonuç gelmesi) durumları için eşsizdir.
