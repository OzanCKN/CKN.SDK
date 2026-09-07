# Görev Şablonu (Task Template)

**Görev Adı:** Provider-Agnostic Messaging Modülü ve Abstraction Katmanı
**Sprint:** Sprint 5
**Durum:** [ ] TODO | [ ] IN PROGRESS | [x] DONE

## 1. Açıklama ve Kapsam
CKN.SDK'nın dışa bağımlılığını en aza indirmek ve "tak-çalıştır" (plug-and-play) mimarisini desteklemek amacıyla Messaging katmanı için soyutlamalar (abstractions) eklenecektir. Bu sayede tüketiciler tek bir satır değiştirerek RabbitMQ'dan Kafka'ya veya ServiceBus'a geçiş yapabilecektir. `IEventBus` ve ortak yapılandırma arayüzleri oluşturulacaktır.

## 2. Etkilenen Sistemler (Glossary Referansları)
- `CKN.Sdk.Messaging`: `IEventBus`, `IntegrationEvent`, `IIntegrationEventHandler<T>` gibi soyutlamalar eklenecek.
- `CKN.Sdk.Messaging.RabbitMQ`: RabbitMQ adaptörü (`IEventBus` implementasyonu) yazılacak.
- `CKN.Sdk.Messaging.Kafka`: Kafka adaptörü (`IEventBus` implementasyonu) yazılacak.
- `CKN.Sdk.Messaging.ServiceBus`: Azure Service Bus adaptörü (`IEventBus` implementasyonu) yazılacak.

## 3. Yapılacaklar (Checklist)
- [x] `IntegrationEvent` base record sınıfının oluşturulması.
- [x] `IEventBus` arayüzünün tasarlanması (Publish, Subscribe).
- [x] Messaging eklentileri (Extensions) için `builder.UseRabbitMQ()`, `builder.UseKafka()` yapılandırma arayüzlerinin oluşturulması.
- [x] HealthCheck yapısının standart hale getirilmesi.
- [x] System Glossary ve Decision Log güncellemeleri.

## 4. İzcilik Kuralı (Scout Rule) Notları
- Gelecek implementasyonlar sırasında oluşabilecek paket sürümü uyumsuzluklarına dikkat edilecektir.
