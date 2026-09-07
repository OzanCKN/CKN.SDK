using System;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using CKN.Sdk.Core.Events;
using CKN.Sdk.Messaging.Webhooks;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Xunit;

namespace CKN.Sdk.Tests.Messaging;

public class WebhookReceiverTests
{
    private readonly Mock<IEventBus> _mockEventBus;
    private readonly WebhookReceiver _receiver;

    public WebhookReceiverTests()
    {
        _mockEventBus = new Mock<IEventBus>();
        _receiver = new WebhookReceiver(_mockEventBus.Object, NullLogger<WebhookReceiver>.Instance);
    }

    [Fact]
    public async Task ReceiveAsync_WithValidSignature_ShouldPublishEventAndReturnTrue()
    {
        // Arrange
        var payload = "{\"data\":\"test\"}";
        var secret = "my-super-secret-key";
        var eventName = "test.webhook";

        var signature = GenerateHmacSha256Signature(payload, secret);

        _mockEventBus
            .Setup(eb => eb.PublishAsync(It.IsAny<WebhookIntegrationEvent>(), default))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _receiver.ReceiveAsync(payload, signature, secret, eventName);

        // Assert
        Assert.True(result);
        _mockEventBus.Verify(eb => eb.PublishAsync(It.Is<WebhookIntegrationEvent>(e => e.Payload == payload && e.ProviderEventName == eventName), default), Times.Once);
    }

    [Fact]
    public async Task ReceiveAsync_WithInvalidSignature_ShouldNotPublishEventAndReturnFalse()
    {
        // Arrange
        var payload = "{\"data\":\"test\"}";
        var secret = "my-super-secret-key";
        var eventName = "test.webhook";
        
        var invalidSignature = "invalid-signature-here";

        // Act
        var result = await _receiver.ReceiveAsync(payload, invalidSignature, secret, eventName);

        // Assert
        Assert.False(result);
        _mockEventBus.Verify(eb => eb.PublishAsync(It.IsAny<WebhookIntegrationEvent>(), default), Times.Never);
    }

    private string GenerateHmacSha256Signature(string payload, string secret)
    {
        using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(secret));
        var hashBytes = hmac.ComputeHash(Encoding.UTF8.GetBytes(payload));
        return BitConverter.ToString(hashBytes).Replace("-", "").ToLowerInvariant();
    }
}
