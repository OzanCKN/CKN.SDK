# CKN.Sdk.Data.RepoDb

Hem yüksek performans sunan hem de Entity Framework benzeri LINQ ve Bulk operasyon yetenekleri (Bulk Insert, Update vb.) sunan **RepoDb** için entegrasyon kütüphanesidir. EF Core'a göre daha hafif, Dapper'a göre daha yeteneklidir.

## Yapılandırma (`appsettings.json`)

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=CknDb;Integrated Security=true;"
  }
}
```

## Servis Kaydı (Dependency Injection)

```csharp
using CKN.Sdk.Data.RepoDb;

var builder = WebApplication.CreateBuilder(args);

// RepoDb başlatıcı ayarları
builder.Services.AddCknRepoDb(builder.Configuration.GetConnectionString("DefaultConnection"));

var app = builder.Build();
```

## Gerçek Hayat Kullanım Senaryosu

**Toplu Veri Ekleme (Bulk Insert) ve Hızlı CRUD**
Bir dış sistemden gelen binlerce ürün verisinin tek seferde veritabanına yazılması.

```csharp
using System.Data;
using RepoDb;

public class ProductSyncRepository
{
    private readonly IDbConnection _dbConnection;

    public ProductSyncRepository(IDbConnection dbConnection)
    {
        _dbConnection = dbConnection;
    }

    public async Task SyncProductsBulkAsync(IEnumerable<Product> products)
    {
        // RepoDb'nin sunduğu Bulk Insert yeteneği ile binlerce kaydı tek seferde aktarma
        // Entity Framework'ün tek tek eklemesinden veya SaveChanges çağrısından kat kat hızlıdır.
        await _dbConnection.BulkInsertAsync(products);
    }
    
    public async Task<Product> GetProductByIdAsync(int id)
    {
        return (await _dbConnection.QueryAsync<Product>(p => p.Id == id)).FirstOrDefault();
    }
}

public class Product { public int Id { get; set; } public string Name { get; set; } }
```
