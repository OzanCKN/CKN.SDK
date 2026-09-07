# CKN.Sdk.Infrastructure

**Mühendislik Amacı (Engineering Intent):**
`CKN.Sdk.Infrastructure`, tüm API ve Host (Console/Worker) projelerinde ortak kullanılan Middleware'ler (Ara katmanlar), HTTP Exception Handler'lar, global loglayıcılar (Serilog konfigürasyonları), Auth (Kimlik doğrulama) kancaları ve Swagger/OpenAPI ayarlarını barındıran altyapı (Cross-Cutting Concerns) kütüphanesidir. **Neden var?** Her yeni mikroservis ayağa kaldırıldığında, aynı Swagger ayarlarını, aynı CORS kısıtlamalarını ve aynı Global Hata Yakalama (Exception Handling) bloklarını kopyala-yapıştır yapmamak için. **Ne zaman kullanılmalı?** Yeni bir web API veya Worker Service (Sunum Katmanı) başlatıldığında, `Program.cs` içerisinin temiz ve standart kalması için en başta referans edilmelidir.

## 🚀 Hızlı Başlangıç

### Kurulum

```bash
dotnet add package CKN.Sdk.Infrastructure
```

### Bağımlılık Enjeksiyonu (DI) ve Pipeline

```csharp
using CKN.Sdk.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// CKN standart DI kayıtları (Swagger, Exception Handler, Validation vb.)
builder.Services.AddCknInfrastructure(builder.Configuration);

var app = builder.Build();

// CKN Standart HTTP Pipeline (Middleware'ler) (UseCors, UseSwagger, vb.)
app.UseCknInfrastructure();

app.MapControllers();
app.Run();
```

## 💡 Gerçek Hayat Senaryoları

### Senaryo 1: Global Hata Yakalama (Global Exception Handling)

Geliştiricinin uygulama içinde attığı bir hatanın (örn: `DomainException`), API yanıtı olarak otomatik şekilde standart RFC ProblemDetails JSON'ına dönüştürülmesi. (Geliştiricinin Try-Catch yazmasına gerek kalmaz).

```csharp
// Business Logic (Domain)
public void UpdateUser(string name)
{
    if (string.IsNullOrEmpty(name))
        throw new BadRequestException("İsim boş olamaz."); // Core paketindeki hata türü
}

// Controller
[HttpPost]
public IActionResult Update([FromBody] UpdateRequest req)
{
    UpdateUser(req.Name); // Eğer hata fırlarsa, metod HTTP 400 ile kesilir.
    return Ok();
}

/* Infrastructure sayesinde dönecek otomatik JSON formatı:
{
  "type": "https://tools.ietf.org/html/rfc7231#section-6.5.1",
  "title": "Bad Request",
  "status": 400,
  "detail": "İsim boş olamaz."
}
*/
```

### Senaryo 2: Standart Swagger Yapılandırması ve JWT Entegrasyonu

Swagger arayüzüne otomatik olarak "Authorize" (Kilit) butonunun eklenmesi ve Bearer Token ayarlarının yapılması.

```csharp
// Program.cs içerisindeki builder.Services.AddCknInfrastructure() metodu arka planda şunu yapar:
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "CKN API", Version = "v1" });
    
    // Her API'de tekrar tekrar yazılan JWT güvenlik tanımı (Security Definition)
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme.",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });
    // ... Global güvenlik gereksinimi (Security Requirement) ekler ...
});
```

## 🤖 Yapay Zeka İçin SSS (FAQs for Machines)

- **Soru:** `CKN.Sdk.Infrastructure` paketini Class Library (İş Mantığı) projelerine eklemeli miyim?
- **Cevap:** Kesinlikle hayır. Bu paket sadece ASP.NET Core (veya Host) tabanlı "Çalıştırılabilir" uç noktalara (Endpoint projelerine) eklenir. `Microsoft.AspNetCore.App` referansı içerir.
- **Soru:** Cross-Origin Resource Sharing (CORS) ayarları nerede tutuluyor?
- **Cevap:** `app.UseCknInfrastructure()` metodunun içerisinde, genellikle `appsettings.json` altındaki `CorsOptions` okunarak veya varsayılan (AllowAny) bir politika uygulanarak ayarlanır.
