# CKN.Sdk.Storage.Azure

CKN.Sdk içerisinde, Microsoft Azure'un **Azure Blob Storage** hizmetiyle haberleşmeyi sağlayan entegrasyon kütüphanesidir. `CKN.Sdk.Storage` altyapısındaki `IStorageClient` arayüzünü uygular. Microsoft'un kendi kurumsal (Enterprise) depolama çözümüne doğrudan erişim sağlar.

## Yapılandırma (`appsettings.json`)

```json
{
  "Storage": {
    "Azure": {
      "ConnectionString": "DefaultEndpointsProtocol=https;AccountName=cknstorage;AccountKey=...;EndpointSuffix=core.windows.net",
      "ContainerName": "uploads"
    }
  }
}
```

## Servis Kaydı (Dependency Injection)

```csharp
using CKN.Sdk.Storage.Azure;

var builder = WebApplication.CreateBuilder(args);

// Azure Blob Storage altyapısını sisteme dahil etme
builder.Services.AddCknAzureStorage(builder.Configuration);

var app = builder.Build();
```

## Gerçek Hayat Kullanım Senaryosu

**PDF Fatura veya Rapor Arşivleme**
Kullanıcılara veya sistemlere ait üretilen PDF belgelerinin Azure Blob'a atılıp güvenle arşivlenmesi.

```csharp
using CKN.Sdk.Storage;

public class InvoiceArchiveService
{
    private readonly IStorageClient _storageClient;

    public InvoiceArchiveService(IStorageClient storageClient)
    {
        _storageClient = storageClient;
    }

    public async Task ArchiveInvoiceAsync(string invoiceId, Stream pdfStream)
    {
        // Container: "invoices", Path: "2026/09/INV-1001.pdf"
        var path = $"{DateTime.Now.Year}/{DateTime.Now.Month:D2}/INV-{invoiceId}.pdf";
        
        await _storageClient.UploadAsync("invoices", path, pdfStream, "application/pdf");
    }
}
```
