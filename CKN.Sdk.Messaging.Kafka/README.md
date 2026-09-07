# CKN.Sdk.Messaging.Kafka

Açık kaynaklı, dağıtık ve yüksek ölçeklenebilir streaming platformu olan **Apache Kafka** için entegrasyon kütüphanesidir. CKN.Sdk içerisindeki `IEventBus` arayüzünü uygular.

## Yapılandırma (`appsettings.json`)

```json
{
  "Messaging": {
    "Kafka": {
      "BootstrapServers": "localhost:9092",
      "GroupId": "ckn_consumer_group",
      "AutoOffsetReset": "Earliest"
    }
  }
}
```

## Servis Kaydı (Dependency Injection)

```csharp
using CKN.Sdk.Messaging.Kafka;

var builder = WebApplication.CreateBuilder(args);

// Kafka EventBus'ı sisteme dahil etme
builder.Services.AddCknKafka(builder.Configuration);

var app = builder.Build();
```

## Gerçek Hayat Kullanım Senaryosu

**Log veya Telemetri Verisi Toplama (Event Sourcing)**
Mikroservislerden gelen on binlerce anlık hareket (tıklama, sayfa görüntüleme, log) verisinin hızlıca Kafka'ya fırlatılıp arkada Data Warehouse'a aktarılması.

```csharp
using CKN.Sdk.Core.Events;

public class ClickTelemetryEvent : IntegrationEvent
{
    public string UserId { get; set; }
    public string PageUrl { get; set; }
}

public class TelemetryService
{
    private readonly IEventBus _eventBus;

    public TelemetryService(IEventBus eventBus)
    {
        _eventBus = eventBus;
    }

    public async Task LogClickAsync(string userId, string pageUrl)
    {
        var evt = new ClickTelemetryEvent { UserId = userId, PageUrl = pageUrl };
        
        // Kafka'ya anında pushlanır. Yüksek throughput ile işlenir.
        await _eventBus.PublishAsync(evt);
    }
}
```
