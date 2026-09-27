using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using CKN.Sdk.Network.Http.Services;
using CKN.Sdk.Network.Options;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Moq;

namespace CKN.Sdk.Tests.Network;

public class CknHttpClientBaseTests
{
    private sealed record TestResponse(string Symbol, decimal Price);

    private sealed class TestHttpClient : CknHttpClientBase
    {
        public TestHttpClient(
            HttpClient httpClient,
            Microsoft.Extensions.Logging.ILoggerFactory loggerFactory,
            IOptionsMonitor<CknHttpClientOptions> options)
            : base(httpClient, loggerFactory, options) { }
    }

    private static TestHttpClient CreateClient(HttpMessageHandler handler)
    {
        var httpClient = new HttpClient(handler) { BaseAddress = new Uri("https://api.test.com/") };

        var loggerFactory = NullLoggerFactory.Instance;

        var optionsMock = new Mock<IOptionsMonitor<CknHttpClientOptions>>();
        optionsMock
            .Setup(m => m.Get(nameof(TestHttpClient)))
            .Returns(new CknHttpClientOptions());

        return new TestHttpClient(httpClient, loggerFactory, optionsMock.Object);
    }

    [Fact]
    public async Task GetJsonAsync_WhenResponseIsSuccessful_ShouldReturnSuccessResult()
    {
        // Arrange
        var payload = new TestResponse("AAPL", 185.50m);
        var json = JsonSerializer.Serialize(payload);
        var handler = new FakeHttpMessageHandler(HttpStatusCode.OK, json);
        var client = CreateClient(handler);

        // Act
        var result = await client.GetJsonAsync<TestResponse>("quote/AAPL");

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Symbol.Should().Be("AAPL");
        result.Value.Price.Should().Be(185.50m);
    }

    [Fact]
    public async Task GetJsonAsync_WhenResponseIs404_ShouldReturnFailureWithHttpError()
    {
        // Arrange
        var handler = new FakeHttpMessageHandler(HttpStatusCode.NotFound, "Not Found");
        var client = CreateClient(handler);

        // Act
        var result = await client.GetJsonAsync<TestResponse>("quote/INVALID");

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("HTTP_404");
    }

    [Fact]
    public async Task GetJsonAsync_WhenResponseIs500_ShouldReturnFailureWithHttpError()
    {
        // Arrange
        var handler = new FakeHttpMessageHandler(HttpStatusCode.InternalServerError, "Server Error");
        var client = CreateClient(handler);

        // Act
        var result = await client.GetJsonAsync<TestResponse>("quote/AAPL");

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("HTTP_500");
    }

    [Fact]
    public async Task GetJsonAsync_WhenNetworkThrows_ShouldReturnNetworkFailure()
    {
        // Arrange
        var handler = new ThrowingHttpMessageHandler(new HttpRequestException("Connection refused"));
        var client = CreateClient(handler);

        // Act
        var result = await client.GetJsonAsync<TestResponse>("quote/AAPL");

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("HTTP_NETWORK");
    }

    [Fact]
    public async Task GetJsonAsync_WhenResponseBodyIsNull_ShouldReturnNullValueFailure()
    {
        // Arrange
        var handler = new FakeHttpMessageHandler(HttpStatusCode.OK, "null");
        var client = CreateClient(handler);

        // Act
        var result = await client.GetJsonAsync<TestResponse>("quote/AAPL");

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(CKN.Sdk.Core.Common.Results.Error.NullValue);
    }

    [Fact]
    public async Task BatchAsync_ShouldReturnResultForEachUrl()
    {
        // Arrange
        var payload = new TestResponse("X", 1m);
        var json = JsonSerializer.Serialize(payload);
        var handler = new FakeHttpMessageHandler(HttpStatusCode.OK, json);
        var client = CreateClient(handler);
        var urls = new[] { "quote/A", "quote/B", "quote/C" };

        // Act
        var results = await client.BatchAsync(
            urls,
            (System.Text.Json.Serialization.Metadata.JsonTypeInfo<TestResponse>)
                JsonSerializerOptions.Default.GetTypeInfo(typeof(TestResponse)));

        // Assert
        results.Should().HaveCount(3);
        results.Should().AllSatisfy(r => r.IsSuccess.Should().BeTrue());
    }

    [Fact]
    public async Task GetJsonAsync_WhenCancelled_ShouldPropagateCancellation()
    {
        // Arrange
        using var cts = new CancellationTokenSource();
        var handler = new DelayedHttpMessageHandler(TimeSpan.FromSeconds(5));
        var client = CreateClient(handler);

        cts.CancelAfter(50);

        // Act
        Func<Task> act = () => client.GetJsonAsync<TestResponse>("quote/AAPL", cts.Token);

        // Assert
        await act.Should().ThrowAsync<OperationCanceledException>();
    }
}

internal sealed class FakeHttpMessageHandler : HttpMessageHandler
{
    private readonly HttpStatusCode _statusCode;
    private readonly string _content;

    internal FakeHttpMessageHandler(HttpStatusCode statusCode, string content)
    {
        _statusCode = statusCode;
        _content = content;
    }

    protected override Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        var response = new HttpResponseMessage(_statusCode)
        {
            Content = new StringContent(_content, Encoding.UTF8, "application/json")
        };
        return Task.FromResult(response);
    }
}

internal sealed class ThrowingHttpMessageHandler : HttpMessageHandler
{
    private readonly Exception _exception;

    internal ThrowingHttpMessageHandler(Exception exception)
    {
        _exception = exception;
    }

    protected override Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
        => Task.FromException<HttpResponseMessage>(_exception);
}

internal sealed class DelayedHttpMessageHandler : HttpMessageHandler
{
    private readonly TimeSpan _delay;

    internal DelayedHttpMessageHandler(TimeSpan delay)
    {
        _delay = delay;
    }

    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        await Task.Delay(_delay, cancellationToken);
        return new HttpResponseMessage(HttpStatusCode.OK);
    }
}
