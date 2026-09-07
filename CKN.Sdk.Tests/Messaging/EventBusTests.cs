using System;
using System.Threading;
using System.Threading.Tasks;
using CKN.Sdk.Core.Events;
using Xunit;

namespace CKN.Sdk.Tests.Messaging;

public class EventBusTests
{
    private record TestIntegrationEvent : IntegrationEvent
    {
        public string Message { get; init; } = string.Empty;
    }

    private class MockEventBus : IEventBus
    {
        public int PublishCount { get; private set; }

        public Task PublishAsync<TEvent>(TEvent @event, CancellationToken cancellationToken = default) 
            where TEvent : IIntegrationEvent
        {
            PublishCount++;
            return Task.CompletedTask;
        }

        public void Subscribe<TEvent, THandler>()
            where TEvent : IIntegrationEvent
            where THandler : IEventHandler<TEvent>
        {
        }

        public void Unsubscribe<TEvent, THandler>()
            where TEvent : IIntegrationEvent
            where THandler : IEventHandler<TEvent>
        {
        }
    }

    [Fact]
    public async Task EventBus_Publish_Should_Succeed()
    {
        // Arrange
        var mockBus = new MockEventBus();
        var @event = new TestIntegrationEvent { Message = "Test" };

        // Act
        await mockBus.PublishAsync(@event);

        // Assert
        Assert.Equal(1, mockBus.PublishCount);
        Assert.NotEqual(Guid.Empty, @event.EventId);
        Assert.True(@event.CreationDate <= DateTime.UtcNow);
    }
}
