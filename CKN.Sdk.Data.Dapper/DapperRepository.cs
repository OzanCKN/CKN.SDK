using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using CKN.Sdk.Core.Data;
using CKN.Sdk.Core.Domain;
using Dapper;
using Dommel;

namespace CKN.Sdk.Data.Dapper;

public class DapperRepository<TEntity, TId> : IRepository<TEntity, TId> where TEntity : Entity<TId>
{
    private readonly IDbConnection _dbConnection;
    private readonly IDbTransaction? _dbTransaction;

    public DapperRepository(IDbConnection dbConnection, IDbTransaction? dbTransaction = null)
    {
        _dbConnection = dbConnection;
        _dbTransaction = dbTransaction;
    }

    public Task<TEntity?> GetByIdAsync(TId id, CancellationToken cancellationToken = default)
    {
        return _dbConnection.GetAsync<TEntity>(id!, transaction: _dbTransaction);
    }

    public async IAsyncEnumerable<TEntity> FindAsync(Expression<Func<TEntity, bool>> predicate, [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var entities = await _dbConnection.SelectAsync(predicate, transaction: _dbTransaction);
        foreach (var entity in entities)
        {
            if (cancellationToken.IsCancellationRequested)
                yield break;
                
            yield return entity;
        }
    }

    public Task AddAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        return _dbConnection.InsertAsync(entity, transaction: _dbTransaction);
    }

    public void Update(TEntity entity)
    {
        _dbConnection.Update(entity, transaction: _dbTransaction);
    }

    public void Delete(TEntity entity)
    {
        _dbConnection.Delete(entity, transaction: _dbTransaction);
    }
}
