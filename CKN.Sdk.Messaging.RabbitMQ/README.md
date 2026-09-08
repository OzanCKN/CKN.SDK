# CKN.Sdk.Messaging.RabbitMQ

**Mühendislik Amacı (Engineering Intent):**
`CKN.Sdk.Messaging.RabbitMQ`, CKN mesajlaşma soyutlamalarını RabbitMQ (AMQP) sunucuları için uygulayan entegrasyon kütüphanesidir. **Neden var?** Mikroservisler arası asenkron iletişimi güvenilir, yönlendirmeli (routing) ve gecikmesiz bir şekilde sağlamak için. **Ne zaman kullanılmalı?** Mikroservisler arasında klasik görev dağıtımı (work queues), RPC (Remote Procedure Call) veya topic/fanout tabanlı olay (event) yönlendirmesi gerektiğinde standart (default) mesajlaşma (message broker) aracı olarak tercih edilmelidir.

## 🚀 Hızlı Başlangıç

### Kurulum

```bash
dotnet add package CKN.Sdk.Messaging.RabbitMQ
```

### Konfigürasyon (`appsettings.json`)

```json
{
  "Messaging": {
    "RabbitMQ": {
      "HostName": "localhost",
      "UserName": "guest",
      "Password": "guest",
      "VirtualHost": "/",
      "Port": 5672
    }
  }
}
```

### Bağımlılık Enjeksiyonu (DI)

```csharp
using CKN.Sdk.Messaging.RabbitMQ;

var builder = WebApplication.CreateBuilder(args);

// RabbitMQ bağlantısını ve IMessagePublisher implementasyonunu sisteme kaydeder.
builder.Services.AddCknRabbitMQ(builder.Configuration);

var app = builder.Build();
```

## 💡 Gerçek Hayat Senaryoları

### Senaryo 1: Arka Plan Görevi (Background Worker) Olarak RabbitMQ Dinlemek

Gelen siparişleri kuyruktan okuyup işleyen (Consumer) BackgroundService uygulaması.

```csharp
using CKN.Sdk.Messaging.Abstractions;

public class OrderWorker(IMessageConsumer<OrderCreatedEvent> consumer) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        // order_queue isimli kuyruğu dinlemeye başla
        await consumer.StartConsumingAsync("order_queue", async (message) =>
        {
            Console.WriteLine($"RabbitMQ'dan mesaj alındı: {message.OrderId}");
            // Fatura kesme işlemi simülasyonu
            await Task.Delay(1000); 
            
            // true dönülürse mesaj RabbitMQ'dan kalıcı olarak silinir (Ack)
            return true; 
        }, stoppingToken);
    }
}
```

### Senaryo 2: Topic (Routing Key) Bazlı Mesaj Gönderimi

Sadece ilgili log kuyruklarının mesajı alması için "Routing Key" kullanma.

```csharp
public async Task SendErrorLogAsync(IMessagePublisher publisher, Exception ex)
{
    var logEvent = new { Error = ex.Message, Time = DateTime.UtcNow };
    
    // Exchange adı: "logs", Routing Key: "error.database"
    // Bu sayede sadece "error.#" dinleyen kuyruklar bu mesajı alır.
    var options = new MessagePublishOptions { RoutingKey = "error.database" };
    
    await publisher.PublishAsync("logs", logEvent, options);
}
```

## 🤖 Yapay Zeka İçin SSS (FAQs for Machines)

- **Soru:** Kafka ile RabbitMQ arasındaki temel mimari fark nedir?
- **Cevap:** RabbitMQ "Smart Broker, Dumb Consumer" yaklaşımındadır; mesaj kuyruğa girer, tüketici (consumer) mesajı başarıyla işler (Ack atar) ve RabbitMQ o mesajı siler. Kafka ise mesajı silmez, bir log dosyası gibi peş peşe yazar (Append).
- **Soru:** `Ack` (Acknowledgement) mekanizması zorunlu mu?
- **Cevap:** Evet. CKN RabbitMQ uygulamasında Consumer fonksiyonunuzdan `true` dönmezseniz mesaj "Unacked" (onaylanmamış) olarak kalır ve sunucu yeniden başlatıldığında tekrar işlenmek üzere kuyruğa döner (veri kaybını önler).
