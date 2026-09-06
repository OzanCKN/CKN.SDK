# System Glossary (Proje Haritası)

**MANDATORY FIRST READ:** Yeni bir Ajan bu projede göreve başladığında, bu dokümanı okuyarak tüm sistemin mevcut yapısını anında öğrenebilir. Projeyi gereksiz yere taramaya (grep/search) gerek kalmamalıdır.

## Mimari Modüller (NuGet Paketleri)

Sistem artık devasa bir monolotik yapı değil, mikro paketler (SDK'lar) halinde modüler bir şekilde tasarlanmıştır.

### 1. `CKN.Sdk.Core`
Tüm sistemin kalbidir. Hiçbir 3. parti kütüphaneye bağımlılığı yoktur (Sıfır Bağımlılık - Zero Dependency).
- **İçerik:** Domain varlıkları (Entity, Tenant), CQRS (ICommand, IQuery) arayüzleri, Özel İstisnalar (Exceptions), Repository arayüzleri.
- **Kullanım:** Yeni bir arayüz veya temel kural eklenecekse burası kullanılır.

### 2. `CKN.Sdk.EntityFramework`
`CKN.Sdk.Core` içindeki `IRepository` ve `IUnitOfWork` arayüzlerinin Microsoft Entity Framework Core implementasyonudur.
- **İçerik:** `ApplicationDbContext`, `DomainEventInterceptor` vb.
- **Kullanım:** Sadece veritabanı ile konuşacak mikroservisler bu paketi kurar.

### 3. `CKN.Sdk.MassTransit`
Olay tabanlı (Event-Driven) mimari, Saga ve asenkron iletişim modülüdür.
- **İçerik:** MassTransit RabbitMQ entegrasyonu, `MeetingProcessingStateMachine`.
- **Kullanım:** Sadece Event yayacak (Publish) veya dinleyecek servisler bu paketi kurar.

### 4. `CKN.Sdk.AI`
Yapay zeka servis entegrasyonlarını içerir.
- **İçerik:** Microsoft.Extensions.AI arayüzleri, OpenAI bağlantıları, Semantic Kernel (Opsiyonel).
- **Kullanım:** LLM veya Embeddings kullanılacaksa bu paket projeye dahil edilir.

### 5. `CKN.Sdk.Infrastructure`
Uygulamaların HTTP iletişimlerini, Caching mekanizmalarını (Redis/Memory), Güvenlik (JWT/License) katmanlarını ve OpenTelemetry bazlı İzlenebilirliğini (Observability) barındıran temel altyapı modülüdür.

### 6. `CKN.Sdk.SourceGenerators`
(Geliştirme Aşamasında)
AOT (Ahead of Time) derleme uyumluluğu sağlamak için C# kod üreten (Roslyn tabanlı) Roslyn Analyzer projesidir. Dependency Injection (DI) kodlarını derleme zamanında yazarak Reflection kullanımını engeller.

*(Not: Yeni klasörler veya modüller eklendiğinde bu dokümanı GÜNCELLEMEYİ UNUTMAYIN)*
