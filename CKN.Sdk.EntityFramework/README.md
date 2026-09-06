# CKN.Sdk.EntityFramework

`CKN.Sdk.Core` içerisindeki veri erişim arayüzlerinin (`IRepository`, `IUnitOfWork`) Entity Framework Core kullanılarak implemente edilmiş halidir. Outbox pattern ve Domain Event yönetimini otomatik halleder.

## 📦 Kurulum (NuGet)
```bash
dotnet add package CKN.Sdk.EntityFramework
```

## 🚀 Kullanım
Sadece veritabanı bağlantısına ihtiyaç duyan Mikroservislerin `Program.cs` dosyasına eklenir:

```csharp
builder.Services.AddCknEntityFramework(options => 
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("Default"));
});
```

**Kullanım Örneği:**
```csharp
public class MyService(IRepository<MyEntity> repository) 
{
    public async Task DoWork() {
        var data = await repository.GetAllAsync();
    }
}
```
