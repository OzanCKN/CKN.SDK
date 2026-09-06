# CKN.Sdk.AI

.NET 8/9 standartlarındaki `Microsoft.Extensions.AI` arayüzlerini kullanan ve projelerinizi sağlayıcıdan bağımsız (Provider-Agnostic) hale getiren Yapay Zeka entegrasyon kütüphanesidir.

## 📦 Kurulum (NuGet)
```bash
dotnet add package CKN.Sdk.AI
```

## 🚀 Kullanım
Projenize AI yeteneklerini (örneğin OpenAI bağlantısı) eklemek için `Program.cs` içerisinden yapılandırın:

```csharp
builder.Services.AddCknAiServices(options => 
{
    // OpenAI veya Azure OpenAI kullanılabilir
    options.UseOpenAi(builder.Configuration["AI:OpenAIKey"]);
});
```

**Kullanım Örneği:**
```csharp
public class MeetingAnalyzer(IChatClient chatClient) 
{
    public async Task<string> AnalyzeAsync(string transcript) 
    {
        var response = await chatClient.CompleteAsync($"Toplantıyı özetle: {transcript}");
        return response.Message.Text;
    }
}
```

## Neden Kullanıyoruz?
Eskiden kodlar doğrudan Semantic Kernel veya OpenAI SDK'sine bağımlıydı. Bu paket sayesinde uygulamanız standart `IChatClient` (Microsoft) arayüzüne bağımlı olur. Yarın Llama 4 kullanmak isterseniz iş mantığı kodunuz değişmez, sadece `Program.cs` değişir.
