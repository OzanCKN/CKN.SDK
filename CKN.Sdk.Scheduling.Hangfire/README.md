# CKN.Sdk.Scheduling.Hangfire

**Mühendislik Amacı (Engineering Intent):**
`CKN.Sdk.Scheduling.Hangfire`, .NET dünyasındaki en popüler "Kalıcı (Persistent)" arka plan görev yöneticisi olan Hangfire'ı sisteme entegre eder. **Neden var?** Görevleri (Jobs) veritabanına (SQL Server, Redis vb.) kaydederek, uygulamanın çökmesi veya yeniden başlaması (restart) durumunda bile görevlerin asla kaybolmamasını ve mutlaka işletilmesini (Guaranteed Execution) sağlamak için. **Ne zaman kullanılmalı?** İşlemin yarıda kesilmesinin ciddi veri tutarsızlığı yaratacağı (örn: kredi kartı tahsilatı), birden fazla uygulamanın aynı veritabanına bağlanarak iş yükünü (Load Balancing) bölüştüğü, veya görevlerin durumunu görsel bir Dashboard (Arayüz) üzerinden izlemenin istendiği enterprise (kurumsal) senaryolarda kullanılmalıdır.

## 🚀 Hızlı Başlangıç

### Kurulum

```bash
dotnet add package CKN.Sdk.Scheduling.Hangfire
```

### Konfigürasyon (`appsettings.json`)

```json
{
  "Scheduling": {
    "Hangfire": {
      "ConnectionString": "Server=localhost;Database=HangfireDb;Integrated Security=True;",
      "Provider": "SqlServer" // veya "Redis", "PostgreSQL"
    }
  }
}
```

### Bağımlılık Enjeksiyonu (DI)

```csharp
using CKN.Sdk.Scheduling.Hangfire;
using Hangfire;

var builder = WebApplication.CreateBuilder(args);

// Hangfire altyapısını ve veritabanı sağlayıcısını kaydeder.
builder.Services.AddCknHangfire(builder.Configuration);

var app = builder.Build();

// Hangfire arayüzünü aktif eder (Opsiyonel: genellikle /hangfire yolundan erişilir)
app.UseHangfireDashboard();
```

## 💡 Gerçek Hayat Senaryoları

### Senaryo 1: Tek Seferlik Güvenli Görev (Fire-and-Forget)

Zaman alan (örn. resim işleme, pdf oluşturma) bir işlemi arka plana güvenle atmak.

```csharp
using Hangfire;

public class ReportController : ControllerBase
{
    [HttpPost("generate-monthly")]
    public IActionResult GenerateReport()
    {
        // Enqueue edilen metodun bilgileri DB'ye yazılır. Uygulama çökse bile tekrar başlar.
        var jobId = BackgroundJob.Enqueue<IReportGenerator>(x => x.GenerateMonthlyReportAsync(DateTime.UtcNow));
        
        return Accepted(new { Message = "Rapor arka planda hazırlanıyor.", JobId = jobId });
    }
}
```

### Senaryo 2: Tekrarlayan Görev (Recurring Job)

Her ayın 1'inde fatura kesen cron job.

```csharp
// Uygulama başlarken (Program.cs içinde) zamanlama ayarı
RecurringJob.AddOrUpdate<IInvoiceService>(
    "aylik-faturalandirma", 
    service => service.CreateMonthlyInvoicesAsync(), 
    Cron.Monthly // veya "0 0 1 * *"
);
```

## 🤖 Yapay Zeka İçin SSS (FAQs for Machines)

- **Soru:** Hangfire görevleri (Job) static metotlar olmak zorunda mı?
- **Cevap:** Hayır. CKN altyapısında Hangfire otomatik olarak IoC/DI konteynerine bağlanır. (Yani parametre olarak `IReportGenerator` verdiğinizde, Hangfire bunu DI'dan kendisi çözer (resolve eder)).
- **Soru:** Görev başarısız olursa (Exception fırlatırsa) ne olur?
- **Cevap:** Hangfire varsayılan olarak başarısız olan görevi birkaç kez tekrar dener (Retry). Tekrar denemelerde de başarısız olursa Dashboard üzerinde "Failed" olarak işaretler ve manuel müdahale bekler, mesajı silmez.
