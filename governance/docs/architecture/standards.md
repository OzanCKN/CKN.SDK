# Mimari ve Kod Standartları (Architecture Standards)

Bu proje genelinde yazılımın sürdürülebilirliği, güvenliği ve kod kalitesini korumak amacıyla aşağıdaki kuralların her zaman uygulanması zorunludur. Tüm AI Ajanları (AI Agents) bu kurallara uymakla yükümlüdür.

## 1. Sıfır Uyarı Politikası (Zero-Warning Policy)
- Hiçbir kod bloğu, derleyici uyarısı (compiler warning) veya statik analiz uyarısı (SonarQube/Linter) bırakılacak şekilde yazılamaz.
- Tüm `null` olabilecek referanslar (nullable reference types) özenle ele alınmalıdır.

## 2. Test Güdümlü Geliştirme (TDD)
- Yeni eklenen tüm özelliklerin Unit Test (Birim Test) karşılığı olmalıdır.
- Test edilmeyen hiçbir kod tam olarak "tamamlanmış" sayılmaz.
- Core katmanı %100 bağımsız ve test edilebilir olmalıdır.

## 3. İzlenebilirlik (Observability)
- Uygulama içerisinde kesinlikle `Console.WriteLine` gibi ilkel yöntemler kullanılamaz.
- Tüm loglama, hata izleme ve metrik işlemleri projenin entegre altyapısı (OpenTelemetry, ILogger vb.) kullanılarak yapılmalıdır.

## 4. Bağımlılık Yönetimi
- Dışarıdan kullanılacak (3rd party) kütüphaneler eklenmeden önce kullanıcının açık onayı alınmalıdır.
- "Sadece bir metot için" tüm bir paketi indirmekten kaçınılmalıdır.

## 5. İzcilik Kuralı (Boy Scout Rule)
- Değişiklik yaptığınız dosyayı, bulduğunuzdan daha temiz bırakın.
- Mevcut bir hata veya "Code Smell" görürseniz, ilgili task'ın kapsamını çok saptırmadan düzeltin.
