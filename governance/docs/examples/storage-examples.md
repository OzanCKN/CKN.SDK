# Storage (Depolama) Sağlayıcıları Kullanım Örnekleri

CKN.SDK, nesne depolama (Object Storage) ihtiyaçlarınız için `IStorageService` arayüzünü kullanarak bulut agnostik bir yapı sunar. Bu sayede S3, Azure Blob veya kendi sunucunuzdaki Minio arasında kod değiştirmeden geçiş yapabilirsiniz.

---

## 1. Minio (veya AWS S3) Kullanımı

S3 uyumlu herhangi bir storage servisi için (AWS S3, DigitalOcean Spaces veya Local Minio) kullanılabilir.

### `appsettings.json` Yapılandırması
```json
{
  "Storage": {
    "Minio": {
      "Endpoint": "localhost:9000",
      "AccessKey": "minioadmin",
      "SecretKey": "minioadmin",
      "UseSsl": false
    },
    "S3": {
      "BucketName": "my-production-bucket",
      "Region": "eu-central-1",
      "AccessKey": "AKI...",
      "SecretKey": "XYZ..."
    }
  }
}
```

### Dependency Injection (DI) Kurulumu
```csharp
using CKN.Sdk.Storage.Minio;

builder.Services.AddCknMinioStorage(opt =>
{
    builder.Configuration.GetSection(MinioStorageOptions.SectionName).Bind(opt);
});
```

### Gerçek Hayat Kullanımı: Profil Fotoğrafı Yükleme (Stream Upload)
Büyük dosyaları sunucu RAM'inde şişirmeden doğrudan Cloud'a (Stream aracılığıyla) aktarma:

```csharp
using CKN.Sdk.Storage;
using Microsoft.AspNetCore.Http;

public class UserProfileService
{
    private readonly IStorageService _storageService;
    private const string BUCKET_NAME = "profile-pictures";

    public UserProfileService(IStorageService storageService)
    {
        _storageService = storageService;
    }

    public async Task<string> UploadProfilePictureAsync(Guid userId, IFormFile file)
    {
        // 1. Bucket'ın var olduğundan emin ol (Eğer yoksa oluşturur)
        await _storageService.CreateBucketIfNotExistsAsync(BUCKET_NAME);

        // 2. Orijinal dosyanın stream'ini aç
        using var stream = file.OpenReadStream();
        string objectName = $"{userId}/{file.FileName}";

        // 3. Dosyayı asenkron olarak storage'a aktar
        await _storageService.UploadFileAsync(BUCKET_NAME, objectName, stream, file.ContentType);

        // 4. İleride kullanıcıya sunmak üzere resmi çekebileceği bir Pre-signed (Geçici) URL oluştur
        // URL 1 saat sonra geçersiz olur (Güvenli indirme)
        var tempUrl = await _storageService.GetPreSignedUrlAsync(BUCKET_NAME, objectName, TimeSpan.FromHours(1));
        
        return tempUrl;
    }
}
```

---

## 2. Azure Blob Storage Kullanımı

Azure'un yerel Blob depolama servisini kullanmak için idealdir.

### `appsettings.json` Yapılandırması
```json
{
  "Storage": {
    "Azure": {
      "ConnectionString": "DefaultEndpointsProtocol=https;AccountName=cknstorage;AccountKey=your_key;EndpointSuffix=core.windows.net"
    }
  }
}
```

### Dependency Injection (DI) Kurulumu
```csharp
using CKN.Sdk.Storage.Azure;

builder.Services.AddCknAzureStorage(opt =>
{
    builder.Configuration.GetSection(AzureStorageOptions.SectionName).Bind(opt);
});
```

### Gerçek Hayat Kullanımı: KVKK Kapsamında Veri İmhası (Dosya Silme)
Kullanıcı hesabını sildiğinde sistemde kalan fatura ve sözleşme PDF'lerinin kalıcı olarak silinmesi senaryosu:

```csharp
public class PrivacyService
{
    private readonly IStorageService _storageService;

    public PrivacyService(IStorageService storageService)
    {
        _storageService = storageService;
    }

    public async Task DeleteUserDocumentsAsync(Guid userId)
    {
        // Farz edelim ki dosyalar user ID ile adlandırılmış
        string invoicePath = $"invoices/{userId}_invoice.pdf";
        string contractPath = $"contracts/{userId}_contract.pdf";

        // İlgili storage'dan dosyaları kalıcı olarak sil
        await _storageService.DeleteFileAsync("company-documents", invoicePath);
        await _storageService.DeleteFileAsync("company-documents", contractPath);
    }
}
```
