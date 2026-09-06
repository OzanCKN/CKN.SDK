# Görev Şablonu: Yönetişim Kurallarının ve SDK Dokümantasyonlarının Sağlanması

**Görev Adı:** SDK Dokümantasyonları ve Glossary Güncellemesi
**Sprint:** Sprint 1
**Durum:** [x] TODO | [x] IN PROGRESS | [x] DONE

## 1. Açıklama ve Kapsam
Projeyi modüler (CKN.Sdk.EntityFramework, MassTransit vb.) yapıya taşırken ihlal edilen Governance kurallarının (System Glossary ve Decision Log güncellemelerinin unutulması) telafi edilmesi. Ayrıca her bir SDK'nin NuGet paket standardına uygun olarak `README.md` ile donatılması.

## 2. Etkilenen Sistemler (Glossary Referansları)
- Kök Dizin (README.md'ler eklenecek)
- `governance/docs/architecture/*` (Tüm mimari dökümanlar güncellenecek)

## 3. Yapılacaklar (Checklist)
- [ ] `system-glossary.md` dosyasının 6 yeni projeyi kapsayacak şekilde baştan yazılması.
- [ ] `decision-log.md` dosyasına Modülerleşme kararının eklenmesi.
- [ ] Her alt proje için (Core, EntityFramework, AI, MassTransit, SourceGenerators) NuGet dostu `README.md` yazılması.

## 4. İzcilik Kuralı (Scout Rule) Notları
- N/A
