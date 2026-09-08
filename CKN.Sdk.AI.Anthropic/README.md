# CKN.Sdk.AI.Anthropic

CKN.Sdk içerisinde **Anthropic (Claude vb.)** yapay zeka modelleriyle entegrasyonu sağlayan kütüphanedir. `CKN.Sdk.AI` içerisindeki soyutlamalara uygun olarak çalışır.

## Yapılandırma (`appsettings.json`)

```json
{
  "AI": {
    "Anthropic": {
      "ApiKey": "sk-ant-api03-...",
      "DefaultModelId": "claude-3-opus-20240229"
    }
  }
}
```

## Servis Kaydı (Dependency Injection)

```csharp
using CKN.Sdk.AI.Anthropic;

var builder = WebApplication.CreateBuilder(args);

// Anthropic Chat Client'ını sisteme dahil etme
builder.Services.AddCknAnthropicChatClient(builder.Configuration);

var app = builder.Build();
```

## Gerçek Hayat Kullanım Senaryosu

**Müşteri Destek Asistanı**
Müşteri mesajlarını analiz edip, şirketin tonuna uygun yanıt veren bir Anthropic (Claude) servisi.

```csharp
using CKN.Sdk.AI;
using CKN.Sdk.AI.Anthropic;
using Microsoft.Extensions.AI;

public class CustomerSupportService
{
    private readonly IChatClient _chatClient;

    public CustomerSupportService(IChatClient chatClient)
    {
        _chatClient = chatClient;
    }

    public async Task<string> GenerateReplyAsync(string customerMessage, CancellationToken cancellationToken = default)
    {
        var messages = new List<ChatMessage>
        {
            new ChatMessage(ChatRole.System, "Sen kibar, yardımsever ve kurumsal bir müşteri hizmetleri asistanısın. Kısa ve net cevap ver."),
            new ChatMessage(ChatRole.User, customerMessage)
        };

        // Claude modelinden yanıt alır. Arka planda Anthropic SDK'sını kullanır.
        var response = await _chatClient.CompleteAsync(messages, cancellationToken: cancellationToken);

        return response.Message.Text;
    }
}
```
