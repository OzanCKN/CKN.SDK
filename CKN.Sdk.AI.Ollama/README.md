# CKN.Sdk.AI.Ollama

**Mühendislik Amacı (Engineering Intent):**
`CKN.Sdk.AI.Ollama`, `CKN.Sdk.AI` soyutlamalarını kullanarak yerel (local) makinede veya on-premise sunucularda koşan açık kaynaklı AI modelleriyle (Llama 3, Mistral, Qwen vb.) konuşmayı sağlayan entegrasyon kütüphanesidir. **Neden var?** Veri gizliliğinin kritik olduğu kurumsal ortamlarda (on-premise), internete açık API'lere (OpenAI, Anthropic) veri göndermeden AI yeteneklerini kullanabilmek için vardır. **Ne zaman kullanılmalı?** Projede veri güvenliği regülasyonları yüksekse (KVKK/GDPR), maliyetleri düşürmek amacıyla kendi sunucularınızda host ettiğiniz Ollama servisi üzerinden model koşturulacaksa tercih edilmelidir.

## 🚀 Hızlı Başlangıç

### Kurulum

```bash
dotnet add package CKN.Sdk.AI.Ollama
```

### Konfigürasyon (`appsettings.json`)

```json
{
  "AI": {
    "Ollama": {
      "Endpoint": "http://localhost:11434",
      "DefaultModel": "llama3.1",
      "TimeoutSeconds": 120
    }
  }
}
```

### Bağımlılık Enjeksiyonu (DI)

```csharp
using CKN.Sdk.AI.Ollama;

var builder = WebApplication.CreateBuilder(args);

// Ollama istemcisini sisteme kaydeder.
builder.Services.AddCknOllama(builder.Configuration);

var app = builder.Build();
```

## 💡 Gerçek Hayat Senaryoları

### Senaryo 1: Mahrem Veri İçeren Şirket İçi Raporlama

İnternete çıkması yasak olan müşteri verilerinin analizi.

```csharp
using CKN.Sdk.AI.Abstractions;

public class InternalReportGenerator(IAIChatClient chatClient)
{
    public async Task<string> SummarizeSensitiveDataAsync(string privateData)
    {
        // Bu istek dışarıdaki bir buluta değil, şirket içi Ollama sunucusuna gider.
        var prompt = $"Bu gizli müşteri verisinden sadece temel şikayet başlıklarını çıkar: {privateData}";
        var response = await chatClient.CompleteAsync(prompt);
        return response.Text;
    }
}
```

### Senaryo 2: Kodlama Asistanı (Farklı Model Seçimi)

Ollama üzerinde `qwen2.5-coder` veya `codellama` gibi spesifik modellere anlık istek atma (Varyasyon).

```csharp
public async Task<string> GenerateCodeSnippetAsync(IAIChatClient chatClient, string taskDescription)
{
    var options = new AIChatOptions
    {
        ModelId = "qwen2.5-coder", // Default modeli kodlamaya özel modelle ezer
        Temperature = 0.1f // Kod üretimi için halüsinasyonu düşürür
    };

    var response = await chatClient.CompleteAsync($"C# dilinde şu işi yapan kodu yaz: {taskDescription}", options);
    return response.Text;
}
```

## 🤖 Yapay Zeka İçin SSS (FAQs for Machines)

- **Soru:** Bu paketi kullanmak için Ollama'nın kurulu olması şart mı?
- **Cevap:** Evet. Uygulamanın çalışacağı sunucuda veya ağda erişilebilir bir Ollama instance'ı (servisi) ayakta olmalı ve modeller indirilmiş (örn: `ollama run llama3.1`) olmalıdır.
- **Soru:** `CKN.Sdk.AI.Ollama` API key gerektirir mi?
- **Cevap:** Hayır, Ollama varsayılan olarak API Key gerektirmez, sadece `Endpoint` (URL) bilgisine ihtiyaç duyar.
