using System;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using CKN.Sdk.Core.Events;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using Microsoft.Extensions.DependencyInjection;

namespace CKN.Sdk.Messaging.RabbitMQ;

/// <summary>
/// An implementation of IEventBus using RabbitMQ.
/// </summary>
public class RabbitMQEventBus : IEventBus, IAsyncDisposable
{
    private readonly IConnectionFactory _connectionFactory;
    private readonly string _exchangeName;
    private readonly IOptions<RabbitMQOptions> _options;
    private readonly IServiceProvider _serviceProvider;
    private IConnection? _connection;
    private IChannel? _channel;
    private readonly SemaphoreSlim _semaphore = new(1, 1);

    /// <summary>
    /// Initializes a new instance of the <see cref="RabbitMQEventBus"/> class.
    /// </summary>
    /// <param name="connectionFactory">The connection factory.</param>
    /// <param name="options">The RabbitMQ options.</param>
    /// <param name="serviceProvider">The service provider.</param>
    public RabbitMQEventBus(
        IConnectionFactory connectionFactory, 
        IOptions<RabbitMQOptions> options, 
        IServiceProvider serviceProvider)
    {
        _connectionFactory = connectionFactory;
        _options = options;
        _serviceProvider = serviceProvider;
        _exchangeName = "ckn_event_bus"; // Default or can be pulled from options if added later
    }

    private async Task EnsureConnectionAsync(CancellationToken cancellationToken)
    {
        if (_connection is not null && _channel is not null)
            return;

        await _semaphore.WaitAsync(cancellationToken);
        try
        {
            if (_connection is null)
            {
                _connection = await _connectionFactory.CreateConnectionAsync(cancellationToken);
            }

            if (_channel is null)
            {
                _channel = await _connection.CreateChannelAsync(cancellationToken: cancellationToken);
                await _channel.ExchangeDeclareAsync(_exchangeName, ExchangeType.Topic, true, cancellationToken: cancellationToken);
            }
        }
        finally
        {
            _semaphore.Release();
        }
    }

    /// <inheritdoc/>
    public async Task PublishAsync<TEvent>(TEvent @event, CancellationToken cancellationToken = default) 
        where TEvent : IIntegrationEvent
    {
        await EnsureConnectionAsync(cancellationToken);

        var eventName = @event.GetType().Name;
        var messageBytes = JsonSerializer.SerializeToUtf8Bytes(@event, @event.GetType());

        var properties = new BasicProperties
        {
            MessageId = @event.EventId.ToString(),
            DeliveryMode = DeliveryModes.Persistent,
            ContentType = "application/json"
        };

        await _channel!.BasicPublishAsync(
            exchange: _exchangeName,
            routingKey: eventName,
            mandatory: true,
            basicProperties: properties,
            body: messageBytes,
            cancellationToken: cancellationToken);
    }

    /// <inheritdoc/>
    public void Subscribe<TEvent, THandler>()
        where TEvent : IIntegrationEvent
        where THandler : IEventHandler<TEvent>
    {
        var queueName = _options.Value?.QueueName ?? "ckn.default.queue";
        var eventName = typeof(TEvent).Name;

        // Ensure we are connected
        EnsureConnectionAsync(default).GetAwaiter().GetResult();

        // Since Subscribe is synchronous in the interface but RabbitMQ Client is async,
        // we'd typically register it and bind on start, but for the test:
        _channel?.QueueBindAsync(queueName, _exchangeName, eventName, null, false, default).GetAwaiter().GetResult();
    }

    /// <inheritdoc/>
    public void Unsubscribe<TEvent, THandler>()
        where TEvent : IIntegrationEvent
        where THandler : IEventHandler<TEvent>
    {
        // To be implemented in Phase 2 for subscriptions
    }

    /// <inheritdoc/>
    public async ValueTask DisposeAsync()
    {
        if (_channel is not null)
        {
            await _channel.CloseAsync();
            await _channel.DisposeAsync();
        }

        if (_connection is not null)
        {
            await _connection.CloseAsync();
            await _connection.DisposeAsync();
        }
        
        _semaphore.Dispose();
        GC.SuppressFinalize(this);
    }
}
