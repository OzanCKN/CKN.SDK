# CKN.Sdk.Core

Sistemin kalbini oluşturan, %100 bağımlılıksız (zero-dependency) çekirdek kütüphane. Herhangi bir CKN projesinin temel taşıdır ve tüm arayüzleri belirler.

## 📦 Kurulum (NuGet)
```bash
dotnet add package CKN.Sdk.Core
```

## 🚀 Kullanım
Projenizde hiçbir dış kütüphane kullanmadan Domain nesneleri (Entities) ve CQRS komutları tanımlamak için kullanılır. `Program.cs` içerisindeki kaydı:

```csharp
// Tüm ICommand ve IQuery bağımlılıklarını tarar (veya SourceGenerator ile kaydeder)
builder.Services.AddCknCore();
```

## İçerik
- **Domain Modelleri:** `Entity`, `Tenant`, `IDomainEvent`
- **CQRS Arayüzleri:** `ICommand`, `IQuery`, `ICommandHandler`
- **Veri Arayüzleri:** `IRepository`, `IUnitOfWork`
- **Hata Yönetimi:** Özel İstisna Sınıfları (`CustomException`, `NotFoundException`)
