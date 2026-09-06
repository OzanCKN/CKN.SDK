# Sprint Master Index

Projenin tüm sprint ve görev süreçlerinin ana yönetim (tracking) belgesidir. 

Yeni bir özellik veya güncelleme yapılmadan önce, ilgili aktif Sprint içerisine bir Task dosyası açılmalı ve buraya referansı eklenmelidir.

## Aktif Sprint
- [Sprint 1 - Foundation & AI Integration](#sprint-1)

## Tüm Sprintler

### Sprint 1
**Hedef:** Proje altyapısının kurulması, kurumsal kimlik entegrasyonu (CKN isimlendirmeleri) ve AI Yönetişim altyapısının oluşturulması.

**Görevler:**
- [x] Projedeki EnAI isimlendirmelerinin CKN olarak güncellenmesi.
- [x] AI Governance yapısının kurulması (`governance/` klasörleri).
- [x] [task-001-implement-redis-cache.md](../tasks/task-001-implement-redis-cache.md) : Redis Önbellek (Cache) Entegrasyonu
- [x] [task-002-sdk-documentation-and-governance.md](../tasks/task-002-sdk-documentation-and-governance.md) : SDK Dokümantasyonları ve Glossary Güncellemesi

## Sprint 2: Temel Mimari ve Kullanıcı Deneyimi (DX)
**Hedef:** SDK'nın dışarıdan tüketimi (DI, Fluent API, Exception handling) ve AOT Native Source Generator entegrasyonu.
**Durum:** TODO
**Görevler:**
- [ ] [task-003-sprint-2-dx.md](../tasks/task-003-sprint-2-dx.md) : DI, Options Pattern, Exceptions ve AOT Source Generator geliştirimi.

## Sprint 3: Dayanıklılık, Performans ve Ölçeklenebilirlik
**Hedef:** Polly, Caching, Pagination, Circuit Breaker ve Health Checks entegrasyonu.
**Durum:** TODO
**Görevler:**
- [ ] [task-004-sprint-3-resilience.md](../tasks/task-004-sprint-3-resilience.md) : Resilience, Circuit Breaker ve Memory Optimization.

## Sprint 4: Güvenlik, Gözlemlenebilirlik ve DevOps
**Hedef:** Otomatik Elastic Logging, OpenTelemetry, Güçlü İsimlendirme, CI/CD ve NetArchTest mimari testleri.
**Durum:** TODO
**Görevler:**
- [ ] [task-005-sprint-4-observability.md](../tasks/task-005-sprint-4-observability.md) : Elastic Logging, OpenTelemetry ve DevOps.

---
*Yeni eklenecek görevler için [task-template.md](../tasks/task-template.md) şablonunu kullanın.*
