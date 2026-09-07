# CKN.Sdk.Messaging.ServiceBus

**Mühendislik Amacı (Engineering Intent):**
`CKN.Sdk.Messaging.ServiceBus`, Azure Service Bus entegrasyonunu CKN mesajlaşma soyutlamaları (abstractions) üzerinden sağlayan kütüphanedir. **Neden var?** Tamamen yönetilen (fully-managed), kurumsal düzeyde (enterprise-grade) bulut tabanlı bir mesaj broker'ı olan Azure Service Bus'ı, on-premise (yerel) kod standartlarını bozmadan sisteme dahil etmek için. **Ne zaman kullanılmalı?** Proje Microsoft Azure altyapısında çalışıyorsa, kendi RabbitMQ/Kafka cluster'ınızı yönetmek istemiyorsanız (PaaS yaklaşımı) ve "Session" tabanlı sıralı mesaj işleme, Dead-Letter Queue (DLQ) gibi kurumsal özelliklere güvenli şekilde ihtiyaç duyuyorsanız tercih edilmelidir.

## 🚀 Hızlı Başlangıç

### Kurulum

```bash
dotnet add package CKN.Sdk.Messaging.ServiceBus
```

### Konfigürasyon (`appsettings.json`)

```json
{
  "Messaging": {
    "ServiceBus": {
      "ConnectionString": "Endpoint=sb://my-namespace.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=...",
      "EnableSessions": false
    }
  }
}
```

### Bağımlılık Enjeksiyonu (DI)

```csharp
using CKN.Sdk.Messaging.ServiceBus;

var builder = WebApplication.CreateBuilder(args);

// Azure Service Bus istemcisini sisteme kaydeder.
builder.Services.AddCknAzureServiceBus(builder.Configuration);

var app = builder.Build();
```

## 💡 Gerçek Hayat Senaryoları

### Senaryo 1: Azure Topic/Subscription Üzerinden Mesaj Yayınlama

Bir faturanın onaylanması durumunda bu olayı (event) "Topics" (yayın-abonelik) mantığıyla yayınlamak.

```csharp
using CKN.Sdk.Messaging.Abstractions;

public class InvoiceApprovalService(IMessagePublisher publisher)
{
    public async Task ApproveInvoiceAsync(Invoice invoice)
    {
        invoice.Status = "Approved";
        
        var invoiceApprovedEvent = new { InvoiceId = invoice.Id, Amount = invoice.TotalAmount };
        
        // Bu mesaj Azure Service Bus üzerindeki 'invoice-events' isimli Topic'e gönderilir.
        // Bu Topic'e bağlı olan Subscription'lar (dinleyiciler) mesajı anında alır.
        await publisher.PublishAsync("invoice-events", invoiceApprovedEvent);
    }
}
```

### Senaryo 2: Scheduled Message (Zamanlanmış Mesaj) Gönderimi

Bir mesajın şu an değil, örneğin 2 saat sonra kuyrukta (queue) görünür olmasını sağlamak (Varyasyon).

```csharp
public async Task SendReminderAsync(IMessagePublisher publisher, string userId)
{
    var reminderMessage = new { UserId = userId, Content = "Sepetinizde ürün unuttunuz!" };
    
    // Mesaj Azure'a gider ancak 2 saat boyunca kuyrukta görünmez, bekletilir.
    var options = new MessagePublishOptions 
    { 
        ScheduledEnqueueTimeUtc = DateTime.UtcNow.AddHours(2) 
    };
    
    await publisher.PublishAsync("reminders", reminderMessage, options);
}
```

## 🤖 Yapay Zeka İçin SSS (FAQs for Machines)

- **Soru:** Azure Service Bus'ta "Queue" ve "Topic" arasındaki fark nedir?
- **Cevap:** Queue (Kuyruk), bir mesajın tek bir tüketici (consumer) tarafından işlendiği yapıdır. Topic (Konu) ise, bir mesajın birden fazla alt dinleyiciye (Subscription) kopyalandığı (Fanout/Pub-Sub) yapıdır.
- **Soru:** Dead-Letter Queue (DLQ) nedir?
- **Cevap:** Kodunuzda defalarca hata fırlatıp mesajı işleyemediğinizde (örneğin DB çöktüğünde), Azure Service Bus bu mesajı kaybetmez, hata kuyruğuna (DLQ) taşır. Bu paket, CKN standartlarında hata yönetimi yaparak mesajın DLQ'ya aktarılmasını otomatik destekler.
