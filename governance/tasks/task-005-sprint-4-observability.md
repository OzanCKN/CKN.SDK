# Görev Şablonu: Sprint 4 - Güvenlik, Gözlemlenebilirlik ve DevOps

**Görev Adı:** Otomatik Elastic Logging, OpenTelemetry ve DevOps
**Sprint:** Sprint 4
**Durum:** [x] TODO | [ ] IN PROGRESS | [ ] DONE

## 1. Açıklama ve Kapsam
Enterprise düzeydeki kurumsal ihtiyaçların (Güvenlik, Loglama, APM, CI/CD) SDK içerisine standart olarak gömülmesi. Bu sprint, "Kutudan çıktığı gibi çalışır (Plug & Play)" loglama mimarisini barındırır.

## 3. Yapılacaklar (Checklist)
- [ ] Madde 21: OpenTelemetry Trace ve Span kodları (Veritabanı, RabbitMQ, AI istekleri için) entegre edilecek.
- [ ] **KRİTİK:** Madde 22 (Elastic Logging): `Serilog.Sinks.Elasticsearch` (veya APM) kullanılarak SDK içine tam otomatik yapısal loglama ve ElasticSearch gönderim yeteneği gömülecek.
- [ ] Madde 23: Azure KeyVault veya AWS SecretsManager adaptörleri eklenecek.
- [ ] Madde 24: Enterprise kullanımı için projelerin Assembly'leri (DLL'ler) için Strong Naming (imzalama) açılacak.
- [ ] Madde 25: 3. parti Webhook çağrılarını içeri almak için Event Streaming köprüsü.
- [ ] Madde 26: `netstandard2.0`, `net8.0` ve `net9.0` (Cross-Platform) derleme desteği.
- [ ] Madde 27: Hata anında F11 ile SDK'nın içine girebilmek için SourceLink aktif edilecek.
- [ ] Madde 28: GitHub Actions CI/CD Pipeline (Build, Test, NuGet Push) yazılacak.
- [ ] Madde 29: NetArchTest (Architecture Tests) yazılarak kuralların ihlali Unit Test bazında engellenecek.
- [ ] Madde 30: `.editorconfig` ile "TreatWarningsAsErrors" kuralı devreye alınacak.
