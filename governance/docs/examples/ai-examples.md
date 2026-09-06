# AI (Yapay Zeka) Sağlayıcıları Kullanım Örnekleri

CKN.SDK, .NET 9+ (Microsoft.Extensions.AI) ekosistemi üzerine kurularak OpenAI, Anthropic, Ollama (Local LLM) ve Microsoft Semantic Kernel orkestrasyon motorunu destekler.

---

## 1. OpenAI / Anthropic (Bulut LLM'leri) Kullanımı

ChatGPT (OpenAI) veya Claude (Anthropic) gibi bulut tabanlı API'leri bağlamak için kullanılır.

### `appsettings.json` Yapılandırması
```json
{
  "AI": {
    "OpenAI": {
      "ApiKey": "sk-your-openai-key",
      "ModelId": "gpt-4o",
      "Endpoint": "https://api.openai.com/v1"
    },
    "Anthropic": {
      "ApiKey": "sk-ant-api03-...",
      "ModelId": "claude-3-5-sonnet-20240620"
    }
  }
}
```

### Dependency Injection (DI) Kurulumu
```csharp
using CKN.Sdk.AI.OpenAI;
using CKN.Sdk.AI.Anthropic;

// OpenAI Kurulumu (IChatClient sağlar)
builder.Services.AddCknOpenAI(opt =>
{
    builder.Configuration.GetSection(OpenAIOptions.SectionName).Bind(opt);
});

// Anthropic Kurulumu (AnthropicClient sağlar)
builder.Services.AddCknAnthropic(opt =>
{
    builder.Configuration.GetSection(AnthropicOptions.SectionName).Bind(opt);
});
```

### Gerçek Hayat Kullanımı: Müşteri Destek Asistanı (Streaming Chat)
`Microsoft.Extensions.AI` kapsamında IChatClient doğrudan kullanılır. Streaming özelliğiyle (kelime kelime akış) ChatGPT-vari uygulamalar yapılabilir.

```csharp
using Microsoft.Extensions.AI;

public class CustomerSupportService
{
    private readonly IChatClient _chatClient;

    public CustomerSupportService(IChatClient chatClient)
    {
        _chatClient = chatClient;
    }

    public async IAsyncEnumerable<string> StreamCustomerSupportResponse(string userMessage)
    {
        var messages = new List<ChatMessage>
        {
            new ChatMessage(ChatRole.System, "Sen kibar bir müşteri temsilcisisin. Lütfen soruları kısa yanıtla."),
            new ChatMessage(ChatRole.User, userMessage)
        };

        // LLM'den yanıtı streaming (akış) şeklinde al
        await foreach (var update in _chatClient.CompleteStreamingAsync(messages))
        {
            if (update.Text != null)
            {
                yield return update.Text; // API'ye veya WebSocket'e parça parça gönderilir
            }
        }
    }
}
```

---

## 2. Ollama Kullanımı (Local LLM - Ücretsiz/Offline)

Gizliliğin önemli olduğu veya API maliyetlerinin düşürülmek istendiği durumlarda Llama3, Mistral gibi modelleri **kendi sunucunuzda (localhost)** çalıştırmanızı sağlar.

### `appsettings.json` Yapılandırması
```json
{
  "AI": {
    "Ollama": {
      "Endpoint": "http://localhost:11434",
      "ModelId": "llama3.1"
    }
  }
}
```

### Dependency Injection (DI) Kurulumu
```csharp
using CKN.Sdk.AI.Ollama;

builder.Services.AddCknOllama(opt =>
{
    builder.Configuration.GetSection(OllamaOptions.SectionName).Bind(opt);
});
```

### Gerçek Hayat Kullanımı: Hasta Raporu Özeti Çıkartma (KVKK Uyumlu)
Kişisel Verilerin Korunması Kanunu (KVKK) gereği, hasta verileri buluta (OpenAI'ye) yollanamaz. Ollama ile yerel analiz yapılır:

```csharp
public class HealthcareAiService
{
    private readonly IChatClient _chatClient;

    public HealthcareAiService(IChatClient chatClient)
    {
        _chatClient = chatClient;
    }

    public async Task<string> SummarizePatientReport(string medicalReportText)
    {
        var prompt = $"Şu tıbbi raporu 3 cümleyle özetle, KVKK kurallarına dikkat et: {medicalReportText}";
        
        var response = await _chatClient.CompleteAsync(prompt);
        return response.Message.Text ?? "Özet çıkartılamadı.";
    }
}
```

---

## 3. Semantic Kernel Kullanımı (AI Orkestrasyonu & Plugin'ler)

Semantic Kernel, yapay zeka ajanlarına (Agent) "eller ve ayaklar" vermek için (Örneğin: E-posta gönderme yeteneği, Veritabanına sorgu atma yeteneği) kullanılır.

### `appsettings.json` Yapılandırması
```json
{
  "AI": {
    "SemanticKernel": {
      "DeploymentName": "gpt-4",
      "Endpoint": "https://your-resource.openai.azure.com",
      "ApiKey": "azure-key" // Azure OpenAI için
    }
  }
}
```

### Dependency Injection (DI) Kurulumu
```csharp
using CKN.Sdk.AI.SemanticKernel;
using Microsoft.SemanticKernel;

builder.Services.AddCknSemanticKernel(kernel =>
{
    // Semantic Kernel native builder'ı kullanılır
    kernel.AddAzureOpenAIChatCompletion(
        deploymentName: "gpt-4",
        endpoint: "https://your-resource.openai.azure.com",
        apiKey: "azure-key"
    );
});
```

### Gerçek Hayat Kullanımı: "Otel Rezervasyonu Yapan Ajan" (Tool Calling)
LLM'e sadece metin göndermezsiniz; LLM'e elinizdeki fonksiyonların listesini de gönderirsiniz. LLM ne zaman kendi kendine bu fonksiyonu çağırması gerektiğine karar verir.

```csharp
using Microsoft.SemanticKernel;

// 1. LLM'in kullanabileceği fonksiyonları (Plugin) tanımlayın
public class HotelBookingPlugin
{
    [KernelFunction("BookHotel")]
    public string BookHotel(string location, string date)
    {
        // Gerçek API'ye istek at.
        Console.WriteLine($"{location} için {date} tarihinde rezervasyon yapılıyor...");
        return "Rezervasyon onaylandı. Rezervasyon Kodunuz: H-12345";
    }
}

public class TravelAgentService
{
    private readonly Kernel _kernel;

    public TravelAgentService(Kernel kernel)
    {
        _kernel = kernel;
        
        // Plugin'i Kernel'e ekle
        _kernel.Plugins.AddFromType<HotelBookingPlugin>();
    }

    public async Task<string> ProcessUserIntent(string userQuery)
    {
        // Örnek Kullanıcı Sorgusu: "Haftaya cuma günü Antalya'da bir otel ayarla"
        
        // Kernel, cümlenin niyetini anlar ve otomatik olarak 'BookHotel' fonksiyonunu çalıştırır!
        var result = await _kernel.InvokePromptAsync(userQuery);
        
        return result.ToString();
    }
}
```
