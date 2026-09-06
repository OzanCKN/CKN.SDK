using Hangfire;
using Hangfire.MemoryStorage;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using CKN.Sdk.Scheduling.Hangfire;
using Xunit;

namespace CKN.Sdk.Tests.Scheduling.Hangfire;

public class HangfireBuilderExtensionsTests
{
    [Fact]
    public void AddCknHangfire_ShouldRegisterHangfireServices()
    {
        // Arrange
        var services = new ServiceCollection();
        
        // Add fake IHostEnvironment if needed by Hangfire AddHangfireServer
        // Hangfire AddHangfireServer internally might need ILoggerFactory etc, let's just add logging.
        services.AddLogging();

        // Act
        services.AddCknHangfire(config => config.UseMemoryStorage());

        // Assert
        var serviceProvider = services.BuildServiceProvider();
        var jobClient = serviceProvider.GetService<IBackgroundJobClient>();
        Assert.NotNull(jobClient);
    }
}
