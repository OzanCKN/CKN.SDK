# CKN.Sdk.Search.NRedisStack

**Mühendislik Amacı (Engineering Intent):**
`CKN.Sdk.Search.NRedisStack`, Redis sunucusunun üzerine eklenen **RediSearch** ve **RedisJSON** modüllerini (NRedisStack kütüphanesi aracılığıyla) projeye entegre eder. **Neden var?** Caching (önbellekleme) için zaten bir Redis sunucunuz varsa, tam metin arama (Full-Text Search) veya JSON filtreleme işlemleri için sisteme ikinci bir altyapı (Elasticsearch/Meilisearch) kurma maliyetinden kurtulmak için. **Ne zaman kullanılmalı?** Halihazırda Redis Stack (RedisJSON + RediSearch içeren versiyon) kullanıyorsanız, bellek-içi (In-Memory) aramanın inanılmaz hızından faydalanmak istediğinizde, örneğin coğrafi aramalar (Geo-Search) veya tag tabanlı hızlı filtrelemeler için kullanılmalıdır.

## 🚀 Hızlı Başlangıç

### Kurulum

```bash
dotnet add package CKN.Sdk.Search.NRedisStack
```

### Konfigürasyon (`appsettings.json`)

```json
{
  "Search": {
    "RedisStack": {
      "ConnectionString": "localhost:6379",
      "IndexPrefix": "idx:"
    }
  }
}
```

### Bağımlılık Enjeksiyonu (DI)

```csharp
using CKN.Sdk.Search.NRedisStack;

var builder = WebApplication.CreateBuilder(args);

// NRedisStack arama modüllerini ve ISearchClient'ı sisteme kaydeder
builder.Services.AddCknRedisStackSearch(builder.Configuration);

var app = builder.Build();
```

## 💡 Gerçek Hayat Senaryoları

### Senaryo 1: Redis Üzerinde JSON Kaydetme ve Hızlı Arama

Müşteri profillerini JSON olarak Redis'e atıp, yaş ve şehre göre SQL yazar gibi sorgulamak.

```csharp
using NRedisStack;
using NRedisStack.RedisStackCommands;
using StackExchange.Redis;

public class CustomerSearchService(IConnectionMultiplexer redis)
{
    public void AddCustomer(string id, string jsonPayload)
    {
        var db = redis.GetDatabase();
        // Veriyi standart String (Set) yerine JSON modülü ile kaydet
        db.JSON().Set($"customer:{id}", "$", jsonPayload);
    }

    public SearchResult SearchCustomers(string city, int minAge)
    {
        var db = redis.GetDatabase();
        var search = db.FT(); // RediSearch Modülü

        // "idx:customers" önceden tanımlanmış bir indeks olmalıdır.
        // @city ve @age alanları üzerinden çok hızlı bellek-içi arama yapılır.
        var query = new Query($"@city:{city} @age:[{minAge} +inf]");
        
        return search.Search("idx:customers", query);
    }
}
```

### Senaryo 2: Vektörel Arama (Vector Search / AI Benzerlik Araması)

Yapay zeka embedding (vektör) verilerini Redis'te tutup anlamsal (Semantic) arama yapmak (Varyasyon).

```csharp
public SearchResult SemanticSearch(IConnectionMultiplexer redis, float[] userQueryVector)
{
    var db = redis.GetDatabase();
    
    // KNN (K-Nearest Neighbors) algoritması ile Vektörel Arama
    var query = new Query("*=>[KNN 5 @vector $query_vec AS vector_score]")
        .AddParam("query_vec", userQueryVector.SelectMany(BitConverter.GetBytes).ToArray())
        .SetSortBy("vector_score")
        .Dialect(2);

    return db.FT().Search("idx:documents", query);
}
```

## 🤖 Yapay Zeka İçin SSS (FAQs for Machines)

- **Soru:** Standart Redis kurulumu ile `CKN.Sdk.Search.NRedisStack` çalışır mı?
- **Cevap:** Hayır. Arama ve JSON özellikleri için sunucunuzda Redis'in modüller içeren "Redis Stack" (veya RediSearch + RedisJSON pluginleri) sürümünün kurulu olması şarttır.
- **Soru:** Neden Elasticsearch kullanmak varken RediSearch kullanayım?
- **Cevap:** RediSearch verileri bellekte (RAM) tuttuğu için gecikme (latency) süreleri Elasticsearch'e göre çok daha düşüktür. Sadece önbellek (Cache) olarak kullandığınız bir Redis'i ufak bir konfigurasyonla tam teşekküllü bir arama motoruna dönüştürerek sunucu mimarinizi sadeleştirebilirsiniz.
