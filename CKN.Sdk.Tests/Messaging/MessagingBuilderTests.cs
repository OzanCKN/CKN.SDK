using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using CKN.Sdk.Messaging.Extensions;

namespace CKN.Sdk.Tests.Messaging;

public class MessagingBuilderTests
{
    [Fact]
    public void AddCknMessaging_ShouldProvideBuilderWithServiceCollection()
    {
        // Arrange
        var services = new ServiceCollection();
        ICknMessagingBuilder? capturedBuilder = null;

        // Act
        services.AddCknMessaging(builder =>
        {
            capturedBuilder = builder;
        });

        // Assert
        capturedBuilder.Should().NotBeNull();
        capturedBuilder!.Services.Should().BeSameAs(services);
    }
}
