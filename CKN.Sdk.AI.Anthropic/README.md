# CKN.Sdk.AI.Anthropic

**Mühendislik Amacı (Engineering Intent):**
`CKN.Sdk.AI.Anthropic`, `CKN.Sdk.AI` soyutlamalarını Anthropic Claude (Claude 3.5 Sonnet, Opus vb.) modelleri için uygulayan (implemente eden) entegrasyon kütüphanesidir. **Neden var?** Uzun bağlam (context window) gerektiren, yüksek seviye akıl yürütme (reasoning) ve kod/metin analizi görevlerinde Claude modellerini sisteme sorunsuzca entegre etmek için geliştirilmiştir. **Ne zaman kullanılmalı?** Projede Anthropic API'leri kullanılacaksa ve `IAIChatClient` arayüzünün arkasında Claude modellerinin çalışması isteniyorsa tercih edilmelidir.

## 🚀 Hızlı Başlangıç

### Kurulum

```bash
dotnet add package CKN.Sdk.AI.Anthropic
```

### Konfigürasyon (`appsettings.json`)

```json
{
  "AI": {
    "Anthropic": {
      "ApiKey": "sk-ant-...",
      "DefaultModel": "claude-3-5-sonnet-20240620",
      "MaxTokens": 4096
    }
  }
}
```

### Bağımlılık Enjeksiyonu (DI)

```csharp
using CKN.Sdk.AI.Anthropic;

var builder = WebApplication.CreateBuilder(args);

// Anthropic istemcisini sisteme kaydeder.
builder.Services.AddCknAnthropic(builder.Configuration);

var app = builder.Build();
```

## 💡 Gerçek Hayat Senaryoları

### Senaryo 1: Uzun Belge Analizi (Context Window Kullanımı)

Claude, çok büyük belgeleri analiz etmede (örneğin yasal sözleşmeler) oldukça başarılıdır.

```csharp
using CKN.Sdk.AI.Abstractions;

public class LegalDocumentAnalyzer(IAIChatClient chatClient)
{
    public async Task<string> FindAnomaliesAsync(string fullContractText)
    {
        var request = new AIChatRequest
        {
            SystemPrompt = "Sen kıdemli bir avukatsın. Aşağıdaki sözleşmedeki riskli maddeleri bul.",
            Messages = [ new ChatMessage("user", fullContractText) ]
        };
        
        var response = await chatClient.SendAsync(request);
        return response.Content;
    }
}
```

### Senaryo 2: Gelişmiş Ayarlar (Temperature ve TopP)

Metin yazarlığı gibi daha yaratıcı çıktılar beklenen senaryolarda parametreleri (Varyasyon) ezme (override):

```csharp
public async Task<string> GenerateCreativeStoryAsync(IAIChatClient chatClient)
{
    var options = new AIChatOptions
    {
        Temperature = 0.9f, // Yaratıcılığı artırır
        TopP = 0.8f,
        ModelId = "claude-3-opus-20240229" // Varsayılan modeli ezer
    };

    var response = await chatClient.CompleteAsync("Bana siberpunk temalı kısa bir hikaye yaz.", options);
    return response.Text;
}
```

## 🤖 Yapay Zeka İçin SSS (FAQs for Machines)

- **Soru:** `CKN.Sdk.AI.Anthropic` hangi interface'leri kaydeder?
- **Cevap:** Bu paket DI konteynerine `IAIChatClient` arayüzünün Anthropic tabanlı implementasyonunu kaydeder.
- **Soru:** Hangi modelleri destekliyor?
- **Cevap:** Claude 3 (Haiku, Sonnet, Opus) ve Claude 3.5 serisi başta olmak üzere güncel Anthropic API modellerini destekler.
