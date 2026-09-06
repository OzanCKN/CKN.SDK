# System Glossary (Proje Haritası)

**MANDATORY FIRST READ:** Yeni bir Ajan bu projede göreve başladığında, bu dokümanı okuyarak tüm sistemin mevcut yapısını anında öğrenebilir. Projeyi gereksiz yere taramaya (grep/search) gerek kalmamalıdır.

## Mimari Modüller (NuGet Paketleri)

Sistem artık devasa bir monolotik yapı değil, mikro paketler (SDK'lar) halinde modüler bir şekilde tasarlanmıştır.

### 1. `CKN.Sdk.Core`
Tüm sistemin kalbidir. Hiçbir 3. parti kütüphaneye bağımlılığı yoktur (Sıfır Bağımlılık - Zero Dependency).
- **İçerik:** Domain varlıkları (Entity, Tenant), CQRS- **Exceptions:** Tüm projede kullanılacak ortak hata sınıfları (`CustomException`, `NotFoundException`, `ValidationException`).
- **Events:** Uygulama çapında ve modüller arası mesajlaşma (EventBus) için temel arayüzler (`IEventBus`, `IIntegrationEvent`, `IEventHandler<T>`).
- **DependencyInjection:** Core bağımlılıkların IoC container'a kolayca eklenebilmesi için genişletmeler.

### 2. `CKN.Sdk.EntityFramework`
`CKN.Sdk.Core` içindeki `IRepository` ve `IUnitOfWork` arayüzlerinin Microsoft Entity Framework Core implementasyonudur.
- **İçerik:** `ApplicationDbContext`, `DomainEventInterceptor` vb.
- **Kullanım:** Sadece veritabanı ile konuşacak mikroservisler bu paketi kurar.

### 3. `CKN.Sdk.MassTransit`
Olay tabanlı (Event-Driven) mimari, Saga ve asenkron iletişim modülüdür.
- **İçerik:** MassTransit RabbitMQ entegrasyonu, `MeetingProcessingStateMachine`.
- **Kullanım:** Sadece Event yayacak (Publish) veya dinleyecek servisler bu paketi kurar.

### 4. `CKN.Sdk.Messaging.RabbitMQ`
Yeni Provider-Agnostic mimarinin ilk mesajlaşma adaptörüdür. `CKN.Sdk.Core` içerisindeki `IEventBus` arayüzünü RabbitMQ Native Client kullanarak uygular. 
- **Kullanım:** Uygulamanın RabbitMQ'ya bağlanması için kullanılır (`builder.AddCknMessaging(m => m.UseRabbitMQ())`).

### 5. `CKN.Sdk.Messaging.Kafka`
Provider-Agnostic mimarinin Apache Kafka adaptörüdür. Confluent.Kafka altyapısını kullanarak yüksek throughput mesajlaşma (Event Streaming) sağlar.
- **Kullanım:** Uygulamanın Kafka'ya bağlanması için kullanılır (`builder.AddCknMessaging(m => m.UseKafka())`).

### 6. `CKN.Sdk.Data.Dapper`
Veri erişim (Data Access) katmanı için geliştirilen Dapper adaptörüdür. `CKN.Sdk.Core` içerisindeki `IRepository` ve `IUnitOfWork` arayüzlerini ultra hızlı Dapper (ve Dommel) ORM'i ile uygular.
- **Kullanım:** Uygulamanın veritabanına bağlanması için kullanılır (`services.AddCknDapper()`).

### 7. `CKN.Sdk.Caching.Garnet`
Yüksek performanslı Microsoft Garnet önbellekleme (Caching) adaptörüdür. Redis protokolü (RESP) üzerinden haberleşir ve .NET `IDistributedCache` arayüzünü destekler.
- **Kullanım:** Uygulamanın Garnet veya Redis tabanlı dağıtık önbelleğe bağlanması için kullanılır (`services.AddCknGarnetCache()`).

### 8. `CKN.Sdk.AI`
Yapay zeka servis entegrasyonlarını içerir. `Microsoft.Extensions.AI` arayüzünü (Örn: `IChatClient`) kullanır.
- **Kullanım:** AI sağlayıcılarının soyutlaması için temel modüldür.

### 9. `CKN.Sdk.AI.OpenAI`
OpenAI (GPT-4 vb.) modellerine bağlanmak için kullanılan Native Provider adaptörüdür. 
- **Kullanım:** `services.AddCknOpenAI(...)` ile `IChatClient` sisteme dahil edilir.

### 10. `CKN.Sdk.Infrastructure`
Uygulamaların altyapısal gereksinimlerini karşılayan, tak-çalıştır mimarisindeki ana modüldür.
- **Caching (Önbellekleme):** `HybridCacheService` ile L1 (Memory) ve L2 (Redis) cache mekanizması (Graceful Degradation destekli).
- **Resilience (Dayanıklılık):** `HttpClientBuilderExtensions` üzerinden Polly v8 entegrasyonu (Retry, Circuit Breaker, Timeout, Rate Limiter).
- **Security (Güvenlik):** JWT Token yönetimi, Lisans Doğrulama ve Feature Flag bazlı Authentication altyapısı.
- **Observability (İzlenebilirlik):** `AddCKNTelemetry()` ile OpenTelemetry (Tracing & Metrics) ve `AddCknElasticLogging()` ile Serilog Elasticsearch PII Masking loglama entegrasyonları.
- **CQRS:** `LoggingBehavior`, `ValidationBehavior` ve `IdempotentBehavior` ile MediatR pipeline'ı destekler.
- **Kullanım:** SDK'yı kullanan ana Host (API) projesinde zorunlu olarak dahil edilir.

### 6. `CKN.Sdk.SourceGenerators`
(Geliştirme Aşamasında)
AOT (Ahead of Time) derleme uyumluluğu sağlamak için C# kod üreten (Roslyn tabanlı) Roslyn Analyzer projesidir. Dependency Injection (DI) kodlarını derleme zamanında yazarak Reflection kullanımını engeller.

*(Not: Yeni klasörler veya modüller eklendiğinde bu dokümanı GÜNCELLEMEYİ UNUTMAYIN)*
