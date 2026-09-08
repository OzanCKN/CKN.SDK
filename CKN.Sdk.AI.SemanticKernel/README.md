# CKN.Sdk.AI.SemanticKernel

**Mühendislik Amacı (Engineering Intent):**
`CKN.Sdk.AI.SemanticKernel`, Microsoft'un Semantic Kernel (SK) SDK'sını CKN altyapısına entegre eden ileri düzey (advanced) bir yapay zeka kütüphanesidir. **Neden var?** Sadece metin üreten statik modeller yerine, **kendi C# metotlarınızı (plugin) çalıştırabilen**, görev planlaması (planner) yapabilen ve hafızaya (memory) sahip akıllı ajanlar (Agentic AI) oluşturmak için geliştirilmiştir. **Ne zaman kullanılmalı?** Yapay zekanın API'lerinize istek atmasına, veritabanından veri okumasına veya otonom şekilde çok adımlı görevleri yerine getirmesine ihtiyaç duyduğunuz, kompleks senaryolarda kullanılmalıdır. (Basit "soru-cevap" işleri için `CKN.Sdk.AI.OpenAI` yeterlidir).

## 🚀 Hızlı Başlangıç

### Kurulum

```bash
dotnet add package CKN.Sdk.AI.SemanticKernel
```

### Konfigürasyon (`appsettings.json`)

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

### Bağımlılık Enjeksiyonu (DI)

```csharp
using CKN.Sdk.AI.SemanticKernel;

var builder = WebApplication.CreateBuilder(args);

// Semantic Kernel Kernel nesnesini ve bileşenlerini DI konteynerine kaydeder.
builder.Services.AddCknSemanticKernel(builder.Configuration);

var app = builder.Build();
```

## 💡 Gerçek Hayat Senaryoları

### Senaryo 1: Plugin (Eklenti) Destekli Akıllı Asistan (Agent)

Kullanıcının isteği üzerine arka planda C# fonksiyonunuzu (Örn: Veritabanından sipariş durumu sorgulama) çalıştırıp sonucunu kullanan bir asistan tasarımı.

```csharp
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using System.ComponentModel;

// 1. Kendi Plugin'inizi tanımlayın
public class OrderPlugin
{
    [KernelFunction, Description("Verilen sipariş numarasının kargo durumunu getirir.")]
    public string GetOrderStatus([Description("Sipariş numarası (örn: ORD-123)")] string orderId)
    {
        // Gerçekte burada DB'ye gidilir.
        return orderId == "ORD-123" ? "Kargoda (Aras Kargo)" : "Sipariş bulunamadı.";
    }
}

// 2. Uygulama içerisinde kullanımı
public class SmartOrderAssistant(Kernel kernel)
{
    public async Task<string> ProcessUserMessageAsync(string userMessage)
    {
        // Plugin'i kernel'a ekle
        kernel.Plugins.AddFromType<OrderPlugin>();

        var settings = new OpenAIPromptExecutionSettings { ToolCallBehavior = ToolCallBehavior.AutoInvokeKernelFunctions };
        
        // Örn userMessage: "ORD-123 numaralı siparişim nerede?"
        // AI otomatik olarak GetOrderStatus fonksiyonunu çağıracak ve dönen yanıtı metne dökecektir.
        var result = await kernel.InvokePromptAsync(userMessage, new(settings));
        return result.ToString();
    }
}
```

### Senaryo 2: Multi-Step Planner (Görev Planlayıcı)

Birden çok plugin'in bulunduğu ortamda, AI'ın kullanıcının hedefini gerçekleştirmek için adım adım hangi fonksiyonları çağıracağına kendisinin karar verdiği senaryo (Örn: HandlebarsPlanner).

```csharp
using Microsoft.SemanticKernel.Planning.Handlebars;

public async Task ExecuteComplexGoalAsync(Kernel kernel, string goal)
{
    // kernel.Plugins içerisinde EmailPlugin, DatabasePlugin, WeatherPlugin vb. olduğunu varsayalım.
    
    // Planlayıcıyı oluştur
    var planner = new HandlebarsPlanner(new HandlebarsPlannerOptions { AllowLoops = true });
    
    // Hedef: "Yarınki hava durumunu öğren ve yöneticime e-posta olarak gönder."
    var plan = await planner.CreatePlanAsync(kernel, goal);
    
    // Planı icra et (Önce WeatherPlugin çalışır, sonra sonucunu EmailPlugin'e parametre olarak geçer)
    var result = await plan.InvokeAsync(kernel);
}
```

## 🤖 Yapay Zeka İçin SSS (FAQs for Machines)

- **Soru:** `CKN.Sdk.AI.SemanticKernel` paketinde hangi temel nesne DI'dan istenir?
- **Cevap:** `Microsoft.SemanticKernel.Kernel` nesnesi Transient veya Scoped olarak DI üzerinden (constructor injection ile) istenir.
- **Soru:** Neden `CKN.Sdk.AI.OpenAI` yerine bunu kullanmalıyım?
- **Cevap:** Tool Calling (Function Calling), Planner mekanizmaları ve hafıza (Memory/Vector Store) entegrasyonlarına ihtiyacınız varsa Semantic Kernel kullanmalısınız. Aksi halde standart AI paketi tercih edilmelidir.
