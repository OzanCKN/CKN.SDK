# CKN.SDK (Enterprise Agentic Orchestra)

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
| Paket Adı | Açıklama |
| :--- | :--- |
| `CKN.Sdk.Messaging` | Temel `IEventBus` ve `IEvent` arayüzleri. |
| `CKN.Sdk.Messaging.RabbitMQ` | RabbitMQ entegrasyonu (MassTransit altyapısı ile). |
| `CKN.Sdk.Messaging.Kafka` | Apache Kafka entegrasyonu (Yüksek veri akışı için). |
| `CKN.Sdk.Messaging.ServiceBus` | Azure Service Bus entegrasyonu. |

### ⚡ Önbellek (Caching)
| Paket Adı | Açıklama |
| :--- | :--- |
| `CKN.Sdk.Caching.Redis` | StackExchange.Redis tabanlı dağıtık önbellek. |
| `CKN.Sdk.Caching.Memcached` | Memcached entegrasyonu. |
| `CKN.Sdk.Caching.Garnet` | Microsoft Research'ün geliştirdiği yeni nesil ultra-hızlı önbellek sistemi. |

### 💾 Veri Erişimi (Data Access)
| Paket Adı | Açıklama |
| :--- | :--- |
| `CKN.Sdk.Data.Dapper` | Dapper tabanlı mikro-ORM, `IRepository` ve `IUnitOfWork` implementasyonları. |
| `CKN.Sdk.Data.RepoDb` | RepoDb tabanlı bulk operasyonlar için özelleşmiş ORM. |
| `CKN.Sdk.EntityFramework` | Entity Framework Core entegrasyonu. |

### 🤖 Yapay Zeka (AI & LLMs)
| Paket Adı | Açıklama |
| :--- | :--- |
| `CKN.Sdk.AI` | Ortak AI modülleri, `Microsoft.Extensions.AI` abstraksiyonları. |
| `CKN.Sdk.AI.OpenAI` | ChatGPT entegrasyonu. |
| `CKN.Sdk.AI.Anthropic` | Claude (Anthropic) entegrasyonu. |
| `CKN.Sdk.AI.Ollama` | Yerel (Local) ve KVKK uyumlu LLM çalıştırmak için Ollama. |
| `CKN.Sdk.AI.SemanticKernel` | Microsoft Semantic Kernel kullanarak AI ajanları ve fonksiyon çağırma (Tool Calling) orkestrasyonu. |

### ☁️ Depolama (Storage)
| Paket Adı | Açıklama |
| :--- | :--- |
| `CKN.Sdk.Storage` | `IStorageService` arayüzü (Nesne depolama abstraksiyonu). |
| `CKN.Sdk.Storage.Minio` | S3 uyumlu yerel/bulut Object Storage. |
| `CKN.Sdk.Storage.S3` | Amazon AWS S3 entegrasyonu. |
| `CKN.Sdk.Storage.Azure` | Azure Blob Storage entegrasyonu. |

### ⏰ Zamanlayıcı (Scheduling)
| Paket Adı | Açıklama |
| :--- | :--- |
| `CKN.Sdk.Scheduling.Hangfire` | Hangfire ile veritabanı destekli, arayüzlü (Dashboard) görev zamanlama. |
| `CKN.Sdk.Scheduling.Quartz` | Quartz.NET entegrasyonu. |
| `CKN.Sdk.Scheduling.Coravel` | Sıfır konfigürasyon, bellek içi zamanlayıcı. |

### 🔍 Arama (Search)
| Paket Adı | Açıklama |
| :--- | :--- |
| `CKN.Sdk.Search` | `ISearchService<T>` arayüzü. |
| `CKN.Sdk.Search.Elasticsearch` | Elasticsearch (Fuzzy search) entegrasyonu. |
| `CKN.Sdk.Search.Meilisearch` | Ultra hızlı, Typo-tolerant Meilisearch entegrasyonu. |
| `CKN.Sdk.Search.NRedisStack` | Redis tabanlı RediSearch entegrasyonu. |


## 📚 Gerçek Hayat Kullanım Örnekleri (Dokümantasyon)

Yukarıdaki paketleri projenizde kullanırken `appsettings.json` yapılandırmalarını nasıl yapacağınızı ve modülleri nasıl DI'a (Dependency Injection) bağlayıp gerçek kod içerisinde kullanacağınızı görmek için aşağıdaki detaylı örneklere göz atabilirsiniz:

- 📬 [Messaging (Kafka, RabbitMQ, Service Bus) Örnekleri](./governance/docs/examples/messaging-examples.md)
- ⚡ [Caching (Redis, Memcached, Garnet) Örnekleri](./governance/docs/examples/caching-examples.md)
- 💾 [Data Access (Dapper, RepoDb) Örnekleri](./governance/docs/examples/data-access-examples.md)
- 🤖 [AI (OpenAI, Anthropic, Ollama, Semantic Kernel) Örnekleri](./governance/docs/examples/ai-examples.md)
- ☁️ [Storage (Minio, Azure Blob, S3) Örnekleri](./governance/docs/examples/storage-examples.md)
- ⏰ [Scheduling (Hangfire, Quartz, Coravel) Örnekleri](./governance/docs/examples/scheduling-examples.md)
- 🔍 [Search (Elasticsearch, Meilisearch, NRedisStack) Örnekleri](./governance/docs/examples/search-examples.md)


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
Bu projenin nasıl tasarlandığını, "Zero-Warning Policy" gibi kuralları ve geliştirici standartlarını merak ediyorsanız [Governance](./governance/index.md) klasörünü inceleyiniz. Yapay Zeka (AI) ajanları için sistem yönergeleri `.agents/` ve `governance/docs/architecture/system-glossary.md` dosyalarında tanımlıdır.
