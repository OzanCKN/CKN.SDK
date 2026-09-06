using System;

namespace CKN.Sdk.Core.Events;

/// <summary>
/// Represents an integration event that is published to the event bus.
/// </summary>
public interface IIntegrationEvent
{
    /// <summary>
    /// Gets the unique identifier for this event.
    /// </summary>
    Guid EventId { get; }

    /// <summary>
    /// Gets the creation date and time of this event.
    /// </summary>
    DateTime CreationDate { get; }
}
