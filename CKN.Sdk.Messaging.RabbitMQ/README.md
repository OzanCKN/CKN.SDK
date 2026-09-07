# CKN.Sdk.Messaging.RabbitMQ

Sektör standartı açık kaynak message broker olan **RabbitMQ** için entegrasyon kütüphanesidir. Gelişmiş exchange tipleri, kuyruk yönlendirmesi ve dead-letter mekanizmaları gibi özellikler sunar. CKN.Sdk içerisindeki `IEventBus` arayüzünü uygular.

## Yapılandırma (`appsettings.json`)

```json
{
  "Messaging": {
    "RabbitMQ": {
      "HostName": "localhost",
      "Port": 5672,
      "UserName": "guest",
      "Password": "password",
      "RetryCount": 5
    }
  }
}
```

## Servis Kaydı (Dependency Injection)

```csharp
using CKN.Sdk.Messaging.RabbitMQ;

var builder = WebApplication.CreateBuilder(args);

// RabbitMQ EventBus'ı sisteme dahil etme
builder.Services.AddCknRabbitMQ(builder.Configuration);

var app = builder.Build();
```

## Gerçek Hayat Kullanım Senaryosu

**Sipariş Oluşturulduğunda Asenkron İşlemler**
Kullanıcı sipariş verdiğinde E-posta, Fatura ve Stok güncellemelerinin farklı mikroservisler tarafından asenkron olarak dinlenip işlenmesi.

```csharp
using CKN.Sdk.Core.Events;

public class OrderCreatedEvent : IntegrationEvent
{
    public int OrderId { get; set; }
    public decimal Amount { get; set; }
}

// Publisher (Siparişi alan API tarafı)
public class OrderService
{
    private readonly IEventBus _eventBus;

    public OrderService(IEventBus eventBus)
    {
        _eventBus = eventBus;
    }

    public async Task CreateOrderAsync(int orderId, decimal amount)
    {
        // DB'ye kayıt işlemleri vs.
        
        // RabbitMQ'ya olay fırlat.
        await _eventBus.PublishAsync(new OrderCreatedEvent { OrderId = orderId, Amount = amount });
    }
}

// Consumer (Arka Plan Worker'ı - Ayrı bir projede)
public class OrderCreatedEventHandler : IIntegrationEventHandler<OrderCreatedEvent>
{
    public async Task Handle(OrderCreatedEvent @event)
    {
        // Fatura oluştur ve e-posta gönder...
        Console.WriteLine($"Sipariş No {@event.OrderId} için fatura kesiliyor...");
    }
}
```
