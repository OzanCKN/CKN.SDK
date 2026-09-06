using System.Collections.Generic;

namespace CKN.Sdk.Core.Domain;

/// <summary>
/// A non-generic interface for entities that hold domain events.
/// This allows interceptors to process events without knowing the TId type.
/// </summary>
public interface IHasDomainEvents
{
    /// <summary>
    /// Gets the list of domain events.
    /// </summary>
    IReadOnlyCollection<IDomainEvent> DomainEvents { get; }

    /// <summary>
    /// Clears the domain events.
    /// </summary>
    void ClearDomainEvents();
}
