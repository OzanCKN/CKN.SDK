# CKN.Sdk.Messaging.ServiceBus

Microsoft Azure ortamında kurumsal, tam yönetilen mesajlaşma hizmeti olan **Azure Service Bus** için entegrasyon kütüphanesidir. Topics, Subscriptions ve ileri düzey mesaj güvenilirliği (Exactly Once Delivery) için uygundur. CKN.Sdk içerisindeki `IEventBus` arayüzünü uygular.

## Yapılandırma (`appsettings.json`)

```json
{
  "Messaging": {
    "ServiceBus": {
      "ConnectionString": "Endpoint=sb://ckn-namespace.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=...",
      "TopicName": "ckn_integration_events"
    }
  }
}
```

## Servis Kaydı (Dependency Injection)

```csharp
using CKN.Sdk.Messaging.ServiceBus;

var builder = WebApplication.CreateBuilder(args);

// Azure Service Bus EventBus'ı sisteme dahil etme
builder.Services.AddCknServiceBus(builder.Configuration);

var app = builder.Build();
```

## Gerçek Hayat Kullanım Senaryosu

**Finansal EFT/Havale Kuyruğa Atma (Güvenilirlik)**
Mesaj kaybının kesinlikle kabul edilmediği finansal işlemlerde, işlemin Service Bus kuyruğuna güvenle atılması.

```csharp
using CKN.Sdk.Core.Events;

public class PaymentInitiatedEvent : IntegrationEvent
{
    public string TransactionId { get; set; }
    public decimal Amount { get; set; }
}

public class PaymentService
{
    private readonly IEventBus _eventBus;

    public PaymentService(IEventBus eventBus)
    {
        _eventBus = eventBus;
    }

    public async Task InitiateTransferAsync(string trxId, decimal amount)
    {
        var evt = new PaymentInitiatedEvent { TransactionId = trxId, Amount = amount };
        
        // Azure Service Bus'a pushlanır. Mesajın kaybolmayacağı garantilidir.
        await _eventBus.PublishAsync(evt);
    }
}
```
