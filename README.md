# CKN.SDK (Enterprise Agentic Orchestra)

CKN.SDK, 2026 Native AOT standartlarına uygun, modüler, olay güdümlü (event-driven) ve yapay zeka destekli mikroservisler inşa etmek için kullanılan resmi altyapı kütüphanesidir.

Devasa bir monolit indirmek yerine, projenizin ihtiyacına göre sadece ilgili SDK modülünü NuGet üzerinden indirebilirsiniz. Bütün paketler merkezi olarak yönetilmektedir.

## 📦 SDK Modülleri (NuGet Paketleri)

Projelerinizin ihtiyaçlarına göre kurabileceğiniz paketlerin detaylı kullanım rehberlerine aşağıdaki bağlantılardan ulaşabilirsiniz:

| Paket Adı | Açıklama | Bağlantı (Detaylı Rehber) |
| :--- | :--- | :--- |
| **CKN.Sdk.Core** | Sistemin kalbi. Sıfır bağımlılık. Tüm domain, CQRS ve temel arayüzler. | [Core README](./CKN.Sdk.Core/README.md) |
| **CKN.Sdk.EntityFramework** | SQL/PostgreSQL veritabanı erişimi, DbContext ve UnitOfWork implementasyonları. | [EF Core README](./CKN.Sdk.EntityFramework/README.md) |
| **CKN.Sdk.MassTransit** | RabbitMQ üzerinden mesajlaşma ve Saga (State Machine) orkestrasyonu. | [MassTransit README](./CKN.Sdk.MassTransit/README.md) |
| **CKN.Sdk.AI** | LLM entegrasyonları (Microsoft.Extensions.AI) ve Semantic Kernel altyapısı. | [AI README](./CKN.Sdk.AI/README.md) |
| **CKN.Sdk.SourceGenerators** | Reflection yerine derleme anında DI ve CQRS kodları üreten Roslyn eklentisi. | [SourceGenerators README](./CKN.Sdk.SourceGenerators/README.md) |
| **CKN.Sdk.Infrastructure** | Ortak HTTP, Güvenlik (JWT) ve Caching altyapısı. | [Infrastructure README](./CKN.Sdk.Infrastructure/README.md) |

## 🚀 Hızlı Başlangıç

Bu SDK'leri projelerinize dahil etmek için şirketinizin (veya projenizin) özel NuGet kaynağını kullanmalısınız. Örnek bir Web API projesine temel paketleri kurmak için:

```bash
dotnet add package CKN.Sdk.Core
dotnet add package CKN.Sdk.EntityFramework
dotnet add package CKN.Sdk.AI
```

Daha sonra `Program.cs` içerisinde tek satırla sistemleri ayağa kaldırabilirsiniz:

```csharp
builder.Services.AddCknCore()
                .AddCknEntityFramework(options => options.UseSqlServer("..."))
                .AddCknAiServices();
```

## 🏗️ Mimari ve Yönetişim (Governance)
Bu projenin nasıl tasarlandığını, hangi kararların neden alındığını ve kod standartlarını merak ediyorsanız [Governance](./governance) klasörünü inceleyiniz. Yapay Zeka (AI) ajanları için kurallar `.agents/rules.md` dosyasında tanımlıdır.
