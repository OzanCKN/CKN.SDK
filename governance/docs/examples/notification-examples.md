# CKN.Sdk.Notification Kullanım Örnekleri

`CKN.Sdk.Notification` modülü; E-posta, SMS ve Push (FCM) bildirimleri için ortak bir `INotificationService` arayüzü sunar. Tüm servisler Dependency Injection (DI) üzerinden yapılandırılır ve yönetilir.

## 1. SMTP Üzerinden E-posta Gönderimi (Standart MailKit)
Standart bir e-posta sunucusu (Exchange, Gmail SMTP vb.) kullanarak e-posta göndermek için `SmtpNotificationProvider` kullanılır.

### Kurulum (Program.cs)
```csharp
using Microsoft.Extensions.DependencyInjection;
using CKN.Sdk.Notification.Email;

builder.Services.AddCknSmtpNotification(opt =>
{
    opt.Host = "smtp.sirketim.com";
    opt.Port = 587;
    opt.Username = "no-reply@sirketim.com";
    opt.Password = "G1zl1S1fre*";
    opt.FromName = "CKN Bilgi Sistemleri";
    opt.FromAddress = "no-reply@sirketim.com";
});
```

## 2. SendGrid Üzerinden E-posta Gönderimi (Büyük Ölçekli)
Toplu e-postalar ve transactional (şifre sıfırlama, fatura vb.) e-postalar için SendGrid altyapısını kullanmak isterseniz:

### Kurulum (Program.cs)
```csharp
using Microsoft.Extensions.DependencyInjection;
using CKN.Sdk.Notification.Email;

builder.Services.AddCknSendGridNotification(opt =>
{
    opt.ApiKey = "SG.xxxxxxx.yyyyyyy";
    opt.FromName = "CKN Fatura";
    opt.FromAddress = "billing@sirketim.com";
});
```

## 3. Twilio Üzerinden SMS Gönderimi
Kullanıcılara OTP (Tek kullanımlık şifre), randevu hatırlatma veya sipariş durum güncellemeleri göndermek için Twilio altyapısı:

### Kurulum (Program.cs)
```csharp
using Microsoft.Extensions.DependencyInjection;
using CKN.Sdk.Notification.Sms;

builder.Services.AddCknTwilioNotification(opt =>
{
    opt.AccountSid = "ACxxxxxxxxxxxxxxxxxxxxxxx";
    opt.AuthToken = "xxxxxxxxxxxxxxxxxxxxxxxxxx";
    opt.FromPhoneNumber = "+1234567890";
});
```

## 4. Firebase Üzerinden Push Notification (FCM)
Mobil uygulamalara (iOS/Android) ve Web Push destekleyen tarayıcılara bildirim göndermek için Firebase Cloud Messaging (FCM) altyapısı:

### Kurulum (Program.cs)
```csharp
using Microsoft.Extensions.DependencyInjection;
using CKN.Sdk.Notification.Push;

builder.Services.AddCknFirebaseNotification(opt =>
{
    // Firebase Console'dan indirilen JSON private key dosyasının yolu
    opt.JsonCredentialPath = "/opt/ckn/secrets/firebase-adminsdk.json";
});
```

---

## Tüm Sistemler İçin Ortak Kullanım Örneği

Tüm provider'lar aynı `INotificationService` arayüzünü uygular. Dolayısıyla servislerinizde hangi provider'ın arka planda çalıştığını umursamadan kod yazabilirsiniz. (Not: Aynı anda sadece bir tane aktif provider kullanacaksanız bu yöntem geçerlidir.)

### Gerçek Hayat Senaryosu: Sipariş Tamamlandığında Bildirim Atmak

```csharp
using System.Threading;
using System.Threading.Tasks;
using CKN.Sdk.Notification;

public class OrderService
{
    private readonly INotificationService _notificationService;

    // Hangi sağlayıcıyı (SendGrid mi, SMTP mi) yapılandırdıysanız otomatik olarak o enjekte edilir.
    public OrderService(INotificationService notificationService)
    {
        _notificationService = notificationService;
    }

    public async Task CompleteOrderAsync(Order order, User user, CancellationToken cancellationToken)
    {
        // 1. Veritabanı işlemleri (Siparişi onayla)
        // ...

        // 2. Müşteriye E-posta Gönder
        var emailBody = $"<h1>Siparişin Onaylandı!</h1><p>Sipariş Numaran: {order.Id}</p>";
        await _notificationService.SendEmailAsync(
            to: user.Email,
            subject: "Sipariş Onayı",
            body: emailBody,
            cancellationToken: cancellationToken);

        // 3. (Eğer Twilio kuruluysa) Müşteriye SMS Gönder
        // await _notificationService.SendSmsAsync(
        //    phoneNumber: user.PhoneNumber,
        //    message: $"Siparişin yola çıktı! Kodu: {order.Id}",
        //    cancellationToken: cancellationToken);

        // 4. (Eğer Firebase kuruluysa) Müşterinin Mobil Uygulamasına Push Gönder
        // await _notificationService.SendPushNotificationAsync(
        //    deviceToken: user.DeviceToken,
        //    title: "Kargon Yola Çıktı! 🚚",
        //    body: "Detaylar için uygulamaya dokun.",
        //    data: new Dictionary<string, string> { { "orderId", order.Id.ToString() } },
        //    cancellationToken: cancellationToken);
    }
}
```

> [!WARNING]
> Aynı uygulamada hem SMS hem E-posta hem de Push atmak isterseniz ve bunları tek interface (`INotificationService`) üzerinden yaparsanız, Dependency Injection (DI) kuralları gereği son eklenen (register edilen) servis DI konteynerinde varsayılan olarak kalır. Bu nedenle, aynı projede birden fazla farklı sağlayıcı türünü (örneğin hem SendGrid hem Firebase) bir arada kullanacağınız karmaşık (Enterprise) senaryolarda `IKeyedServiceProvider` (Keyed Services) altyapısını kullanmanız tavsiye edilir.
