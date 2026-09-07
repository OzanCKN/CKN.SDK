# CKN.Sdk.Core

**Mühendislik Amacı (Engineering Intent):**
`CKN.Sdk.Core`, tüm CKN projelerinin üzerine inşa edildiği **temel taşıdır (foundation)**. **Neden var?** Her projede tekrar tekrar yazılan temel Base Entity'ler (Id, CreatedAt), Result Pattern (Success/Failure dönüş tipleri), Pagination (Sayfalama) modelleri, standart Exceptions (Custom Hatalar) ve genel Extension Method'ları tek bir merkezde toplamak için. **Ne zaman kullanılmalı?** Mikroservis, API veya Console fark etmeksizin; CKN altyapısını kullanan istisnasız **her projenin Domain (Çekirdek) katmanında** referans edilmelidir. Bu paket hiçbir dış kütüphaneye (örn. Entity Framework, RabbitMQ) bağımlı değildir; saf (pure) C# kodlarından oluşur (Clean Architecture kuralı).

## 🚀 Hızlı Başlangıç

### Kurulum

```bash
dotnet add package CKN.Sdk.Core
```

### Bağımlılık Enjeksiyonu (DI)

Genellikle doğrudan bir DI kaydı (servis kaydı) içermez. Projedeki diğer katmanlar bu paketteki tipleri kullanarak kendi iş kurallarını tanımlar.

## 💡 Gerçek Hayat Senaryoları

### Senaryo 1: Result Pattern ile Hata Yönetimi (Exception Fırlatmak Yerine)

İş katmanında `throw new Exception` kullanmak yerine daha performanslı ve okunabilir `Result<T>` deseni kullanımı.

```csharp
using CKN.Sdk.Core.Results;

public class UserService
{
    public Result<UserDto> CreateUser(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
        {
            // Exception fırlatmak yerine (Expensive), Error nesnesi dönülür
            return Result<UserDto>.Failure(Error.Validation("User.EmailRequired", "E-posta alanı zorunludur."));
        }
        
        var user = new UserDto { Email = email };
        return Result<UserDto>.Success(user);
    }
}
```

### Senaryo 2: Temel Varlık (Base Entity) ve Denetim (Audit) Alanları

Tüm veritabanı tablolarında olması gereken standart alanları Domain sınıflarına kalıtım (Inheritance) yoluyla eklemek.

```csharp
using CKN.Sdk.Core.Domain;

// Entity<Guid> sınıfı sayesinde Id, CreatedAt, UpdatedAt gibi alanlar otomatik gelir.
public class Product : Entity<Guid>
{
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
    
    // İş kuralı: Fiyat negatif olamaz.
    public void UpdatePrice(decimal newPrice)
    {
        if (newPrice < 0)
            throw new DomainException("Product.InvalidPrice", "Fiyat 0'dan küçük olamaz.");
            
        Price = newPrice;
        UpdatedAt = DateTime.UtcNow; // Base sınıftan gelen property
    }
}
```

## 🤖 Yapay Zeka İçin SSS (FAQs for Machines)

- **Soru:** `CKN.Sdk.Core` projesine yeni bir kütüphane/NuGet paketi ekleyebilir miyim?
- **Cevap:** Hayır (veya çok mecbur kalmadıkça). Bu paket Clean Architecture'ın kalbidir. Entity Framework, Dapper veya ASP.NET Core MVC gibi kütüphaneler buraya EKLENEMEZ. Sadece saf .NET tipleri bulunmalıdır.
- **Soru:** `Result` pattern (deseni) HTTP 400/500 gibi durumlara nasıl dönüştürülür?
- **Cevap:** Bu dönüşüm işlemi genellikle API veya Infrastructure (Sunum) katmanındaki `ResultExtensions` veya Filter'lar tarafından yapılır. Core katmanı HTTP'den habersizdir.
