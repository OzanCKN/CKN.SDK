# CKN.Sdk.Search.Elasticsearch

CKN.Sdk içerisinde, endüstri standardı Full-Text arama, Log analizi ve NoSQL veritabanı platformu olan **Elasticsearch** entegrasyonudur. Karmaşık sorgular (Fuzzy, wildcard, aggregations) gerektiren senaryolarda `CKN.Sdk.Search` arayüzünü uygular.

## Yapılandırma (`appsettings.json`)

```json
{
  "Search": {
    "Elasticsearch": {
      "Nodes": [ "http://localhost:9200" ],
      "DefaultIndex": "ckn_documents",
      "Username": "elastic",
      "Password": "changeme"
    }
  }
}
```

## Servis Kaydı (Dependency Injection)

```csharp
using CKN.Sdk.Search.Elasticsearch;

var builder = WebApplication.CreateBuilder(args);

// Elasticsearch altyapısını sisteme dahil etme
builder.Services.AddCknElasticsearch(builder.Configuration);

var app = builder.Build();
```

## Gerçek Hayat Kullanım Senaryosu

**Gelişmiş Doküman İndeksleme ve Arama**
Kullanıcıların yanlış harfle (typo) arasa bile (Fuzzy Search) ilgili blog yazılarını veya dokümanları bulmasını sağlamak.

```csharp
using CKN.Sdk.Search;

public class BlogSearchService
{
    private readonly ISearchClient<BlogPost> _searchClient;

    public BlogSearchService(ISearchClient<BlogPost> searchClient)
    {
        _searchClient = searchClient;
    }

    public async Task IndexPostAsync(BlogPost post)
    {
        // Blog yazısını Elastic'e gönder
        await _searchClient.IndexAsync(post);
    }

    public async Task<List<BlogPost>> SearchPostsAsync(string keyword)
    {
        // Kullanıcı "elma" yerine "elms" bile yazsa bulabilecek (Fuzzy yeteneği Elasticsearch tarafından desteklenir)
        var result = await _searchClient.SearchAsync(keyword);
        return result.Documents.ToList();
    }
}

public class BlogPost { public string Id { get; set; } public string Title { get; set; } public string Content { get; set; } }
```
