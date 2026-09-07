# CKN.Sdk.Storage

CKN.Sdk içerisinde dosya ve nesne depolama (Blob Storage) işlemleri için **ortak soyutlamaları (Abstractions)** içeren çekirdek kütüphanedir. Bu proje tek başına bir şey yapmaz, AWS S3, Azure Blob Storage veya Minio gibi Provider'lara standart bir arayüz sağlar.

## Ortak Arayüzler

Bu kütüphane, kodun Amazon'a mı yoksa Azure'a mı bağımlı olduğunu gizlemek (Provider-Agnostic) için aşağıdaki arayüzü sunar:

```csharp
public interface IStorageClient
{
    Task UploadAsync(string bucketName, string objectName, Stream data, string contentType = null);
    Task<Stream> DownloadAsync(string bucketName, string objectName);
    Task DeleteAsync(string bucketName, string objectName);
    Task<bool> ExistsAsync(string bucketName, string objectName);
    Task<string> GetPreSignedUrlAsync(string bucketName, string objectName, TimeSpan expiry);
}
```

## Gerçek Hayat Kullanım Senaryosu

**Profil Fotoğrafı Yükleme (Provider Bağımsız)**
Geliştirici nereye kaydedildiğini bilmeden `IStorageClient` ile dosyasını buluta yükler. Lokal geliştirmede Minio'ya, Production'da S3'e gidebilir.

```csharp
using CKN.Sdk.Storage;

public class ProfileService
{
    private readonly IStorageClient _storageClient;

    public ProfileService(IStorageClient storageClient)
    {
        // Hangi provider kayıtlıysa (S3, Azure vb.) o gelir.
        _storageClient = storageClient;
    }

    public async Task UpdateAvatarAsync(string userId, Stream imageStream)
    {
        var bucket = "user-avatars";
        var fileName = $"{userId}/profile.jpg";

        await _storageClient.UploadAsync(bucket, fileName, imageStream, "image/jpeg");
    }
    
    public async Task<string> GetAvatarUrlAsync(string userId)
    {
        // Kullanıcıya indirmesi için 1 saat geçerli geçici link verir.
        return await _storageClient.GetPreSignedUrlAsync("user-avatars", $"{userId}/profile.jpg", TimeSpan.FromHours(1));
    }
}
```
