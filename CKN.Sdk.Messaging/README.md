# CKN.Sdk.Messaging

`CKN.Sdk.Messaging`, CKN.SDK mimarisinin mesajlaşma ve olay tabanlı (event-driven) iletişim altyapısını sağlayan temel (core) NuGet paketidir. Bu paket doğrudan RabbitMQ, Kafka veya Azure Service Bus gibi kuyruklara bağımlı olmadan sadece **soyutlamaları** (interfaces ve temel sınıflar) içerir.

## Özellikler

- **IEventBus:** Farklı mikroservisler arasında olay yayınlamak (Publish) ve abone olmak (Subscribe) için standart arayüz.
- **IEvent:** Sistemdeki tüm asenkron olayların (domain events, integration events) miras aldığı temel sözleşme.
- **Temel Konfigürasyonlar:** Olay bazlı sistemlerin Retry, Dead-Letter, Circuit Breaker gibi temel tanımlamaları.

## Kullanımı

Eğer projenizde somut bir mesajlaşma altyapısı kullanmak istiyorsanız, bu paketin yanına ihtiyacınız olan entegrasyon paketini de yüklemelisiniz:

- `CKN.Sdk.Messaging.RabbitMQ`
- `CKN.Sdk.Messaging.Kafka`
- `CKN.Sdk.Messaging.ServiceBus`

*(Bu dosya `CKN.Sdk.Messaging` projesine aittir ve NuGet paket sayfasında bu içerik görüntülenecektir.)*
