using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CKN.Sdk.Network.Http.DelegatingHandlers;
using CKN.Sdk.Network.Options;
using FluentAssertions;

namespace CKN.Sdk.Tests.Network;

public class RateLimiterHandlerTests
{
    [Fact]
    public async Task SendAsync_WhenWithinRateLimit_ShouldForwardRequest()
    {
        // Arrange
        var options = new CknRateLimiterOptions { RequestsPerPeriod = 10, QueueLimit = 100 };
        var inner = new FakeHttpMessageHandler(HttpStatusCode.OK, "{}");
        var handler = new RateLimiterHandler(options) { InnerHandler = inner };
        using var invoker = new HttpMessageInvoker(handler);
        using var request = new HttpRequestMessage(HttpMethod.Get, "https://api.test.com/data");

        // Act
        var response = await invoker.SendAsync(request, CancellationToken.None);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task SendAsync_WhenQueueIsFull_ShouldReturn429()
    {
        // Arrange — 1 token per 10 seconds, queue limit 0
        var options = new CknRateLimiterOptions
        {
            RequestsPerPeriod = 1,
            Period = System.TimeSpan.FromSeconds(10),
            QueueLimit = 0
        };
        var inner = new FakeHttpMessageHandler(HttpStatusCode.OK, "{}");
        var handler = new RateLimiterHandler(options) { InnerHandler = inner };
        using var invoker = new HttpMessageInvoker(handler);

        // First request consumes the token
        using var req1 = new HttpRequestMessage(HttpMethod.Get, "https://api.test.com/data");
        await invoker.SendAsync(req1, CancellationToken.None);

        // Second request — token exhausted, queue limit 0
        using var req2 = new HttpRequestMessage(HttpMethod.Get, "https://api.test.com/data");

        // Act
        var response = await invoker.SendAsync(req2, CancellationToken.None);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.TooManyRequests);
    }
}
