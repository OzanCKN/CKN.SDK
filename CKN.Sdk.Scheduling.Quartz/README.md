# CKN.Sdk.Scheduling.Quartz

Özellikle saniye bazlı veya karmaşık Cron ifadelerine ihtiyaç duyan enterprise zamanlanmış görev (Cron Job) kütüphanesi olan **Quartz.NET** entegrasyonudur. Cluster (kümeleme) desteği sayesinde dağıtık mimaride bir job'ın aynı anda iki sunucuda çalışmasını engeller.

## Yapılandırma (`appsettings.json`)

```json
{
  "Scheduling": {
    "Quartz": {
      "IsClustered": true,
      "ConnectionString": "Server=localhost;Database=QuartzDb;Integrated Security=true;"
    }
  }
}
```

## Servis Kaydı (Dependency Injection)

```csharp
using CKN.Sdk.Scheduling.Quartz;

var builder = WebApplication.CreateBuilder(args);

// Quartz'ı sisteme dahil etme
builder.Services.AddCknQuartz(builder.Configuration);

var app = builder.Build();
```

## Gerçek Hayat Kullanım Senaryosu

**Aylık Fatura Kesimi (Her ayın 1'i saat 00:00)**
Bütün abonelerin faturalarını toplu halde kesen ve kesinlikle sadece 1 kez (Cluster mode) çalışması gereken görev.

```csharp
using Quartz;

// Job Sınıfı
[DisallowConcurrentExecution] // Aynı job bitmeden yenisi başlamasın
public class MonthlyInvoiceJob : IJob
{
    public async Task Execute(IJobExecutionContext context)
    {
        Console.WriteLine("Aylık faturalar kesiliyor...");
        // Fatura kesim işlemleri
    }
}

// Program.cs içerisindeki ayar (Trigger)
builder.Services.AddQuartz(q =>
{
    var jobKey = new JobKey("MonthlyInvoiceJob");
    q.AddJob<MonthlyInvoiceJob>(opts => opts.WithIdentity(jobKey));

    q.AddTrigger(opts => opts
        .ForJob(jobKey)
        .WithIdentity("MonthlyInvoiceTrigger")
        .WithCronSchedule("0 0 0 1 * ?")); // Her ayın 1'i Gece Yarısı
});
```
