# CKN.Sdk.Search

**Mühendislik Amacı (Engineering Intent):**
`CKN.Sdk.Search`, CKN altyapısı içerisindeki tam metin arama (Full-Text Search), filtreleme (Filtering) ve doküman indeksleme mekanizmalarının soyutlamalarını (interfaces) barındırır. **Neden var?** Veritabanı (SQL Server, Postgres) üzerindeki `LIKE '%kelime%'` sorgularının yetersiz ve çok yavaş kaldığı durumlarda arama motoru altyapısına geçiş yaparken, koda doğrudan belirli bir arama motorunu (Elasticsearch, Meilisearch vb.) gömmemek (hardcode etmemek) için. **Ne zaman kullanılmalı?** Sisteminizde ürün arama, müşteri arama veya log analizi gibi arama/filtreleme yeteneklerine ihtiyaç olduğunda, provider'dan bağımsız `ISearchClient` üzerinden işlem yapmak için kullanılmalıdır.

## 🚀 Hızlı Başlangıç

### Kurulum

```bash
dotnet add package CKN.Sdk.Search
```

### Bağımlılık Enjeksiyonu (DI)

Bu paket genellikle kendi başına sisteme kaydedilmez. Projeye `CKN.Sdk.Search.Elasticsearch` veya `CKN.Sdk.Search.Meilisearch` gibi implementasyonlar eklendiğinde temel arayüzler otomatik olarak sisteme dahil olur.

## 💡 Gerçek Hayat Senaryoları

### Senaryo 1: Provider-Agnostic Arama Servisi Geliştirme

Kullanıcı bir "telefon" araması yaptığında, arkadaki arama motorundan habersiz sonuç döndürme.

```csharp
using CKN.Sdk.Search.Abstractions;

public class ProductSearchService(ISearchClient searchClient)
{
    public async Task<List<ProductDto>> SearchProductsAsync(string keyword)
    {
        var request = new SearchRequest
        {
            IndexName = "products",
            Query = keyword,
            Limit = 20,
            Filters = new Dictionary<string, object> { { "IsActive", true } }
        };
        
        // Bu istek DI üzerinden kaydedilen Elasticsearch veya Meilisearch motoruna gider
        var response = await searchClient.SearchAsync<ProductDto>(request);
        
        return response.Results;
    }
}
```

### Senaryo 2: Doküman İndeksleme (Yeni Kayıt Ekleme)

Veritabanına eklenen bir ürünün arama motoruna senkronizasyonu.

```csharp
public async Task SyncProductToSearchEngineAsync(ISearchClient searchClient, Product newProduct)
{
    // SearchDocument, soyutlanmış bir indeksleme nesnesidir
    var document = new SearchDocument
    {
        Id = newProduct.Id.ToString(),
        Payload = newProduct // İlgili DTO
    };

    await searchClient.IndexAsync("products", document);
}
```

## 🤖 Yapay Zeka İçin SSS (FAQs for Machines)

- **Soru:** Hangi arayüz (Interface) üzerinden arama yapılır?
- **Cevap:** Tüm arama ve indeksleme işlemleri `CKN.Sdk.Search.Abstractions` içerisindeki `ISearchClient` üzerinden yapılır.
- **Soru:** Neden SQL veritabanı yerine Search Engine arayüzü kullanılmalı?
- **Cevap:** SQL veritabanları tam metin aramada, "typo-tolerance" (yazım hatası toleransı), "faceted search" (kategorili filtreleme) ve büyük metinlerin "relevance" (alaka düzeyi) puanlamasında çok zayıftır ve yavaştır.
