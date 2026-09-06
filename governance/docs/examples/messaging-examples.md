# Messaging (Mesajlaşma) Sağlayıcıları Kullanım Örnekleri

CKN.SDK, farklı mesajlaşma broker'larını tak-çalıştır mimarisinde kullanmanızı sağlar. Aşağıdaki örnekler, her bir sağlayıcının nasıl yapılandırılacağını ve gerçek bir projede nasıl kullanılacağını göstermektedir.

---

## 1. RabbitMQ Kullanımı

RabbitMQ, standart AMQP tabanlı mesajlaşma için idealdir. 

### `appsettings.json` Yapılandırması
```json
{
  "Messaging": {
    "RabbitMQ": {
      "HostName": "localhost",
      "UserName": "guest",
      "Password": "guest",
      "QueueName": "ckn.events.queue",
      "RetryCount": 3
    }
  }
}
```

### Dependency Injection (DI) Kurulumu
`Program.cs` içerisinde `UseRabbitMQ` extension metodunu çağırın:

```csharp
using CKN.Sdk.Messaging.RabbitMQ;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCknMessaging(msg =>
{
    // Konfigürasyonu appsettings'ten otomatik okuyarak bağlama
    msg.UseRabbitMQ(opt => 
    {
        var config = builder.Configuration.GetSection(RabbitMQOptions.SectionName).Get<RabbitMQOptions>();
        opt.HostName = config?.HostName ?? "localhost";
        opt.UserName = config?.UserName ?? "guest";
        opt.Password = config?.Password ?? "guest";
        opt.QueueName = config?.QueueName ?? "ckn.events.queue";
    });
});
```

### Gerçek Hayat Kullanımı: Sipariş Oluşturulduğunda Mesaj Fırlatma

Aşağıdaki örnekte, yeni bir sipariş geldiğinde `IEventBus` üzerinden RabbitMQ'ya nasıl mesaj atılacağı ve ayrı bir worker tarafından nasıl dinleneceği gösterilmiştir:

```csharp
using CKN.Sdk.Messaging;

public record OrderCreatedEvent(Guid OrderId, decimal Amount, string CustomerEmail);

public class OrderService
{
    private readonly IEventBus _eventBus;

    public OrderService(IEventBus eventBus)
    {
        _eventBus = eventBus;
    }

    public async Task CreateOrderAsync(OrderCreatedEvent newOrder)
    {
        // ... Veritabanına siparişi kaydet ...
        
        // Asenkron olarak kuyruğa mesaj bırak (Publish)
        await _eventBus.PublishAsync("order.created", newOrder);
    }
}

// Background Worker veya Controller içerisinde mesajı dinleme (Subscribe)
public class NotificationWorker : BackgroundService
{
    private readonly IEventBus _eventBus;
    private readonly ILogger<NotificationWorker> _logger;

    public NotificationWorker(IEventBus eventBus, ILogger<NotificationWorker> logger)
    {
        _eventBus = eventBus;
        _logger = logger;
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _eventBus.Subscribe<OrderCreatedEvent>("order.created", async (orderEvent) =>
        {
            _logger.LogInformation($"Sipariş alındı: {orderEvent.OrderId}. Müşteriye email gönderiliyor...");
            // Email gönderme işlemi...
            await Task.CompletedTask;
        });

        return Task.CompletedTask;
    }
}
```

---

## 2. Kafka Kullanımı

Kafka, yüksek veri akışı gerektiren olay odaklı mimariler (Event-Driven) için uygundur.

### `appsettings.json` Yapılandırması
```json
{
  "Messaging": {
    "Kafka": {
      "BootstrapServers": "localhost:9092",
      "GroupId": "ckn-payment-group",
      "AutoOffsetReset": 1 // 1: Earliest, 2: Latest vb.
    }
  }
}
```

### Dependency Injection (DI) Kurulumu
```csharp
using CKN.Sdk.Messaging.Kafka;

builder.Services.AddCknMessaging(msg =>
{
    msg.UseKafka(opt =>
    {
        builder.Configuration.GetSection(KafkaOptions.SectionName).Bind(opt);
    });
});
```

### Gerçek Hayat Kullanımı: Log ve Telemetri Verisi Akışı
```csharp
// Publish
await _eventBus.PublishAsync("telemetry.logs", new { DeviceId = 1, Temperature = 42.5 });

// Subscribe (Consumer Group kullanılarak paralel tüketim yapılabilir)
_eventBus.Subscribe<dynamic>("telemetry.logs", async data => 
{
    Console.WriteLine($"Cihazdan gelen sıcaklık: {data.Temperature}");
    await Task.CompletedTask;
});
```

---

## 3. Azure Service Bus Kullanımı

Bulut tabanlı, yüksek erişilebilirliğe sahip, Topic ve Subscription tabanlı gelişmiş bir mesaj kuyruk servisidir.

### `appsettings.json` Yapılandırması
```json
{
  "Messaging": {
    "ServiceBus": {
      "ConnectionString": "Endpoint=sb://ckn-namespace.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=your_key",
      "TopicOrQueueName": "invoice-events"
    }
  }
}
```

### Dependency Injection (DI) Kurulumu
```csharp
using CKN.Sdk.Messaging.ServiceBus;

builder.Services.AddCknMessaging(msg =>
{
    msg.UseServiceBus(opt =>
    {
        builder.Configuration.GetSection(ServiceBusOptions.SectionName).Bind(opt);
    });
});
```

### Gerçek Hayat Kullanımı: Fatura Onay Süreci
```csharp
public record InvoiceApprovedEvent(string InvoiceNo, decimal TotalAmount);

// Fatura onaylandığında publish et (Uygulamanızın Fatura Servisi'nde)
await _eventBus.PublishAsync("invoice.approved", new InvoiceApprovedEvent("INV-100", 500m));

// Dinleyici tarafı (Finans veya Muhasebe Servisi'nde)
_eventBus.Subscribe<InvoiceApprovedEvent>("invoice.approved", async invoice =>
{
    // Muhasebe programına (Örn: Logo, SAP) entegrasyonu sağla
    await ErpIntegrationService.SyncInvoiceAsync(invoice.InvoiceNo);
});
```
