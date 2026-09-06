namespace CKN.Sdk.Core.Domain;

/// <summary>
/// The base class for all Domain Entities.
/// Provides a typed Identifier and encapsulates Domain Event dispatching.
/// </summary>
/// <typeparam name="TId">The type of the primary key (e.g., Guid, int).</typeparam>
public abstract class Entity<TId> : IHasDomainEvents
{
    /// <summary>
    /// The unique identifier of the entity.
    /// </summary>
    public virtual TId Id { get; protected set; } = default!;

    private readonly List<IDomainEvent> _domainEvents = new();

    /// <summary>
    /// A read-only collection of domain events currently registered on this entity.
    /// </summary>
    public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    /// <summary>
    /// Adds a new domain event to the entity's internal queue.
    /// </summary>
    /// <param name="domainEvent">The event to add.</param>
    public void AddDomainEvent(IDomainEvent domainEvent)
    {
        _domainEvents.Add(domainEvent);
    }

    /// <summary>
    /// Clears all registered domain events from the entity.
    /// Used by the DbContext/Interceptor after events have been dispatched.
    /// </summary>
    public void ClearDomainEvents()
    {
        _domainEvents.Clear();
    }
}
