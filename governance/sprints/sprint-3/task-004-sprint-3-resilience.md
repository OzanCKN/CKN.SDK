# Görev Şablonu: Sprint 3 - Dayanıklılık, Performans ve Ölçeklenebilirlik

**Görev Adı:** Resilience, Caching ve Ölçeklenebilirlik Altyapısı
**Sprint:** Sprint 3
**Durum:** [x] TODO | [ ] IN PROGRESS | [ ] DONE

## 1. Açıklama ve Kapsam
Büyük yük altındaki sistemlerde SDK'nın çökmemesi, API kotalarını aşmaması ve hafıza (RAM) verimliliğini sağlaması için dayanıklılık prensiplerinin entegrasyonu.

## 3. Yapılacaklar (Checklist)
- [ ] Madde 11: Polly.NET ile Resilience & Retry (Exponential Backoff) politikaları eklenecek.
- [ ] Madde 12: Devre Kesici (Circuit Breaker) kuralı eklenecek.
- [ ] Madde 13: 429 Too Many Requests kısıtlama yönetimi (Rate Limiting) sağlanacak.
- [ ] Madde 14: Yapay Zeka veya veritabanı yanıt vermediğinde Graceful Degradation (Zarif Düşüş) mekanizması kodlanacak.
- [ ] Madde 15: Finansal işlem güvenliği için `Idempotency-Key` başlık desteği.
- [ ] Madde 16: Büyük listeler `IAsyncEnumerable` ile sayfalama akışına (Stream) dönüştürülecek.
- [ ] Madde 17: Yüksek performans gerektiren alanlarda `Span<T>` ve `Memory<T>` kullanılacak.
- [ ] Madde 19: SDK modülleri için hazır `IHealthCheck` sınıfları yazılacak.
- [ ] Madde 20: `IDistributedCache` ve `IMemoryCache` kullanımı SDK içine standartlaşacak.
