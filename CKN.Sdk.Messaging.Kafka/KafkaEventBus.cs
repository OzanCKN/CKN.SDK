using System;
using System.Threading;
using System.Threading.Tasks;
using CKN.Sdk.Core.Events;
using Confluent.Kafka;
using Microsoft.Extensions.Options;

namespace CKN.Sdk.Messaging.Kafka;

/// <summary>
/// An implementation of IEventBus using Apache Kafka.
/// </summary>
public class KafkaEventBus : IEventBus
{
    private readonly IOptions<KafkaOptions> _options;
    private readonly IProducer<Null, string> _producer;

    public KafkaEventBus(IOptions<KafkaOptions> options, IProducer<Null, string> producer)
    {
        _options = options;
        _producer = producer;
    }

    /// <inheritdoc/>
    public async Task PublishAsync<TEvent>(TEvent @event, CancellationToken cancellationToken = default) 
        where TEvent : IIntegrationEvent
    {
        await _producer.ProduceAsync(@event.GetType().Name, new Message<Null, string> { Value = System.Text.Json.JsonSerializer.Serialize(@event) }, cancellationToken);
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
