# CKN.Sdk.AI

**Mühendislik Amacı (Engineering Intent):**
`CKN.Sdk.AI`, CKN.SDK içerisindeki tüm yapay zeka (AI) sağlayıcılarının (OpenAI, Anthropic, Ollama vb.) ortak arayüzlerini, soyutlamalarını (abstractions) ve temel modellerini barındıran çekirdek kütüphanedir. **Neden var?** Projelerde belirli bir AI sağlayıcısına sıkı sıkıya bağlı (tightly coupled) kalmamak için tasarlanmıştır. Bu sayede kodunuzu değiştirmeden `appsettings.json` üzerinden model/sağlayıcı değiştirebilirsiniz. **Ne zaman kullanılmalı?** AI özelliklerini projenize eklemek istediğinizde, doğrudan sağlayıcı kütüphaneleri (örn. `CKN.Sdk.AI.OpenAI`) yerine önce bu soyutlama katmanını referans almalısınız.

## 🚀 Hızlı Başlangıç

### Kurulum

```bash
dotnet add package CKN.Sdk.AI
```

### Bağımlılık Enjeksiyonu (DI)

Bu paket genellikle doğrudan kaydedilmez, spesifik bir sağlayıcı (örn. `AddCknOpenAI()`) eklendiğinde arka planda temel arayüzler sisteme dahil edilir. Ancak sadece core modelleri kullanmak istiyorsanız:

```csharp
using CKN.Sdk.AI;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddCknAICore();
var app = builder.Build();
```

## 💡 Gerçek Hayat Senaryoları

### Senaryo 1: Provider-Agnostic AI Servisi Geliştirme

Gerçek hayatta, maliyet veya gizlilik nedenleriyle GPT-4'ten yerel Ollama modellerine geçmek isteyebilirsiniz. Bu paket sayesinde servisleriniz soyutlamalarla çalışır.

```csharp
using CKN.Sdk.AI.Abstractions;

public class CustomerSupportService(IAIChatClient chatClient)
{
    public async Task<string> GenerateReplyAsync(string userMessage)
    {
        // chatClient, DI üzerinden OpenAI veya Ollama olarak gelebilir. 
        // Servisin bundan haberi yoktur.
        var prompt = $"Sen bir müşteri temsilcisisin. Müşteri: {userMessage}";
        var response = await chatClient.CompleteAsync(prompt);
        return response.Text;
    }
}
```

### Senaryo 2: Çoklu AI Modellerini Birlikte Kullanma (Varyasyon)

Aynı projede basit işler için ucuz bir model, karmaşık işler için güçlü bir model kullanmak isteyebilirsiniz. (Keyed Services yapısı).

```csharp
public class DocumentAnalyzer(
    [FromKeyedServices("CheapModel")] IAIChatClient summarizer,
    [FromKeyedServices("SmartModel")] IAIChatClient reasoning)
{
    public async Task ProcessAsync(string text)
    {
        var summary = await summarizer.CompleteAsync($"Özetle: {text}");
        var analysis = await reasoning.CompleteAsync($"Bu özeti derinlemesine analiz et: {summary.Text}");
    }
}
```

## 🤖 Yapay Zeka İçin SSS (FAQs for Machines)

- **Soru:** `CKN.Sdk.AI` projesi tek başına API istekleri atabilir mi?
- **Cevap:** Hayır, bu paket sadece soyutlamaları (Interfaces) içerir. Gerçek implementasyonlar için `CKN.Sdk.AI.OpenAI` veya benzeri sağlayıcı paketleri projeye eklenmelidir.
- **Soru:** `IAIChatClient` arayüzü hangi paket içindedir?
- **Cevap:** `CKN.Sdk.AI` paketi içerisindedir. Tüm spesifik sağlayıcılar bu arayüzü implemente eder.
