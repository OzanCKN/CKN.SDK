using Microsoft.Extensions.DependencyInjection;
using Coravel.Scheduling.Schedule.Interfaces;
using Coravel.Queuing.Interfaces;
using Xunit;
using CKN.Sdk.Scheduling.Coravel;

namespace CKN.Sdk.Tests.Scheduling.Coravel;

public class CoravelBuilderExtensionsTests
{
    [Fact]
    public void AddCknCoravel_ShouldRegisterCoravelServices()
    {
        // Arrange
        var services = new ServiceCollection();
        
        // Act
        services.AddCknCoravel();

        // Assert
        var serviceProvider = services.BuildServiceProvider();
        
        var scheduler = serviceProvider.GetService<IScheduler>();
        Assert.NotNull(scheduler);

        var queue = serviceProvider.GetService<IQueue>();
        Assert.NotNull(queue);
    }
}
