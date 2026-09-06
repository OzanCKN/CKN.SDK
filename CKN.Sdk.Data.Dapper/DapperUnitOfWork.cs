using System;
using System.Data;
using System.Threading;
using System.Threading.Tasks;
using CKN.Sdk.Core.Data;

namespace CKN.Sdk.Data.Dapper;

public class DapperUnitOfWork : IUnitOfWork, IDisposable
{
    private readonly IDbConnection _connection;
    private IDbTransaction? _transaction;
    private bool _disposed;

    public DapperUnitOfWork(IDbConnection connection)
    {
        _connection = connection;
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        if (_transaction == null)
            throw new InvalidOperationException("No active transaction.");
            
        try
        {
            _transaction.Commit();
        }
        catch
        {
            _transaction.Rollback();
            throw;
        }
        finally
        {
            _transaction.Dispose();
            _transaction = null;
        }
        
        return await Task.FromResult(1);
    }

    public void BeginTransaction()
    {
        if (_connection.State != ConnectionState.Open)
            _connection.Open();
            
        _transaction = _connection.BeginTransaction();
    }

    public void Dispose()
    {
        if (_disposed) return;
        
        _transaction?.Dispose();
        _connection?.Dispose();
        
        _disposed = true;
        GC.SuppressFinalize(this);
    }
}
