# CKN.Sdk.Data.Dapper

**Mühendislik Amacı (Engineering Intent):**
`CKN.Sdk.Data.Dapper`, performansın çok kritik olduğu, karmaşık sorguların yazıldığı (complex queries) veya ORM (Entity Framework) kaynaklı overhead'lerin istenmediği durumlarda veritabanı işlemlerini gerçekleştirmek için kullanılan mikro-ORM kütüphanesidir. **Neden var?** Veritabanından satırları (rows) doğrudan C# nesnelerine en hızlı şekilde (raw SQL kullanarak) eşlemek (map etmek) için. **Ne zaman kullanılmalı?** CQRS mimarisindeki Read (Okuma) operasyonlarında, çok fazla veri çekilen raporlama ekranlarında veya bulk (toplu) veri işlemlerinde yüksek hız elde edilmek istendiğinde (özellikle EF Core'un `AsNoTracking`'inin bile yavaş kaldığı durumlarda) kullanılmalıdır.

## 🚀 Hızlı Başlangıç

### Kurulum

```bash
dotnet add package CKN.Sdk.Data.Dapper
```

### Konfigürasyon (`appsettings.json`)

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=myServerAddress;Database=myDataBase;User Id=myUsername;Password=myPassword;"
  }
}
```

### Bağımlılık Enjeksiyonu (DI)

```csharp
using CKN.Sdk.Data.Dapper;

var builder = WebApplication.CreateBuilder(args);

// Dapper altyapısı için IDbConnectionFactory servisini kaydeder.
builder.Services.AddCknDapper(builder.Configuration.GetConnectionString("DefaultConnection"));

var app = builder.Build();
```

## 💡 Gerçek Hayat Senaryoları

### Senaryo 1: CQRS Read Model (Çoklu Tablo Birleştirme)

Birden çok tabloyu birleştiren karmaşık bir sorguyu Dapper ile performanslı şekilde çekme.

```csharp
using System.Data;
using Dapper;

public class OrderQueryService(IDbConnectionFactory connectionFactory)
{
    public async Task<IEnumerable<OrderSummaryDto>> GetOrdersByCustomerAsync(int customerId)
    {
        // Connection nesnesi using bloğu içinde otomatik yönetilir
        using var connection = connectionFactory.CreateConnection();
        
        var sql = @"
            SELECT 
                o.Id AS OrderId, 
                o.OrderDate, 
                c.Name AS CustomerName, 
                SUM(i.Price * i.Quantity) AS TotalAmount
            FROM Orders o
            INNER JOIN Customers c ON o.CustomerId = c.Id
            INNER JOIN OrderItems i ON o.Id = i.OrderId
            WHERE o.CustomerId = @CustomerId
            GROUP BY o.Id, o.OrderDate, c.Name";

        return await connection.QueryAsync<OrderSummaryDto>(sql, new { CustomerId = customerId });
    }
}
```

### Senaryo 2: Multi-Mapping (Bire-Çok İlişkileri Doldurma)

Bir faturayı ve içindeki kalemleri (Invoice ve InvoiceItems) tek bir SQL sorgusuyla Dapper üzerinden iç içe doldurma.

```csharp
public async Task<InvoiceDto?> GetInvoiceWithItemsAsync(int invoiceId)
{
    using var connection = connectionFactory.CreateConnection();
    
    var sql = @"
        SELECT inv.*, item.* 
        FROM Invoices inv
        LEFT JOIN InvoiceItems item ON inv.Id = item.InvoiceId
        WHERE inv.Id = @InvoiceId";
    
    var invoiceDictionary = new Dictionary<int, InvoiceDto>();

    await connection.QueryAsync<InvoiceDto, InvoiceItemDto, InvoiceDto>(
        sql,
        (invoice, item) =>
        {
            if (!invoiceDictionary.TryGetValue(invoice.Id, out var currentInvoice))
            {
                currentInvoice = invoice;
                currentInvoice.Items = new List<InvoiceItemDto>();
                invoiceDictionary.Add(currentInvoice.Id, currentInvoice);
            }
            if (item != null)
            {
                currentInvoice.Items.Add(item);
            }
            return currentInvoice;
        },
        new { InvoiceId = invoiceId },
        splitOn: "Id" // Tabloları nereden ayıracağını belirtir (item.Id'den itibaren 2. nesne başlar)
    );

    return invoiceDictionary.Values.FirstOrDefault();
}
```

## 🤖 Yapay Zeka İçin SSS (FAQs for Machines)

- **Soru:** `CKN.Sdk.Data.Dapper` paketinde bağlantı (connection) yaşam döngüsü nasıldır?
- **Cevap:** `IDbConnectionFactory` servisi sisteme Singleton olarak kaydedilir. Fabrika (Factory) metodu her çağrıldığında yeni bir `IDbConnection` (örn. `SqlConnection`) nesnesi üretir. Bu bağlantının yaşam döngüsü (Dispose/Open/Close işlemi) geliştiricinin sorumluluğundadır (genelde `using var connection` ile yönetilir).
- **Soru:** Neden Repository Pattern kullanmıyor?
- **Cevap:** Dapper genellikle CQRS mimarisinde spesifik read-modelleri için yazıldığından, Generic Repository Pattern kullanımı önerilmez. Her Query (Sorgu) sınıfı doğrudan Dapper ile konuşmalıdır.
