using System;
using Azure.Messaging.ServiceBus;
using CKN.Sdk.Core.Events;
using CKN.Sdk.Messaging.ServiceBus;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace CKN.Sdk.Tests.Messaging.ServiceBus;

public class ServiceBusMessagingBuilderExtensionsTests
{
    [Fact]
    public void AddCknServiceBus_ShouldRegisterServiceBusClient()
    {
        // Arrange
        var services = new ServiceCollection();
        
        // Act
        services.AddCknServiceBus(options =>
        {
            options.ConnectionString = "Endpoint=sb://test.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=test";
        });
        
        var serviceProvider = services.BuildServiceProvider();
        var client = serviceProvider.GetService<ServiceBusClient>();
        var eventBus = serviceProvider.GetService<IEventBus>();

        // Assert
        Assert.NotNull(client);
        Assert.NotNull(eventBus);
        Assert.IsType<ServiceBusEventBus>(eventBus);
    }
}
