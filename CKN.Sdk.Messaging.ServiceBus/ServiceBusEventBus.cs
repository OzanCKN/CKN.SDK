using System;
using System.Threading;
using System.Threading.Tasks;
using CKN.Sdk.Core.Events;

namespace CKN.Sdk.Messaging.ServiceBus;

/// <summary>
/// An implementation of IEventBus using Azure Service Bus.
/// </summary>
public class ServiceBusEventBus : IEventBus
{
    /// <inheritdoc/>
    public Task PublishAsync<TEvent>(TEvent @event, CancellationToken cancellationToken = default) 
        where TEvent : IIntegrationEvent
    {
        // To be implemented in Phase 2
        throw new NotImplementedException("Azure Service Bus implementation will be added in Phase 2.");
    }

    /// <inheritdoc/>
    public void Subscribe<TEvent, THandler>()
        where TEvent : IIntegrationEvent
        where THandler : IEventHandler<TEvent>
    {
        // To be implemented in Phase 2 for subscriptions
    }

    /// <inheritdoc/>
    public void Unsubscribe<TEvent, THandler>()
        where TEvent : IIntegrationEvent
        where THandler : IEventHandler<TEvent>
    {
        // To be implemented in Phase 2 for subscriptions
    }
}
