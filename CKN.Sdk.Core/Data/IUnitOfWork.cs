using System;
using System.Threading;
using System.Threading.Tasks;

namespace CKN.Sdk.Core.Data;

/// <summary>
/// Represents the Unit of Work interface for committing transactions.
/// </summary>
public interface IUnitOfWork : IDisposable
{
    /// <summary>
    /// Saves all changes made in this context to the database.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The number of state entries written to the database.</returns>
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
