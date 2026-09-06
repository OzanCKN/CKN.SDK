using System;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Azure.Messaging.ServiceBus;
using CKN.Sdk.Core.Events;

namespace CKN.Sdk.Messaging.ServiceBus;

public class ServiceBusEventBus : IEventBus
{
    private readonly ServiceBusClient _client;

    public ServiceBusEventBus(ServiceBusClient client)
    {
        _client = client;
    }

    public async Task PublishAsync<TEvent>(TEvent @event, CancellationToken cancellationToken = default) where TEvent : IIntegrationEvent
    {
        var sender = _client.CreateSender(@event.GetType().Name);
        var body = JsonSerializer.Serialize(@event);
        var message = new ServiceBusMessage(Encoding.UTF8.GetBytes(body));

        await sender.SendMessageAsync(message, cancellationToken).ConfigureAwait(false);
        await sender.DisposeAsync().ConfigureAwait(false);
    }

    public void Subscribe<TEvent, THandler>()
        where TEvent : IIntegrationEvent
        where THandler : IEventHandler<TEvent>
    {
        // For a full implementation, you'd manage ServiceBusProcessor instances and resolve handlers from DI.
        throw new NotImplementedException("Subscription requires a background service and DI scope management.");
    }

    public void Unsubscribe<TEvent, THandler>()
        where TEvent : IIntegrationEvent
        where THandler : IEventHandler<TEvent>
    {
        throw new NotImplementedException("Unsubscription requires processor management.");
    }
}
