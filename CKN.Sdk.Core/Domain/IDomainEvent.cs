using MediatR;

namespace CKN.Sdk.Core.Domain;

/// <summary>
/// Represents a domain event in the system.
/// Inherits from MediatR INotification to be dispatchable across the application.
/// </summary>
public interface IDomainEvent : INotification
{
    /// <summary>
    /// The unique identifier of the event.
    /// </summary>
    Guid EventId { get; }

    /// <summary>
    /// The exact UTC timestamp when the event occurred.
    /// </summary>
    DateTime OccurredOn { get; }
}
