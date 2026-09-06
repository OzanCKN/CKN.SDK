using System;
using FluentAssertions;
using Xunit;
using CKN.Sdk.Core.Events;

namespace CKN.Sdk.Tests.Core.Events;

public class IntegrationEventTests
{
    private record TestEvent : IntegrationEvent;

    [Fact]
    public void Constructor_ShouldInitializeEventIdAndCreationDate()
    {
        // Act
        var @event = new TestEvent();

        // Assert
        @event.EventId.Should().NotBeEmpty();
        @event.CreationDate.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void TwoDifferentEvents_ShouldHaveUniqueEventIds()
    {
        // Act
        var event1 = new TestEvent();
        var event2 = new TestEvent();

        // Assert
        event1.EventId.Should().NotBe(event2.EventId);
    }
}
