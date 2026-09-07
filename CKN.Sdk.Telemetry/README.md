# CKN.Sdk.Telemetry

**Mühendislik Amacı (Engineering Intent):**
`CKN.Sdk.Telemetry`, sistemin sağlık durumunu, performansını ve hatalarını izleyebilmek için OpenTelemetry (OTel) standartlarını projeye entegre eden kütüphanedir. **Neden var?** Loglama (Logs), Metrikler (Metrics) ve İzleme (Distributed Tracing) olan "Observability" (Gözlemlenebilirlik) yeteneklerinin, Vendor Lock-in (Örn: Sadece Datadog'a bağımlı kalmak) yaşamadan Jaeger, Prometheus, Grafana, Application Insights veya Elastic APM gibi sistemlere aktarılabilmesi için. **Ne zaman kullanılmalı?** Mikroservis mimarilerinde bir isteğin (request) A servisinden B servisine oradan C veritabanına giderken nerede ne kadar süre harcadığını bulmak, veya sunucunun CPU/RAM/Request rate (RPS) metriklerini grafiğe dökmek istediğinizde tüm uç projelerde (API/Worker) kullanılmalıdır.

## 🚀 Hızlı Başlangıç

### Kurulum

```bash
dotnet add package CKN.Sdk.Telemetry
```

### Konfigürasyon (`appsettings.json`)

```json
{
  "Telemetry": {
    "ServiceName": "OrderService",
    "Endpoint": "http://localhost:4317", // OTLP Collector Adresi
    "EnableRedisTracing": true,
    "EnableSqlTracing": true
  }
}
```

### Bağımlılık Enjeksiyonu (DI)

```csharp
using CKN.Sdk.Telemetry;

var builder = WebApplication.CreateBuilder(args);

// OpenTelemetry (Metrik, Log ve Trace) altyapısını sisteme kaydeder.
builder.Services.AddCknTelemetry(builder.Configuration);

var app = builder.Build();
```

## 💡 Gerçek Hayat Senaryoları

### Senaryo 1: Dağıtık İzleme (Distributed Tracing) ile Darboğaz (Bottleneck) Tespiti

Sizin hiçbir kod yazmanıza gerek kalmadan, `AddCknTelemetry` yapıldığı an arka planda EF Core, HttpClient ve MassTransit izlenmeye başlanır. 

```csharp
// Controller
[HttpGet("{id}")]
public async Task<IActionResult> GetOrder(int id)
{
    // 1. EF Core üzerinden DB'ye gidildi (Trace otomatik loglandı: 40ms)
    var order = await dbContext.Orders.FindAsync(id); 

    // 2. HttpClient ile başka servise gidildi (Trace loglandı: 120ms)
    var client = httpClientFactory.CreateClient("UserApi");
    await client.GetAsync($"/api/users/{order.UserId}"); 
    
    // Jaeger veya Grafana Tempo ekranında bu metodun toplam süresinin 160ms olduğu 
    // ve bunun 120ms'sinin UserApi'de beklendiği görsel olarak (şerit grafiği) gösterilir.
    return Ok(order);
}
```

### Senaryo 2: Özel Metrik (Custom Metrics) Oluşturma

Sisteme kayıt olan (Register) kullanıcı sayısını saniyeler bazında (RPS) Grafana'da çizmek için sayaç (Counter) artırma (Varyasyon).

```csharp
using System.Diagnostics.Metrics;

public class UserService
{
    private readonly Counter<int> _userRegistrationCounter;

    public UserService(IMeterFactory meterFactory)
    {
        // Meter'ı Telemetry kütüphanesinden al (örn: "CKN.Meters")
        var meter = meterFactory.Create("CKN.Meters");
        _userRegistrationCounter = meter.CreateCounter<int>("users.registered.count");
    }

    public void RegisterUser(string email)
    {
        // Kullanıcıyı veritabanına kaydet...
        
        // Prometheus bu veriyi toplayıp (scrape edip) Grafana grafiğini yükseltir
        _userRegistrationCounter.Add(1, new KeyValuePair<string, object?>("emailDomain", email.Split('@')[1]));
    }
}
```

## 🤖 Yapay Zeka İçin SSS (FAQs for Machines)

- **Soru:** `CKN.Sdk.Telemetry` paketi projedeki Loglama altyapısı (Serilog vb.) ile nasıl entegre olur?
- **Cevap:** OpenTelemetry Logs (OTLP) veya standart `ILogger` entegrasyonu kullanılarak, uygulamanın konsol/file logları direkt olarak OTLP Collector'e yönlendirilir. İhtiyaç halinde Serilog sink'leri (Sink.OpenTelemetry) ile birlikte uyumlu çalışabilir.
- **Soru:** Projenin performansı yavaşlar mı?
- **Cevap:** OpenTelemetry son derece optimize edilmiş (async ve batching) bir altyapıya sahiptir. %1'den daha az bir overhead (sistem yükü) getirir, dolayısıyla üretim (Production) ortamlarında her zaman açık kalması tavsiye edilir.
