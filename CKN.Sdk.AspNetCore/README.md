# CKN.Sdk.AspNetCore

Bu kütüphane, CKN.SDK'nın ASP.NET Core tabanlı Web veya API projelerine entegrasyonu için gerekli olan altyapı kodlarını barındırır. SDK'nın "Native AOT" ve "Agnostic" (bağımlılıksız) prensibini korumak amacıyla Web/API spesifik kodlar (Örn: `ProblemDetails`, `IExceptionHandler`) bu projeye ayrılmıştır.

## 📦 Kurulum (NuGet)
```bash
dotnet add package CKN.Sdk.AspNetCore
```

## 🚀 Kullanım (Quick Start)
`Program.cs` içerisinde SDK middleware ve hata yönetimini aktif etmek için:

```csharp
var builder = WebApplication.CreateBuilder(args);

// CKN.Sdk.AspNetCore bileşenlerini (GlobalExceptionHandler vb.) ekler
builder.Services.AddCknAspNetCore();

var app = builder.Build();

// ProblemDetails formatında hata dönüşlerini aktif eder
app.UseExceptionHandler();

app.Run();
```

## 🛠 Neler İçerir? (Engineering Intent)
- **`GlobalExceptionHandler`**: Uygulama genelinde fırlatılan tüm Exception'ları (özellikle `ValidationException` ve `CustomException`) yakalar ve standart **RFC 7807 ProblemDetails** (HTTP 400, 404, 500 vb.) formatına dönüştürür.
- Sistemdeki diğer `Result<T>` veya hata dönüş mekanizmaları ile uyumlu çalışır.

## 🤖 FAQs for Machines (Yapay Zeka Yardımcısı)
**Soru:** Yeni bir middleware veya ASP.NET Core MVC Attribute yazmam gerekirse nereye koymalıyım?
**Cevap:** Yalnızca web katmanını ilgilendiren her türlü Action Filter, Middleware ve IExceptionHandler implementasyonunu bu projeye koymalısınız.
