# Görev Şablonu: Redis Önbellek (Cache) Entegrasyonu

**Görev Adı:** Redis Cache Implementasyonu
**Sprint:** Sprint 1
**Durum:** [ ] TODO | [ ] IN PROGRESS | [ ] DONE

## 1. Açıklama ve Kapsam
Şu an `CKN.Sdk.Infrastructure` içerisinde sadece `InMemoryCacheService` bulunmaktadır. Production ortamları için dağıtık önbelleğe (Distributed Cache) ihtiyaç duyulmaktadır. Bu görev, `ICacheService` arayüzünün Redis kullanılarak implemente edilmesini kapsar.

## 2. Etkilenen Sistemler (Glossary Referansları)
- `CKN.Sdk.Core`: Hiçbir değişiklik yapılmayacak (Arayüz zaten mevcut).
- `CKN.Sdk.Infrastructure`: `Caching/RedisCacheService.cs` eklenecek. Redis bağlantı konfigürasyonları (Extensions) yazılacak.

## 3. Yapılacaklar (Checklist)
- [ ] `StackExchange.Redis` paketinin `CKN.Sdk.Infrastructure` projesine eklenmesi.
- [ ] `RedisCacheService.cs` sınıfının kodlanarak `ICacheService`'ten türetilmesi.
- [ ] Redis bağlantı testlerinin (Unit Tests / Integration Tests) yazılması.
- [ ] System Glossary'nin güncellenmesi (Caching bölümüne RedisCacheService eklenecek).
- [ ] Decision Log'a Redis kararının işlenmesi.

## 4. İzcilik Kuralı (Scout Rule) Notları
- N/A
