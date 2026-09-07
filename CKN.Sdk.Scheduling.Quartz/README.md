# CKN.Sdk.Scheduling.Quartz

**Mühendislik Amacı (Engineering Intent):**
`CKN.Sdk.Scheduling.Quartz`, Java ekosisteminden port edilmiş, oldukça köklü, gelişmiş ve karmaşık zamanlama senaryolarını destekleyen Quartz.NET kütüphanesini projeye entegre eder. **Neden var?** Hangfire'ın sunmadığı çok gelişmiş takvimleme (iş günleri, tatiller hariç), karmaşık CRON ifadeleri, Cron-Trigger kombinasyonları ve "Stateful" (durumlu) görevler gibi spesifik gereksinimleri karşılamak için. **Ne zaman kullanılmalı?** "Her ayın 3. cuma günü saat 15:00'te çalışsın ama o gün resmi tatilse bir sonraki iş günü çalışsın" gibi çok gelişmiş bir takvim/zamanlama mimarisi gerektiren B2B veya ERP tarzı sistemlerde tercih edilmelidir.

## 🚀 Hızlı Başlangıç

### Kurulum

```bash
dotnet add package CKN.Sdk.Scheduling.Quartz
```

### Bağımlılık Enjeksiyonu (DI)

```csharp
using CKN.Sdk.Scheduling.Quartz;
using Quartz;

var builder = WebApplication.CreateBuilder(args);

// Quartz'ı .NET Hosted Service olarak arka planda çalışmak üzere kaydeder
builder.Services.AddCknQuartz(q =>
{
    // Örnek bir Job'u DI'a tanıtma
    var jobKey = new JobKey("EmailCampaignJob");
    q.AddJob<EmailCampaignJob>(opts => opts.WithIdentity(jobKey));
    
    // Bu Job için tetikleyici (Trigger) oluşturma (Örn: Her saat başı)
    q.AddTrigger(opts => opts
        .ForJob(jobKey)
        .WithIdentity("EmailCampaignJob-trigger")
        .WithCronSchedule("0 0 * * * ?")); // Cron expression
});

var app = builder.Build();
```

## 💡 Gerçek Hayat Senaryoları

### Senaryo 1: Gelişmiş IJob İmplementasyonu (Durumlu Görev)

Aynı anda sadece tek bir örneğinin çalışması gereken (Concurrency kontrolü) ve kendi içinde durum tutan (Stateful) görev tanımı.

```csharp
using Quartz;

// DisallowConcurrentExecution: Aynı job henüz bitmemişse, sıradaki trigger'ı bekletir.
[DisallowConcurrentExecution]
public class EmailCampaignJob(ILogger<EmailCampaignJob> logger) : IJob
{
    public async Task Execute(IJobExecutionContext context)
    {
        logger.LogInformation("E-posta kampanyası çalışıyor...");
        
        // JobDataMap üzerinden göreve özel parametrelere erişim
        var campaignId = context.JobDetail.JobDataMap.GetIntValue("CampaignId");
        
        await Task.Delay(5000); // Uzun süren işlem
        
        logger.LogInformation($"Kampanya {campaignId} tamamlandı.");
    }
}
```

### Senaryo 2: Dinamik Job Eklemek (Çalışma Zamanı)

Kod yazarken değil, kullanıcı arayüzünden yeni bir görev kurduğunda (Runtime) dinamik olarak Quartz'a görev ekleme (Varyasyon).

```csharp
public class SchedulerService(ISchedulerFactory schedulerFactory)
{
    public async Task ScheduleNewCampaignAsync(int campaignId, DateTime runAt)
    {
        var scheduler = await schedulerFactory.GetScheduler();

        var job = JobBuilder.Create<EmailCampaignJob>()
            .WithIdentity($"campaign-{campaignId}")
            .UsingJobData("CampaignId", campaignId)
            .Build();

        var trigger = TriggerBuilder.Create()
            .WithIdentity($"campaign-trigger-{campaignId}")
            .StartAt(runAt) // İstenen spesifik tarihte çalıştır
            .Build();

        await scheduler.ScheduleJob(job, trigger);
    }
}
```

## 🤖 Yapay Zeka İçin SSS (FAQs for Machines)

- **Soru:** Quartz, görevleri (Jobs) veritabanında (persistent) tutabilir mi?
- **Cevap:** Evet, `appsettings.json` ve DI yapılandırması üzerinden AdoJobStore (SQL Server, Postgres vb.) kullanılarak görevlerin ve tetikleyicilerin (triggers) veritabanında tutulması sağlanabilir.
- **Soru:** Neden Hangfire yerine Quartz seçeyim?
- **Cevap:** Yönetim paneline (Dashboard) ihtiyacınız yoksa ve "Sadece hafta içi çalış, tatilleri atla" gibi karmaşık Cron/Takvim kurallarına ihtiyacınız varsa Quartz rakipsizdir. Hangfire ise daha çok kullanım kolaylığı ve hazır arayüzü ile öne çıkar.
