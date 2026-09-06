# CKN.SDK (Enterprise Agentic Orchestra)

[![NuGet CKN.Sdk.Core](https://img.shields.io/nuget/v/CKN.Sdk.Core.svg?label=CKN.Sdk.Core&style=flat-square&color=blue)](https://www.nuget.org/packages/CKN.Sdk.Core/)
[![NuGet CKN.Sdk.AI](https://img.shields.io/nuget/v/CKN.Sdk.AI.svg?label=CKN.Sdk.AI&style=flat-square&color=blue)](https://www.nuget.org/packages/CKN.Sdk.AI/)
[![NuGet CKN.Sdk.Messaging](https://img.shields.io/nuget/v/CKN.Sdk.Messaging.svg?label=CKN.Sdk.Messaging&style=flat-square&color=blue)](https://www.nuget.org/packages/CKN.Sdk.Messaging/)
[![NuGet CKN.Sdk.Caching](https://img.shields.io/nuget/v/CKN.Sdk.Caching.Redis.svg?label=CKN.Sdk.Caching.Redis&style=flat-square&color=blue)](https://www.nuget.org/packages/CKN.Sdk.Caching.Redis/)
[![GitHub Actions CI](https://github.com/OzanCKN/CKN.SDK/actions/workflows/nuget-publish.yml/badge.svg)](https://github.com/OzanCKN/CKN.SDK/actions/workflows/nuget-publish.yml)

CKN.SDK, 2026 Native AOT standartlarına uygun, modüler, olay güdümlü (event-driven) ve yapay zeka destekli mikroservisler inşa etmek için kullanılan resmi altyapı kütüphanesidir. 

**Multi-Provider (Çoklu Sağlayıcı) Mimarisi** sayesinde, projenizde kullanacağınız veritabanı, mesaj kuyruğu veya yapay zeka sağlayıcısını tek satır kodla değiştirebilirsiniz.

## 📦 Temel Katmanlar

- **`CKN.Sdk.Core`**: Sistemin kalbi. Sıfır bağımlılık. Tüm domain nesneleri, CQRS (ICommand, IQuery) ve temel arayüzler burada yer alır.
- **`CKN.Sdk.Infrastructure`**: Ortak HTTP, Güvenlik (JWT), Resiliency ve temel altyapı bileşenleri.
- **`CKN.Sdk.Tests`**: Kütüphanenin %100 kapsama sahip unit testlerini barındırır.
- **`CKN.Sdk.SourceGenerators`**: Reflection yerine derleme anında DI ve CQRS kodları üreten Roslyn eklentisi.

## 🧩 Tak-Çalıştır Modüller (NuGet Paketleri)

Projelerinizin ihtiyaçlarına göre sadece ilgili paketi indirerek kullanabilirsiniz:

### 📬 Mesajlaşma (Messaging)
| Paket Adı | Sürüm | Açıklama |
| :--- | :--- | :--- |
| `CKN.Sdk.Messaging` | [![NuGet](https://img.shields.io/nuget/v/CKN.Sdk.Messaging.svg?style=flat-square&label=)](https://www.nuget.org/packages/CKN.Sdk.Messaging/) | Temel `IEventBus` ve `IEvent` arayüzleri. |
| `CKN.Sdk.Messaging.RabbitMQ` | [![NuGet](https://img.shields.io/nuget/v/CKN.Sdk.Messaging.RabbitMQ.svg?style=flat-square&label=)](https://www.nuget.org/packages/CKN.Sdk.Messaging.RabbitMQ/) | RabbitMQ entegrasyonu (MassTransit altyapısı ile). |
| `CKN.Sdk.Messaging.Kafka` | [![NuGet](https://img.shields.io/nuget/v/CKN.Sdk.Messaging.Kafka.svg?style=flat-square&label=)](https://www.nuget.org/packages/CKN.Sdk.Messaging.Kafka/) | Apache Kafka entegrasyonu (Yüksek veri akışı için). |
| `CKN.Sdk.Messaging.ServiceBus` | [![NuGet](https://img.shields.io/nuget/v/CKN.Sdk.Messaging.ServiceBus.svg?style=flat-square&label=)](https://www.nuget.org/packages/CKN.Sdk.Messaging.ServiceBus/) | Azure Service Bus entegrasyonu. |

### ⚡ Önbellek (Caching)
| Paket Adı | Sürüm | Açıklama |
| :--- | :--- | :--- |
| `CKN.Sdk.Caching.Redis` | [![NuGet](https://img.shields.io/nuget/v/CKN.Sdk.Caching.Redis.svg?style=flat-square&label=)](https://www.nuget.org/packages/CKN.Sdk.Caching.Redis/) | StackExchange.Redis tabanlı dağıtık önbellek. |
| `CKN.Sdk.Caching.Memcached` | [![NuGet](https://img.shields.io/nuget/v/CKN.Sdk.Caching.Memcached.svg?style=flat-square&label=)](https://www.nuget.org/packages/CKN.Sdk.Caching.Memcached/) | Memcached entegrasyonu. |
| `CKN.Sdk.Caching.Garnet` | [![NuGet](https://img.shields.io/nuget/v/CKN.Sdk.Caching.Garnet.svg?style=flat-square&label=)](https://www.nuget.org/packages/CKN.Sdk.Caching.Garnet/) | Microsoft Research'ün geliştirdiği yeni nesil ultra-hızlı önbellek sistemi. |

### 💾 Veri Erişimi (Data Access)
| Paket Adı | Sürüm | Açıklama |
| :--- | :--- | :--- |
| `CKN.Sdk.Data.Dapper` | [![NuGet](https://img.shields.io/nuget/v/CKN.Sdk.Data.Dapper.svg?style=flat-square&label=)](https://www.nuget.org/packages/CKN.Sdk.Data.Dapper/) | Dapper tabanlı mikro-ORM, `IRepository` ve `IUnitOfWork` implementasyonları. |
| `CKN.Sdk.Data.RepoDb` | [![NuGet](https://img.shields.io/nuget/v/CKN.Sdk.Data.RepoDb.svg?style=flat-square&label=)](https://www.nuget.org/packages/CKN.Sdk.Data.RepoDb/) | RepoDb tabanlı bulk operasyonlar için özelleşmiş ORM. |
| `CKN.Sdk.EntityFramework` | [![NuGet](https://img.shields.io/nuget/v/CKN.Sdk.EntityFramework.svg?style=flat-square&label=)](https://www.nuget.org/packages/CKN.Sdk.EntityFramework/) | Entity Framework Core entegrasyonu. |

### 🤖 Yapay Zeka (AI & LLMs)
| Paket Adı | Sürüm | Açıklama |
| :--- | :--- | :--- |
| `CKN.Sdk.AI` | [![NuGet](https://img.shields.io/nuget/v/CKN.Sdk.AI.svg?style=flat-square&label=)](https://www.nuget.org/packages/CKN.Sdk.AI/) | Ortak AI modülleri, `Microsoft.Extensions.AI` abstraksiyonları. |
| `CKN.Sdk.AI.OpenAI` | [![NuGet](https://img.shields.io/nuget/v/CKN.Sdk.AI.OpenAI.svg?style=flat-square&label=)](https://www.nuget.org/packages/CKN.Sdk.AI.OpenAI/) | ChatGPT entegrasyonu. |
| `CKN.Sdk.AI.Anthropic` | [![NuGet](https://img.shields.io/nuget/v/CKN.Sdk.AI.Anthropic.svg?style=flat-square&label=)](https://www.nuget.org/packages/CKN.Sdk.AI.Anthropic/) | Claude (Anthropic) entegrasyonu. |
| `CKN.Sdk.AI.Ollama` | [![NuGet](https://img.shields.io/nuget/v/CKN.Sdk.AI.Ollama.svg?style=flat-square&label=)](https://www.nuget.org/packages/CKN.Sdk.AI.Ollama/) | Yerel (Local) ve KVKK uyumlu LLM çalıştırmak için Ollama. |
| `CKN.Sdk.AI.SemanticKernel` | [![NuGet](https://img.shields.io/nuget/v/CKN.Sdk.AI.SemanticKernel.svg?style=flat-square&label=)](https://www.nuget.org/packages/CKN.Sdk.AI.SemanticKernel/) | Microsoft Semantic Kernel kullanarak AI ajanları ve fonksiyon çağırma (Tool Calling) orkestrasyonu. |

### ☁️ Depolama (Storage)
| Paket Adı | Sürüm | Açıklama |
| :--- | :--- | :--- |
| `CKN.Sdk.Storage` | [![NuGet](https://img.shields.io/nuget/v/CKN.Sdk.Storage.svg?style=flat-square&label=)](https://www.nuget.org/packages/CKN.Sdk.Storage/) | `IStorageService` arayüzü (Nesne depolama abstraksiyonu). |
| `CKN.Sdk.Storage.Minio` | [![NuGet](https://img.shields.io/nuget/v/CKN.Sdk.Storage.Minio.svg?style=flat-square&label=)](https://www.nuget.org/packages/CKN.Sdk.Storage.Minio/) | S3 uyumlu yerel/bulut Object Storage. |
| `CKN.Sdk.Storage.S3` | [![NuGet](https://img.shields.io/nuget/v/CKN.Sdk.Storage.S3.svg?style=flat-square&label=)](https://www.nuget.org/packages/CKN.Sdk.Storage.S3/) | Amazon AWS S3 entegrasyonu. |
| `CKN.Sdk.Storage.Azure` | [![NuGet](https://img.shields.io/nuget/v/CKN.Sdk.Storage.Azure.svg?style=flat-square&label=)](https://www.nuget.org/packages/CKN.Sdk.Storage.Azure/) | Azure Blob Storage entegrasyonu. |

### ⏰ Zamanlayıcı (Scheduling)
| Paket Adı | Sürüm | Açıklama |
| :--- | :--- | :--- |
| `CKN.Sdk.Scheduling.Hangfire` | [![NuGet](https://img.shields.io/nuget/v/CKN.Sdk.Scheduling.Hangfire.svg?style=flat-square&label=)](https://www.nuget.org/packages/CKN.Sdk.Scheduling.Hangfire/) | Hangfire ile veritabanı destekli, arayüzlü (Dashboard) görev zamanlama. |
| `CKN.Sdk.Scheduling.Quartz` | [![NuGet](https://img.shields.io/nuget/v/CKN.Sdk.Scheduling.Quartz.svg?style=flat-square&label=)](https://www.nuget.org/packages/CKN.Sdk.Scheduling.Quartz/) | Quartz.NET entegrasyonu. |
| `CKN.Sdk.Scheduling.Coravel` | [![NuGet](https://img.shields.io/nuget/v/CKN.Sdk.Scheduling.Coravel.svg?style=flat-square&label=)](https://www.nuget.org/packages/CKN.Sdk.Scheduling.Coravel/) | Sıfır konfigürasyon, bellek içi zamanlayıcı. |

### 🔍 Arama (Search)
| Paket Adı | Sürüm | Açıklama |
| :--- | :--- | :--- |
| `CKN.Sdk.Search` | [![NuGet](https://img.shields.io/nuget/v/CKN.Sdk.Search.svg?style=flat-square&label=)](https://www.nuget.org/packages/CKN.Sdk.Search/) | `ISearchService<T>` arayüzü. |
| `CKN.Sdk.Search.Elasticsearch` | [![NuGet](https://img.shields.io/nuget/v/CKN.Sdk.Search.Elasticsearch.svg?style=flat-square&label=)](https://www.nuget.org/packages/CKN.Sdk.Search.Elasticsearch/) | Elasticsearch (Fuzzy search) entegrasyonu. |
| `CKN.Sdk.Search.Meilisearch` | [![NuGet](https://img.shields.io/nuget/v/CKN.Sdk.Search.Meilisearch.svg?style=flat-square&label=)](https://www.nuget.org/packages/CKN.Sdk.Search.Meilisearch/) | Ultra hızlı, Typo-tolerant Meilisearch entegrasyonu. |
| `CKN.Sdk.Search.NRedisStack` | [![NuGet](https://img.shields.io/nuget/v/CKN.Sdk.Search.NRedisStack.svg?style=flat-square&label=)](https://www.nuget.org/packages/CKN.Sdk.Search.NRedisStack/) | Redis tabanlı RediSearch entegrasyonu. |


## 📚 Gerçek Hayat Kullanım Örnekleri (Dokümantasyon)

Yukarıdaki paketleri projenizde kullanırken `appsettings.json` yapılandırmalarını nasıl yapacağınızı ve modülleri nasıl DI'a (Dependency Injection) bağlayıp gerçek kod içerisinde kullanacağınızı görmek için aşağıdaki detaylı örneklere göz atabilirsiniz:

- 📬 [Messaging (Kafka, RabbitMQ, Service Bus) Örnekleri](https://github.com/OzanCKN/CKN.SDK/wiki/Examples-Messaging)
- ⚡ [Caching (Redis, Memcached, Garnet) Örnekleri](https://github.com/OzanCKN/CKN.SDK/wiki/Examples-Caching)
- 💾 [Data Access (Dapper, RepoDb) Örnekleri](https://github.com/OzanCKN/CKN.SDK/wiki/Examples-Data-Access)
- 🤖 [AI (OpenAI, Anthropic, Ollama, Semantic Kernel) Örnekleri](https://github.com/OzanCKN/CKN.SDK/wiki/Examples-AI)
- ☁️ [Storage (Minio, Azure Blob, S3) Örnekleri](https://github.com/OzanCKN/CKN.SDK/wiki/Examples-Storage)
- ⏰ [Scheduling (Hangfire, Quartz, Coravel) Örnekleri](https://github.com/OzanCKN/CKN.SDK/wiki/Examples-Scheduling)
- 🔍 [Search (Elasticsearch, Meilisearch, NRedisStack) Örnekleri](https://github.com/OzanCKN/CKN.SDK/wiki/Examples-Search)


## 🚀 Hızlı Başlangıç

Örnek bir projede RabbitMQ, Dapper ve Redis kullanmak için terminalden ilgili paketleri yükleyin:

```bash
dotnet add package CKN.Sdk.Core
dotnet add package CKN.Sdk.Messaging.RabbitMQ
dotnet add package CKN.Sdk.Data.Dapper
dotnet add package CKN.Sdk.Caching.Redis
```

Daha sonra `Program.cs` içerisinde servislerinizi ayağa kaldırın:

```csharp
using CKN.Sdk.Messaging.RabbitMQ;
using CKN.Sdk.Data.Dapper;
using CKN.Sdk.Caching.Redis;

var builder = WebApplication.CreateBuilder(args);

// Core Servisler
builder.Services.AddCknCore();

// Seçtiğiniz sağlayıcıları (Providers) sisteme dahil edin
builder.Services.AddCknRabbitMQ(opt => builder.Configuration.GetSection("RabbitMQ").Bind(opt));
builder.Services.AddCknDapper(opt => builder.Configuration.GetSection("Database").Bind(opt));
builder.Services.AddCknRedis(opt => builder.Configuration.GetSection("Redis").Bind(opt));

var app = builder.Build();
app.Run();
```

## 🏗️ Mimari ve Yönetişim (Governance)
Bu projenin nasıl tasarlandığını, "Zero-Warning Policy" gibi kuralları ve geliştirici standartlarını merak ediyorsanız [Wiki Mimari İndeksi](https://github.com/OzanCKN/CKN.SDK/wiki/Architecture-Index) sekmesini inceleyiniz. Yapay Zeka (AI) ajanları için sistem yönergeleri `.agents/` klasöründe yer alır.
