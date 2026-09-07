# CKN.Sdk.Notification

CKN.Sdk içerisinde sistem genelindeki **E-posta, SMS ve Push Bildirim** altyapılarını tek bir merkezden (Provider-Agnostic) yönetmek için oluşturulmuş modüldür.

## Yapılandırma (`appsettings.json`)

```json
{
  "Notification": {
    "Email": {
      "Provider": "Smtp", // veya SendGrid, Mailgun vb.
      "Smtp": {
        "Host": "smtp.gmail.com",
        "Port": 587,
        "Username": "info@ckn.com",
        "Password": "your-password"
      }
    }
  }
}
```

## Servis Kaydı (Dependency Injection)

```csharp
// Örnek kullanım (Henüz Notification servis soyutlamaları tasarlandığı için temsili koddur)
// builder.Services.AddCknNotification(builder.Configuration);
```

## Gerçek Hayat Kullanım Senaryosu

**Kullanıcı Kaydı Sonrası Hoşgeldin E-postası**
Kullanıcı kayıt olduğunda arka planda dinamik HTML şablonuyla e-posta göndermek.

```csharp
public class UserRegistrationService
{
    private readonly INotificationSender _notificationSender;

    public UserRegistrationService(INotificationSender notificationSender)
    {
        _notificationSender = notificationSender;
    }

    public async Task RegisterUserAsync(string email)
    {
        // ... kullanıcıyı kaydet ...

        // Dinamik e-posta gönder
        await _notificationSender.SendEmailAsync(
            to: email,
            subject: "CKN Sistemine Hoşgeldiniz!",
            body: "<h1>Hoşgeldiniz!</h1> Sisteme kaydınız başarıyla tamamlanmıştır."
        );
    }
}
```
