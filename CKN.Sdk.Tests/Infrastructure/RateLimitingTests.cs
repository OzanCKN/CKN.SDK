using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Xunit;
using CKN.Sdk.Infrastructure.RateLimiting;

namespace CKN.Sdk.Tests.Infrastructure;

public class RateLimitingTests
{
    [Fact]
    public void AddCknRateLimiting_ShouldRegisterRateLimiterOptions()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddCknRateLimiting();
        var provider = services.BuildServiceProvider();

        // Assert
        var options = provider.GetService<IOptions<RateLimiterOptions>>();
        Assert.NotNull(options);
        Assert.NotNull(options.Value);
        Assert.Equal(429, options.Value.RejectionStatusCode);
        Assert.NotNull(options.Value.OnRejected);
    }
}
