# Sprint 6: Finance SDK Gap Closure

> **AI AGENT Ä°Ã‡Ä°N ZORUNLU BÄ°LDÄ°RÄ°M:** Bu sprint planÄ±nÄ± incelerken **KESÄ°NLÄ°KLE `.agents/rules.md`** dosyasÄ±ndaki kurallarÄ± gÃ¶zetmek zorundasÄ±n. Mimari deÄŸiÅŸiklikler `decision-log.md`'ye, yeni modÃ¼ller `system-glossary.md`'ye kaydedilmeli.

## ðŸŽ¯ Sprint Hedefi

CKN.Finance projesinde Ã¼Ã§ kritik eksiklik nedeniyle SDK sarmalayÄ±cÄ±sÄ± kuralÄ± Ã§iÄŸnenmek zorunda kalÄ±ndÄ± (Finance Decision 13). Bu sprint, o boÅŸluklarÄ± kapatarak Finance'in `CKN.Sdk.*` paketleri Ã¼zerinden tÃ¼m altyapÄ±yÄ± kullanmasÄ±nÄ± saÄŸlar.

Ä°lk eksiklik, HTTP istemci altyapÄ±sÄ±dÄ±r: Finance'te Ã¼Ã§ dÄ±ÅŸ fiyat saÄŸlayÄ±cÄ±sÄ± ham `HttpClient` ile yazÄ±lmÄ±ÅŸ, her birinde elle yazÄ±lmÄ±ÅŸ rate limiting (`Task.Delay`), retry ve maskeleme mantÄ±ÄŸÄ± bulunmaktadÄ±r. Ä°kinci eksiklik, piyasa takvimi ve portfÃ¶y matematiksel hesaplamalarÄ±nÄ±n handler'lara daÄŸÄ±lmÄ±ÅŸ olmasÄ±dÄ±r. ÃœÃ§Ã¼ncÃ¼sÃ¼ `CKN.Sdk.AI.OpenAI` paketindeki NU1608 baÄŸÄ±mlÄ±lÄ±k uyarÄ±sÄ± ve eksik XML dokÃ¼mantasyonudur.

## ðŸ“Š Kapsam ve Ä°sterler

- `CKN.Sdk.Network` â€” HTTP istemci soyutlamasÄ± (ICknHttpClient, CknHttpClientOptions, auth stratejileri)
- `CKN.Sdk.Network.Http` â€” Raw HttpClient provider (Microsoft.Extensions.Http.Resilience tabanlÄ±)
- `CKN.Sdk.AI` XML docs + `CKN.Sdk.AI.OpenAI` OutputType bug fix + NU1608 Ã§Ã¶zÃ¼mÃ¼
- `CKN.Sdk.Financial` â€” Piyasa takvimi (BIST/NYSE/NASDAQ/Crypto), OHLC aggregation, portfÃ¶y matematiÄŸi

## ðŸ›  Teknik Mimari Kararlar

- Network paketi provider-agnostic: `CKN.Sdk.Network` (abstraction) + `CKN.Sdk.Network.Http` (Faz 1). Flurl/RestSharp/Refit provider'larÄ± Faz 2'ye bÄ±rakÄ±ldÄ±.
- SÄ±fÄ±r yeni 3rd-party baÄŸÄ±mlÄ±lÄ±k: `Microsoft.Extensions.Http.Resilience` zaten `Directory.Packages.props`'ta, `System.Threading.RateLimiting` .NET 7+ built-in.
- `CKN.Sdk.Financial` iÃ§in NodaTime yerine .NET 6+ built-in IANA timezone desteÄŸi (`TimeZoneInfo`) tercih edildi.
- Decision-log kayÄ±tlarÄ± yapÄ±ldÄ± (ADR-009, ADR-010).

## ðŸ“ Task Listesi

- [x] **Task 007:** CKN.Sdk.Network + CKN.Sdk.Network.Http GeliÅŸtirimi (`tasks/task-007-network-http-client.md`)
- [x] **Task 008:** CKN.Sdk.AI Bug Fix + XML Docs + NU1608 (`tasks/task-008-ai-dependency-fix.md`)
- [x] **Task 009:** CKN.Sdk.Financial â€” Piyasa Takvimi + PortfÃ¶y MatematiÄŸi (`tasks/task-009-financial.md`)

## âš ï¸ Riskler ve DarboÄŸazlar

- `Microsoft.Extensions.AI.OpenAI` sÃ¼rÃ¼m yÃ¼kseltmesi `IChatClient` API deÄŸiÅŸikliÄŸi iÃ§eriyorsa breaking change riski; test ile doÄŸrula.
- `CKN.Sdk.Financial` tatil verileri static olarak gÃ¶mÃ¼ldÃ¼ÄŸÃ¼ iÃ§in yÄ±llÄ±k gÃ¼ncelleme gerektirir â€” `IHolidayProvider` interface'i extensibility saÄŸlar.

