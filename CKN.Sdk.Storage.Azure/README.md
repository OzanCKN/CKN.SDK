# CKN.Sdk.Storage.Azure

**Mühendislik Amacı (Engineering Intent):**
`CKN.Sdk.Storage.Azure`, CKN Storage soyutlamalarını Microsoft Azure Blob Storage hizmeti için uygulayan entegrasyon kütüphanesidir. **Neden var?** Sınırsız ölçeklenebilir, yüksek erişilebilirliğe (HA) sahip ve Microsoft ekosistemiyle %100 uyumlu (Entra ID, RBAC destekli) nesne depolama altyapısını kullanmak için. **Ne zaman kullanılmalı?** Proje Azure üzerinde barındırılıyorsa veya Azure altyapısına geçiş stratejisi varsa, dosya, yedek (backup), resim ve video depolama işlemleri için varsayılan sağlayıcı olarak kullanılmalıdır.

## 🚀 Hızlı Başlangıç

### Kurulum

```bash
dotnet add package CKN.Sdk.Storage.Azure
```

### Konfigürasyon (`appsettings.json`)

```json
{
  "Storage": {
    "Azure": {
      "ConnectionString": "DefaultEndpointsProtocol=https;AccountName=myaccount;AccountKey=...;EndpointSuffix=core.windows.net"
    }
  }
}
```

### Bağımlılık Enjeksiyonu (DI)

```csharp
using CKN.Sdk.Storage.Azure;

var builder = WebApplication.CreateBuilder(args);

// Azure Blob Storage implementasyonunu IStorageService olarak sisteme kaydeder.
builder.Services.AddCknAzureStorage(builder.Configuration);

var app = builder.Build();
```

## 💡 Gerçek Hayat Senaryoları

### Senaryo 1: Azure Storage Üzerinden Dosya İndirme

Bir dökümanı belleğe (stream) alıp kullanıcıya dosya olarak döndürme.

```csharp
using CKN.Sdk.Storage.Abstractions;
using Microsoft.AspNetCore.Mvc;

public class DocumentController(IStorageService storageService) : ControllerBase
{
    [HttpGet("download/{fileName}")]
    public async Task<IActionResult> Download(string fileName)
    {
        var response = await storageService.DownloadAsync("documents", fileName);
        
        if (response.ContentStream == null) return NotFound();

        return File(response.ContentStream, response.ContentType, fileName);
    }
}
```

### Senaryo 2: Konteyner (Container) Seviyesinde Erişim Yetkisi Verme (Varyasyon)

Public (herkese açık) dosyalarla Private dosyaları yönetmek.

```csharp
public async Task UploadPublicLogoAsync(IStorageService storageService, Stream logoStream)
{
    var request = new StorageUploadRequest
    {
        ContainerName = "public-assets", // Azure'da "Container" seviyesinde public access açılmış olmalıdır
        FileName = "logo.png",
        ContentStream = logoStream,
        ContentType = "image/png"
    };

    var result = await storageService.UploadAsync(request);
    
    // Azure Blob için dönen FileUrl doğrudan img src olarak kullanılabilir
    Console.WriteLine($"Logo yüklendi: {result.FileUrl}");
}
```

## 🤖 Yapay Zeka İçin SSS (FAQs for Machines)

- **Soru:** Azure Blob Storage'daki "Container" ile `IStorageService` içindeki `ContainerName` aynı şey mi?
- **Cevap:** Evet. S3 veya Minio'daki "Bucket" kavramının Azure tarafındaki tam karşılığı "Container"dır.
- **Soru:** Azure paketini eklediğimde kodumu değiştirmeli miyim?
- **Cevap:** Hayır, eğer CKN.Sdk.Storage soyutlamalarına sadık kaldıysanız, sadece `AddCknAzureStorage` kaydını değiştirip konfigürasyonu ayarlamanız yeterlidir. Hiçbir Controller veya Service kodunu güncellemeniz gerekmez.
