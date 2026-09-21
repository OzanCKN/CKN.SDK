using System.Threading;
using System.Threading.Tasks;

namespace CKN.Sdk.Core.Domain;

/// <summary>
/// Represents a dispatcher for domain events. Implementations (e.g., MediatR or an EventBus) 
/// will handle the actual broadcasting of these events.
/// </summary>
public interface IDomainEventDispatcher
{
    /// <summary>
    /// Dispatches the specified domain event.
    /// </summary>
    /// <param name="domainEvent">The domain event to dispatch.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task DispatchAsync(IDomainEvent domainEvent, CancellationToken cancellationToken = default);
}
