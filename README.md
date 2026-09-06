<div align="center">
  <h1>🚀 CKN.SDK (Enterprise Agentic Orchestra)</h1>
  <p><b>2026 Standartlarında, Sıfır Bağımlılık (Zero-Dependency) Hedefli, Plugin Tabanlı Enterprise .NET Framework'ü</b></p>
  
  [![Build Status](https://github.com/OzanCKN/CKN.SDK/actions/workflows/ci.yml/badge.svg)](https://github.com/OzanCKN/CKN.SDK/actions)
  [![NuGet Version](https://img.shields.io/badge/nuget-v1.0.0--preview-blue.svg)](https://www.nuget.org/)
  [![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)
  [![Native AOT](https://img.shields.io/badge/Native_AOT-Ready-success.svg)]()
</div>

---

## 🌟 Vizyonumuz: Provider-Agnostic Mimari
Geleneksel SDK'lar belirli teknolojilere (ör. MassTransit, EF Core, Redis) sıkı sıkıya bağlıdır. **CKN.SDK** ise yeni nesil **Multi-Provider (Plugin Tabanlı)** bir mimari sunar. Altyapı kodunuz tamamen soyutlanır; RabbitMQ'dan Kafka'ya veya EF Core'dan Dapper'a geçmek sadece tek bir satır kod değiştirerek mümkün olur!

### 🔥 Geliştirici Deneyimi (DX) Harikası
```csharp
// Program.cs
var builder = WebApplication.CreateBuilder(args);

// Modülleri tek satırla tak-çalıştır yöntemiyle ekleyin!
builder.Services.AddCknMessaging(msg => 
{
    msg.UseRabbitMQ("amqp://localhost"); 
    // veya msg.UseKafka("localhost:9092");
});

builder.Services.AddCknData(data => 
{
    data.UseEntityFramework(o => o.UseSqlServer("..."));
    // veya data.UseDapper();
});

builder.Services.AddCknCaching(cache => cache.UseGarnet());
```

---

## 📦 Envanter ve Modüller (Roadmap)

Aşağıdaki tablo, SDK'nın sunduğu ve sunmayı planladığı (30 Maddelik Genişleme Planı) altyapı modüllerini göstermektedir. Projenize sadece ihtiyacınız olan paketi kurarsınız (Örn: `dotnet add package CKN.Sdk.Messaging`).

| Modül Paketi | Açıklama | Desteklenen Sağlayıcılar (Providers) | Durum |
| :--- | :--- | :--- | :--- |
| **`CKN.Sdk.Core`** | Sistemin kalbi. Tüm arayüzler ve CQRS. | *Sıfır Bağımlılık (Zero Dependency)* | 🟢 Hazır |
| **`CKN.Sdk.Messaging`** | Olay güdümlü EventBus mimarisi. | RabbitMQ, Kafka, Azure Service Bus | 🟡 Yapım Aşamasında |
| **`CKN.Sdk.Data`** | Veritabanı ve ORM soyutlaması. | EF Core, Dapper, RepoDB | 🟡 Yapım Aşamasında |
| **`CKN.Sdk.Caching`** | L1/L2 Hybrid Önbellekleme. | Redis, Microsoft Garnet, Memcached | 🟡 Yapım Aşamasında |
| **`CKN.Sdk.AI`** | LLM entegrasyonları ve Ajan altyapısı. | OpenAI, Anthropic, Ollama | 🟡 Yapım Aşamasında |
| **`CKN.Sdk.Infrastructure`** | Güvenlik (JWT) ve Resilience (Polly). | Polly v8, JWT, Feature Flags | 🟢 Hazır |
| **`CKN.Sdk.Observability`** | Loglama, İzleme ve Metrik. | Elasticsearch, Datadog, Prometheus | 🟢 Hazır (Kısmi) |

> 📚 *Her bir modülün detaylı ve kopyala-yapıştır yapabileceğiniz kullanım örnekleri için modülün kendi klasöründeki `README.md` dosyalarına bakabilirsiniz.*

---

## 🧪 Test ve Güvenilirlik (TDD)
CKN.SDK kod tabanında **%100 Test Zorunluluğu (TDD)** uygulanmaktadır. Her yeni provider, xUnit ile test edilir, Moq ve FluentAssertions ile doğrulanır. GitHub Actions (CI/CD) tüm süreçleri otomatik denetler. 
*(SonarQube 0 Bug / 0 Warning politikası uygulanmaktadır).*

---

## 🏗️ Mimari Yönetişim (AI Governance)
Bu projenin nasıl tasarlandığını merak ediyorsanız veya projeye yapay zeka ajanları ile katkı sağlamak istiyorsanız [Governance](./governance) klasörünü inceleyiniz.
- 🗺️ [Sistem Sözlüğü ve Kod Haritası](./governance/docs/architecture/system-glossary.md)
- 📜 [Mimari Karar Defteri (ADR)](./governance/docs/architecture/decision-log.md)
- 📋 [Aktif Sprint ve Görevler](./governance/sprints/index.md)

---
<div align="center">
  <i>2026 © CKN Enterprise Architecture Team</i>
</div>
