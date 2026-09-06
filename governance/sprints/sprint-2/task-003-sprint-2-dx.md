# Görev Şablonu: Sprint 2 - Temel Mimari ve DX

**Görev Adı:** Temel Mimari ve Kullanıcı Deneyimi (DX) Kurulumu
**Sprint:** Sprint 2
**Durum:** [x] TODO | [ ] IN PROGRESS | [ ] DONE

## 1. Açıklama ve Kapsam
CKN.SDK'nin 2026 standartlarında geliştirici dostu olabilmesi için temel (Core) yeteneklerin ve Native AOT için Source Generator (Roslyn) altyapısının kurulması.

## 3. Yapılacaklar (Checklist)
- [ ] Madde 1: `builder.Services.AddCknCore()` DI Extension metotları yazılacak.
- [ ] Madde 2: `IOptions<T>` ile Configuration Binding (Options Pattern) altyapısı kurulacak.
- [ ] Madde 3: Gelişmiş yapılandırma için Fluent Builder Pattern kodlanacak.
- [ ] Madde 4: Asenkron olmayan metotlar yasaklanacak, `CancellationToken` zorunlu tutulacak.
- [ ] Madde 5: SDK içindeki statik metotlar temizlenip Interface odaklı tasarıma geçilecek.
- [ ] Madde 6: XML Dokumentasyonu (`<summary>`) devreye alınacak.
- [ ] Madde 7: Veri transfer objeleri (DTO'lar) Immutable `record` tipine çevrilecek.
- [ ] Madde 8 & 9: Özel `Exception` sınıfları ve RFC 7807 (ProblemDetails) desteği eklenecek.
- [ ] Madde 10: Semantik Versiyonlama standartları ayarlanacak.
- [ ] **KRİTİK:** Madde 18: `CKN.Sdk.SourceGenerators` kodlanarak DI/CQRS Reflection işlemleri AOT'ye uygun hale getirilecek.
