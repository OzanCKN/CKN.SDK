using System;
using System.Threading;
using System.Threading.Tasks;
using CKN.Sdk.Core.Data;

namespace CKN.Sdk.EntityFramework;

/// <summary>
/// EF Core implementation of the Unit of Work.
/// </summary>
public class UnitOfWork : IUnitOfWork
{
    private readonly CknDbContext _dbContext;

    public UnitOfWork(CknDbContext dbContext)
    {
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public void Dispose()
    {
        _dbContext.Dispose();
        GC.SuppressFinalize(this);
    }
}
