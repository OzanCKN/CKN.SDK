# CKN.Sdk.MassTransit

Sistemin olay güdümlü (Event-Driven) mimariyi, Message Broker iletişimlerini ve Saga State Machine yapılarını yönetmesini sağlayan kütüphanedir.

## 📦 Kurulum (NuGet)
```bash
dotnet add package CKN.Sdk.MassTransit
```

## 🚀 Kullanım
`Program.cs` dosyasına tek satırla entegre edilir. Arka planda RabbitMQ veya Azure Service Bus yapılandırmaları merkezi olarak halledilir:

```csharp
builder.Services.AddCknMassTransit(options => 
{
    options.UseRabbitMq(builder.Configuration.GetConnectionString("RabbitMQ"));
});
```

## Özellikler
- **Mesajlaşma:** Event Publish/Consume altyapısı.
- **Sagas:** `MeetingProcessingStateMachine` gibi kompleks iş süreçlerinin orkestrasyonu.
- **Entegrasyon:** Core katmanından fırlatılan Domain Event'lerin Integration Event'lere otomatik dönüşüm köprüsü.
