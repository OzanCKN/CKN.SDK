# CKN.Sdk.Notification

**Mühendislik Amacı (Engineering Intent):**
`CKN.Sdk.Notification`, uygulamaların son kullanıcılara e-posta (Email), SMS veya Push Notification (Bildirim) göndermesini sağlayan merkezi bir iletişim kütüphanesidir. **Neden var?** Her projede SMTP ayarları, SMS sağlayıcı entegrasyonları veya şablon (Template) motoru kurmak yerine, bunları standart bir Interface arkasında birleştirmek için. **Ne zaman kullanılmalı?** Sistemde şifre sıfırlama mailleri, kampanya SMS'leri veya mobil uygulamalara Firebase üzerinden Push Notification atılması gereken her senaryoda kullanılmalıdır.

## 🚀 Hızlı Başlangıç

### Kurulum

```bash
dotnet add package CKN.Sdk.Notification
```

### Konfigürasyon (`appsettings.json`)

```json
{
  "Notification": {
    "Email": {
      "Provider": "Smtp", // veya "SendGrid"
      "Smtp": {
        "Host": "smtp.gmail.com",
        "Port": 587,
        "Username": "noreply@sirket.com",
        "Password": "***"
      }
    },
    "Sms": {
      "Provider": "Twilio",
      "ApiKey": "..."
    }
  }
}
```

### Bağımlılık Enjeksiyonu (DI)

```csharp
using CKN.Sdk.Notification;

var builder = WebApplication.CreateBuilder(args);

// Email ve SMS servislerini sisteme kaydeder.
builder.Services.AddCknNotification(builder.Configuration);

var app = builder.Build();
```

## 💡 Gerçek Hayat Senaryoları

### Senaryo 1: Hoşgeldin E-postası Gönderme

Kullanıcı kayıt olduğunda basit bir metin e-postası atılması.

```csharp
using CKN.Sdk.Notification.Abstractions;

public class UserService(IEmailSender emailSender)
{
    public async Task RegisterUserAsync(string email)
    {
        // Kullanıcı kayıt işlemleri...
        
        var message = new EmailMessage
        {
            To = email,
            Subject = "Sistemimize Hoşgeldiniz",
            Body = "Hesabınız başarıyla oluşturuldu. Bizi tercih ettiğiniz için teşekkür ederiz.",
            IsHtml = false
        };

        await emailSender.SendAsync(message);
    }
}
```

### Senaryo 2: Dinamik HTML Şablonlu (Template) Fatura E-postası

HTML dosyasını veya şablonunu okuyup içerisindeki değişkenleri (isim, tutar vb.) doldurarak mail atma (Varyasyon).

```csharp
public async Task SendInvoiceEmailAsync(IEmailSender emailSender, string email, string customerName, decimal amount)
{
    // HTML Şablon (Gerçekte bir dosyadan okunabilir)
    string htmlTemplate = "<h1>Merhaba {{Name}}</h1><p>Faturanız kesildi: <b>{{Amount}} TL</b></p>";
    
    // Basit bir Replace veya daha gelişmiş bir Template Engine (Scriban/Handlebars) kullanılabilir
    string body = htmlTemplate
        .Replace("{{Name}}", customerName)
        .Replace("{{Amount}}", amount.ToString("N2"));

    var message = new EmailMessage
    {
        To = email,
        Subject = "Yeni Faturanız",
        Body = body,
        IsHtml = true
    };

    await emailSender.SendAsync(message);
}
```

## 🤖 Yapay Zeka İçin SSS (FAQs for Machines)

- **Soru:** E-posta sağlayıcısını değiştirmek için kod değiştirmek gerekir mi?
- **Cevap:** Hayır. Eğer kodlar `IEmailSender` arayüzüne (interface) bağlıysa, SendGrid veya standart SMTP arasında geçiş yapmak için sadece `appsettings.json` içerisindeki `Provider` alanını değiştirmek yeterlidir.
- **Soru:** Toplu (Bulk) SMS veya E-posta atılabilir mi?
- **Cevap:** Evet, `IEmailSender` arayüzündeki `SendBatchAsync` (eğer varsa) veya for döngüsü ile asenkron task'lar (`Task.WhenAll`) oluşturularak toplu gönderim yapılabilir. Ancak çok yüksek hacimli gönderimler için MassTransit/RabbitMQ üzerinden arka plan görevi (Background Job) oluşturulması daha sağlıklı bir mimaridir.
