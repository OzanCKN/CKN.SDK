# Data Access (Veri Erişimi) Sağlayıcıları Kullanım Örnekleri

CKN.SDK, Entity Framework Core dışındaki hafif (Micro-ORM) ve yüksek performanslı veri erişim alternatiflerini standart Unit of Work (UoW) ve Repository mimarisi altında sunar.

---

## 1. Dapper Kullanımı

Dapper, doğrudan SQL sorguları yazarak Entity Framework'e kıyasla maksimum okuma performansı sağlamak istediğiniz senaryolar için idealdir.

### `appsettings.json` Yapılandırması
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=CknDb;User Id=sa;Password=yourStrong(!)Password;"
  }
}
```

### Dependency Injection (DI) Kurulumu
```csharp
using CKN.Sdk.Data.Dapper;
using Npgsql; // veya Microsoft.Data.SqlClient

// PostgreSql kullanacağımızı varsayalım
builder.Services.UseDapper(opt =>
{
    opt.ConnectionString = builder.Configuration.GetConnectionString("DefaultConnection");
    opt.DbConnectionFactory = (connectionString) => new NpgsqlConnection(connectionString);
});
```

### Gerçek Hayat Kullanımı: Gelişmiş Raporlama ve DapperRepository

Karmaşık JOIN işlemleri barındıran Raporlama ekranlarında veya çok yüksek verinin tek seferde çekildiği durumlarda Dapper hayat kurtarır.

```csharp
using CKN.Sdk.Core; // IRepository<T>, IUnitOfWork gibi core arayüzler
using Dapper;

public class Employee
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Department { get; set; }
    public decimal Salary { get; set; }
}

public class ReportingService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IRepository<Employee> _employeeRepository;

    public ReportingService(IUnitOfWork unitOfWork, IRepository<Employee> employeeRepository)
    {
        _unitOfWork = unitOfWork;
        _employeeRepository = employeeRepository;
    }

    public async Task CreateDepartmentAndTransferEmployeeAsync(Employee employee)
    {
        // Transactional Unit of Work Kullanımı
        await _unitOfWork.BeginTransactionAsync();

        try
        {
            // Yeni departman aç
            var connection = ((DapperUnitOfWork)_unitOfWork).Connection;
            var transaction = ((DapperUnitOfWork)_unitOfWork).Transaction;

            await connection.ExecuteAsync(
                "INSERT INTO Departments (Name) VALUES (@Name)", 
                new { Name = employee.Department }, 
                transaction
            );

            // Yeni personel kaydet (Repository deseni ile)
            await _employeeRepository.AddAsync(employee);

            // Hata çıkmazsa veritabanına yansıt
            await _unitOfWork.CommitTransactionAsync();
        }
        catch (Exception)
        {
            // Hata durumunda işlemi geri al
            await _unitOfWork.RollbackTransactionAsync();
            throw;
        }
    }
}
```

---

## 2. RepoDb Kullanımı

RepoDb, tıpkı Dapper gibi bir Micro-ORM'dir, ancak C#'a özgü `IQueryable` tarzı expression (LINQ) destekleriyle Bulk-Operations (Toplu İşlemler) konusunda çok daha yeteneklidir.

### `appsettings.json` Yapılandırması
```json
{
  "Data": {
    "RepoDb": {
      "ConnectionString": "Server=localhost;Database=CknDb;User Id=postgres;Password=mypassword;"
    }
  }
}
```

### Dependency Injection (DI) Kurulumu
```csharp
using CKN.Sdk.Data.RepoDb;

builder.Services.UseRepoDb(opt =>
{
    opt.ConnectionString = builder.Configuration.GetConnectionString("DefaultConnection");
});
```

### Gerçek Hayat Kullanımı: Milyonlarca Satırlık Bulk Insert (Log Yedekleme)

RepoDb'nin gücü, standart Entity Framework `.AddRange()` metodunun saatler sürebileceği milyonluk verileri, saniyeler içinde BulkInsert metoduyla veritabanına yazmasıdır.

```csharp
using CKN.Sdk.Data.RepoDb;
using RepoDb;

public class SensorData
{
    public Guid Id { get; set; }
    public int SensorId { get; set; }
    public double Temperature { get; set; }
    public DateTime Timestamp { get; set; }
}

public class IotDeviceService
{
    private readonly RepoDbRepository<SensorData> _sensorRepository;

    public IotDeviceService(IRepository<SensorData> sensorRepository)
    {
        _sensorRepository = (RepoDbRepository<SensorData>)sensorRepository;
    }

    public async Task ProcessAndSaveBulkDataAsync(List<SensorData> batchData)
    {
        // Standart repository'de olmayan RepoDb'ye özel BulkInsert yeteneği
        // 1.000.000 satırı birkaç saniye içerisinde aktarır.
        using var connection = new NpgsqlConnection("Server=...;");
        
        // Bu metot, PostgreSQL'in COPY özelliğini veya SQL Server'ın SqlBulkCopy'sini tetikler
        await connection.BulkInsertAsync(batchData);
    }
}
```
