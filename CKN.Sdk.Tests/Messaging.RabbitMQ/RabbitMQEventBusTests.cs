using System;
using System.Threading;
using System.Threading.Tasks;
using CKN.Sdk.Core.Events;
using CKN.Sdk.Messaging.RabbitMQ;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Moq;
using RabbitMQ.Client;
using Xunit;

namespace CKN.Sdk.Tests.Messaging.RabbitMQ;

public class RabbitMQEventBusTests
{
    private record TestEvent : IntegrationEvent;
    
    private class TestEventHandler : IEventHandler<TestEvent>
    {
        public Task HandleAsync(TestEvent @event, CancellationToken cancellationToken = default) => Task.CompletedTask;
    }

    [Fact]
    public async Task PublishAsync_ShouldCallBasicPublishOnChannel()
    {
        // Arrange
        var mockConnectionFactory = new Mock<IConnectionFactory>();
        var mockConnection = new Mock<IConnection>();
        var mockChannel = new Mock<IChannel>();

        mockConnectionFactory.Setup(f => f.CreateConnectionAsync(It.IsAny<CancellationToken>()))
                             .ReturnsAsync(mockConnection.Object);
        mockConnection.Setup(c => c.CreateChannelAsync(It.IsAny<CreateChannelOptions>(), It.IsAny<CancellationToken>()))
                      .ReturnsAsync(mockChannel.Object);

        var options = Options.Create(new RabbitMQOptions { QueueName = "test.queue" });
        var serviceProvider = new ServiceCollection().BuildServiceProvider();

        var eventBus = new RabbitMQEventBus(mockConnectionFactory.Object, options, serviceProvider);
        var @event = new TestEvent();

        // Act
        await eventBus.PublishAsync(@event);

        // Assert
        mockChannel.Verify(c => c.BasicPublishAsync(
            It.Is<string>(e => e == "ckn_event_bus"),
            It.Is<string>(r => r == "TestEvent"),
            It.IsAny<bool>(),
            It.IsAny<BasicProperties>(),
            It.IsAny<ReadOnlyMemory<byte>>(),
            It.IsAny<CancellationToken>()), Times.Once);
    }
    
    [Fact]
    public void Subscribe_ShouldQueueBind()
    {
        // Arrange
        var mockConnectionFactory = new Mock<IConnectionFactory>();
        var mockConnection = new Mock<IConnection>();
        var mockChannel = new Mock<IChannel>();

        mockConnectionFactory.Setup(f => f.CreateConnectionAsync(It.IsAny<CancellationToken>()))
                             .ReturnsAsync(mockConnection.Object);
        mockConnection.Setup(c => c.CreateChannelAsync(It.IsAny<CreateChannelOptions>(), It.IsAny<CancellationToken>()))
                      .ReturnsAsync(mockChannel.Object);

        var options = Options.Create(new RabbitMQOptions { QueueName = "test.queue" });
        var serviceProvider = new ServiceCollection().BuildServiceProvider();

        var eventBus = new RabbitMQEventBus(mockConnectionFactory.Object, options, serviceProvider);

        // Act
        eventBus.Subscribe<TestEvent, TestEventHandler>();

        // Assert
        mockChannel.Verify(c => c.QueueBindAsync(
            It.Is<string>(q => q == "test.queue"),
            It.Is<string>(e => e == "ckn_event_bus"),
            It.Is<string>(r => r == "TestEvent"),
            It.IsAny<System.Collections.Generic.IDictionary<string, object?>>(),
            It.IsAny<bool>(),
            It.IsAny<CancellationToken>()), Times.Once);
    }
}
