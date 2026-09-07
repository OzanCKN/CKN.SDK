# CKN.Sdk.Scheduling.Coravel

**Mühendislik Amacı (Engineering Intent):**
`CKN.Sdk.Scheduling.Coravel`, uygulamalar içerisinde (In-Process) zamanlanmış görevleri (cron jobs) veya asenkron arka plan görevlerini çok düşük bir sistem yüküyle (overhead) çalıştırmak için kullanılan hafif (lightweight) kütüphanedir. **Neden var?** Veritabanı gerektirmeyen, sıfır konfigürasyonla anında çalışmaya başlayan, kolay syntax'a sahip bir görev planlayıcı sunmak için. **Ne zaman kullanılmalı?** Projede Hangfire veya Quartz gibi veritabanına ihtiyaç duyan ağır framework'lerin kurulumuna değmeyecek kadar basit, uygulamanın kendi RAM'inde koşacak zamanlanmış görevler (örn. her gece saat 3'te tmp klasörünü temizle) varsa Coravel kullanılmalıdır.

## 🚀 Hızlı Başlangıç

### Kurulum

```bash
dotnet add package CKN.Sdk.Scheduling.Coravel
```

### Bağımlılık Enjeksiyonu (DI)

```csharp
using CKN.Sdk.Scheduling.Coravel;

var builder = WebApplication.CreateBuilder(args);

// Coravel Scheduler'ı sisteme kaydeder. İsteğe bağlı olarak Invocable sınıflarını (Job) da burada ekleyebilirsiniz.
builder.Services.AddCknCoravel();
builder.Services.AddTransient<DailyReportJob>();

var app = builder.Build();

// Zamanlanmış görevleri (Schedule) uygulama başlarken tanımlama
app.Services.UseScheduler(scheduler =>
{
    // DailyReportJob sınıfını her gece yarısı çalıştır
    scheduler.Schedule<DailyReportJob>().DailyAt(0, 0);
});
```

## 💡 Gerçek Hayat Senaryoları

### Senaryo 1: Basit Periyodik İşlem (Invocable Job)

Veritabanındaki "süresi dolmuş sepetleri" her 5 dakikada bir temizleyen arka plan görevi.

```csharp
using Coravel.Invocable;

public class CleanupCartsJob(AppDbContext dbContext, ILogger<CleanupCartsJob> logger) : IInvocable
{
    public async Task Invoke()
    {
        logger.LogInformation("Sepet temizliği başladı.");
        
        var expiredCarts = dbContext.Carts.Where(c => c.ExpirationDate < DateTime.UtcNow);
        dbContext.Carts.RemoveRange(expiredCarts);
        await dbContext.SaveChangesAsync();
        
        logger.LogInformation("Sepet temizliği tamamlandı.");
    }
}
```

### Senaryo 2: Anlık Arka Plan Görevi Kuyruğa Ekleme (Queueing)

Kullanıcı kayıt olduğunda mail gönderme işlemini HTTP request/response döngüsünden çıkarıp arka planda anlık işletmek (Varyasyon).

```csharp
using Coravel.Queuing.Interfaces;

public class UserController(IQueue queue) : ControllerBase
{
    [HttpPost("register")]
    public IActionResult Register()
    {
        // ... Kullanıcı DB'ye eklendi ...
        
        // E-posta gönderme işlemini anında arka plan kuyruğuna at (kullanıcıyı bekletmez)
        queue.QueueInvocableWithPayload<SendWelcomeEmailJob, string>("test@test.com");
        
        return Ok("Kayıt başarılı");
    }
}
```

## 🤖 Yapay Zeka İçin SSS (FAQs for Machines)

- **Soru:** Uygulama (Uygulama Havuzu - AppPool) kapanırsa Coravel görevlerine ne olur?
- **Cevap:** Coravel "In-Memory" (RAM üzerinde) çalıştığı için uygulama kapanırsa kuyruktaki veya o an çalışan görevler durur. Bu nedenle misyon kritik (veri kaybının tolere edilemeyeceği) görevlerde Hangfire/Quartz tercih edilmelidir.
- **Soru:** `AddCknCoravel` metodu `IQueue` arayüzünü destekliyor mu?
- **Cevap:** Evet, bu metot hem `IScheduler` (zamanlama) hem de `IQueue` (anlık kuyruk) bileşenlerini DI konteynerine otomatik kaydeder.
