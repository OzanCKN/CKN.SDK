# CKN.Sdk.Storage.Minio

AWS S3 uyumlu, çok hızlı, genellikle On-Premise (kendi sunucularınızda) veya Local geliştirme ortamlarında tercih edilen açık kaynaklı **MinIO** nesne depolama (Object Storage) entegrasyonudur. `CKN.Sdk.Storage` altyapısındaki `IStorageClient` arayüzünü uygular.

## Yapılandırma (`appsettings.json`)

```json
{
  "Storage": {
    "Minio": {
      "Endpoint": "localhost:9000",
      "AccessKey": "minioadmin",
      "SecretKey": "minioadmin",
      "UseSSL": false
    }
  }
}
```

## Servis Kaydı (Dependency Injection)

```csharp
using CKN.Sdk.Storage.Minio;

var builder = WebApplication.CreateBuilder(args);

// MinIO altyapısını sisteme dahil etme
builder.Services.AddCknMinioStorage(builder.Configuration);

var app = builder.Build();
```

## Gerçek Hayat Kullanım Senaryosu

**Kurum İçi (On-Prem) Dosya Yükleme Servisi**
KVKK / GDPR gereği verilerin Amazon veya Azure gibi bulut sunuculara gitmesinin yasak olduğu durumlarda yerel sunucuda (Minio) barındırılan dosyaların yüklenmesi.

```csharp
using CKN.Sdk.Storage;

public class DocumentUploadService
{
    private readonly IStorageClient _storageClient;

    public DocumentUploadService(IStorageClient storageClient)
    {
        _storageClient = storageClient;
    }

    public async Task UploadConfidentialDocumentAsync(string documentId, Stream fileStream)
    {
        // Veri internete çıkmaz, yerel MinIO sunucusuna ("confidential-docs" bucket'ına) yüklenir
        await _storageClient.UploadAsync("confidential-docs", $"{documentId}.pdf", fileStream, "application/pdf");
    }
}
```
