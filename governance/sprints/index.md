# Sprints (Sprintler) İndeksi

## 📌 Amaç
Bu dizin, projenin tüm çevik geliştirme (agile) döngülerinin, geçmiş ve aktif sprint süreçlerinin ana yönetim (tracking) belgesi olarak oluşturulmuştur.

## 📂 İçerik
Yeni bir özellik veya güncelleme yapılmadan önce, ilgili aktif Sprint içerisine bir Task dosyası açılmalı ve buraya referansı eklenmelidir.

### 🏃 Aktif Sprint
- **[Sprint 1 - Foundation & AI Integration](#sprint-1)**

### Tüm Sprintler

#### Sprint 1
**Hedef:** Proje altyapısının kurulması, kurumsal kimlik entegrasyonu (CKN isimlendirmeleri) ve AI Yönetişim altyapısının oluşturulması.

**Görevler:**
- [x] Projedeki EnAI isimlendirmelerinin CKN olarak güncellenmesi.
- [x] AI Governance yapısının kurulması (`governance/` klasörleri).
- [x] [task-001-implement-redis-cache.md](../tasks/task-001-implement-redis-cache.md) : Redis Önbellek (Cache) Entegrasyonu
- [x] [task-002-sdk-documentation-and-governance.md](../tasks/task-002-sdk-documentation-and-governance.md) : SDK Dokümantasyonları ve Glossary Güncellemesi

#### Sprint 2: Temel Mimari ve Kullanıcı Deneyimi (DX)
**Hedef:** SDK'nın dışarıdan tüketimi (DI, Fluent API, Exception handling) ve AOT Native Source Generator entegrasyonu.
**Durum:** TODO
**Görevler:**
- [ ] [task-003-sprint-2-dx.md](../tasks/task-003-sprint-2-dx.md) : DI, Options Pattern, Exceptions ve AOT Source Generator geliştirimi.

#### Sprint 3: Dayanıklılık, Performans ve Ölçeklenebilirlik
**Hedef:** Polly, Caching, Pagination, Circuit Breaker ve Health Checks entegrasyonu.
**Durum:** DONE
**Görevler:**
- [x] [task-004-sprint-3-resilience.md](../tasks/task-004-sprint-3-resilience.md) : Resilience, Circuit Breaker ve Memory Optimization.

#### Sprint 4: Güvenlik, Gözlemlenebilirlik ve DevOps
**Hedef:** Otomatik Elastic Logging, OpenTelemetry, Güçlü İsimlendirme, CI/CD ve NetArchTest mimari testleri.
**Durum:** DONE
**Görevler:**
- [x] [task-005-sprint-4-observability.md](../tasks/task-005-sprint-4-observability.md) : Elastic Logging, OpenTelemetry ve DevOps.

#### Sprint 5: Provider-Agnostic Architecture Phase 1
**Hedef:** Farklı AI Provider'ları (OpenAI, Anthropic, Gemini) için soyutlama (abstraction) katmanının oluşturulması ve ortak messaging arayüzlerinin tanımlanması.
**Durum:** TODO
**Görevler:**
- [x] [task-006-messaging-abstraction.md](../tasks/task-006-messaging-abstraction.md) : Provider-Agnostic Abstractions & Messaging Modülü.

## 📝 Diğer Bilgiler (Şablon Dışı Notlar)
Yapay Zeka (AI) ajanları, yeni bir geliştirme talebi geldiğinde öncelikle hangi sprinte ait olacağına karar vermeli ve `governance/tasks` altında oluşturulan görevi bu Sprint dizinindeki ilgili Sprint başlığının altına eklemelidir. Yeni görevler için daima `task-template.md` şablonu baz alınmalıdır.
