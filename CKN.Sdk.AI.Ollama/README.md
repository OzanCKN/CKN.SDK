# CKN.Sdk.AI.Ollama

CKN.Sdk içerisinde **Ollama** destekli açık kaynak (Local) modeller (Llama, Mistral vb.) ile haberleşmeyi sağlayan kütüphanedir. KVKK (GDPR) hassasiyeti olan senaryolarda tercih edilir.

## Yapılandırma (`appsettings.json`)

```json
{
  "AI": {
    "Ollama": {
      "Endpoint": "http://localhost:11434",
      "DefaultModelId": "llama3"
    }
  }
}
```

## Servis Kaydı (Dependency Injection)

```csharp
using CKN.Sdk.AI.Ollama;

var builder = WebApplication.CreateBuilder(args);

// Ollama Chat Client'ını sisteme dahil etme
builder.Services.AddCknOllamaChatClient(builder.Configuration);

var app = builder.Build();
```

## Gerçek Hayat Kullanım Senaryosu

**Hassas Veri Sınıflandırma (KVKK Uyumlu)**
Şirket içi hassas evrakları internete çıkarmadan lokal model üzerinden sınıflandırmak.

```csharp
using CKN.Sdk.AI;
using Microsoft.Extensions.AI;

public class DocumentClassificationService
{
    private readonly IChatClient _chatClient;

    public DocumentClassificationService(IChatClient chatClient)
    {
        _chatClient = chatClient;
    }

    public async Task<string> ClassifyDocumentAsync(string documentContent)
    {
        var prompt = $@"Aşağıdaki metni 'Finansal', 'Hukuki' veya 'İK' olarak sınıflandır. Sadece kategori adını döndür.
Metin:
{documentContent}";

        // Veri lokal (Ollama) sunucuya gider, internete çıkmaz.
        var result = await _chatClient.CompleteAsync(prompt);

        return result.Message.Text.Trim();
    }
}
```
