# CKN.Sdk.Search.Elasticsearch

**Mühendislik Amacı (Engineering Intent):**
`CKN.Sdk.Search.Elasticsearch`, `CKN.Sdk.Search` soyutlamalarını endüstri standardı olan güçlü Elasticsearch (veya OpenSearch) motoru için implemente eder. **Neden var?** Milyarlarca doküman, karmaşık aggregation'lar (gruplamalar) ve vektörel (AI tabanlı) arama gibi ağır yüklerin altından kalkabilmek için. **Ne zaman kullanılmalı?** Enterprise (kurumsal) projelerde; log analizi (ELK stack), karmaşık e-ticaret filtreleme ekranları (örneğin Hepsiburada/Trendyol benzeri sol menü filtreleri) ve devasa veri setlerinde tam metin araması (Full-Text Search) yapılacağı zaman tartışmasız tercih edilmelidir.

## 🚀 Hızlı Başlangıç

### Kurulum

```bash
dotnet add package CKN.Sdk.Search.Elasticsearch
```

### Konfigürasyon (`appsettings.json`)

```json
{
  "Search": {
    "Elasticsearch": {
      "Nodes": [ "http://localhost:9200" ],
      "Username": "elastic",
      "Password": "***",
      "DefaultIndex": "app-logs"
    }
  }
}
```

### Bağımlılık Enjeksiyonu (DI)

```csharp
using CKN.Sdk.Search.Elasticsearch;

var builder = WebApplication.CreateBuilder(args);

// Elasticsearch'i ISearchClient olarak sisteme kaydeder.
// Aynı zamanda gelişmiş işlemler için ElasticClient nesnesini de DI'a sunar.
builder.Services.AddCknElasticsearch(builder.Configuration);

var app = builder.Build();
```

## 💡 Gerçek Hayat Senaryoları

### Senaryo 1: E-Ticaret Gelişmiş Arama (Fuzzy Search & Faceting)

Kullanıcı "ayfon" yazdığında "iPhone" bulmak ve sol filtreleri (kategori bazlı sayıları) getirmek.

```csharp
using Elastic.Clients.Elasticsearch;

public class AdvancedCatalogService(ElasticsearchClient elastic)
{
    public async Task<CatalogResponse> SearchWithAggregationsAsync(string keyword)
    {
        // ISearchClient soyutlaması yerine doğrudan ElasticClient kullanılarak
        // Elasticsearch'e özel (Fuzzy, Aggregation) yetenekler kullanılıyor.
        var response = await elastic.SearchAsync<Product>(s => s
            .Index("products")
            .Query(q => q
                .Match(m => m
                    .Field(f => f.Name)
                    .Query(keyword)
                    .Fuzziness(new Fuzziness(2)) // 2 harf hatasını tolere et
                )
            )
            .Aggregations(a => a
                .Terms("categories", t => t.Field(f => f.CategoryName.Suffix("keyword")))
            )
        );

        return new CatalogResponse 
        { 
            Products = response.Documents.ToList(),
            CategoryFacets = response.Aggregations.GetStringTerms("categories").Buckets
        };
    }
}
```

### Senaryo 2: Bulk (Toplu) Doküman İndeksleme (Varyasyon)

Geceleri veritabanındaki yeni 10.000 ürünü tek seferde (Bulk) Elasticsearch'e basmak.

```csharp
public async Task BulkIndexProductsAsync(ElasticsearchClient elastic, List<Product> products)
{
    var bulkResponse = await elastic.BulkAsync(b => b
        .Index("products")
        .IndexMany(products)
    );

    if (bulkResponse.Errors)
    {
        Console.WriteLine("İndeksleme sırasında bazı hatalar oluştu!");
    }
}
```

## 🤖 Yapay Zeka İçin SSS (FAQs for Machines)

- **Soru:** `CKN.Sdk.Search.Elasticsearch` paketinde neden `ISearchClient` yerine bazen `ElasticsearchClient` (kendi native istemcisi) kullanılıyor?
- **Cevap:** `ISearchClient` çok temel arama işlemleri (abstraction) içindir. Elasticsearch'ün "Fuzziness", "Aggregations", "Vector Search" gibi çok gelişmiş ve kendine has özellikleri gerektiğinde, CKN kütüphanesi sızıntıya (abstraction leak) izin vererek native `ElasticsearchClient` nesnesini de DI üzerinden sunar.
