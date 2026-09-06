# Mimari Karar Günlüğü (Architecture Decision Log - ADR)

Bu belge, proje yaşam döngüsü boyunca alınan kritik mimari ve altyapı kararlarının kayıt altına alındığı yerdir. "Neden bu yolu seçtik?" sorusunun cevabı burada bulunur.

## Format Şablonu

```markdown
### [Tarih] - [Karar Başlığı]
- **Durum (Context):** Kararın alınmasına neden olan problem veya ihtiyaç.
- **Karar (Decision):** Seçilen çözüm ve uygulanan teknoloji/desen.
- **Sonuçlar (Consequences):** Bu kararın getirdiği avantajlar, dezavantajlar ve teknik borçlar.
```

---

### 2026-09-06 - SDK'nın Mikro Paketlere Bölünmesi (Modularity)
- **Durum:** `CKN.Sdk.Infrastructure` projesi; EF Core, MassTransit, OpenAI ve Security gibi devasa kütüphaneleri tek potada eritiyordu. SDK'yi kullanan bir projenin kullanmayacağı bağımlılıkları indirmesi gerekiyordu (Dependency Bloat).
- **Karar:** Infrastructure yapısı parçalanarak; `CKN.Sdk.EntityFramework`, `CKN.Sdk.MassTransit`, `CKN.Sdk.AI` ve `CKN.Sdk.Infrastructure` olmak üzere modüler kütüphanelere ayrıştırıldı.
- **Sonuçlar:** Tüketici (Consumer) projeler sadece ihtiyaç duydukları paketi (Örn: Sadece MassTransit) indirebilecek. NuGet boyutu küçüldü. Ajanların projeyi haritalandırması ve dosyalara müdahalesi çok daha izole hale geldi.

### 2026-09-06 - Merkezi Paket Yönetimine (CPM) Geçiş
- **Durum:** Farklı projelerde (.Core, .AI, .MassTransit vs.) NuGet versiyon çatışmaları (DLL Hell) yaşanma riski mevcuttu. Ayrıca modülerleşme sonrasında paketlerin yönetimi zorlaşmıştı.
- **Karar:** `Directory.Packages.props` dosyası oluşturularak Central Package Management (CPM) aktif edildi.
- **Sonuçlar:** `NU1015` hataları engellendi. Artık herhangi bir projeye kurulan NuGet paketinin sürümü, kök dizindeki tek bir dosyadan kontrol ediliyor.

### 2026-09-06 - Native AOT Uyumluluğu ve Source Generators
- **Durum:** Projenin 2026 bulut (Cloud-Native) standartlarında mikroservis yapısında, milisaniyelik başlatma (startup) hızlarına ulaşması bekleniyordu. Reflection tabanlı işlemler AOT'yi engelliyordu.
- **Karar:** `<IsAotCompatible>true</IsAotCompatible>` bayrağı `Directory.Build.props` ile global olarak zorunlu kılındı. CQRS (MediatR) ve DI işlemleri için Reflection yerine derleme anında kod üreten `CKN.Sdk.SourceGenerators` projesi yapıya eklendi.
- **Sonuçlar:** Performans artışı sağlandı ancak geliştirme karmaşıklığı (Source Generator yazımı) arttı.

### 2026-09-06 - AI Governance ve Klasör Yapısının Kurulumu
- **Durum:** Projede bir veya daha fazla AI ajanının (Agent) aynı standartlarda ve birbiriyle uyumlu çalışması için rehber eksikliği mevcuttu.
- **Karar:** `governance` klasörünün oluşturulmasına, görev ve sprint yönetiminin buraya taşınmasına karar verildi. `.agents/rules.md` eklenerek projenin AI dostu hale getirilmesi sağlandı. Ayrıca proje ismi CKN.SDK olarak güncellendi.
- **Sonuçlar:** Ajanlar artık kodu körlemesine okumak yerine bu klasörlerdeki dokümanlara bakarak doğrudan hedef odaklı çalışabilecek. Geliştirme süreci ve token kullanımı daha efektif hale geldi.

### 2026-09-06 - Hibrid Önbellekleme (Hybrid Caching) ve Graceful Degradation
- **Durum:** Salt Redis kullanımında Redis çöktüğünde sistemin tamamen durması riski (Single Point of Failure) vardı.
- **Karar:** `HybridCacheService` yazılarak L1 (MemoryCache) ve L2 (Redis) mimarisi kuruldu. Redis'e ulaşılamadığında sistem MemoryCache ile çalışmaya devam eder (Graceful Degradation).
- **Sonuçlar:** Sistem dayanıklılığı artırıldı. Ancak verilerin iki katmanda senkronize tutulması karmaşıklığı eklendi.

### 2026-09-06 - CQRS Idempotency (MediatR Pipeline)
- **Durum:** Aynı isteğin (örn. Ödeme, Sipariş) ağ kopmaları sebebiyle birden çok kez gönderilmesi veri bütünlüğünü bozuyordu.
- **Karar:** `IdempotentBehavior` adlı MediatR Pipeline Behavior yazılarak, `X-Idempotency-Key` başlığı üzerinden dağıtık kilit (Distributed Lock) ve işleme kontrolü eklendi.
- **Sonuçlar:** Tüm CKN SDK kullanan projeler otomatik olarak tekrarlanan istek korumasına sahip oldu.

### 2026-09-06 - Kurumsal İzlenebilirlik (Elasticsearch & OpenTelemetry)
- **Durum:** Mikroservis mimarisinde hata takibi ve performans darboğazlarını bulmak imkansızlaşıyordu. Hassas veriler (PII) loglara sızıyordu.
- **Karar:** Loglama için Serilog (Elasticsearch Sink) entegre edildi ve PII Masking eklendi. Metrik ve Tracing için OpenTelemetry standardı kabul edildi.
- **Sonuçlar:** `AddCknElasticLogging()` ve `AddCKNTelemetry()` eklentileri ile tek satır kodla devasa bir izlenebilirlik ağı kuruldu.
