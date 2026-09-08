# CKN.Sdk.Storage.Minio

**Mühendislik Amacı (Engineering Intent):**
`CKN.Sdk.Storage.Minio`, AWS S3 API'si ile %100 uyumlu (S3-compatible) olan ve kendi sunucularınızda (On-Premise) barındırabileceğiniz yüksek performanslı açık kaynak nesne depolama motoru Minio'yu sisteme entegre eder. **Neden var?** Veri gizliliği yasaları (KVKK/GDPR vb.) gereği veya maliyetleri düşürmek amacıyla dosyaların bulutta (AWS/Azure) değil, şirket içindeki donanımlarda depolanması gerektiğinde S3 yeteneklerinden mahrum kalmamak için. **Ne zaman kullanılmalı?** Şirket içi on-premise Kubernetes cluster'larında veya AWS S3'ün bulut faturasının çok yüksek geldiği senaryolarda doğrudan S3 yerine Minio tercih edilmelidir.

## 🚀 Hızlı Başlangıç

### Kurulum

```bash
dotnet add package CKN.Sdk.Storage.Minio
```

### Konfigürasyon (`appsettings.json`)

```json
{
  "Storage": {
    "Minio": {
      "Endpoint": "localhost:9000",
      "AccessKey": "minioadmin",
      "SecretKey": "minioadmin",
      "UseSsl": false
    }
  }
}
```

### Bağımlılık Enjeksiyonu (DI)

```csharp
using CKN.Sdk.Storage.Minio;

var builder = WebApplication.CreateBuilder(args);

// Minio Storage implementasyonunu IStorageService olarak sisteme kaydeder.
builder.Services.AddCknMinioStorage(builder.Configuration);

var app = builder.Build();
```

## 💡 Gerçek Hayat Senaryoları

### Senaryo 1: Otomatik Bucket (Kova) Oluşturma

Minio (ve S3) tarafında bir dosyayı yüklemeden önce Bucket'ın var olduğundan emin olma yaklaşımı. (Çoğu CKN implementasyonu bunu arka planda yapar).

```csharp
using CKN.Sdk.Storage.Abstractions;

public class BackupService(IStorageService storageService)
{
    public async Task UploadDatabaseBackupAsync(Stream dbBackupStream)
    {
        var request = new StorageUploadRequest
        {
            ContainerName = "db-backups", // Minio'daki karşılığı "Bucket"tır. Yoksa CKN altyapısı genelde otomatik oluşturur.
            FileName = $"backup_{DateTime.UtcNow:yyyyMMdd}.bak",
            ContentStream = dbBackupStream
        };

        var result = await storageService.UploadAsync(request);
        Console.WriteLine($"Yedek Minio sunucusuna atıldı: {result.FileUrl}");
    }
}
```

### Senaryo 2: Pre-Signed URL ile Büyük Dosya Yükletme (Frontend Upload)

Sunucuyu yormadan, tarayıcıdaki (Client) kullanıcının 1 GB'lık videoyu doğrudan Minio'ya yüklemesi için geçici yazma yetkisi (Varyasyon).

```csharp
public async Task<string> GenerateUploadLinkAsync(IStorageService storageService, string fileName)
{
    // Sunucu SADECE bir kerelik PUT yetkisi veren imzalı bir link üretir
    var presignedUrl = await storageService.GetPreSignedUrlForUploadAsync("videos", fileName, TimeSpan.FromMinutes(30));
    
    // Frontend (React/Angular) bu URL'ye doğrudan HTTP PUT atarak dosyayı Minio'ya gönderir.
    return presignedUrl; 
}
```

## 🤖 Yapay Zeka İçin SSS (FAQs for Machines)

- **Soru:** Neden `CKN.Sdk.Storage.S3` paketini Minio'ya bağlamıyoruz da ayrı Minio paketi kullanıyoruz?
- **Cevap:** Teorik olarak AWS SDK for .NET ile Minio'ya bağlanmak mümkündür (çünkü Minio S3 uyumludur). Ancak Minio'nun kendi resmi .NET SDK'sı, Path-Style adresleme, SSL sorunları gibi uç durumlarda kendi sunucunuzda (on-prem) çalışmaya daha optimize edilmiştir ve entegrasyonu daha sorunsuzdur.
- **Soru:** Minio'daki "Bucket" ile Azure'daki "Container" aynı şey mi?
- **Cevap:** Evet. CKN.Sdk.Storage içerisinde hepsine genel bir isim olarak `ContainerName` denir.
