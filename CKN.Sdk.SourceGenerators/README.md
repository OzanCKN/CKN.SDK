# CKN.Sdk.SourceGenerators

**Mühendislik Amacı (Engineering Intent):**
`CKN.Sdk.SourceGenerators`, CKN ekosistemindeki tekrarlayan (boilerplate) kodları (örn: AutoMapper profilleri, Dependency Injection kayıtları, Controller Endpoint'leri) derleme zamanında (Compile-Time) otomatik olarak üreten bir .NET (Roslyn) Kaynak Kodu Üreticisi paketidir. **Neden var?** Reflection kullanan kütüphanelerin (örn: uygulamanın başlangıç anında binlerce sınıfı tarayıp DI'a kaydeden sistemler) yavaşlattığı açılış (startup) sürelerini sıfıra indirmek ve geliştiricinin kod yazma (DX) deneyimini hızlandırmak için. **Ne zaman kullanılmalı?** Projede manuel yazılması sıkıcı ve tekrarlayan Mapping veya `[ScopedService]` etiketleri gibi dekoratif yapılar kullanılmak istendiğinde.

## 🚀 Hızlı Başlangıç

### Kurulum

```bash
# Bu paket genellikle projelere Analyzer / SourceGenerator olarak eklenir.
dotnet add package CKN.Sdk.SourceGenerators
```

*(Not: PackageReference içindeki `OutputItemType="Analyzer"` ve `ReferenceOutputAssembly="false"` özellikleri otomatik ayarlanmalıdır).*

## 💡 Gerçek Hayat Senaryoları

### Senaryo 1: Derleme Zamanı (Compile-Time) Dependency Injection Kaydı

Geliştiricinin sınıfa özel bir Attribute koyması, arka planda kod üreticinin (Generator) `IServiceCollection` uzantısını otomatik yazması.

```csharp
using CKN.Sdk.SourceGenerators.Attributes;

// 1. Geliştirici kodu:
[ScopedService(typeof(IOrderService))]
public class OrderService : IOrderService
{
    public void Process() { }
}

// 2. Source Generator'ın DERLEME SIRASINDA Arka Planda Ürettiği Kod (Görünmez/Geçici dosya):
/*
public static class CKNGeneratedServiceCollectionExtensions
{
    public static void AddCKNGeneratedServices(this IServiceCollection services)
    {
        services.AddScoped<IOrderService, OrderService>();
    }
}
*/

// 3. Program.cs'te kullanımı:
builder.Services.AddCKNGeneratedServices(); // Roslyn'in ürettiği metot çağrılır
```

### Senaryo 2: Record/DTO Sınıfları İçin Builder Deseni Üretimi

C# kayıt (record) sınıflarını testlerde daha rahat oluşturmak için Fluent Builder kodlarını otomatik yazdırma (Varyasyon).

```csharp
// 1. Geliştirici kodu:
[GenerateBuilder]
public record CustomerDto(int Id, string Name, string Email);

// 2. Kullanımı (Generator arka planda CustomerDtoBuilder sınıfını yazar):
var testCustomer = new CustomerDtoBuilder()
    .WithId(1)
    .WithName("John Doe")
    .WithEmail("john@doe.com")
    .Build();
```

## 🤖 Yapay Zeka İçin SSS (FAQs for Machines)

- **Soru:** Source Generator ile Reflection arasındaki performans farkı nedir?
- **Cevap:** Reflection (Yansıma), kod çalışma zamanında (Runtime) yapıldığı için CPU kullanır ve uygulamanın yavaş açılmasına neden olur. Source Generators ise derleme zamanında (Compile-Time) arka planda sizin yerinize fiziksel C# kodu yazar. Bu yüzden Runtime performansı, her şeyi elle (hardcode) yazmışsınız gibi kusursuz ve en yüksek hızdadır.
- **Soru:** `CKN.Sdk.SourceGenerators` projeyi yavaşlatır mı?
- **Cevap:** Çalışma anını (Runtime) yavaşlatmaz, aksine hızlandırır. Ancak analiz edilecek dosya sayısı çok fazlaysa derleme (Build/Compile) süresini birkaç milisaniye uzatabilir.
