using System;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using CKN.Sdk.Messaging.Abstractions;
using RabbitMQ.Client;

namespace CKN.Sdk.Messaging.RabbitMQ;

public class RabbitMqPublisher : ICknMessagePublisher
{
    private readonly IConnection _connection;

    public RabbitMqPublisher(IConnection connection)
    {
        _connection = connection;
    }

    public async Task PublishAsync<T>(string exchange, string routingKey, T message, CancellationToken cancellationToken = default)
    {
        using var channel = await _connection.CreateChannelAsync(cancellationToken: cancellationToken);
        var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(message));

        var properties = new BasicProperties
        {
            Persistent = true
        };

        await channel.BasicPublishAsync(
            exchange,
            routingKey,
            false,
            properties,
            body,
            cancellationToken);
    }
}
