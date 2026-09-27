# Sprint 6: Finance SDK Gap Closure

> **AI AGENT İÇİN ZORUNLU BİLDİRİM:** Bu sprint planını incelerken **KESİNLİKLE `.agents/rules.md`** dosyasındaki kuralları gözetmek zorundasın. Mimari değişiklikler `decision-log.md`'ye, yeni modüller `system-glossary.md`'ye kaydedilmeli.

## 🎯 Sprint Hedefi

CKN.Finance projesinde üç kritik eksiklik nedeniyle SDK sarmalayıcısı kuralı çiğnenmek zorunda kalındı (Finance Decision 13). Bu sprint, o boşlukları kapatarak Finance'in `CKN.Sdk.*` paketleri üzerinden tüm altyapıyı kullanmasını sağlar.

İlk eksiklik, HTTP istemci altyapısıdır: Finance'te üç dış fiyat sağlayıcısı ham `HttpClient` ile yazılmış, her birinde elle yazılmış rate limiting (`Task.Delay`), retry ve maskeleme mantığı bulunmaktadır. İkinci eksiklik, piyasa takvimi ve portföy matematiksel hesaplamalarının handler'lara dağılmış olmasıdır. Üçüncüsü `CKN.Sdk.AI.OpenAI` paketindeki NU1608 bağımlılık uyarısı ve eksik XML dokümantasyonudur.

## 📊 Kapsam ve İsterler

- `CKN.Sdk.Network` — HTTP istemci soyutlaması (ICknHttpClient, CknHttpClientOptions, auth stratejileri)
- `CKN.Sdk.Network.Http` — Raw HttpClient provider (Microsoft.Extensions.Http.Resilience tabanlı)
- `CKN.Sdk.AI` XML docs + `CKN.Sdk.AI.OpenAI` OutputType bug fix + NU1608 çözümü
- `CKN.Sdk.Financial` — Piyasa takvimi (BIST/NYSE/NASDAQ/Crypto), OHLC aggregation, portföy matematiği

## 🛠 Teknik Mimari Kararlar

- Network paketi provider-agnostic: `CKN.Sdk.Network` (abstraction) + `CKN.Sdk.Network.Http` (Faz 1). Flurl/RestSharp/Refit provider'ları Faz 2'ye bırakıldı.
- Sıfır yeni 3rd-party bağımlılık: `Microsoft.Extensions.Http.Resilience` zaten `Directory.Packages.props`'ta, `System.Threading.RateLimiting` .NET 7+ built-in.
- `CKN.Sdk.Financial` için NodaTime yerine .NET 6+ built-in IANA timezone desteği (`TimeZoneInfo`) tercih edildi.
- Decision-log kayıtları yapıldı (ADR-009, ADR-010).

## 📝 Task Listesi

- [x] **Task 007:** CKN.Sdk.Network + CKN.Sdk.Network.Http Geliştirimi (`tasks/task-007-network-http-client.md`)
- [x] **Task 008:** CKN.Sdk.AI Bug Fix + XML Docs + NU1608 (`tasks/task-008-ai-dependency-fix.md`)
- [ ] **Task 009:** CKN.Sdk.Financial — Piyasa Takvimi + Portföy Matematiği (`tasks/task-009-financial.md`)

## ⚠️ Riskler ve Darboğazlar

- `Microsoft.Extensions.AI.OpenAI` sürüm yükseltmesi `IChatClient` API değişikliği içeriyorsa breaking change riski; test ile doğrula.
- `CKN.Sdk.Financial` tatil verileri static olarak gömüldüğü için yıllık güncelleme gerektirir — `IHolidayProvider` interface'i extensibility sağlar.
