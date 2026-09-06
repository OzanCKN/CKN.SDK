using System.Threading;
using System.Threading.Tasks;
using CKN.Sdk.Core.Events;
using CKN.Sdk.Messaging.Kafka;
using Confluent.Kafka;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;

namespace CKN.Sdk.Tests.Messaging.Kafka;

public class KafkaEventBusTests
{
    private record TestEvent : IntegrationEvent;

    [Fact]
    public async Task PublishAsync_ShouldCallProduceAsyncOnProducer()
    {
        // Arrange
        var mockProducer = new Mock<IProducer<Null, string>>();
        var options = Options.Create(new KafkaOptions { BootstrapServers = "localhost:9092" });

        var eventBus = new KafkaEventBus(options, mockProducer.Object);
        var @event = new TestEvent();

        // Act
        await eventBus.PublishAsync(@event);

        // Assert
        mockProducer.Verify(p => p.ProduceAsync(
            It.Is<string>(topic => topic == "TestEvent"),
            It.IsAny<Message<Null, string>>(),
            It.IsAny<CancellationToken>()), Times.Once);
    }
}
