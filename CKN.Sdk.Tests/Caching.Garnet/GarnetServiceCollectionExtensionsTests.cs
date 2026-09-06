using System.Linq;
using CKN.Sdk.Caching.Garnet;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Xunit;

namespace CKN.Sdk.Tests.Caching.Garnet;

public class GarnetServiceCollectionExtensionsTests
{
    [Fact]
    public void AddCknGarnetCache_ShouldRegisterIDistributedCache()
    {
        // Arrange
        var services = new ServiceCollection();
        
        // Act
        services.AddCknGarnetCache(options =>
        {
            options.Configuration = "127.0.0.1:3278";
            options.InstanceName = "TestInstance_";
        });
        var serviceProvider = services.BuildServiceProvider();
        var cache = serviceProvider.GetService<IDistributedCache>();

        // Assert
        Assert.NotNull(cache);
    }
}
