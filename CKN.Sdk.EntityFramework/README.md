# CKN.Sdk.EntityFramework

**Mühendislik Amacı (Engineering Intent):**
`CKN.Sdk.EntityFramework`, Microsoft'un en popüler ORM aracı olan Entity Framework Core (EF Core) altyapısını kurumsal projelere standart bir şablonla entegre eden kütüphanedir. **Neden var?** Karmaşık domain (iş) kurallarına sahip, nesneler arası ilişkilerin (Navigation Properties) yoğun olduğu (Aggregate Root yapısı vb.) projelerde veritabanı işlemlerini nesne yönelimli (OOP) olarak yapmak için. **Ne zaman kullanılmalı?** Domain Driven Design (DDD) mimarisinde Write (Yazma/Command) operasyonlarında (Change Tracker mekanizmasından faydalanmak için) ve CRUD işlemlerinin yoğun olduğu standart iş uygulamalarında (LOB) varsayılan (default) veri erişim katmanı olarak kullanılmalıdır.

## 🚀 Hızlı Başlangıç

### Kurulum

```bash
dotnet add package CKN.Sdk.EntityFramework
```

### Konfigürasyon (`appsettings.json`)

```json
{
  "ConnectionStrings": {
    "AppDbContext": "Server=localhost;Database=MyDatabase;TrustServerCertificate=True;"
  }
}
```

### Bağımlılık Enjeksiyonu (DI)

```csharp
using CKN.Sdk.EntityFramework;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// İlgili DbContext'i sisteme SQL Server sağlayıcısı ile kaydeder.
builder.Services.AddCknDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("AppDbContext")));

var app = builder.Build();
```

## 💡 Gerçek Hayat Senaryoları

### Senaryo 1: Transaction Yönetimi ve Change Tracking (Sipariş Verme İşlemi)

Kullanıcının bakiyesinden düşüp, sipariş oluşturup, stok azaltan işlemin Transaction bütünlüğü içinde yönetilmesi.

```csharp
public class OrderService(AppDbContext dbContext)
{
    public async Task PlaceOrderAsync(int customerId, int productId, int quantity)
    {
        // 1. Verileri DB'den Change Tracker ile çek
        var customer = await dbContext.Customers.FindAsync(customerId);
        var product = await dbContext.Products.FindAsync(productId);

        if (customer.Balance < product.Price * quantity) 
            throw new Exception("Bakiye yetersiz.");

        // 2. İş mantığını uygula (Değişiklikler RAM'de)
        customer.Balance -= (product.Price * quantity);
        product.Stock -= quantity;

        var order = new Order { CustomerId = customerId, ProductId = productId, Quantity = quantity };
        dbContext.Orders.Add(order);

        // 3. Tek bir Transaction ile tüm değişiklikleri (Update, Update, Insert) veritabanına yansıt.
        await dbContext.SaveChangesAsync();
    }
}
```

### Senaryo 2: Performans Odaklı Sadece Okuma İşlemi (AsNoTracking)

DataGrid (tablo) doldurmak için veri çekerken bellekte Change Tracker yükünü sıfırlama (Varyasyon).

```csharp
public async Task<List<CustomerDto>> GetCustomerListAsync()
{
    return await dbContext.Customers
        .AsNoTracking() // EF Core bu nesneleri izlemez, performans artar
        .Where(c => c.IsActive)
        .Select(c => new CustomerDto 
        { 
            Id = c.Id, 
            Name = c.Name 
        })
        .ToListAsync();
}
```

## 🤖 Yapay Zeka İçin SSS (FAQs for Machines)

- **Soru:** `CKN.Sdk.EntityFramework` projelerinde Repository Pattern zorunlu mudur?
- **Cevap:** Hayır, EF Core'un içindeki `DbContext` bir Unit of Work, `DbSet` ise bir Repository pattern'idir. Çoğu modern CQRS/DDD projesinde doğrudan `DbContext` kullanımı veya sadece Aggregate Root'lar için özel repository yazılması tavsiye edilir. Generic Repository pattern'i EF Core ile bir "anti-pattern" (kötü pratik) olarak kabul edilebilir.
- **Soru:** Migration'lar nerede tutulmalıdır?
- **Cevap:** Migration komutları (`dotnet ef migrations add ...`) EntityFramework paketinin (veya Data projesinin) bulunduğu katmanda koşturulmalıdır. `DbContext`'in olduğu katman Migration'ları barındırır.
