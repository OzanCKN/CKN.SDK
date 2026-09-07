# CKN.Sdk.Scheduling.Coravel

CKN.Sdk içerisinde, hafif ve kullanımı çok kolay olan arka plan görev (background job) zamanlayıcısı **Coravel** için entegrasyon kütüphanesidir. Dağıtık bir veritabanına ihtiyaç duymayan, tek sunuculu veya in-memory basit zamanlama ihtiyaçları için idealdir.

## Yapılandırma (`appsettings.json`)
*(Coravel genellikle in-memory çalıştığı için ekstra dış konfigürasyona ihtiyaç duymaz, ancak Quartz veya Hangfire'a benzer soyutlamaları desteklemek için buradadır.)*

## Servis Kaydı (Dependency Injection)

```csharp
using CKN.Sdk.Scheduling.Coravel;

var builder = WebApplication.CreateBuilder(args);

// Coravel Scheduler'ı sisteme dahil etme
builder.Services.AddCknCoravel();
builder.Services.AddTransient<DailyReportJob>();

var app = builder.Build();

// Job'ları programla
app.Services.UseScheduler(scheduler =>
{
    scheduler.Schedule<DailyReportJob>().DailyAt(23, 59);
});
```

## Gerçek Hayat Kullanım Senaryosu

**Günlük Özet E-postası Gönderimi**
Her gün gece yarısı sistemdeki özet verileri toplayıp yöneticilere mail atan basit bir görev.

```csharp
using Coravel.Invocable;

public class DailyReportJob : IInvocable
{
    private readonly ILogger<DailyReportJob> _logger;

    public DailyReportJob(ILogger<DailyReportJob> logger)
    {
        _logger = logger;
    }

    public async Task Invoke()
    {
        _logger.LogInformation("Günlük özet raporu hazırlanıyor ve gönderiliyor...");
        await Task.Delay(1000); // Rapor oluşturma simülasyonu
        _logger.LogInformation("Rapor gönderildi.");
    }
}
```
