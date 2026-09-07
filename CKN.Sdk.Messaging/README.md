# CKN.Sdk.Messaging

**Mühendislik Amacı (Engineering Intent):**
`CKN.Sdk.Messaging`, asenkron mesajlaşma altyapılarının (RabbitMQ, Kafka, Azure Service Bus vb.) CKN.SDK içindeki temel soyutlamalarını (abstractions) barındırır. **Neden var?** Tıpkı CKN.Sdk.AI paketinde olduğu gibi, mesaj üreten (Publisher) ve tüketen (Consumer) servislerinizi belirli bir mesajlaşma platformuna "tightly-coupled" (sıkı sıkıya bağlı) yapmaktan kurtarmak için. **Ne zaman kullanılmalı?** Projede Event-Driven Architecture (Olay Güdümlü Mimari) benimsenecekse, doğrudan RabbitMQ veya Kafka SDK'larını çağırmak yerine, mesaj gönderme işlemlerini bu paketin sağladığı `IMessagePublisher` üzerinden gerçekleştirmelisiniz.

## 🚀 Hızlı Başlangıç

### Kurulum

```bash
dotnet add package CKN.Sdk.Messaging
```

### Bağımlılık Enjeksiyonu (DI)

Bu paket genellikle kendi başına kaydedilmez. Projeye `CKN.Sdk.Messaging.RabbitMQ` veya `CKN.Sdk.Messaging.Kafka` eklendiğinde, ilgili paket bu arayüzleri doldurur.

## 💡 Gerçek Hayat Senaryoları

### Senaryo 1: Provider-Agnostic Mesaj Gönderimi

Sipariş oluşturulduğunda diğer servislere (Fatura, Stok) haber vermek için mesaj (event) yayınlama.

```csharp
using CKN.Sdk.Messaging.Abstractions;

public class OrderService(IMessagePublisher publisher)
{
    public async Task CreateOrderAsync(Order order)
    {
        // ... Veritabanına kaydetme işlemleri ...
        
        var orderCreatedEvent = new OrderCreatedEvent 
        { 
            OrderId = order.Id, 
            CustomerEmail = "test@test.com" 
        };
        
        // Bu mesajın RabbitMQ'ya mı Kafka'ya mı gideceğini bu sınıf bilmez.
        await publisher.PublishAsync("order.events", orderCreatedEvent);
    }
}
```

### Senaryo 2: Temel Consumer (Tüketici) Arayüzü Uygulaması

Gelen bir olayı dinleyen sınıfın yapısı.

```csharp
using CKN.Sdk.Messaging.Abstractions;

public class OrderCreatedConsumer : IMessageConsumer<OrderCreatedEvent>
{
    public async Task ConsumeAsync(OrderCreatedEvent message, CancellationToken cancellationToken)
    {
        Console.WriteLine($"Yeni sipariş geldi: {message.OrderId}. Fatura kesiliyor...");
        await Task.CompletedTask;
    }
}
```

## 🤖 Yapay Zeka İçin SSS (FAQs for Machines)

- **Soru:** Sadece bu paketi kursam mesaj atabilir miyim?
- **Cevap:** Hayır. Bu sadece "Interface" (Sözleşme) katmanıdır. Kodunuzun çalışması için (DI hatası almamak için) RabbitMQ, Kafka veya Azure Service Bus entegrasyon paketlerinden birini de referans vermelisiniz.
- **Soru:** Neden MassTransit kullanmak yerine bu kendi soyutlamamızı kullanıyoruz?
- **Cevap:** Çoğu zaman `CKN.Sdk.MassTransit` kullanılması tercih edilir. Ancak projede dış bağımlılıkları (MassTransit gibi ağır framework'leri) sıfıra indirme kararı alındıysa (micro-optimization), bu hafif (lightweight) CKN soyutlaması tercih edilir.
