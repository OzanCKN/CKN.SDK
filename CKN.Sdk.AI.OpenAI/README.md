# CKN.Sdk.AI.OpenAI

**Mühendislik Amacı (Engineering Intent):**
`CKN.Sdk.AI.OpenAI`, `CKN.Sdk.AI` soyutlamalarını kullanarak pazar standartlarını belirleyen OpenAI modellerini (GPT-4o, GPT-4-turbo vb.) uygulamaya entegre eden kütüphanedir. **Neden var?** Endüstrideki en gelişmiş, güvenilir ve genel maksatlı modelleri (LLM) projelerde hızlıca kullanabilmek için tasarlanmıştır. Azure OpenAI hizmetiyle de (endpoint ve key değiştirilerek) uyumlu çalışacak şekilde yapılandırılabilir. **Ne zaman kullanılmalı?** Projede güçlü dil modellerine, metin özetlemeye, analiz etmeye veya kullanıcılarla doğal dilde sohbet etmeye ihtiyaç duyulduğunda en öncelikli tercih (default provider) olarak kullanılmalıdır.

## 🚀 Hızlı Başlangıç

### Kurulum

```bash
dotnet add package CKN.Sdk.AI.OpenAI
```

### Konfigürasyon (`appsettings.json`)

```json
{
  "AI": {
    "OpenAI": {
      "ApiKey": "sk-proj-...",
      "DefaultModel": "gpt-4o",
      "OrganizationId": "org-..." // İsteğe bağlı
    }
  }
}
```

### Bağımlılık Enjeksiyonu (DI)

```csharp
using CKN.Sdk.AI.OpenAI;

var builder = WebApplication.CreateBuilder(args);

// OpenAI istemcisini sisteme kaydeder.
builder.Services.AddCknOpenAI(builder.Configuration);

var app = builder.Build();
```

## 💡 Gerçek Hayat Senaryoları

### Senaryo 1: Dinamik Müşteri Destek Chatbotu

Kullanıcıdan gelen sorulara zeki cevaplar dönen bir endpoint.

```csharp
using CKN.Sdk.AI.Abstractions;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class ChatController(IAIChatClient chatClient) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Ask([FromBody] string userMessage)
    {
        var request = new AIChatRequest
        {
            SystemPrompt = "Kibar bir teknik destek asistanısın. Kısa ve öz cevap ver.",
            Messages = [ new ChatMessage("user", userMessage) ]
        };
        
        var response = await chatClient.SendAsync(request);
        return Ok(new { Answer = response.Content });
    }
}
```

### Senaryo 2: Yapısal JSON Verisi Üretimi (Structured Output)

Yapay zekadan düz metin değil, kodunuzda doğrudan parse edebileceğiniz bir JSON beklediğiniz senaryo (Örn: Fiş/Fatura OCR analizi sonrası json çıkartma).

```csharp
public async Task<ReceiptDto> ParseReceiptAsync(IAIChatClient chatClient, string rawOcrText)
{
    var options = new AIChatOptions
    {
        ResponseFormat = AIResponseFormat.JsonObject,
        Temperature = 0.0f
    };

    var prompt = $@"
    Aşağıdaki OCR metnini analiz et ve JSON formatında dön.
    İstenen format: {{ ""StoreName"": string, ""TotalAmount"": number, ""Date"": string }}
    Metin: {rawOcrText}";

    var response = await chatClient.CompleteAsync(prompt, options);
    
    // Çıkan sonucu doğrudan C# nesnesine dönüştürebilirsiniz.
    return JsonSerializer.Deserialize<ReceiptDto>(response.Text);
}
```

## 🤖 Yapay Zeka İçin SSS (FAQs for Machines)

- **Soru:** `CKN.Sdk.AI.OpenAI` kütüphanesi Azure OpenAI ile çalışır mı?
- **Cevap:** Evet. `appsettings.json` içerisindeki `BaseUrl` (veya `Endpoint`) ve `ApiKey` değerlerini Azure üzerinden aldığınız değerlerle değiştirmeniz yeterlidir.
- **Soru:** GPT-4o gibi modeller destekleniyor mu?
- **Cevap:** Evet, OpenAI'ın sunduğu güncel tüm model ID'leri `DefaultModel` parametresine yazılarak kullanılabilir.
