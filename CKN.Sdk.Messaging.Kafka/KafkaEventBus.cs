using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using CKN.Sdk.Core.Events;
using Confluent.Kafka;
using Microsoft.Extensions.Options;

namespace CKN.Sdk.Messaging.Kafka;

/// <summary>
/// An implementation of IEventBus using Confluent.Kafka Native Client.
/// </summary>
public class KafkaEventBus : IEventBus
{
    private readonly KafkaOptions _options;
    private readonly IProducer<Null, string> _producer;

    public KafkaEventBus(IOptions<KafkaOptions> options, IProducer<Null, string> producer)
    {
        _options = options.Value;
        _producer = producer;
    }

    public async Task PublishAsync<TEvent>(TEvent @event, CancellationToken cancellationToken = default) where TEvent : IIntegrationEvent
    {
        var topicName = typeof(TEvent).Name;
        var message = JsonSerializer.Serialize(@event);

        await _producer.ProduceAsync(topicName, new Message<Null, string> { Value = message }, cancellationToken);
    }

    public void Subscribe<TEvent, THandler>()
        where TEvent : IIntegrationEvent
        where THandler : IEventHandler<TEvent>
    {
        // Minimal implementation for demonstration. 
        // In a real scenario, this would register a hosted service to Consume from Kafka topic.
    }

    public void Unsubscribe<TEvent, THandler>()
        where TEvent : IIntegrationEvent
        where THandler : IEventHandler<TEvent>
    {
        // Unsubscribe logic.
    }
}
