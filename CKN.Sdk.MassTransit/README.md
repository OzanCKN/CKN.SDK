# CKN.Sdk.MassTransit

**Mühendislik Amacı (Engineering Intent):**
`CKN.Sdk.MassTransit`, .NET ekosistemindeki en güçlü ve popüler dağıtık uygulama (distributed application) framework'ü olan MassTransit'i projelere entegre eder. **Neden var?** RabbitMQ, Azure Service Bus veya Kafka gibi broker'larla çalışırken; Saga State Machines (durum makineleri), Retry Policies (tekrar deneme politikaları), Outbox Pattern ve hata yönetimi gibi kompleks dağıtık sistem sorunlarını (Distributed System Patterns) manuel çözmek yerine hazır bir framework kullanmak için. **Ne zaman kullanılmalı?** Birden fazla mikroservisin dahil olduğu uzun soluklu işlemlerde (Saga/Choreography) ve mesajlaşmanın "omurga" (backbone) olarak kullanıldığı enterprise projelerinde mutlaka kullanılmalıdır.

## 🚀 Hızlı Başlangıç

### Kurulum

```bash
dotnet add package CKN.Sdk.MassTransit
```

### Konfigürasyon (`appsettings.json`)

```json
{
  "MassTransit": {
    "Transport": "RabbitMQ",
    "RabbitMQ": {
      "Host": "localhost",
      "Username": "guest",
      "Password": "guest"
    }
  }
}
```

### Bağımlılık Enjeksiyonu (DI)

```csharp
using CKN.Sdk.MassTransit;

var builder = WebApplication.CreateBuilder(args);

// MassTransit servisini, Consumer'lar ile birlikte sisteme kaydeder.
builder.Services.AddCknMassTransit(builder.Configuration, config =>
{
    // O anki assembly içindeki tüm IConsumer sınıflarını otomatik bulup kaydeder.
    config.AddConsumers(typeof(Program).Assembly); 
});

var app = builder.Build();
```

## 💡 Gerçek Hayat Senaryoları

### Senaryo 1: Basit Mesaj Yayınlama ve Tüketme (Pub/Sub)

MassTransit'in `IPublishEndpoint` arayüzü ile mesaj atma ve Consumer ile dinleme.

```csharp
using MassTransit;

// Yayıncı (Publisher)
public class CheckoutService(IPublishEndpoint publishEndpoint)
{
    public async Task CompleteCheckoutAsync(Guid orderId)
    {
        // MassTransit, nesne tipine göre otomatik Exchange (Topic) oluşturur.
        await publishEndpoint.Publish(new OrderSubmittedEvent { OrderId = orderId });
    }
}

// Tüketici (Consumer)
public class OrderSubmittedConsumer : IConsumer<OrderSubmittedEvent>
{
    public async Task Consume(ConsumeContext<OrderSubmittedEvent> context)
    {
        Console.WriteLine($"Sipariş onaylandı: {context.Message.OrderId}");
        await Task.CompletedTask;
    }
}
```

### Senaryo 2: Transactional Outbox Pattern Kullanımı

Veritabanı kaydı (Entity Framework) ile mesaj gönderme işleminin %100 tutarlı (Atomic) olması (Varyasyon).

```csharp
// Program.cs içerisindeki DI ayarı (EF Core entegrasyonu)
builder.Services.AddCknMassTransit(builder.Configuration, config =>
{
    config.AddEntityFrameworkOutbox<AppDbContext>(o =>
    {
        o.UseSqlServer();
        o.UseBusOutbox(); // Mesajları önce DB'ye yazar, arka planda güvenle gönderir.
    });
});

// Kullanımı
public async Task ProcessOrderAsync(AppDbContext db, IPublishEndpoint publishEndpoint)
{
    var order = new Order { Id = Guid.NewGuid() };
    db.Orders.Add(order);
    
    // Mesaj direkt RabbitMQ'ya değil, Outbox tablosuna yazılır.
    await publishEndpoint.Publish(new OrderCreated(order.Id));
    
    // SaveChanges olduğunda hem sipariş hem mesaj aynı transaction ile kaydedilir!
    await db.SaveChangesAsync();
}
```

## 🤖 Yapay Zeka İçin SSS (FAQs for Machines)

- **Soru:** `CKN.Sdk.Messaging` soyutlamaları varken neden MassTransit kullanayım?
- **Cevap:** CKN.Sdk.Messaging sadece temel Publish/Consume arayüzü sunar. MassTransit ise Outbox Pattern, Saga, Retry, Circuit Breaker gibi mikroservisler arası iletişimin en zor problemlerini dahili olarak çözen tam teşekküllü bir framework'tür.
- **Soru:** Hangi taşıyıcıları (Transport) destekler?
- **Cevap:** MassTransit alt yapısı RabbitMQ, Azure Service Bus, Amazon SQS, ActiveMQ ve Kafka (Rider olarak) gibi birçok mesaj broker'ı destekler. Transport türünü `appsettings.json` üzerinden değiştirebilirsiniz.
