# NuGet Paketi Yayınlama Stratejisi (Publishing)

CKN.SDK, tüm mikroservisler tarafından paylaşılan merkezi bir kütüphanedir. Herhangi bir proje (`CKN.Sdk.Core`, `CKN.Sdk.Notification`, `CKN.Sdk.Messaging` vb.) güncellendiğinde, bunun bir NuGet paketi olarak iç ağda veya dış dünyada (nuget.org vb.) yayınlanması gerekir.

## 1. Otomatik Paketleme (Pack)
Projede `Directory.Build.props` kullanılarak ortak NuGet meta verileri tanımlanmıştır. Tüm projeler derlendiğinde otomatik olarak `.nupkg` paketleri `bin/Release` veya `bin/Debug` klasörüne çıkar.

- Paket oluşturmak için terminalden:
  ```bash
  dotnet pack CKN.SDK.sln -c Release
  ```

## 2. Yerel Test Paketi Yayınlama (Local Feed)
Uzak sunucuya (CI/CD) gitmeden önce kendi makinenizde başka bir projede test etmek için `pack-local.ps1` betiğini kullanabilirsiniz. Bu betik paketleri oluşturup yerel NuGet havuzunuza atar.

- Kullanım:
  ```powershell
  .\pack-local.ps1
  ```

## 3. CI/CD Otomasyonu (GitHub Actions)
`main` dalına (branch) her push işlemi yapıldığında otomatik olarak GitHub Actions devreye girer.
- `.github/workflows/publish.yml` dosyası, tüm `CKN.Sdk.*` projelerini derler ve paketler.
- Başarılı olan paketler, GitHub Packages veya nuget.org ortamına pushlanır.

## 4. Versiyonlama (Semantic Versioning)
Versiyonlama `Directory.Build.props` içinden yönetilir. Büyük (Major), Küçük (Minor) ve Yama (Patch) kurallarına göre `1.0.0-preview.1` gibi isimlendirmeler kullanılır. Versiyon numarası yükseltilmeden yayınlanan paketler (aynı numarayla) NuGet tarafından genellikle **reddedilir**.

## 5. Yeni Proje Ekleme
Eğer `CKN.Sdk.Notification` gibi yeni bir modül eklediyseniz:
1. `dotnet sln add CKN.Sdk.Notification\CKN.Sdk.Notification.csproj` ile projeyi çözüme (Solution) ekleyin.
2. `Directory.Packages.props` dosyasına bağımlılıklarını kaydedin.
3. `dotnet pack` sırasında bu proje de otomatik paketlenecektir. Ekstra bir ayar yapmanıza gerek yoktur.
