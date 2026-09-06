using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using CKN.Sdk.Core.Domain;
using System.Linq;

namespace CKN.Sdk.EntityFramework.Interceptors;

/// <summary>
/// Intercepts DbContext SaveChanges calls to extract Domain Events from entities
/// before they are saved to the database. These events will be relayed to the Outbox.
/// </summary>
public sealed class DomainEventInterceptor : SaveChangesInterceptor
{
    /// <summary>
    /// Triggered before SaveChangesAsync completes.
    /// </summary>
    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        var dbContext = eventData.Context;
        if (dbContext is null) return base.SavingChangesAsync(eventData, result, cancellationToken);

        var entries = dbContext.ChangeTracker
            .Entries<IHasDomainEvents>()
            .ToList();

        // In a real Outbox scenario, we would serialize these events and write them 
        // to an OutboxMessage table here within the same transaction.
        // For Sprint 2 Task 1, we just clear them to prevent memory leaks,
        // pending the MassTransit Outbox configuration in Task 3.

        foreach (var entry in entries)
        {
            entry.Entity.ClearDomainEvents();
        }

        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }
}
