# Task 008: CKN.Sdk.AI Bug Fix + XML Docs + NU1608

> **AI AGENT ZORUNLU:** `.agents/rules.md` okundu, Zero-Warning Policy uygulanacak.

## 1. 🎯 Görev Amacı

Üç küçük ama kritik düzeltme:
1. `CKN.Sdk.AI.OpenAI.csproj`'ta hatalı `<OutputType>Exe</OutputType>` satırı var — provider paketi library olmalı.
2. `ICknAiChatService`, `CknAiRequest`, `CknAiResponse` tiplerinde XML doc yok.
3. NU1608: `Microsoft.Extensions.AI.OpenAI 10.9.0` `OpenAI >= 2.12.0 && < 2.13.0` istiyor, Finance 2.14.0 resolve ediyor.

## 3. 🛠 Teknik Değişiklikler

| Dosya | Değişiklik |
|---|---|
| `CKN.Sdk.AI.OpenAI/CKN.Sdk.AI.OpenAI.csproj` | `<OutputType>Exe</OutputType>` satırını sil |
| `CKN.Sdk.AI/Abstractions/ICknAiChatService.cs` | Tüm public type ve member'lara `/// <summary>` ekle |
| `Directory.Packages.props` | `Microsoft.Extensions.AI.OpenAI` + `OpenAI` sürümlerini uyumlu şekilde pinle |

## 5. ⚠️ Breaking Change Değerlendirmesi

- `OutputType` kaldırma → breaking change yok (hatalı davranış düzeltiliyor)
- XML doc → breaking change yok
- Paket versiyonu yükseltme → `IChatClient` Microsoft.Extensions.AI arayüzü değişmediği sürece breaking change yok
