<div align="center">
  <h1>ðŸš€ CKN.SDK (AI FRIENDLY SDK)</h1>
  <p><b>2026 StandartlarÄ±nda, SÄ±fÄ±r BaÄŸÄ±mlÄ±lÄ±k (Zero-Dependency) Hedefli, Plugin TabanlÄ± Enterprise .NET Framework'Ã¼</b></p>

  [![Build Status](https://github.com/OzanCKN/CKN.SDK/actions/workflows/ci.yml/badge.svg)](https://github.com/OzanCKN/CKN.SDK/actions)
  [![NuGet Publish](https://github.com/OzanCKN/CKN.SDK/actions/workflows/nuget-publish.yml/badge.svg)](https://github.com/OzanCKN/CKN.SDK/actions/workflows/nuget-publish.yml)
  [![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)
  [![Native AOT](https://img.shields.io/badge/Native_AOT-Ready-success.svg)]()
  [![.NET](https://img.shields.io/badge/.NET-8%20%7C%209%20%7C%2010-512BD4.svg)](https://dotnet.microsoft.com)
</div>

CKN.SDK, Native AOT uyumlu, modÃ¼ler, olay gÃ¼dÃ¼mlÃ¼ (event-driven) ve yapay zeka destekli mikroservisler inÅŸa etmek iÃ§in geliÅŸtirilmiÅŸ resmi enterprise altyapÄ± kÃ¼tÃ¼phanesidir. Her modÃ¼l baÄŸÄ±msÄ±z bir NuGet paketi olarak daÄŸÄ±tÄ±lÄ±r; projenize yalnÄ±zca ihtiyaÃ§ duyduÄŸunuz paketi kurarsÄ±nÄ±z.

---

## ðŸŒŸ Provider-Agnostic Mimari

Geleneksel SDK'lar belirli teknolojilere sÄ±kÄ±ca baÄŸlÄ±dÄ±r. **CKN.SDK** bunun yerine **Multi-Provider (Plugin TabanlÄ±)** bir mimari sunar:

```csharp
// RabbitMQ â†’ Kafka geÃ§iÅŸi: tek satÄ±r
services.AddCknMessaging(m => m.UseKafka(...));   // eskiden m.UseRabbitMQ(...)

// Redis â†’ Garnet geÃ§iÅŸi: tek satÄ±r
services.AddCknGarnetCache(...);   // eskiden AddCknRedisCache(...)
```

AltyapÄ± seÃ§imini deÄŸiÅŸtirmek iÃ§in domain kodunuza dokunmazsÄ±nÄ±z.

---

## ðŸ§© Tak-Ã‡alÄ±ÅŸtÄ±r ModÃ¼ller (NuGet Paketleri)

### ðŸ—ï¸ Temel Katman (Foundation)

| Paket AdÄ± | SÃ¼rÃ¼m | AÃ§Ä±klama | DokÃ¼mantasyon |
| :--- | :--- | :--- | :--- |
| `CKN.Sdk.Core` | [![NuGet](https://img.shields.io/nuget/v/CKN.Sdk.Core.svg?style=flat-square&label=)](https://www.nuget.org/packages/CKN.Sdk.Core/) | Sistemin kalbi. SÄ±fÄ±r baÄŸÄ±mlÄ±lÄ±k. Domain nesneleri, CQRS, `Result<T>`, `Error`. | [ðŸ“– README](CKN.Sdk.Core/README.md) |
| `CKN.Sdk.AspNetCore` | [![NuGet](https://img.shields.io/nuget/v/CKN.Sdk.AspNetCore.svg?style=flat-square&label=)](https://www.nuget.org/packages/CKN.Sdk.AspNetCore/) | Web katmanÄ±: `GlobalExceptionHandler`, ProblemDetails, `AddCknAspNetCore()`. | [ðŸ“– README](CKN.Sdk.AspNetCore/README.md) |
| `CKN.Sdk.Infrastructure` | [![NuGet](https://img.shields.io/nuget/v/CKN.Sdk.Infrastructure.svg?style=flat-square&label=)](https://www.nuget.org/packages/CKN.Sdk.Infrastructure/) | HTTP resilience, Hybrid Cache, MediatR pipeline behaviors. | [ðŸ“– README](CKN.Sdk.Infrastructure/README.md) |
| `CKN.Sdk.Security` | [![NuGet](https://img.shields.io/nuget/v/CKN.Sdk.Security.svg?style=flat-square&label=)](https://www.nuget.org/packages/CKN.Sdk.Security/) | JWT kimlik doÄŸrulama, `AddCknSecurity()`. | [ðŸ“– README](CKN.Sdk.Security/README.md) |
| `CKN.Sdk.Telemetry` | [![NuGet](https://img.shields.io/nuget/v/CKN.Sdk.Telemetry.svg?style=flat-square&label=)](https://www.nuget.org/packages/CKN.Sdk.Telemetry/) | OpenTelemetry tracing & metrics, `AddCKNTelemetry()`. | [ðŸ“– README](CKN.Sdk.Telemetry/README.md) |
| `CKN.Sdk.SourceGenerators` | [![NuGet](https://img.shields.io/nuget/v/CKN.Sdk.SourceGenerators.svg?style=flat-square&label=)](https://www.nuget.org/packages/CKN.Sdk.SourceGenerators/) | Roslyn source generator â€” AOT iÃ§in compile-time DI/CQRS Ã¼retimi. | [ðŸ“– README](CKN.Sdk.SourceGenerators/README.md) |

### ðŸ“¬ MesajlaÅŸma (Messaging)

| Paket AdÄ± | SÃ¼rÃ¼m | AÃ§Ä±klama | DokÃ¼mantasyon |
| :--- | :--- | :--- | :--- |
| `CKN.Sdk.Messaging` | [![NuGet](https://img.shields.io/nuget/v/CKN.Sdk.Messaging.svg?style=flat-square&label=)](https://www.nuget.org/packages/CKN.Sdk.Messaging/) | Provider-agnostic `IEventBus` ve `IEvent` arayÃ¼zleri. | [ðŸ“– README](CKN.Sdk.Messaging/README.md) |
| `CKN.Sdk.Messaging.RabbitMQ` | [![NuGet](https://img.shields.io/nuget/v/CKN.Sdk.Messaging.RabbitMQ.svg?style=flat-square&label=)](https://www.nuget.org/packages/CKN.Sdk.Messaging.RabbitMQ/) | RabbitMQ native client entegrasyonu, `AddCknRabbitMQ()`. | [ðŸ“– README](CKN.Sdk.Messaging.RabbitMQ/README.md) |
| `CKN.Sdk.Messaging.Kafka` | [![NuGet](https://img.shields.io/nuget/v/CKN.Sdk.Messaging.Kafka.svg?style=flat-square&label=)](https://www.nuget.org/packages/CKN.Sdk.Messaging.Kafka/) | Apache Kafka (Confluent) entegrasyonu, yÃ¼ksek throughput. | [ðŸ“– README](CKN.Sdk.Messaging.Kafka/README.md) |
| `CKN.Sdk.Messaging.ServiceBus` | [![NuGet](https://img.shields.io/nuget/v/CKN.Sdk.Messaging.ServiceBus.svg?style=flat-square&label=)](https://www.nuget.org/packages/CKN.Sdk.Messaging.ServiceBus/) | Azure Service Bus entegrasyonu, `AddCknServiceBus()`. | [ðŸ“– README](CKN.Sdk.Messaging.ServiceBus/README.md) |

### âš¡ Ã–nbellek (Caching)

| Paket AdÄ± | SÃ¼rÃ¼m | AÃ§Ä±klama | DokÃ¼mantasyon |
| :--- | :--- | :--- | :--- |
| `CKN.Sdk.Caching.Redis` | [![NuGet](https://img.shields.io/nuget/v/CKN.Sdk.Caching.Redis.svg?style=flat-square&label=)](https://www.nuget.org/packages/CKN.Sdk.Caching.Redis/) | StackExchange.Redis tabanlÄ± daÄŸÄ±tÄ±k Ã¶nbellek, `AddCknRedisCache()`. | [ðŸ“– README](CKN.Sdk.Caching.Redis/README.md) |
| `CKN.Sdk.Caching.Garnet` | [![NuGet](https://img.shields.io/nuget/v/CKN.Sdk.Caching.Garnet.svg?style=flat-square&label=)](https://www.nuget.org/packages/CKN.Sdk.Caching.Garnet/) | Microsoft Research'Ã¼n yeni nesil ultra-hÄ±zlÄ± Garnet Ã¶nbelleÄŸi. | [ðŸ“– README](CKN.Sdk.Caching.Garnet/README.md) |
| `CKN.Sdk.Caching.Memcached` | [![NuGet](https://img.shields.io/nuget/v/CKN.Sdk.Caching.Memcached.svg?style=flat-square&label=)](https://www.nuget.org/packages/CKN.Sdk.Caching.Memcached/) | EnyimMemcached tabanlÄ± Memcached entegrasyonu. | [ðŸ“– README](CKN.Sdk.Caching.Memcached/README.md) |

### ðŸ’¾ Veri EriÅŸimi (Data Access)

| Paket AdÄ± | SÃ¼rÃ¼m | AÃ§Ä±klama | DokÃ¼mantasyon |
| :--- | :--- | :--- | :--- |
| `CKN.Sdk.EntityFramework` | [![NuGet](https://img.shields.io/nuget/v/CKN.Sdk.EntityFramework.svg?style=flat-square&label=)](https://www.nuget.org/packages/CKN.Sdk.EntityFramework/) | EF Core ile `IRepository` ve `IUnitOfWork` implementasyonu. | [ðŸ“– README](CKN.Sdk.EntityFramework/README.md) |
| `CKN.Sdk.Data.Dapper` | [![NuGet](https://img.shields.io/nuget/v/CKN.Sdk.Data.Dapper.svg?style=flat-square&label=)](https://www.nuget.org/packages/CKN.Sdk.Data.Dapper/) | Dapper tabanlÄ± mikro-ORM, `AddCknDapper()`. | [ðŸ“– README](CKN.Sdk.Data.Dapper/README.md) |
| `CKN.Sdk.Data.RepoDb` | [![NuGet](https://img.shields.io/nuget/v/CKN.Sdk.Data.RepoDb.svg?style=flat-square&label=)](https://www.nuget.org/packages/CKN.Sdk.Data.RepoDb/) | RepoDb ile bulk operasyonlar, `AddCknRepoDbPostgres()`. | [ðŸ“– README](CKN.Sdk.Data.RepoDb/README.md) |

### ðŸ¤– Yapay Zeka (AI & LLMs)

| Paket AdÄ± | SÃ¼rÃ¼m | AÃ§Ä±klama | DokÃ¼mantasyon |
| :--- | :--- | :--- | :--- |
| `CKN.Sdk.AI` | [![NuGet](https://img.shields.io/nuget/v/CKN.Sdk.AI.svg?style=flat-square&label=)](https://www.nuget.org/packages/CKN.Sdk.AI/) | `Microsoft.Extensions.AI` tabanlÄ± `ICknAiChatService` arayÃ¼zÃ¼. | [ðŸ“– README](CKN.Sdk.AI/README.md) |
| `CKN.Sdk.AI.OpenAI` | [![NuGet](https://img.shields.io/nuget/v/CKN.Sdk.AI.OpenAI.svg?style=flat-square&label=)](https://www.nuget.org/packages/CKN.Sdk.AI.OpenAI/) | GPT-4o ve diÄŸer OpenAI modelleri, `AddCknOpenAI()`. | [ðŸ“– README](CKN.Sdk.AI.OpenAI/README.md) |
| `CKN.Sdk.AI.Anthropic` | [![NuGet](https://img.shields.io/nuget/v/CKN.Sdk.AI.Anthropic.svg?style=flat-square&label=)](https://www.nuget.org/packages/CKN.Sdk.AI.Anthropic/) | Claude modelleri (Anthropic), `AddCknAnthropic()`. | [ðŸ“– README](CKN.Sdk.AI.Anthropic/README.md) |
| `CKN.Sdk.AI.Ollama` | [![NuGet](https://img.shields.io/nuget/v/CKN.Sdk.AI.Ollama.svg?style=flat-square&label=)](https://www.nuget.org/packages/CKN.Sdk.AI.Ollama/) | KVKK uyumlu yerel LLM Ã§alÄ±ÅŸtÄ±rmak iÃ§in Ollama, `AddCknOllama()`. | [ðŸ“– README](CKN.Sdk.AI.Ollama/README.md) |
| `CKN.Sdk.AI.SemanticKernel` | [![NuGet](https://img.shields.io/nuget/v/CKN.Sdk.AI.SemanticKernel.svg?style=flat-square&label=)](https://www.nuget.org/packages/CKN.Sdk.AI.SemanticKernel/) | Microsoft Semantic Kernel ile AI ajan orkestrasyonu ve Tool Calling. | [ðŸ“– README](CKN.Sdk.AI.SemanticKernel/README.md) |

### ðŸŒ HTTP Ä°stemci (Network)

| Paket AdÄ± | SÃ¼rÃ¼m | AÃ§Ä±klama | DokÃ¼mantasyon |
| :--- | :--- | :--- | :--- |
| `CKN.Sdk.Network` | [![NuGet](https://img.shields.io/nuget/v/CKN.Sdk.Network.svg?style=flat-square&label=)](https://www.nuget.org/packages/CKN.Sdk.Network/) | Provider-agnostic `ICknHttpClient`, auth stratejileri, resilience options. | [ðŸ“– README](CKN.Sdk.Network/README.md) |
| `CKN.Sdk.Network.Http` | [![NuGet](https://img.shields.io/nuget/v/CKN.Sdk.Network.Http.svg?style=flat-square&label=)](https://www.nuget.org/packages/CKN.Sdk.Network.Http/) | HttpClient provider: retry, circuit breaker, rate limiting, maskeleme. | [ðŸ“– README](CKN.Sdk.Network.Http/README.md) |

### ðŸ“£ Bildirim (Notification)

| Paket AdÄ± | SÃ¼rÃ¼m | AÃ§Ä±klama | DokÃ¼mantasyon |
| :--- | :--- | :--- | :--- |
| `CKN.Sdk.Notification` | [![NuGet](https://img.shields.io/nuget/v/CKN.Sdk.Notification.svg?style=flat-square&label=)](https://www.nuget.org/packages/CKN.Sdk.Notification/) | E-posta (SMTP/SendGrid), SMS (Twilio), Push (Firebase) bildirim servisleri. | [ðŸ“– README](CKN.Sdk.Notification/README.md) |

### â˜ï¸ Depolama (Storage)

| Paket AdÄ± | SÃ¼rÃ¼m | AÃ§Ä±klama | DokÃ¼mantasyon |
| :--- | :--- | :--- | :--- |
| `CKN.Sdk.Storage` | [![NuGet](https://img.shields.io/nuget/v/CKN.Sdk.Storage.svg?style=flat-square&label=)](https://www.nuget.org/packages/CKN.Sdk.Storage/) | Provider-agnostic `IStorageService` nesne depolama arayÃ¼zÃ¼. | [ðŸ“– README](CKN.Sdk.Storage/README.md) |
| `CKN.Sdk.Storage.Minio` | [![NuGet](https://img.shields.io/nuget/v/CKN.Sdk.Storage.Minio.svg?style=flat-square&label=)](https://www.nuget.org/packages/CKN.Sdk.Storage.Minio/) | S3 uyumlu yerel/bulut object storage, `AddCknMinioStorage()`. | [ðŸ“– README](CKN.Sdk.Storage.Minio/README.md) |
| `CKN.Sdk.Storage.S3` | [![NuGet](https://img.shields.io/nuget/v/CKN.Sdk.Storage.S3.svg?style=flat-square&label=)](https://www.nuget.org/packages/CKN.Sdk.Storage.S3/) | Amazon AWS S3, `AddCknS3Storage()`. | [ðŸ“– README](CKN.Sdk.Storage.S3/README.md) |
| `CKN.Sdk.Storage.Azure` | [![NuGet](https://img.shields.io/nuget/v/CKN.Sdk.Storage.Azure.svg?style=flat-square&label=)](https://www.nuget.org/packages/CKN.Sdk.Storage.Azure/) | Azure Blob Storage, `AddCknAzureStorage()`. | [ðŸ“– README](CKN.Sdk.Storage.Azure/README.md) |

### â° ZamanlayÄ±cÄ± (Scheduling)

| Paket AdÄ± | SÃ¼rÃ¼m | AÃ§Ä±klama | DokÃ¼mantasyon |
| :--- | :--- | :--- | :--- |
| `CKN.Sdk.Scheduling.Hangfire` | [![NuGet](https://img.shields.io/nuget/v/CKN.Sdk.Scheduling.Hangfire.svg?style=flat-square&label=)](https://www.nuget.org/packages/CKN.Sdk.Scheduling.Hangfire/) | Hangfire â€” veritabanÄ± destekli, dashboard'lu gÃ¶rev zamanlama. | [ðŸ“– README](CKN.Sdk.Scheduling.Hangfire/README.md) |
| `CKN.Sdk.Scheduling.Quartz` | [![NuGet](https://img.shields.io/nuget/v/CKN.Sdk.Scheduling.Quartz.svg?style=flat-square&label=)](https://www.nuget.org/packages/CKN.Sdk.Scheduling.Quartz/) | Quartz.NET ile cron tabanlÄ± zamanlama, `AddCknQuartz()`. | [ðŸ“– README](CKN.Sdk.Scheduling.Quartz/README.md) |
| `CKN.Sdk.Scheduling.Coravel` | [![NuGet](https://img.shields.io/nuget/v/CKN.Sdk.Scheduling.Coravel.svg?style=flat-square&label=)](https://www.nuget.org/packages/CKN.Sdk.Scheduling.Coravel/) | SÄ±fÄ±r konfigÃ¼rasyon, bellek iÃ§i zamanlayÄ±cÄ±. | [ðŸ“– README](CKN.Sdk.Scheduling.Coravel/README.md) |

### ðŸ” Arama (Search)

| Paket AdÄ± | SÃ¼rÃ¼m | AÃ§Ä±klama | DokÃ¼mantasyon |
| :--- | :--- | :--- | :--- |
| `CKN.Sdk.Search` | [![NuGet](https://img.shields.io/nuget/v/CKN.Sdk.Search.svg?style=flat-square&label=)](https://www.nuget.org/packages/CKN.Sdk.Search/) | Provider-agnostic `ISearchService<T>` full-text arama arayÃ¼zÃ¼. | [ðŸ“– README](CKN.Sdk.Search/README.md) |
| `CKN.Sdk.Search.Elasticsearch` | [![NuGet](https://img.shields.io/nuget/v/CKN.Sdk.Search.Elasticsearch.svg?style=flat-square&label=)](https://www.nuget.org/packages/CKN.Sdk.Search.Elasticsearch/) | Elasticsearch fuzzy search, `AddCknElasticsearch()`. | [ðŸ“– README](CKN.Sdk.Search.Elasticsearch/README.md) |
| `CKN.Sdk.Search.Meilisearch` | [![NuGet](https://img.shields.io/nuget/v/CKN.Sdk.Search.Meilisearch.svg?style=flat-square&label=)](https://www.nuget.org/packages/CKN.Sdk.Search.Meilisearch/) | Ultra hÄ±zlÄ±, typo-tolerant Meilisearch, `AddCknMeilisearch()`. | [ðŸ“– README](CKN.Sdk.Search.Meilisearch/README.md) |
| `CKN.Sdk.Search.NRedisStack` | [![NuGet](https://img.shields.io/nuget/v/CKN.Sdk.Search.NRedisStack.svg?style=flat-square&label=)](https://www.nuget.org/packages/CKN.Sdk.Search.NRedisStack/) | Redis tabanlÄ± RediSearch (NRedisStack), `AddCknNRedisStack()`. | [ðŸ“– README](CKN.Sdk.Search.NRedisStack/README.md) |

---

## ðŸš€ HÄ±zlÄ± BaÅŸlangÄ±Ã§

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

// HTTP saÄŸlayÄ±cÄ±larÄ± iÃ§in provider-agnostic istemci
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

## ðŸ—ï¸ Mimari ve YÃ¶netiÅŸim (Governance)

Bu projenin nasÄ±l tasarlandÄ±ÄŸÄ±nÄ±, "Zero-Warning Policy" gibi standartlarÄ± ve geliÅŸtirici kurallarÄ±nÄ± aÅŸaÄŸÄ±daki dokÃ¼manlarda bulabilirsiniz:

| DokÃ¼man | AÃ§Ä±klama |
| :--- | :--- |
| [System Glossary](governance/docs/architecture/system-glossary.md) | TÃ¼m modÃ¼llerin ve kavramlarÄ±n aÃ§Ä±klandÄ±ÄŸÄ± proje haritasÄ±. |
| [Architecture Decision Log](governance/docs/architecture/decision-log.md) | "Neden bu teknolojiyi seÃ§tik?" sorularÄ±nÄ±n cevabÄ± (ADR). |
| [Sprint Ä°ndeksi](governance/sprints/index.md) | Aktif ve tamamlanmÄ±ÅŸ sprint geÃ§miÅŸi. |
| [AI Ajan KurallarÄ±](.agents/rules.md) | AI ajanlarÄ±nÄ±n projede uymasÄ± gereken standartlar. |

---

## ðŸ“Š Paket Durum Tablosu

| ModÃ¼l | SaÄŸlayÄ±cÄ±lar | Durum |
| :--- | :--- | :--- |
| **`CKN.Sdk.Core`** | SÄ±fÄ±r baÄŸÄ±mlÄ±lÄ±k â€” tÃ¼m arayÃ¼zler, CQRS, Result | ðŸŸ¢ HazÄ±r |
| **`CKN.Sdk.AspNetCore`** | GlobalExceptionHandler, ProblemDetails | ðŸŸ¢ HazÄ±r |
| **`CKN.Sdk.Infrastructure`** | Hybrid Cache, MediatR behaviors, HTTP resilience | ðŸŸ¢ HazÄ±r |
| **`CKN.Sdk.Security`** | JWT kimlik doÄŸrulama | ðŸŸ¢ HazÄ±r |
| **`CKN.Sdk.Telemetry`** | OpenTelemetry (Traces, Metrics) | ðŸŸ¢ HazÄ±r |
| **`CKN.Sdk.Messaging`** | RabbitMQ Â· Kafka Â· Azure Service Bus | ðŸŸ¢ HazÄ±r |
| **`CKN.Sdk.Caching`** | Redis Â· Garnet Â· Memcached | ðŸŸ¢ HazÄ±r |
| **`CKN.Sdk.Data`** | EF Core Â· Dapper Â· RepoDb | ðŸŸ¢ HazÄ±r |
| **`CKN.Sdk.AI`** | OpenAI Â· Anthropic Â· Ollama Â· Semantic Kernel | ðŸŸ¢ HazÄ±r |
| **`CKN.Sdk.Network`** | HttpClient (Faz 1) | ðŸŸ¢ HazÄ±r |
| **`CKN.Sdk.Notification`** | SMTP Â· SendGrid Â· Twilio Â· Firebase | ðŸŸ¢ HazÄ±r |
| **`CKN.Sdk.Storage`** | Minio Â· AWS S3 Â· Azure Blob | ðŸŸ¢ HazÄ±r |
| **`CKN.Sdk.Scheduling`** | Hangfire Â· Quartz Â· Coravel | ðŸŸ¢ HazÄ±r |
| **`CKN.Sdk.Search`** | Elasticsearch Â· Meilisearch Â· NRedisStack | ðŸŸ¢ HazÄ±r |
| **`CKN.Sdk.SourceGenerators`** | Roslyn AOT source generator | ðŸŸ¡ GeliÅŸtiriliyor |
| **`CKN.Sdk.Financial`** | Market takvimi, OHLC, portfÃ¶y matematiÄŸi | ðŸŸ¢ HazÄ±r |
| **`CKN.Sdk.Network.Flurl`** | Flurl.Http provider | ðŸ”µ PlanlanÄ±yor |
| **`CKN.Sdk.Network.RestSharp`** | RestSharp provider | ðŸ”µ PlanlanÄ±yor |
| **`CKN.Sdk.Network.Refit`** | Refit (interface-based) provider | ðŸ”µ PlanlanÄ±yor |

---

## ðŸ§ª Test ve GÃ¼venilirlik

- **%100 TDD zorunluluÄŸu** â€” her provider xUnit, Moq ve FluentAssertions ile test edilir.
- **Zero-Warning Policy** â€” `TreatWarningsAsErrors=true` global olarak aktif, CI'da uyarÄ± = hata.
- **3 TFM hedefi** â€” `net8.0`, `net9.0`, `net10.0` Ã¼Ã§Ã¼nde de testler Ã§alÄ±ÅŸÄ±r.
- **Native AOT uyumlu** â€” `IsAotCompatible=true` tÃ¼m paketlerde zorunlu.

---

<div align="center">
  <i>2026 Â© CKN Enterprise Architecture Team</i>
</div>

