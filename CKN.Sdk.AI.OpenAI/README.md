# CKN.Sdk.AI.OpenAI

CKN.Sdk içerisinde **OpenAI (GPT-4 vb.)** veya Azure OpenAI yapay zeka modelleriyle entegrasyonu sağlayan kütüphanedir.

## Yapılandırma (`appsettings.json`)

```json
{
  "AI": {
    "OpenAI": {
      "ApiKey": "sk-...",
      "DefaultModelId": "gpt-4o",
      "IsAzure": false,
      "Endpoint": ""
    }
  }
}
```

## Servis Kaydı (Dependency Injection)

```csharp
using CKN.Sdk.AI.OpenAI;

var builder = WebApplication.CreateBuilder(args);

// OpenAI Chat Client'ını sisteme dahil etme
builder.Services.AddCknOpenAIChatClient(builder.Configuration);

var app = builder.Build();
```

## Gerçek Hayat Kullanım Senaryosu

**Yapısal Veri (JSON) Çıkarma Aracı**
Serbest metin formatındaki fiş veya fatura detaylarından yapısal JSON çıkaran bir süreç.

```csharp
using System.Text.Json;
using Microsoft.Extensions.AI;

public class InvoiceParserService
{
    private readonly IChatClient _chatClient;

    public InvoiceParserService(IChatClient chatClient)
    {
        _chatClient = chatClient;
    }

    public async Task<InvoiceDto> ParseInvoiceAsync(string rawText)
    {
        var messages = new[]
        {
            new ChatMessage(ChatRole.System, "Aşağıdaki metinden fatura verilerini çıkart. Sadece JSON döndür."),
            new ChatMessage(ChatRole.User, rawText)
        };

        var options = new ChatOptions
        {
            ResponseFormat = ChatResponseFormat.Json
        };

        var response = await _chatClient.CompleteAsync(messages, options);
        return JsonSerializer.Deserialize<InvoiceDto>(response.Message.Text);
    }
}

public class InvoiceDto { public decimal TotalAmount { get; set; } public string VendorName { get; set; } }
```
