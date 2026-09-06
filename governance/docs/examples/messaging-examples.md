# Messaging (Mesajlaşma) Sağlayıcıları Kullanım Örnekleri

CKN.SDK, farklı mesajlaşma broker'larını tak-çalıştır mimarisinde kullanmanızı sağlar. Bu dökümanda projede uygulanan GERÇEK altyapı örnekleri (MassTransit, Kafka, ServiceBus) yer almaktadır.

---

## 1. RabbitMQ (MassTransit) Kullanımı

RabbitMQ entegrasyonu, Outbox pattern desteği olan `CKN.Sdk.MassTransit` paketi üzerinden sağlanır. MassTransit kullandığımız için kuyruk isimlerini (queue/endpoints) tek tek yazmak yerine, MassTransit `IConsumer` sınıflarından isimleri (Örn: `OrderCreatedConsumer` -> `order-created`) otomatik türetir ve eşleştirir.

### Dependency Injection (DI) Kurulumu
`Program.cs` içerisinde:

```csharp
using CKN.Sdk.MassTransit.Messaging;

var builder = WebApplication.CreateBuilder(args);
var rmqConnection = builder.Configuration.GetConnectionString("RabbitMQ") ?? "amqp://guest:guest@localhost:5672";

// TDbContext: Outbox pattern için projenizdeki Entity Framework Core DbContext'i
builder.Services.AddCKNMessaging<AppDbContext>(
    rabbitMqConnectionString: rmqConnection,
    configureConsumers: cfg => 
    {
        // Dinleyici (Subscriber) servislerini burada kaydediyoruz
        cfg.AddConsumer<OrderCreatedConsumer>();
    }
);
```

### Gerçek Hayat Kullanımı: Sipariş Oluşturulduğunda Mesaj Fırlatma

Aşağıdaki örnekte, yeni bir sipariş geldiğinde `IPublishEndpoint` (MassTransit arayüzü) üzerinden RabbitMQ'ya mesaj atılması ve başka bir serviste dinlenmesi gösterilmiştir. (Topic adını manuel vermiyoruz, tip adı üzerinden otomatik çözülüyor).

```csharp
using MassTransit;

public record OrderCreatedEvent(Guid OrderId, decimal Amount, string CustomerEmail);

public class OrderService
{
    private readonly IPublishEndpoint _publishEndpoint;

    public OrderService(IPublishEndpoint publishEndpoint)
    {
        _publishEndpoint = publishEndpoint;
    }

    public async Task CreateOrderAsync(OrderCreatedEvent newOrder)
    {
        // ... Veritabanına siparişi kaydet ...
        
        // Asenkron olarak kuyruğa mesaj bırak (Publish). Kuyruk adı ve routing MassTransit tarafından halledilir.
        await _publishEndpoint.Publish(newOrder);
    }
}

// Mesajı Dinleyen Sınıf (Consumer)
public class OrderCreatedConsumer : IConsumer<OrderCreatedEvent>
{
    private readonly ILogger<OrderCreatedConsumer> _logger;

    public OrderCreatedConsumer(ILogger<OrderCreatedConsumer> logger)
    {
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<OrderCreatedEvent> context)
    {
        var orderEvent = context.Message;
        _logger.LogInformation($"Sipariş alındı: {orderEvent.OrderId}. Müşteriye email gönderiliyor...");
        // Email gönderme işlemi...
    }
}
```

---

## 2. Kafka Kullanımı

Kafka entegrasyonu, saf `IEventBus` arayüzünü (CKN.Sdk.Core) implemente eden `CKN.Sdk.Messaging.Kafka` paketiyle sağlanır.

### `appsettings.json` Yapılandırması
```json
{
  "Messaging": {
    "Kafka": {
      "BootstrapServers": "localhost:9092",
      "GroupId": "ckn-payment-group"
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
        builder.Configuration.GetSection("Messaging:Kafka").Bind(opt);
    });
});
```

### Gerçek Hayat Kullanımı: Telemetri Verisi Akışı
Saf `IEventBus` arayüzü, topic/queue adını manuel string olarak sormaz; bunun yerine C# objesinin kendi ismini (`typeof(TEvent).Name`) Topic olarak Kafka'ya kaydeder.

```csharp
using CKN.Sdk.Core.Events;

public class TelemetryLogEvent : IIntegrationEvent 
{ 
    public int DeviceId { get; set; }
    public decimal Temperature { get; set; }
}

// Publish işlemi
await _eventBus.PublishAsync(new TelemetryLogEvent { DeviceId = 1, Temperature = 42.5m });

// Subscribe işlemi (IEventHandler arayüzünü implemente eden bir sınıf ile)
public class TelemetryLogHandler : IEventHandler<TelemetryLogEvent>
{
    public Task HandleAsync(TelemetryLogEvent @event)
    {
        Console.WriteLine($"Cihazdan gelen sıcaklık: {@event.Temperature}");
        return Task.CompletedTask;
    }
}

// Uygulama başlarken dinlemeyi başlatma:
_eventBus.Subscribe<TelemetryLogEvent, TelemetryLogHandler>();
```

---

## 3. Azure Service Bus Kullanımı

Service Bus entegrasyonu `CKN.Sdk.Messaging.ServiceBus` üzerinden çalışır.

### `appsettings.json` Yapılandırması
```json
{
  "Messaging": {
    "ServiceBus": {
      "ConnectionString": "Endpoint=sb://ckn-namespace.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=your_key"
    }
  }
}
```

### Dependency Injection (DI) Kurulumu
```csharp
using CKN.Sdk.Messaging.ServiceBus;

builder.Services.AddCknServiceBus(opt => 
{
    builder.Configuration.GetSection("Messaging:ServiceBus").Bind(opt);
});
```

### Gerçek Hayat Kullanımı: Fatura Onay Süreci
```csharp
public record InvoiceApprovedEvent(string InvoiceNo, decimal TotalAmount) : IIntegrationEvent;

// Fatura onaylandığında publish et (Otomatik olarak "InvoiceApprovedEvent" isimli kuyruğa/topic'e yazar)
await _eventBus.PublishAsync(new InvoiceApprovedEvent("INV-100", 500m));
```
