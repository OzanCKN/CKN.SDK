using System;

namespace CKN.Sdk.Core.Events;

/// <summary>
/// A base class for integration events with automatic EventId and CreationDate generation.
/// </summary>
public abstract record IntegrationEvent : IIntegrationEvent
{
    /// <inheritdoc />
    public Guid EventId { get; init; } = Guid.NewGuid();

    /// <inheritdoc />
    public DateTime CreationDate { get; init; } = DateTime.UtcNow;
}
