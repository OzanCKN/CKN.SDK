<div align="center">
  <h1>🚀 CKN.SDK (AI FRIENDLY SDK)</h1>
  <p><b>2026 Standartlarında, Sıfır Bağımlılık (Zero-Dependency) Hedefli, Plugin Tabanlı Enterprise .NET Framework'ü</b></p>

  [![Build Status](https://github.com/OzanCKN/CKN.SDK/actions/workflows/ci.yml/badge.svg)](https://github.com/OzanCKN/CKN.SDK/actions)
  [![NuGet Publish](https://github.com/OzanCKN/CKN.SDK/actions/workflows/nuget-publish.yml/badge.svg)](https://github.com/OzanCKN/CKN.SDK/actions/workflows/nuget-publish.yml)
  [![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)
  [![Native AOT](https://img.shields.io/badge/Native_AOT-Ready-success.svg)]()
  [![.NET](https://img.shields.io/badge/.NET-8%20%7C%209%20%7C%2010-512BD4.svg)](https://dotnet.microsoft.com)
</div>

CKN.SDK, Native AOT uyumlu, modüler, olay güdümlü (event-driven) ve yapay zeka destekli mikroservisler inşa etmek için geliştirilmiş resmi enterprise altyapı kütüphanesidir. Her modül bağımsız bir NuGet paketi olarak dağıtılır; projenize yalnızca ihtiyaç duyduğunuz paketi kurarsınız.

---

## 🌟 Provider-Agnostic Mimari

Geleneksel SDK'lar belirli teknolojilere sıkıca bağlıdır. **CKN.SDK** bunun yerine **Multi-Provider (Plugin Tabanlı)** bir mimari sunar:

```csharp
// RabbitMQ → Kafka geçişi: tek satır
services.AddCknMessaging(m => m.UseKafka(...));   // eskiden m.UseRabbitMQ(...)

// Redis → Garnet geçişi: tek satır
services.AddCknGarnetCache(...);   // eskiden AddCknRedisCache(...)
```

Altyapı seçimini değiştirmek için domain kodunuza dokunmazsınız.

---

## 🧩 Tak-Çalıştır Modüller (NuGet Paketleri)

### 🏗️ Temel Katman (Foundation)

| Paket Adı | Sürüm | Açıklama | Dokümantasyon |
| :--- | :--- | :--- | :--- |
| `CKN.Sdk.Core` | [![NuGet](https://img.shields.io/nuget/v/CKN.Sdk.Core.svg?style=flat-square&label=)](https://www.nuget.org/packages/CKN.Sdk.Core/) | Sistemin kalbi. Sıfır bağımlılık. Domain nesneleri, CQRS, `Result<T>`, `Error`. | [📖 README](CKN.Sdk.Core/README.md) |
| `CKN.Sdk.AspNetCore` | [![NuGet](https://img.shields.io/nuget/v/CKN.Sdk.AspNetCore.svg?style=flat-square&label=)](https://www.nuget.org/packages/CKN.Sdk.AspNetCore/) | Web katmanı: `GlobalExceptionHandler`, ProblemDetails, `AddCknAspNetCore()`. | [📖 README](CKN.Sdk.AspNetCore/README.md) |
| `CKN.Sdk.Infrastructure` | [![NuGet](https://img.shields.io/nuget/v/CKN.Sdk.Infrastructure.svg?style=flat-square&label=)](https://www.nuget.org/packages/CKN.Sdk.Infrastructure/) | HTTP resilience, Hybrid Cache, MediatR pipeline behaviors. | [📖 README](CKN.Sdk.Infrastructure/README.md) |
| `CKN.Sdk.Security` | [![NuGet](https://img.shields.io/nuget/v/CKN.Sdk.Security.svg?style=flat-square&label=)](https://www.nuget.org/packages/CKN.Sdk.Security/) | JWT kimlik doğrulama, `AddCknSecurity()`. | [📖 README](CKN.Sdk.Security/README.md) |
| `CKN.Sdk.Telemetry` | [![NuGet](https://img.shields.io/nuget/v/CKN.Sdk.Telemetry.svg?style=flat-square&label=)](https://www.nuget.org/packages/CKN.Sdk.Telemetry/) | OpenTelemetry tracing & metrics, `AddCKNTelemetry()`. | [📖 README](CKN.Sdk.Telemetry/README.md) |
| `CKN.Sdk.SourceGenerators` | [![NuGet](https://img.shields.io/nuget/v/CKN.Sdk.SourceGenerators.svg?style=flat-square&label=)](https://www.nuget.org/packages/CKN.Sdk.SourceGenerators/) | Roslyn source generator — AOT için compile-time DI/CQRS üretimi. | [📖 README](CKN.Sdk.SourceGenerators/README.md) |

### 📬 Mesajlaşma (Messaging)

| Paket Adı | Sürüm | Açıklama | Dokümantasyon |
| :--- | :--- | :--- | :--- |
| `CKN.Sdk.Messaging` | [![NuGet](https://img.shields.io/nuget/v/CKN.Sdk.Messaging.svg?style=flat-square&label=)](https://www.nuget.org/packages/CKN.Sdk.Messaging/) | Provider-agnostic `IEventBus` ve `IEvent` arayüzleri. | [📖 README](CKN.Sdk.Messaging/README.md) |
| `CKN.Sdk.Messaging.RabbitMQ` | [![NuGet](https://img.shields.io/nuget/v/CKN.Sdk.Messaging.RabbitMQ.svg?style=flat-square&label=)](https://www.nuget.org/packages/CKN.Sdk.Messaging.RabbitMQ/) | RabbitMQ native client entegrasyonu, `AddCknRabbitMQ()`. | [📖 README](CKN.Sdk.Messaging.RabbitMQ/README.md) |
| `CKN.Sdk.Messaging.Kafka` | [![NuGet](https://img.shields.io/nuget/v/CKN.Sdk.Messaging.Kafka.svg?style=flat-square&label=)](https://www.nuget.org/packages/CKN.Sdk.Messaging.Kafka/) | Apache Kafka (Confluent) entegrasyonu, yüksek throughput. | [📖 README](CKN.Sdk.Messaging.Kafka/README.md) |
| `CKN.Sdk.Messaging.ServiceBus` | [![NuGet](https://img.shields.io/nuget/v/CKN.Sdk.Messaging.ServiceBus.svg?style=flat-square&label=)](https://www.nuget.org/packages/CKN.Sdk.Messaging.ServiceBus/) | Azure Service Bus entegrasyonu, `AddCknServiceBus()`. | [📖 README](CKN.Sdk.Messaging.ServiceBus/README.md) |

### ⚡ Önbellek (Caching)

| Paket Adı | Sürüm | Açıklama | Dokümantasyon |
| :--- | :--- | :--- | :--- |
| `CKN.Sdk.Caching.Redis` | [![NuGet](https://img.shields.io/nuget/v/CKN.Sdk.Caching.Redis.svg?style=flat-square&label=)](https://www.nuget.org/packages/CKN.Sdk.Caching.Redis/) | StackExchange.Redis tabanlı dağıtık önbellek, `AddCknRedisCache()`. | [📖 README](CKN.Sdk.Caching.Redis/README.md) |
| `CKN.Sdk.Caching.Garnet` | [![NuGet](https://img.shields.io/nuget/v/CKN.Sdk.Caching.Garnet.svg?style=flat-square&label=)](https://www.nuget.org/packages/CKN.Sdk.Caching.Garnet/) | Microsoft Research'ün yeni nesil ultra-hızlı Garnet önbelleği. | [📖 README](CKN.Sdk.Caching.Garnet/README.md) |
| `CKN.Sdk.Caching.Memcached` | [![NuGet](https://img.shields.io/nuget/v/CKN.Sdk.Caching.Memcached.svg?style=flat-square&label=)](https://www.nuget.org/packages/CKN.Sdk.Caching.Memcached/) | EnyimMemcached tabanlı Memcached entegrasyonu. | [📖 README](CKN.Sdk.Caching.Memcached/README.md) |

### 💾 Veri Erişimi (Data Access)

| Paket Adı | Sürüm | Açıklama | Dokümantasyon |
| :--- | :--- | :--- | :--- |
| `CKN.Sdk.EntityFramework` | [![NuGet](https://img.shields.io/nuget/v/CKN.Sdk.EntityFramework.svg?style=flat-square&label=)](https://www.nuget.org/packages/CKN.Sdk.EntityFramework/) | EF Core ile `IRepository` ve `IUnitOfWork` implementasyonu. | [📖 README](CKN.Sdk.EntityFramework/README.md) |
| `CKN.Sdk.Data.Dapper` | [![NuGet](https://img.shields.io/nuget/v/CKN.Sdk.Data.Dapper.svg?style=flat-square&label=)](https://www.nuget.org/packages/CKN.Sdk.Data.Dapper/) | Dapper tabanlı mikro-ORM, `AddCknDapper()`. | [📖 README](CKN.Sdk.Data.Dapper/README.md) |
| `CKN.Sdk.Data.RepoDb` | [![NuGet](https://img.shields.io/nuget/v/CKN.Sdk.Data.RepoDb.svg?style=flat-square&label=)](https://www.nuget.org/packages/CKN.Sdk.Data.RepoDb/) | RepoDb ile bulk operasyonlar, `AddCknRepoDbPostgres()`. | [📖 README](CKN.Sdk.Data.RepoDb/README.md) |

### 🤖 Yapay Zeka (AI & LLMs)

| Paket Adı | Sürüm | Açıklama | Dokümantasyon |
| :--- | :--- | :--- | :--- |
| `CKN.Sdk.AI` | [![NuGet](https://img.shields.io/nuget/v/CKN.Sdk.AI.svg?style=flat-square&label=)](https://www.nuget.org/packages/CKN.Sdk.AI/) | `Microsoft.Extensions.AI` tabanlı `ICknAiChatService` arayüzü. | [📖 README](CKN.Sdk.AI/README.md) |
| `CKN.Sdk.AI.OpenAI` | [![NuGet](https://img.shields.io/nuget/v/CKN.Sdk.AI.OpenAI.svg?style=flat-square&label=)](https://www.nuget.org/packages/CKN.Sdk.AI.OpenAI/) | GPT-4o ve diğer OpenAI modelleri, `AddCknOpenAI()`. | [📖 README](CKN.Sdk.AI.OpenAI/README.md) |
| `CKN.Sdk.AI.Anthropic` | [![NuGet](https://img.shields.io/nuget/v/CKN.Sdk.AI.Anthropic.svg?style=flat-square&label=)](https://www.nuget.org/packages/CKN.Sdk.AI.Anthropic/) | Claude modelleri (Anthropic), `AddCknAnthropic()`. | [📖 README](CKN.Sdk.AI.Anthropic/README.md) |
| `CKN.Sdk.AI.Ollama` | [![NuGet](https://img.shields.io/nuget/v/CKN.Sdk.AI.Ollama.svg?style=flat-square&label=)](https://www.nuget.org/packages/CKN.Sdk.AI.Ollama/) | KVKK uyumlu yerel LLM çalıştırmak için Ollama, `AddCknOllama()`. | [📖 README](CKN.Sdk.AI.Ollama/README.md) |
| `CKN.Sdk.AI.SemanticKernel` | [![NuGet](https://img.shields.io/nuget/v/CKN.Sdk.AI.SemanticKernel.svg?style=flat-square&label=)](https://www.nuget.org/packages/CKN.Sdk.AI.SemanticKernel/) | Microsoft Semantic Kernel ile AI ajan orkestrasyonu ve Tool Calling. | [📖 README](CKN.Sdk.AI.SemanticKernel/README.md) |

### 🌐 HTTP İstemci (Network)

| Paket Adı | Sürüm | Açıklama | Dokümantasyon |
| :--- | :--- | :--- | :--- |
| `CKN.Sdk.Network` | [![NuGet](https://img.shields.io/nuget/v/CKN.Sdk.Network.svg?style=flat-square&label=)](https://www.nuget.org/packages/CKN.Sdk.Network/) | Provider-agnostic `ICknHttpClient`, auth stratejileri, resilience options. | [📖 README](CKN.Sdk.Network/README.md) |
| `CKN.Sdk.Network.Http` | [![NuGet](https://img.shields.io/nuget/v/CKN.Sdk.Network.Http.svg?style=flat-square&label=)](https://www.nuget.org/packages/CKN.Sdk.Network.Http/) | HttpClient provider: retry, circuit breaker, rate limiting, maskeleme. | [📖 README](CKN.Sdk.Network.Http/README.md) |

### 📣 Bildirim (Notification)

| Paket Adı | Sürüm | Açıklama | Dokümantasyon |
| :--- | :--- | :--- | :--- |
| `CKN.Sdk.Notification` | [![NuGet](https://img.shields.io/nuget/v/CKN.Sdk.Notification.svg?style=flat-square&label=)](https://www.nuget.org/packages/CKN.Sdk.Notification/) | E-posta (SMTP/SendGrid), SMS (Twilio), Push (Firebase) bildirim servisleri. | [📖 README](CKN.Sdk.Notification/README.md) |

### ☁️ Depolama (Storage)

| Paket Adı | Sürüm | Açıklama | Dokümantasyon |
| :--- | :--- | :--- | :--- |
| `CKN.Sdk.Storage` | [![NuGet](https://img.shields.io/nuget/v/CKN.Sdk.Storage.svg?style=flat-square&label=)](https://www.nuget.org/packages/CKN.Sdk.Storage/) | Provider-agnostic `IStorageService` nesne depolama arayüzü. | [📖 README](CKN.Sdk.Storage/README.md) |
| `CKN.Sdk.Storage.Minio` | [![NuGet](https://img.shields.io/nuget/v/CKN.Sdk.Storage.Minio.svg?style=flat-square&label=)](https://www.nuget.org/packages/CKN.Sdk.Storage.Minio/) | S3 uyumlu yerel/bulut object storage, `AddCknMinioStorage()`. | [📖 README](CKN.Sdk.Storage.Minio/README.md) |
| `CKN.Sdk.Storage.S3` | [![NuGet](https://img.shields.io/nuget/v/CKN.Sdk.Storage.S3.svg?style=flat-square&label=)](https://www.nuget.org/packages/CKN.Sdk.Storage.S3/) | Amazon AWS S3, `AddCknS3Storage()`. | [📖 README](CKN.Sdk.Storage.S3/README.md) |
| `CKN.Sdk.Storage.Azure` | [![NuGet](https://img.shields.io/nuget/v/CKN.Sdk.Storage.Azure.svg?style=flat-square&label=)](https://www.nuget.org/packages/CKN.Sdk.Storage.Azure/) | Azure Blob Storage, `AddCknAzureStorage()`. | [📖 README](CKN.Sdk.Storage.Azure/README.md) |

### ⏰ Zamanlayıcı (Scheduling)

| Paket Adı | Sürüm | Açıklama | Dokümantasyon |
| :--- | :--- | :--- | :--- |
| `CKN.Sdk.Scheduling.Hangfire` | [![NuGet](https://img.shields.io/nuget/v/CKN.Sdk.Scheduling.Hangfire.svg?style=flat-square&label=)](https://www.nuget.org/packages/CKN.Sdk.Scheduling.Hangfire/) | Hangfire — veritabanı destekli, dashboard'lu görev zamanlama. | [📖 README](CKN.Sdk.Scheduling.Hangfire/README.md) |
| `CKN.Sdk.Scheduling.Quartz` | [![NuGet](https://img.shields.io/nuget/v/CKN.Sdk.Scheduling.Quartz.svg?style=flat-square&label=)](https://www.nuget.org/packages/CKN.Sdk.Scheduling.Quartz/) | Quartz.NET ile cron tabanlı zamanlama, `AddCknQuartz()`. | [📖 README](CKN.Sdk.Scheduling.Quartz/README.md) |
| `CKN.Sdk.Scheduling.Coravel` | [![NuGet](https://img.shields.io/nuget/v/CKN.Sdk.Scheduling.Coravel.svg?style=flat-square&label=)](https://www.nuget.org/packages/CKN.Sdk.Scheduling.Coravel/) | Sıfır konfigürasyon, bellek içi zamanlayıcı. | [📖 README](CKN.Sdk.Scheduling.Coravel/README.md) |

### 🔍 Arama (Search)

| Paket Adı | Sürüm | Açıklama | Dokümantasyon |
| :--- | :--- | :--- | :--- |
| `CKN.Sdk.Search` | [![NuGet](https://img.shields.io/nuget/v/CKN.Sdk.Search.svg?style=flat-square&label=)](https://www.nuget.org/packages/CKN.Sdk.Search/) | Provider-agnostic `ISearchService<T>` full-text arama arayüzü. | [📖 README](CKN.Sdk.Search/README.md) |
| `CKN.Sdk.Search.Elasticsearch` | [![NuGet](https://img.shields.io/nuget/v/CKN.Sdk.Search.Elasticsearch.svg?style=flat-square&label=)](https://www.nuget.org/packages/CKN.Sdk.Search.Elasticsearch/) | Elasticsearch fuzzy search, `AddCknElasticsearch()`. | [📖 README](CKN.Sdk.Search.Elasticsearch/README.md) |
| `CKN.Sdk.Search.Meilisearch` | [![NuGet](https://img.shields.io/nuget/v/CKN.Sdk.Search.Meilisearch.svg?style=flat-square&label=)](https://www.nuget.org/packages/CKN.Sdk.Search.Meilisearch/) | Ultra hızlı, typo-tolerant Meilisearch, `AddCknMeilisearch()`. | [📖 README](CKN.Sdk.Search.Meilisearch/README.md) |
| `CKN.Sdk.Search.NRedisStack` | [![NuGet](https://img.shields.io/nuget/v/CKN.Sdk.Search.NRedisStack.svg?style=flat-square&label=)](https://www.nuget.org/packages/CKN.Sdk.Search.NRedisStack/) | Redis tabanlı RediSearch (NRedisStack), `AddCknNRedisStack()`. | [📖 README](CKN.Sdk.Search.NRedisStack/README.md) |

---

## 🚀 Hızlı Başlangıç

```bash
dotnet add package CKN.Sdk.Core
dotnet add package CKN.Sdk.Messaging.RabbitMQ
dotnet add package CKN.Sdk.Data.Dapper
dotnet add package CKN.Sdk.Caching.Redis
dotnet add package CKN.Sdk.Network
dotnet add package CKN.Sdk.Network.Http
```

```csharp
// Program.cs
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCknAspNetCore();
builder.Services.AddCknSecurity(builder.Configuration);

builder.Services.AddCknRabbitMQ(opt =>
    builder.Configuration.GetSection("RabbitMQ").Bind(opt));

builder.Services.AddCknDapper(opt =>
    builder.Configuration.GetSection("Database").Bind(opt));

builder.Services.AddCknRedisCache(opt =>
    builder.Configuration.GetSection("Redis").Bind(opt));

// HTTP sağlayıcıları için provider-agnostic istemci
builder.Services.AddCknNetwork(net =>
    net.AddCknHttpClient<MyApiClient>(opt =>
    {
        opt.BaseAddress = "https://api.example.com/";
        opt.Auth = new ApiKeyHeaderAuthStrategy("X-API-Key", builder.Configuration["Api:Key"]!);
        opt.RateLimit = new CknRateLimiterOptions { RequestsPerPeriod = 30 };
    }));

var app = builder.Build();
app.MapControllers();
app.Run();
```

---

## 🏗️ Mimari ve Yönetişim (Governance)

Bu projenin nasıl tasarlandığını, "Zero-Warning Policy" gibi standartları ve geliştirici kurallarını aşağıdaki dokümanlarda bulabilirsiniz:

| Doküman | Açıklama |
| :--- | :--- |
| [System Glossary](governance/docs/architecture/system-glossary.md) | Tüm modüllerin ve kavramların açıklandığı proje haritası. |
| [Architecture Decision Log](governance/docs/architecture/decision-log.md) | "Neden bu teknolojiyi seçtik?" sorularının cevabı (ADR). |
| [Sprint İndeksi](governance/sprints/index.md) | Aktif ve tamamlanmış sprint geçmişi. |
| [AI Ajan Kuralları](.agents/rules.md) | AI ajanlarının projede uyması gereken standartlar. |

---

## 📊 Paket Durum Tablosu

| Modül | Sağlayıcılar | Durum |
| :--- | :--- | :--- |
| **`CKN.Sdk.Core`** | Sıfır bağımlılık — tüm arayüzler, CQRS, Result | 🟢 Hazır |
| **`CKN.Sdk.AspNetCore`** | GlobalExceptionHandler, ProblemDetails | 🟢 Hazır |
| **`CKN.Sdk.Infrastructure`** | Hybrid Cache, MediatR behaviors, HTTP resilience | 🟢 Hazır |
| **`CKN.Sdk.Security`** | JWT kimlik doğrulama | 🟢 Hazır |
| **`CKN.Sdk.Telemetry`** | OpenTelemetry (Traces, Metrics) | 🟢 Hazır |
| **`CKN.Sdk.Messaging`** | RabbitMQ · Kafka · Azure Service Bus | 🟢 Hazır |
| **`CKN.Sdk.Caching`** | Redis · Garnet · Memcached | 🟢 Hazır |
| **`CKN.Sdk.Data`** | EF Core · Dapper · RepoDb | 🟢 Hazır |
| **`CKN.Sdk.AI`** | OpenAI · Anthropic · Ollama · Semantic Kernel | 🟢 Hazır |
| **`CKN.Sdk.Network`** | HttpClient (Faz 1) | 🟢 Hazır |
| **`CKN.Sdk.Notification`** | SMTP · SendGrid · Twilio · Firebase | 🟢 Hazır |
| **`CKN.Sdk.Storage`** | Minio · AWS S3 · Azure Blob | 🟢 Hazır |
| **`CKN.Sdk.Scheduling`** | Hangfire · Quartz · Coravel | 🟢 Hazır |
| **`CKN.Sdk.Search`** | Elasticsearch · Meilisearch · NRedisStack | 🟢 Hazır |
| **`CKN.Sdk.SourceGenerators`** | Roslyn AOT source generator — `IRequestHandler` otomatik DI kaydı | 🟢 Hazır |
| **`CKN.Sdk.Financial`** | Market takvimi, OHLC, portföy matematiği | 🔴 Henüz yok |
| **`CKN.Sdk.Network.Flurl`** | Flurl.Http provider | 🔵 Planlanıyor |
| **`CKN.Sdk.Network.RestSharp`** | RestSharp provider | 🔵 Planlanıyor |
| **`CKN.Sdk.Network.Refit`** | Refit (interface-based) provider | 🔵 Planlanıyor |

---

## 🧪 Test ve Güvenilirlik

- **%100 TDD zorunluluğu** — her provider xUnit, Moq ve FluentAssertions ile test edilir.
- **Zero-Warning Policy** — `TreatWarningsAsErrors=true` global olarak aktif, CI'da uyarı = hata.
- **3 TFM hedefi** — `net8.0`, `net9.0`, `net10.0` üçünde de testler çalışır.
- **Native AOT uyumlu** — `IsAotCompatible=true` tüm paketlerde zorunlu.

---

<div align="center">
  <i>2026 © CKN Enterprise Architecture Team</i>
</div>
