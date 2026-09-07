# CKN.Sdk.Scheduling.Hangfire

Dağıtık ve kalıcı (persistent) arka plan işleri için endüstri standardı olan **Hangfire** entegrasyon kütüphanesidir. İşlerin durumunu takip edebileceğiniz bir arayüzü (Dashboard) vardır ve işler sunucu çökse bile yeniden başlar.

## Yapılandırma (`appsettings.json`)

```json
{
  "Scheduling": {
    "Hangfire": {
      "ConnectionString": "Server=localhost;Database=HangfireDb;Integrated Security=true;",
      "DashboardPath": "/hangfire"
    }
  }
}
```

## Servis Kaydı (Dependency Injection)

```csharp
using CKN.Sdk.Scheduling.Hangfire;

var builder = WebApplication.CreateBuilder(args);

// Hangfire'ı sisteme SQL Server kalıcılığı ile dahil etme
builder.Services.AddCknHangfire(builder.Configuration);

var app = builder.Build();

// Hangfire Dashboard (İzleme arayüzü) aktif edilir
app.UseCknHangfireDashboard();
```

## Gerçek Hayat Kullanım Senaryosu

**Video İşleme veya Uzun Süren Raporlar**
Kullanıcının sisteme yüklediği videonun asenkron olarak arka planda sıkıştırılması ve tamamlandığında bildirim atılması. Bu işlem saatler sürebilir ve Hangfire bunu güvenle yönetir.

```csharp
using Hangfire;

public class VideoUploadController : ControllerBase
{
    private readonly IBackgroundJobClient _backgroundJobClient;

    public VideoUploadController(IBackgroundJobClient backgroundJobClient)
    {
        _backgroundJobClient = backgroundJobClient;
    }

    [HttpPost]
    public IActionResult UploadVideo(IFormFile file)
    {
        // Dosyayı diske/S3'e kaydet (Örn: fileId = 123)
        var fileId = "123";

        // Arka plan işine (Fire-and-forget) gönder. Controller anında 200 döner.
        _backgroundJobClient.Enqueue<VideoProcessingService>(x => x.ProcessVideoAsync(fileId));

        return Ok(new { Message = "Video işleme kuyruğuna alındı.", JobId = fileId });
    }
}

public class VideoProcessingService
{
    public async Task ProcessVideoAsync(string fileId)
    {
        // Uzun süren video işleme algoritması (FFMPEG vb.)
    }
}
```
