# Scheduling (Zamanlanmış Görev) Sağlayıcıları Kullanım Örnekleri

Zamanlanmış (Cron tabanlı) arka plan görevleri için CKN.SDK 3 popüler alternatifi (Hangfire, Quartz ve Coravel) kapsar. Hepsi `IHostedService` altyapısında çalışır veya kendi worker yapılarına entegre olur.

---

## 1. Hangfire Kullanımı

Hangfire, çok gelişmiş ve Dashboard'u (Arayüzü) olan en popüler Scheduling çözümüdür. Hata yönetimi, Retry mekanizmaları ve Job takibi sağlar.

### `appsettings.json` Yapılandırması
```json
{
  "Scheduling": {
    "Hangfire": {
      "ConnectionString": "Server=localhost;Database=HangfireDb;User Id=sa;Password=your_password;"
    }
  }
}
```

### Dependency Injection (DI) Kurulumu
```csharp
using CKN.Sdk.Scheduling.Hangfire;
using Hangfire;

// Servisi sisteme dahil et
builder.Services.AddCknHangfire(config =>
{
    config.UseSqlServerStorage(builder.Configuration.GetConnectionString("HangfireConnection"));
});

var app = builder.Build();

// Hangfire Dashboard (Arayüz) eklemek isterseniz:
app.UseHangfireDashboard("/jobs");
```

### Gerçek Hayat Kullanımı: Her Gece Ödenmemiş Faturaları Tespit Etmek (Cron Job)
```csharp
public class InvoiceJob
{
    public void CheckUnpaidInvoices()
    {
        Console.WriteLine("Ödenmeyen faturalar kontrol ediliyor...");
        // Veritabanı işlemleri...
    }
}

// Uygulama başlatılırken Recurring Job (Tekrarlı Görev) tanımlama
var recurringJobManager = app.Services.GetRequiredService<IRecurringJobManager>();

// Her gece saat 00:00'da çalışır (Cron ifadesi: "0 0 * * *")
recurringJobManager.AddOrUpdate<InvoiceJob>(
    "check-unpaid-invoices",
    job => job.CheckUnpaidInvoices(),
    "0 0 * * *",
    new RecurringJobOptions { TimeZone = TimeZoneInfo.Local }
);
```

---

## 2. Quartz.NET Kullanımı

Daha enterprise seviyede, In-Memory veya Cluster (Dağıtık) senaryolar için esnekliğiyle öne çıkan, Java ekosisteminden portlanmış çok güçlü bir zamanlayıcıdır.

### `appsettings.json` Yapılandırması
```json
{
  "Scheduling": {
    "Quartz": {
      "ConnectionString": "Server=localhost;Database=QuartzDb;...",
      "UseInMemoryStore": true // Eğer veritabanı olmadan RAM üzerinde tutmak isterseniz
    }
  }
}
```

### Dependency Injection (DI) Kurulumu
```csharp
using CKN.Sdk.Scheduling.Quartz;
using Quartz;

builder.Services.AddCknQuartz(q =>
{
    // Quartz native ayarlamaları burada yapılır
    // q.UsePersistentStore(s => ...);
});
```

### Gerçek Hayat Kullanımı: Saatlik Sepet Terk Edilme (Cart Abandonment) Uyarıları
```csharp
// 1. Görevi (Job) Tanımla
public class AbandonedCartJob : IJob
{
    public Task Execute(IJobExecutionContext context)
    {
        Console.WriteLine("Kullanıcılara sepetlerinde unuttukları ürünler için SMS atılıyor...");
        return Task.CompletedTask;
    }
}

// 2. DI içerisinde Job'ı planla (Trigger)
builder.Services.AddQuartz(q =>
{
    var jobKey = new JobKey("AbandonedCartJob");
    
    q.AddJob<AbandonedCartJob>(opts => opts.WithIdentity(jobKey));
    
    q.AddTrigger(opts => opts
        .ForJob(jobKey)
        .WithIdentity("AbandonedCartJob-trigger")
        .WithCronSchedule("0 0 * ? * *") // Her saat başı çalış
    );
});
```

---

## 3. Coravel Kullanımı

Coravel, "Sıfır Yapılandırma (Zero Config)" mottosuyla Laravel (PHP) ekosistemine benzeyen çok temiz ve okunaklı bir kod dizilimi sunar. Küçük ve orta ölçekli projeler için veritabanına ihtiyaç duymadan RAM üzerinde çalışır.

### Dependency Injection (DI) Kurulumu
```csharp
using CKN.Sdk.Scheduling.Coravel;
using Coravel;

// Herhangi bir options belirtmeden kurulum yapılabilir
builder.Services.AddCknCoravel();
```

### Gerçek Hayat Kullanımı: Basit Günlük Rapor Gönderimi
```csharp
using Coravel.Invocable;

// İş tanımı
public class SendDailyReportJob : IInvocable
{
    public Task Invoke()
    {
        Console.WriteLine("Günlük satış raporları yöneticilere email ile gönderiliyor...");
        return Task.CompletedTask;
    }
}

// Servislere ekle
builder.Services.AddTransient<SendDailyReportJob>();

var app = builder.Build();

// Kolay okunabilir Fluent API planlaması
app.Services.UseScheduler(scheduler =>
{
    scheduler
        .Schedule<SendDailyReportJob>()
        .DailyAt(17, 30); // Her gün 17:30'da çalış
});
```
