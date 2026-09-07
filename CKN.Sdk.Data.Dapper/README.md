# CKN.Sdk.Data.Dapper

Mikro ORM standardı olan **Dapper** için entegrasyon kütüphanesidir. Ham SQL (Raw SQL) yazarak Entity Framework'ten çok daha yüksek performans elde edilmesi gereken (raporlama, toplu listeleme vb.) senaryolarda tercih edilir.

## Yapılandırma (`appsettings.json`)

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=myServerAddress;Database=myDataBase;User Id=myUsername;Password=myPassword;"
  }
}
```

## Servis Kaydı (Dependency Injection)

```csharp
using CKN.Sdk.Data.Dapper;

var builder = WebApplication.CreateBuilder(args);

// IDbConnection factory eklenmesi
builder.Services.AddCknDapper(builder.Configuration.GetConnectionString("DefaultConnection"));

var app = builder.Build();
```

## Gerçek Hayat Kullanım Senaryosu

**Yüksek Performanslı Rapor Çekimi**
On binlerce satırlık satış verisinin karmaşık SQL sorgularıyla (JOIN, GROUP BY) saniyeler içinde çekilmesi.

```csharp
using System.Data;
using Dapper;

public class SalesReportRepository
{
    private readonly IDbConnection _dbConnection;

    public SalesReportRepository(IDbConnection dbConnection)
    {
        _dbConnection = dbConnection;
    }

    public async Task<IEnumerable<SalesReportDto>> GetDailySalesReportAsync(DateTime date)
    {
        var sql = @"
            SELECT 
                Region, 
                SUM(TotalAmount) as TotalRevenue,
                COUNT(*) as OrderCount
            FROM Orders
            WHERE CAST(OrderDate as DATE) = @Date
            GROUP BY Region";

        // Dapper ile veritabanından doğrudan nesneye (DTO) map etme
        return await _dbConnection.QueryAsync<SalesReportDto>(sql, new { Date = date });
    }
}

public class SalesReportDto { public string Region { get; set; } public decimal TotalRevenue { get; set; } }
```
