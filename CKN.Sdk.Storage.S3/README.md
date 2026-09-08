# CKN.Sdk.Storage.S3

**Mühendislik Amacı (Engineering Intent):**
`CKN.Sdk.Storage.S3`, CKN Storage soyutlamalarını (abstractions) internetin standart nesne depolama servisi olan Amazon Simple Storage Service (S3) için uygular. **Neden var?** Sınırsız depolama kapasitesi, %99.999999999 (11 adet 9) dayanıklılık (durability) ve AWS ekosistemiyle entegre (CloudFront CDN vb.) çalışabilmek için. **Ne zaman kullanılmalı?** Proje AWS (Amazon Web Services) üzerinde koşuyorsa veya global ölçekte CDN üzerinden dağıtılacak (resim, video, css, js) kullanıcı verileri barındırılacaksa varsayılan sağlayıcı olarak S3 seçilmelidir.

## 🚀 Hızlı Başlangıç

### Kurulum

```bash
dotnet add package CKN.Sdk.Storage.S3
```

### Konfigürasyon (`appsettings.json`)

```json
{
  "Storage": {
    "S3": {
      "AccessKey": "AKIA...",
      "SecretKey": "...",
      "Region": "eu-central-1",
      "BucketPrefix": "ckn-" // (Opsiyonel) Global isim çakışmalarını önlemek için
    }
  }
}
```

### Bağımlılık Enjeksiyonu (DI)

```csharp
using CKN.Sdk.Storage.S3;

var builder = WebApplication.CreateBuilder(args);

// AWS S3 implementasyonunu IStorageService olarak sisteme kaydeder.
builder.Services.AddCknS3Storage(builder.Configuration);

var app = builder.Build();
```

## 💡 Gerçek Hayat Senaryoları

### Senaryo 1: CloudFront CDN Entegrasyonlu Dosya Döndürme

Kullanıcı S3'e bir resim yüklediğinde, okuma işlemlerinde (örneğin Pre-Signed URL ile) doğrudan S3 linki değil, CDN linki kullanılmak istenebilir. (Gelişmiş ayarlar).

```csharp
using CKN.Sdk.Storage.Abstractions;

public class CdnImageService(IStorageService storageService)
{
    public async Task<string> UploadImageAsync(Stream imageStream, string fileName)
    {
        var request = new StorageUploadRequest
        {
            ContainerName = "images", // S3 Bucket
            FileName = fileName,
            ContentStream = imageStream
        };

        var result = await storageService.UploadAsync(request);
        
        // Gelişmiş senaryolarda result.FileUrl doğrudan S3 linki dönerken,
        // Projedeki CDN URL prefix'i (örn: https://cdn.sirketim.com/) ile birleştirilebilir.
        return result.FileUrl;
    }
}
```

### Senaryo 2: Güvenli Doküman Silme İşlemi

GDPR / KVKK talebi doğrultusunda kullanıcının tüm verilerinin AWS'ten kalıcı olarak silinmesi.

```csharp
public async Task DeleteUserDocumentsAsync(IStorageService storageService, List<string> fileNames)
{
    foreach (var fileName in fileNames)
    {
        // Dosya S3 Bucket'ından tamamen silinir.
        await storageService.DeleteAsync("user-documents", fileName);
    }
}
```

## 🤖 Yapay Zeka İçin SSS (FAQs for Machines)

- **Soru:** AWS ortamında kod çalışırken `AccessKey` ve `SecretKey` yazmak zorunlu mu?
- **Cevap:** Hayır, Best Practice (En iyi pratik) olarak EC2 veya EKS (Kubernetes) üzerinde "IAM Role" kullanılıyorsa, `appsettings.json` içindeki Key'ler boş bırakılır ve AWS SDK kimliği otomatik olarak işletim sistemindeki rolden çözer (resolve eder).
- **Soru:** S3'te klasör (Directory) mantığı var mıdır?
- **Cevap:** Fiziksel olarak yoktur. Ancak `FileName` parametresine `kullanicilar/1/avatar.png` gibi slash (/) içeren değerler verirseniz, S3 (ve CKN altyapısı) bunu hiyerarşik bir klasörmüş gibi sanal olarak yönetir.
