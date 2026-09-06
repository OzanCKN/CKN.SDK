using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Quartz;
using Xunit;
using CKN.Sdk.Scheduling.Quartz;
using System.Linq;

namespace CKN.Sdk.Tests.Scheduling.Quartz;

public class QuartzBuilderExtensionsTests
{
    [Fact]
    public void AddCknQuartz_ShouldRegisterQuartzServices()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddSingleton(Moq.Mock.Of<IHostApplicationLifetime>());
        
        // Act
        services.AddCknQuartz();

        // Assert
        var serviceProvider = services.BuildServiceProvider();
        var schedulerFactory = serviceProvider.GetService<ISchedulerFactory>();
        Assert.NotNull(schedulerFactory);

        // Check if hosted service is registered
        var hostedServices = serviceProvider.GetServices<IHostedService>();
        Assert.Contains(hostedServices, s => s.GetType().Name.Contains("QuartzHostedService"));
    }
}
