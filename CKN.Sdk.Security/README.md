# CKN.Sdk.Security

Bu paket, CKN ekosistemi için merkezi **Şifreleme (Cryptography)** ve **Yetkilendirme (Authentication - JWT)** altyapısını soyutlar.

## 🚀 Amacı
Projelerde doğrudan `BCrypt.Net-Next` veya JWT metotlarının kargaşasını yaşatmamak, güvenliği (şifreleme gücü, tuzlama, secret yönetimi) tek bir merkezden standart olarak yönetmektir.
Böylece mikroservislerde kod karmaşası azalır ve kurumsal güvenlik politikaları tek noktadan değiştirilebilir (Örn: İleride Argon2'ye geçiş).

## 📦 Kurulum ve DI (Dependency Injection) Entegrasyonu

`Program.cs` veya `Startup.cs` dosyanıza şu şekilde ekleyebilirsiniz:

### 1. `appsettings.json` Konfigürasyonu
Projelerinizdeki `appsettings.json` dosyasına JWT bilgilerini ekleyin:

```json
{
  "Jwt": {
    "Secret": "SUPER_SECRET_KEY_MINIMUM_32_CHARS_LONG_OR_IT_WILL_FAIL",
    "Issuer": "CKN.Auth.Service",
    "Audience": "CKN.Microservices"
  }
}
```

### 2. Servis Kaydı (DI)

```csharp
using CKN.Sdk.Security;

builder.Services.AddCknSecurity(options =>
{
    options.JwtSecret = builder.Configuration["Jwt:Secret"] 
        ?? throw new ArgumentNullException("Jwt:Secret missing!");
    options.JwtIssuer = builder.Configuration["Jwt:Issuer"];
    options.JwtAudience = builder.Configuration["Jwt:Audience"];
    
    // (Opsiyonel) BCrypt Work Factor. Default: 11
    // options.BCryptWorkFactor = 12; 
});
```

## 🛠️ Örnek Kullanımlar

### 1. Şifre Hashleme ve Doğrulama (`IPasswordHasherService`)

```csharp
using CKN.Sdk.Security.Abstractions;

public class UserService
{
    private readonly IPasswordHasherService _passwordHasher;

    public UserService(IPasswordHasherService passwordHasher)
    {
        _passwordHasher = passwordHasher;
    }

    public void Register(string rawPassword)
    {
        // Güvenli Hash oluşturma
        string hashedPassword = _passwordHasher.HashPassword(rawPassword);
        // db.Save(hashedPassword);
    }

    public bool Login(string inputPassword, string dbHashedPassword)
    {
        // Şifre doğrulama
        return _passwordHasher.VerifyPassword(inputPassword, dbHashedPassword);
    }
}
```

### 2. JWT Token Üretimi (`ITokenGeneratorService`)

```csharp
using CKN.Sdk.Security.Abstractions;

public class AuthService
{
    private readonly ITokenGeneratorService _tokenGenerator;

    public AuthService(ITokenGeneratorService tokenGenerator)
    {
        _tokenGenerator = tokenGenerator;
    }

    public string GenerateUserToken(string userId)
    {
        var request = new CknTokenRequest
        {
            UserId = userId,
            Roles = new[] { "Admin", "User" },
            Expiration = TimeSpan.FromHours(2)
        };

        // Standart JWT üretimi
        return _tokenGenerator.GenerateToken(request);
    }
}
```

## 🧩 Clean Architecture Uyumluluğu
*   Mikroservisler sadece `CKN.Sdk.Security.Abstractions` arayüzlerini tanır. 
*   Arkada `BCrypt.Net-Next` gibi dış kütüphaneler kullanılması, projenizi (Domain/Application layer) doğrudan dışarıya bağımlı yapmaz. 
*   "Tak & Çalıştır" DI yapısı sayesinde `Program.cs` üzerinden tek hamle ile sisteme dahil olur.
