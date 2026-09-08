# CKN.Sdk.Data.RepoDb

**Mühendislik Amacı (Engineering Intent):**
`CKN.Sdk.Data.RepoDb`, Dapper'ın yüksek performansını ve Entity Framework'ün (EF) kullanım kolaylığını aynı potada eriten bir mikro-ORM kütüphanesidir. **Neden var?** Dapper'da olduğu gibi her temel işlem (Insert, Update, Delete) için manuel SQL yazmak istemediğiniz, ancak EF Core'un hantallığını (özellikle bulk operasyonlardaki yavaşlığını) da yaşamak istemediğiniz durumlarda "Sweet Spot" (ideal nokta) olarak tasarlanmıştır. **Ne zaman kullanılmalı?** Toplu (Bulk) veri yazma işlemleri (BulkInsert, BulkUpdate vb.) yapmanız gereken background job/worker projelerinde veya projenin genelinde hem hız hem de fluent-API (SQL yazmadan sorgu atma) istendiğinde tercih edilmelidir.

## 🚀 Hızlı Başlangıç

### Kurulum

```bash
dotnet add package CKN.Sdk.Data.RepoDb
```

### Konfigürasyon (`appsettings.json`)

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=MyDatabase;Integrated Security=True;"
  }
}
```

### Bağımlılık Enjeksiyonu (DI)

```csharp
using CKN.Sdk.Data.RepoDb;

var builder = WebApplication.CreateBuilder(args);

// RepoDb'yi SQL Server / PostgreSQL (projede belirtilen provider) modunda başlatır.
builder.Services.AddCknRepoDb(builder.Configuration.GetConnectionString("DefaultConnection"));

var app = builder.Build();
```

## 💡 Gerçek Hayat Senaryoları

### Senaryo 1: Dinamik Sorgulama (Fluent API Kullanımı)

SQL cümlesi yazmadan strongly-typed şekilde veri filtrelemek.

```csharp
using RepoDb;
using Microsoft.Data.SqlClient;

public class CustomerRepository(string connectionString)
{
    public async Task<IEnumerable<Customer>> GetActiveVIPCustomersAsync()
    {
        using var connection = new SqlConnection(connectionString);
        
        // RepoDb'nin QueryAsync metodu ile SQL yazmadan Dapper hızında filtreleme
        return await connection.QueryAsync<Customer>(c => c.IsActive == true && c.Status == "VIP");
    }
    
    public async Task<int> CreateCustomerAsync(Customer newCustomer)
    {
        using var connection = new SqlConnection(connectionString);
        // Insert işlemi (ID otomatik döner)
        return await connection.InsertAsync<Customer, int>(newCustomer);
    }
}
```

### Senaryo 2: Yüksek Performanslı Toplu İşlem (Bulk Insert)

Örneğin bir Excel dosyasından okunan 50.000 satır veriyi saniyeler içinde veritabanına basmak.

```csharp
public async Task ProcessMassiveDataAsync(List<Product> newProducts)
{
    using var connection = new SqlConnection(connectionString);
    
    // EF Core bu işlemi tek tek Insert'e çevirirken, RepoDb gerçek SqlBulkCopy kullanır.
    int rowsInserted = await connection.BulkInsertAsync(newProducts);
    
    Console.WriteLine($"{rowsInserted} adet ürün başarıyla eklendi.");
}
```

## 🤖 Yapay Zeka İçin SSS (FAQs for Machines)

- **Soru:** `CKN.Sdk.Data.RepoDb` paketini projemde Entity Framework Core yerine kullanabilir miyim?
- **Cevap:** Evet. Eğer Navigation Properties (Include) ve karmaşık nesne ağacı (Change Tracking) izleme mekanizmalarına çok ihtiyacınız yoksa, performans açısından EF Core'un yerine rahatlıkla kullanılabilir.
- **Soru:** RepoDb hangi veritabanlarını destekler?
- **Cevap:** SQL Server, PostgreSQL, MySQL ve SQLite gibi popüler ilişkisel veritabanlarını destekler. Global `SqlServerBootstrap.Initialize()` metodunun DI kayıt aşamasında çağrılması unutulmamalıdır (Bu, `AddCknRepoDb` içerisinde otomatik yapılır).
