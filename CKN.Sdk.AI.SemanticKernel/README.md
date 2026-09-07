# CKN.Sdk.AI.SemanticKernel

CKN.Sdk içerisinde **Microsoft Semantic Kernel** kullanarak daha kompleks AI senaryoları (Pluginler, Memory, Planner vb.) oluşturulmasını sağlayan entegrasyon kütüphanesidir.

## Yapılandırma (`appsettings.json`)

```json
{
  "SemanticKernel": {
    "OpenAI": {
      "ApiKey": "sk-...",
      "ModelId": "gpt-4o"
    }
  }
}
```

## Servis Kaydı (Dependency Injection)

```csharp
using CKN.Sdk.AI.SemanticKernel;

var builder = WebApplication.CreateBuilder(args);

// Semantic Kernel ve varsayılan bağımlılıkları ekler
builder.Services.AddCknSemanticKernel(builder.Configuration);

var app = builder.Build();
```

## Gerçek Hayat Kullanım Senaryosu

**Eklenti Destekli Akıllı Asistan (Agent)**
Sadece metin üreten değil, arka planda C# fonksiyonlarınızı çalıştırabilen bir ajan oluşturma.

```csharp
using Microsoft.SemanticKernel;
using System.ComponentModel;

public class EmailPlugin
{
    [KernelFunction, Description("Belirtilen kişiye e-posta gönderir.")]
    public string SendEmail(
        [Description("Alıcının e-posta adresi")] string to, 
        [Description("E-posta içeriği")] string content)
    {
        // Burada gerçek SMTP e-posta gönderme mantığı olur
        return $"E-posta {to} adresine başarıyla gönderildi.";
    }
}

public class AgentService
{
    private readonly Kernel _kernel;

    public AgentService(Kernel kernel)
    {
        _kernel = kernel;
        // Plugini kernel'a ekliyoruz
        _kernel.Plugins.AddFromType<EmailPlugin>();
    }

    public async Task<string> ExecuteAgentTask(string userPrompt)
    {
        // Model, görevi yerine getirmek için gerekirse EmailPlugin'i tetikleyecektir.
        var result = await _kernel.InvokePromptAsync(userPrompt);
        return result.GetValue<string>();
    }
}
```
