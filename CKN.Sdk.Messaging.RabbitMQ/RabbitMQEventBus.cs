using System;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using CKN.Sdk.Core.Events;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;

namespace CKN.Sdk.Messaging.RabbitMQ;

/// <summary>
/// An implementation of IEventBus using RabbitMQ Native Client.
/// </summary>
public class RabbitMQEventBus : IEventBus, IDisposable
{
    private readonly IConnection _connection;
    private readonly IChannel _channel;
    private readonly IServiceProvider _serviceProvider;
    private readonly RabbitMQOptions _options;
    private const string ExchangeName = "ckn_event_bus";

    public RabbitMQEventBus(
        IConnectionFactory connectionFactory, 
        IOptions<RabbitMQOptions> options,
        IServiceProvider serviceProvider)
    {
        _options = options.Value;
        _serviceProvider = serviceProvider;
        _connection = connectionFactory.CreateConnectionAsync().GetAwaiter().GetResult();
        _channel = _connection.CreateChannelAsync().GetAwaiter().GetResult();
        
        _channel.ExchangeDeclareAsync(exchange: ExchangeName, type: ExchangeType.Direct).GetAwaiter().GetResult();
    }

    public Task PublishAsync<TEvent>(TEvent @event, CancellationToken cancellationToken = default) where TEvent : IIntegrationEvent
    {
        var eventName = typeof(TEvent).Name;
        var message = JsonSerializer.Serialize(@event);
        var body = Encoding.UTF8.GetBytes(message);

        return _channel.BasicPublishAsync(
            exchange: ExchangeName,
            routingKey: eventName,
            mandatory: false,
            basicProperties: new BasicProperties(),
            body: body, cancellationToken).AsTask();
    }

    public void Subscribe<TEvent, THandler>()
        where TEvent : IIntegrationEvent
        where THandler : IEventHandler<TEvent>
    {
        var eventName = typeof(TEvent).Name;
        
        _channel.QueueDeclareAsync(queue: _options.QueueName, durable: true, exclusive: false, autoDelete: false, arguments: null).GetAwaiter().GetResult();
        _channel.QueueBindAsync(queue: _options.QueueName, exchange: ExchangeName, routingKey: eventName).GetAwaiter().GetResult();
    }

    public void Unsubscribe<TEvent, THandler>()
        where TEvent : IIntegrationEvent
        where THandler : IEventHandler<TEvent>
    {
        var eventName = typeof(TEvent).Name;
        _channel.QueueUnbindAsync(queue: _options.QueueName, exchange: ExchangeName, routingKey: eventName).GetAwaiter().GetResult();
    }

    public void Dispose()
    {
        _channel?.Dispose();
        _connection?.Dispose();
    }
}
