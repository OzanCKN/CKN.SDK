# CKN.Sdk.Storage.S3

Amazon Web Services'in endüstri lideri nesne depolama çözümü olan **Amazon S3 (Simple Storage Service)** entegrasyon kütüphanesidir. Sınırsız ölçeklenebilirlik gereken senaryolarda `CKN.Sdk.Storage` altyapısındaki `IStorageClient` arayüzünü uygular.

## Yapılandırma (`appsettings.json`)

```json
{
  "Storage": {
    "S3": {
      "AccessKeyId": "AKIA...",
      "SecretAccessKey": "...",
      "Region": "eu-central-1"
    }
  }
}
```

## Servis Kaydı (Dependency Injection)

```csharp
using CKN.Sdk.Storage.S3;

var builder = WebApplication.CreateBuilder(args);

// Amazon S3 altyapısını sisteme dahil etme
builder.Services.AddCknS3Storage(builder.Configuration);

var app = builder.Build();
```

## Gerçek Hayat Kullanım Senaryosu

**Video veya Büyük Dosya Yükleme (Presigned URL İle Güvenli İndirme)**
Kullanıcıların e-eğitim platformuna yükledikleri gigabaytlarca veriyi saklamak ve sadece o videoyu satın alan kullanıcılara S3 üzerinden (sunucuyu yormadan) izleme linki (Pre-Signed URL) vermek.

```csharp
using CKN.Sdk.Storage;

public class VideoStreamingService
{
    private readonly IStorageClient _storageClient;

    public VideoStreamingService(IStorageClient storageClient)
    {
        _storageClient = storageClient;
    }

    public async Task<string> GenerateSecureVideoLinkAsync(string videoId)
    {
        // AWS S3 sunucusuna yönlendiren ve 2 saat (TimeSpan.FromHours(2)) sonra süresi dolacak geçici link oluşturur.
        // Bu sayede uygulamanız (API) bant genişliği tüketmez, müşteri videoyu direkt S3'ten çeker.
        var secureUrl = await _storageClient.GetPreSignedUrlAsync("training-videos", $"{videoId}.mp4", TimeSpan.FromHours(2));
        return secureUrl;
    }
}
```
