# Sprints (Sprintler) Ä°ndeksi

## ðŸ“Œ AmaÃ§
Bu dizin, projenin tÃ¼m Ã§evik geliÅŸtirme (agile) dÃ¶ngÃ¼lerinin, geÃ§miÅŸ ve aktif sprint sÃ¼reÃ§lerinin ana yÃ¶netim (tracking) belgesi olarak oluÅŸturulmuÅŸtur.

## ðŸ“‚ Ä°Ã§erik
Yeni bir Ã¶zellik veya gÃ¼ncelleme yapÄ±lmadan Ã¶nce, ilgili aktif Sprint iÃ§erisine bir Task dosyasÄ± aÃ§Ä±lmalÄ± ve buraya referansÄ± eklenmelidir.

### ðŸƒ Aktif Sprint
- **[Sprint 1 - Foundation & AI Integration](#sprint-1)**

### TÃ¼m Sprintler

#### Sprint 1
**Hedef:** Proje altyapÄ±sÄ±nÄ±n kurulmasÄ±, kurumsal kimlik entegrasyonu (CKN isimlendirmeleri) ve AI YÃ¶netiÅŸim altyapÄ±sÄ±nÄ±n oluÅŸturulmasÄ±.

**GÃ¶revler:**
- [x] Projedeki EnAI isimlendirmelerinin CKN olarak gÃ¼ncellenmesi.
- [x] AI Governance yapÄ±sÄ±nÄ±n kurulmasÄ± (`governance/` klasÃ¶rleri).
- [x] [task-001-implement-redis-cache.md](../tasks/task-001-implement-redis-cache.md) : Redis Ã–nbellek (Cache) Entegrasyonu
- [x] [task-002-sdk-documentation-and-governance.md](../tasks/task-002-sdk-documentation-and-governance.md) : SDK DokÃ¼mantasyonlarÄ± ve Glossary GÃ¼ncellemesi

#### Sprint 2: Temel Mimari ve KullanÄ±cÄ± Deneyimi (DX)
**Hedef:** SDK'nÄ±n dÄ±ÅŸarÄ±dan tÃ¼ketimi (DI, Fluent API, Exception handling) ve AOT Native Source Generator entegrasyonu.
**Durum:** TODO
**GÃ¶revler:**
- [ ] [task-003-sprint-2-dx.md](../tasks/task-003-sprint-2-dx.md) : DI, Options Pattern, Exceptions ve AOT Source Generator geliÅŸtirimi.

#### Sprint 3: DayanÄ±klÄ±lÄ±k, Performans ve Ã–lÃ§eklenebilirlik
**Hedef:** Polly, Caching, Pagination, Circuit Breaker ve Health Checks entegrasyonu.
**Durum:** DONE
**GÃ¶revler:**
- [x] [task-004-sprint-3-resilience.md](../tasks/task-004-sprint-3-resilience.md) : Resilience, Circuit Breaker ve Memory Optimization.

#### Sprint 4: GÃ¼venlik, GÃ¶zlemlenebilirlik ve DevOps
**Hedef:** Otomatik Elastic Logging, OpenTelemetry, GÃ¼Ã§lÃ¼ Ä°simlendirme, CI/CD ve NetArchTest mimari testleri.
**Durum:** DONE
**GÃ¶revler:**
- [x] [task-005-sprint-4-observability.md](../tasks/task-005-sprint-4-observability.md) : Elastic Logging, OpenTelemetry ve DevOps.

#### Sprint 5: Provider-Agnostic Architecture Phase 1
**Hedef:** FarklÄ± AI Provider'larÄ± (OpenAI, Anthropic, Gemini) iÃ§in soyutlama (abstraction) katmanÄ±nÄ±n oluÅŸturulmasÄ± ve ortak messaging arayÃ¼zlerinin tanÄ±mlanmasÄ±.
**Durum:** TODO
**GÃ¶revler:**
- [x] [task-006-messaging-abstraction.md](../tasks/task-006-messaging-abstraction.md) : Provider-Agnostic Abstractions & Messaging ModÃ¼lÃ¼.

#### Sprint 6: Finance SDK Gap Closure
**Hedef:** CKN.Finance'in ham HttpClient ve daÄŸÄ±nÄ±k hesaplama kodundan kurtulmasÄ± iÃ§in eksik paketleri geliÅŸtir.
**Durum:** IN PROGRESS
**GÃ¶revler:**
- [x] [task-007-network-http-client.md](../tasks/task-007-network-http-client.md) : CKN.Sdk.Network + CKN.Sdk.Network.Http
- [x] [task-008-ai-dependency-fix.md](../tasks/task-008-ai-dependency-fix.md) : AI bug fix + XML docs + NU1608
- [x] [task-009-financial.md](../tasks/task-009-financial.md) : CKN.Sdk.Financial

## ðŸ“ DiÄŸer Bilgiler (Åžablon DÄ±ÅŸÄ± Notlar)
Yapay Zeka (AI) ajanlarÄ±, yeni bir geliÅŸtirme talebi geldiÄŸinde Ã¶ncelikle hangi sprinte ait olacaÄŸÄ±na karar vermeli ve `governance/tasks` altÄ±nda oluÅŸturulan gÃ¶revi bu Sprint dizinindeki ilgili Sprint baÅŸlÄ±ÄŸÄ±nÄ±n altÄ±na eklemelidir. Yeni gÃ¶revler iÃ§in daima `task-template.md` ÅŸablonu baz alÄ±nmalÄ±dÄ±r.

