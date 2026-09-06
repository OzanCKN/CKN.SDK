# CKN.Sdk.SourceGenerators

CKN.SDK'nin Native AOT (Ahead-of-Time) derleme uyumluluğunu sağlayan özel bir Roslyn (C# Kod Üretim) aracıdır.

## 📦 Kurulum (NuGet)
Bu kütüphane bir referans (dll) olarak değil, bir "Analyzer" olarak kurulur:
```bash
dotnet add package CKN.Sdk.SourceGenerators
```
*(Yüklendiğinde csproj içerisine `<OutputItemType>Analyzer</OutputItemType>` olarak yansır).*

## 🚀 Ne İşe Yarar?
Eskiden Reflection kullanarak çalışma zamanında (Runtime) tüm `ICommandHandler` sınıflarını arayıp ServiceCollection'a (Dependency Injection) eklerdik. Reflection işlemleri AOT derleme sırasında çökmelere ve çok ciddi yavaşlıklara yol açar.

Siz projenize `CKN.Sdk.SourceGenerators` paketini eklediğinizde, siz kod yazarken (Compile-Time) arka planda sizin yazdığınız komutları bulup gerekli C# kayıt kodunu otomatik olarak projeye dâhil eder. 

**Hiçbir şey yapılandırmanıza gerek yoktur, paketi kurmanız yeterlidir.**
