using System.Net.Http;
using CKN.Sdk.Network.Abstractions;
using CKN.Sdk.Network.Http.Services;
using CKN.Sdk.Network.Options;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace CKN.Sdk.Tests.Network;

public class NetworkDiRegistrationTests
{
    private sealed class SampleApiClient : CknHttpClientBase
    {
        public SampleApiClient(
            HttpClient httpClient,
            ILoggerFactory loggerFactory,
            IOptionsMonitor<CknHttpClientOptions> options)
            : base(httpClient, loggerFactory, options) { }
    }

    [Fact]
    public void AddCknNetwork_AddCknHttpClient_ShouldRegisterTypedClient()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddLogging();

        // Act
        services.AddCknNetwork(net =>
            net.AddCknHttpClient<SampleApiClient>(opt =>
            {
                opt.BaseAddress = "https://api.test.com/";
                opt.Timeout = System.TimeSpan.FromSeconds(5);
            }));

        var provider = services.BuildServiceProvider();

        // Assert — typed client should be resolvable
        var client = provider.GetService<SampleApiClient>();
        client.Should().NotBeNull();
        client.Should().BeAssignableTo<ICknHttpClient>();
    }

    [Fact]
    public void AddCknNetwork_WithRateLimit_ShouldRegisterTypedClient()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddLogging();

        // Act
        services.AddCknNetwork(net =>
            net.AddCknHttpClient<SampleApiClient>(opt =>
            {
                opt.BaseAddress = "https://api.test.com/";
                opt.RateLimit = new CknRateLimiterOptions { RequestsPerPeriod = 2 };
            }));

        var provider = services.BuildServiceProvider();

        // Assert
        var client = provider.GetService<SampleApiClient>();
        client.Should().NotBeNull();
    }

    [Fact]
    public void AddCknNetwork_NamedOptions_ShouldBeStoredUnderTypeName()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddLogging();

        // Act
        services.AddCknNetwork(net =>
            net.AddCknHttpClient<SampleApiClient>(opt =>
            {
                opt.BaseAddress = "https://api.test.com/";
                opt.MaxDegreeOfParallelism = 8;
            }));

        var provider = services.BuildServiceProvider();
        var monitor = provider.GetRequiredService<IOptionsMonitor<CknHttpClientOptions>>();

        // Assert
        var opts = monitor.Get(nameof(SampleApiClient));
        opts.MaxDegreeOfParallelism.Should().Be(8);
    }
}
