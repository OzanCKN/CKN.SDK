# CKN.Sdk.Telemetry

CKN.Sdk içerisinde **OpenTelemetry (Gözlemlenebilirlik / Observability)** yeteneklerini sunan tamamen bağımsız ve modüler kütüphanedir. 
Mikroservis mimarisindeki Trace (İz), Metric (Ölçüm) ve Logların Jaeger, Prometheus veya Datadog (OTLP üzerinden) gibi sistemlere aktarılmasını sağlar.

*Eğer projenizde izleme veya metrik toplamaya ihtiyaç yoksa bu paketi kurmanıza gerek yoktur, böylece uygulamanızın bağımlılıkları temiz kalır.*

## Yapılandırma (`appsettings.json`)

```json
{
  "Telemetry": {
    "ServiceName": "MyMicroservice",
    "OtlpEndpoint": "http://localhost:4317"
  }
}
```

## Servis Kaydı (Dependency Injection)

```csharp
using CKN.Sdk.Telemetry;

var builder = WebApplication.CreateBuilder(args);

var serviceName = builder.Configuration["Telemetry:ServiceName"] ?? "DefaultService";
var otlpEndpoint = builder.Configuration["Telemetry:OtlpEndpoint"];

// OpenTelemetry (Metrikler ve İzleme) ekleme
// Jaeger, Prometheus vb. platformlara veri gönderimini (OTLP) aktifleştirir.
// Arka planda HttpClient, Entity Framework ve ASP.NET Core için otomatik enstrümantasyon yapar.
builder.Services.AddCKNTelemetry(serviceName: serviceName, otlpEndpoint: otlpEndpoint);

var app = builder.Build();
```

## Gerçek Hayat Kullanım Senaryosu

**Mikroservisler Arası Dağıtık İzleme (Distributed Tracing)**
Bir e-ticaret uygulamasında "Sipariş Oluştur" isteğinin API Gateway'den geçip, Order Service'e ulaşması, oradan RabbitMQ'ya mesaj atılması ve Payment Service'de sonlanması sürecindeki tüm sürenin (Gecikmeler, Hatalar) **Jaeger** gibi bir arayüzde şelale (Waterfall) şeklinde görselleştirilmesi.

Bu paket sisteme eklendiğinde EF Core, HTTP Client ve Web request'leri için kod değiştirmeden (Zero-Code Instrumentation) metrikler toplanmaya başlar.
