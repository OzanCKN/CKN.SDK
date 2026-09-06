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
    private readonly IModel _channel;
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
        _connection = connectionFactory.CreateConnection();
        _channel = _connection.CreateModel();
        
        _channel.ExchangeDeclare(exchange: ExchangeName, type: ExchangeType.Direct);
    }

    public Task PublishAsync<TEvent>(TEvent @event, CancellationToken cancellationToken = default) where TEvent : IIntegrationEvent
    {
        var eventName = typeof(TEvent).Name;
        var message = JsonSerializer.Serialize(@event);
        var body = Encoding.UTF8.GetBytes(message);

        _channel.BasicPublish(
            exchange: ExchangeName,
            routingKey: eventName,
            basicProperties: null,
            body: body);

        return Task.CompletedTask;
    }

    public void Subscribe<TEvent, THandler>()
        where TEvent : IIntegrationEvent
        where THandler : IEventHandler<TEvent>
    {
        // Minimal implementasyon: Gerçek hayatta burada Consumer (EventingBasicConsumer) yazılıp dinlenir.
        var eventName = typeof(TEvent).Name;
        
        _channel.QueueDeclare(queue: _options.QueueName, durable: true, exclusive: false, autoDelete: false, arguments: null);
        _channel.QueueBind(queue: _options.QueueName, exchange: ExchangeName, routingKey: eventName);
    }

    public void Unsubscribe<TEvent, THandler>()
        where TEvent : IIntegrationEvent
        where THandler : IEventHandler<TEvent>
    {
        var eventName = typeof(TEvent).Name;
        _channel.QueueUnbind(queue: _options.QueueName, exchange: ExchangeName, routingKey: eventName);
    }

    public void Dispose()
    {
        _channel?.Dispose();
        _connection?.Dispose();
    }
}
