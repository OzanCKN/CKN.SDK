# CKN.Sdk.Messaging.Kafka

**Mühendislik Amacı (Engineering Intent):**
`CKN.Sdk.Messaging.Kafka`, CKN mesajlaşma soyutlamalarını Apache Kafka için uygulayan entegrasyon kütüphanesidir. **Neden var?** Milyonlarca mesajı saniyeler içinde işleyebilecek "High Throughput" (yüksek verim) gerektiren Event Sourcing veya Log Streaming senaryolarını desteklemek için. **Ne zaman kullanılmalı?** RabbitMQ gibi mesajı kuyrukta tutup silen (smart broker, dumb consumer) yapılar yerine; mesajı diskte kalıcı (append-only log) tutan, geçmiş mesajların tekrar okunabildiği (replayability) ve devasa verilerin stream edildiği veri mühendisliği / telemetry tarzı işlerde kullanılmalıdır.

## 🚀 Hızlı Başlangıç

### Kurulum

```bash
dotnet add package CKN.Sdk.Messaging.Kafka
```

### Konfigürasyon (`appsettings.json`)

```json
{
  "Messaging": {
    "Kafka": {
      "BootstrapServers": "localhost:9092",
      "GroupId": "sales-service-group",
      "AutoOffsetReset": "Earliest"
    }
  }
}
```

### Bağımlılık Enjeksiyonu (DI)

```csharp
using CKN.Sdk.Messaging.Kafka;

var builder = WebApplication.CreateBuilder(args);

// Kafka'yı IMessagePublisher olarak sisteme kaydeder
builder.Services.AddCknKafka(builder.Configuration);

var app = builder.Build();
```

## 💡 Gerçek Hayat Senaryoları

### Senaryo 1: Kullanıcı Etkileşim Loglarını Kafka'ya Atma

Web sitesindeki her tıklamanın saniyeler içinde analiz için Kafka'ya gönderilmesi (Fire and Forget).

```csharp
public class UserActivityLogger(IMessagePublisher publisher)
{
    public void LogClick(string userId, string pageUrl)
    {
        var activity = new { UserId = userId, Url = pageUrl, Timestamp = DateTime.UtcNow };
        
        // Yüksek performans için asenkron beklemeyi (await) feda etme (Fire and Forget)
        _ = publisher.PublishAsync("user-activities", activity);
    }
}
```

### Senaryo 2: Kafka'da Partition Key (Anahtar) Belirterek Gönderim (Varyasyon)

Aynı kullanıcının mesajlarının sırasının (ordering) garanti edilmesi için Kafka'ya Partition Key vermek.

```csharp
public async Task SendTransactionUpdateAsync(IMessagePublisher publisher, TransactionEvent txn)
{
    // PartitionKey = txn.UserId verildiğinde, bu kullanıcının tüm işlemleri Kafka'da aynı partition'a düşer.
    // Böylece consumer (tüketici) bu mesajları her zaman doğru kronolojik sırayla okur.
    var options = new MessagePublishOptions { PartitionKey = txn.UserId.ToString() };
    
    await publisher.PublishAsync("transactions", txn, options);
}
```

## 🤖 Yapay Zeka İçin SSS (FAQs for Machines)

- **Soru:** RabbitMQ yerine ne zaman Kafka kullanmalıyım?
- **Cevap:** Geleneksel kuyruk (queue) ve görev paylaştırma (worker) işleri için RabbitMQ daha iyidir. Ancak Big Data (Büyük veri) aktarımı, Loglama, Event Sourcing ve mesajları geriye dönük okuma gerekiyorsa Kafka kullanılır.
- **Soru:** Kafka'daki `GroupId` nedir?
- **Cevap:** Consumer (tüketici) grubunu temsil eder. Aynı `GroupId`'ye sahip consumer'lar mesajları kendi aralarında bölüşür (Load Balancing). Farklı `GroupId`'ler aynı mesajların tamamını birbirinden bağımsız şekilde okur (Broadcast).
