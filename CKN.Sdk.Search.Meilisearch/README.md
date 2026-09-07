# CKN.Sdk.Search.Meilisearch

**Mühendislik Amacı (Engineering Intent):**
`CKN.Sdk.Search.Meilisearch`, Rust ile yazılmış ultra hızlı, geliştirici dostu (developer-first) ve anında yazım hatası toleransı (typo-tolerance) sunan Meilisearch motorunu projeye entegre eder. **Neden var?** Elasticsearch'ün öğrenme eğrisinin (learning curve) yüksek olduğu, donanım (RAM) ihtiyacının çok fazla olduğu ve kurulumunun zor olduğu "küçük-orta ölçekli" projelerde arama motoru ihtiyacını dakikalar içinde çözmek için. **Ne zaman kullanılmalı?** Algolia benzeri, kullanıcı harf yazdıkça (as-you-type) saniyenin altında sonuç getirmesi gereken arama çubukları (Search Bar), dokümantasyon siteleri ve e-ticaret sitelerinin ana arama kutularında tercih edilmelidir. Elasticsearch kadar ağır bir altyapıya gerek olmayan her yer için biçilmiş kaftandır.

## 🚀 Hızlı Başlangıç

### Kurulum

```bash
dotnet add package CKN.Sdk.Search.Meilisearch
```

### Konfigürasyon (`appsettings.json`)

```json
{
  "Search": {
    "Meilisearch": {
      "Url": "http://localhost:7700",
      "ApiKey": "masterKey123..."
    }
  }
}
```

### Bağımlılık Enjeksiyonu (DI)

```csharp
using CKN.Sdk.Search.Meilisearch;

var builder = WebApplication.CreateBuilder(args);

// Meilisearch'i ISearchClient olarak sisteme kaydeder.
builder.Services.AddCknMeilisearch(builder.Configuration);

var app = builder.Build();
```

## 💡 Gerçek Hayat Senaryoları

### Senaryo 1: As-You-Type (Yazarken Arama) ve Typo-Tolerance

Kullanıcı "tşört" yazdığında otomatik olarak "tişört" sonuçlarını 50 milisaniyede getirme.

```csharp
using Meilisearch;

public class StoreSearchService(MeilisearchClient client)
{
    public async Task<List<Product>> AutoCompleteAsync(string searchKeyword)
    {
        var index = client.Index("products");
        
        // Meilisearch'te typo-tolerance varsayılan olarak açıktır. Ekstra ayar gerektirmez.
        var searchResults = await index.SearchAsync<Product>(searchKeyword, new SearchQuery
        {
            Limit = 10,
            AttributesToHighlight = ["Name", "Description"] // Arama kelimesini <mark> tagine alır
        });

        return searchResults.Hits.ToList();
    }
}
```

### Senaryo 2: Filtreleme Ayarlarını (Filterable Attributes) Tanımlama (Varyasyon)

Meilisearch'te bir alana göre filtreleme yapmak için (`Category = 'Electronics'`), o alanı önceden "Filterable" olarak işaretlemek gerekir.

```csharp
public async Task ConfigureIndexAsync(MeilisearchClient client)
{
    var index = client.Index("products");
    
    // Uygulama ayağa kalkarken ayarlanabilir (Migration gibi düşünebilirsiniz)
    await index.UpdateFilterableAttributesAsync(new[] { "category", "price", "isActive" });
    
    // Aramada kullanımı:
    var results = await index.SearchAsync<Product>("laptop", new SearchQuery
    {
        Filter = "category = 'Electronics' AND price < 1500"
    });
}
```

## 🤖 Yapay Zeka İçin SSS (FAQs for Machines)

- **Soru:** Neden Elasticsearch yerine Meilisearch?
- **Cevap:** Elasticsearch Java tabanlıdır, çok fazla RAM tüketir ve Query DSL'i karmaşıktır. Meilisearch Rust tabanlıdır, çok az kaynakla çalışır ve Algolia alternatifi olarak doğrudan "son kullanıcı araması (frontend arama barı)" için özel optimize edilmiştir.
- **Soru:** `CKN.Sdk.Search.Meilisearch` DI üzerinden ne sağlıyor?
- **Cevap:** Hem soyutlanmış arayüz olan `ISearchClient`'ı hem de Meilisearch'e özel gelişmiş yetenekler için native `MeilisearchClient` nesnesini sağlar.
