using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using CKN.Sdk.Core.Data;
using CKN.Sdk.Core.Domain;
using Microsoft.EntityFrameworkCore;

namespace CKN.Sdk.EntityFramework;

/// <summary>
/// EF Core implementation of the generic repository.
/// </summary>
public class Repository<TEntity, TId> : IRepository<TEntity, TId> where TEntity : Entity<TId>
{
    protected readonly CknDbContext DbContext;
    protected readonly DbSet<TEntity> DbSet;

    public Repository(CknDbContext dbContext)
    {
        DbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        DbSet = dbContext.Set<TEntity>();
    }

    public async Task<TEntity?> GetByIdAsync(TId id, CancellationToken cancellationToken = default)
    {
        return await DbSet.FindAsync(new object[] { id! }, cancellationToken);
    }

    public IAsyncEnumerable<TEntity> FindAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
    {
        return DbSet.Where(predicate).AsAsyncEnumerable();
    }

    public async Task AddAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        await DbSet.AddAsync(entity, cancellationToken);
    }

    public void Update(TEntity entity)
    {
        DbSet.Update(entity);
    }

    public void Delete(TEntity entity)
    {
        DbSet.Remove(entity);
    }
}
