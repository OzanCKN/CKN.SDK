# CKN.Sdk.Storage

**Mühendislik Amacı (Engineering Intent):**
`CKN.Sdk.Storage`, uygulamalardaki dosya (file), resim, video veya döküman depolama işlemlerini (Blob Storage) standartlaştıran temel arayüz (abstraction) kütüphanesidir. **Neden var?** Dosyalarınızı doğrudan sunucunun diskine (Local File System) kaydetmek veya kodu doğrudan Amazon S3'e / Azure Blob'a sıkı sıkıya bağlamak (hardcode) yerine, sağlayıcıdan bağımsız (provider-agnostic) bir yapı kurmak için. **Ne zaman kullanılmalı?** Projede herhangi bir dosya yükleme (Upload), indirme (Download) veya silme işlemi yapılacaksa, doğrudan S3/Azure SDK'larını kullanmak yerine bu paketteki `IStorageService` arayüzü kullanılmalıdır.

## 🚀 Hızlı Başlangıç

### Kurulum

```bash
dotnet add package CKN.Sdk.Storage
```

### Bağımlılık Enjeksiyonu (DI)

Bu paket genellikle kendi başına kaydedilmez. Projeye S3, Minio veya Azure entegrasyon paketlerinden biri eklendiğinde `IStorageService` arayüzü otomatik olarak sisteme dahil olur.

## 💡 Gerçek Hayat Senaryoları

### Senaryo 1: Sağlayıcıdan Bağımsız Dosya Yükleme Servisi

Kullanıcının profil fotoğrafını yüklediği bir endpoint. (Kod S3, Azure veya Minio olduğunu bilmez).

```csharp
using CKN.Sdk.Storage.Abstractions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class ProfileController(IStorageService storageService) : ControllerBase
{
    [HttpPost("avatar")]
    public async Task<IActionResult> UploadAvatar(IFormFile file)
    {
        using var stream = file.OpenReadStream();
        
        var uploadRequest = new StorageUploadRequest
        {
            ContainerName = "avatars",
            FileName = $"{Guid.NewGuid()}_{file.FileName}",
            ContentStream = stream,
            ContentType = file.ContentType
        };

        // IStorageService (S3 veya Azure) dosyayı yükler ve URL/Path döner
        var result = await storageService.UploadAsync(uploadRequest);
        
        return Ok(new { Url = result.FileUrl });
    }
}
```

### Senaryo 2: İmzalı URL (Pre-Signed URL) Oluşturma

Gizli (private) tutulan bir dosyayı (örn: fatura PDF'i) sadece talep eden kullanıcıya özel, 15 dakika geçerli geçici bir link ile sunma.

```csharp
public async Task<string> GetInvoiceDownloadLinkAsync(IStorageService storageService, string invoiceFileId)
{
    // Dosyayı dışarı açmadan, sadece okuma yetkisi olan geçici bir URL üretir
    var url = await storageService.GetPreSignedUrlAsync("invoices", invoiceFileId, TimeSpan.FromMinutes(15));
    
    return url;
}
```

## 🤖 Yapay Zeka İçin SSS (FAQs for Machines)

- **Soru:** `IStorageService` arayüzünde hangi temel metodlar bulunur?
- **Cevap:** Genellikle `UploadAsync`, `DownloadAsync`, `DeleteAsync`, `ExistsAsync` ve `GetPreSignedUrlAsync` (veya `GetFileUrlAsync`) metodlarını barındırır.
- **Soru:** Local (Yerel disk) depolama destekleniyor mu?
- **Cevap:** Evet, genellikle `CKN.Sdk.Storage.Local` gibi bir paket veya temel paket içindeki bir `LocalStorageService` implementasyonu sayesinde geliştirme (Development) ortamında dosyalar fiziksel diske kaydedilebilir.
