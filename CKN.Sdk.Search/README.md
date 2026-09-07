# CKN.Sdk.Search

CKN.Sdk içerisinde metin (Full-Text Search), vektör (Semantic Search) veya doküman arama altyapıları için **ortak soyutlamaları (Abstractions)** ve modelleri içeren çekirdek kütüphanedir. Bu proje tek başına iş yapmaz, Elasticsearch veya Meilisearch gibi provider'lara arayüz sağlar.

## Ortak Arayüzler

Bu kütüphane, CKN ekosisteminde arama işlemlerinin provider bağımsız (Provider-Agnostic) yapılabilmesi için aşağıdaki gibi arayüzler sunar:

```csharp
public interface ISearchClient<T> where T : class
{
    Task IndexAsync(T document);
    Task IndexManyAsync(IEnumerable<T> documents);
    Task<SearchResult<T>> SearchAsync(string query, SearchOptions options = null);
    Task DeleteAsync(string id);
}
```

## Gerçek Hayat Kullanım Senaryosu

**Farklı Arama Motorlarını Aynı Kodla Kullanma**
Geliştirici, altyapının Elasticsearch mi yoksa Meilisearch mi olduğunu bilmeden (Dependency Injection aracılığıyla) sadece `ISearchClient<T>` kullanarak e-ticaret araması yapar.

```csharp
using CKN.Sdk.Search;

public class ProductSearchService
{
    private readonly ISearchClient<ProductDocument> _searchClient;

    public ProductSearchService(ISearchClient<ProductDocument> searchClient)
    {
        // DI üzerinden Meilisearch veya Elasticsearch implementasyonu gelecektir
        _searchClient = searchClient;
    }

    public async Task<List<ProductDocument>> FindProductsAsync(string keyword)
    {
        var options = new SearchOptions 
        { 
            Limit = 20, 
            Filters = new Dictionary<string, string> { { "category", "electronics" } } 
        };

        var result = await _searchClient.SearchAsync(keyword, options);
        return result.Documents.ToList();
    }
}

public class ProductDocument { public string Id { get; set; } public string Name { get; set; } }
```
