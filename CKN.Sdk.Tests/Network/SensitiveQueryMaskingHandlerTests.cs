using System;
using System.Collections.Generic;
using CKN.Sdk.Network.Http.DelegatingHandlers;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;

namespace CKN.Sdk.Tests.Network;

public class SensitiveQueryMaskingHandlerTests
{
    private readonly SensitiveQueryMaskingHandler _handler;

    public SensitiveQueryMaskingHandlerTests()
    {
        _handler = new SensitiveQueryMaskingHandler(
            sensitiveParams: ["token", "apikey", "api_key", "key", "secret"],
            logger: NullLogger.Instance,
            clientName: "TestClient");
    }

    [Fact]
    public void MaskUrl_WhenUrlHasSensitiveToken_ShouldRedactValue()
    {
        // Arrange
        var uri = new Uri("https://finnhub.io/api/v1/quote?symbol=AAPL&token=abc123secret");

        // Act
        var masked = _handler.MaskUrl(uri);

        // Assert
        masked.Should().Contain("token=***REDACTED***");
        masked.Should().NotContain("abc123secret");
        masked.Should().Contain("symbol=AAPL");
    }

    [Fact]
    public void MaskUrl_WhenUrlHasNoSensitiveParams_ShouldReturnUnchanged()
    {
        // Arrange
        var uri = new Uri("https://api.example.com/data?symbol=GOOG&interval=1d");

        // Act
        var masked = _handler.MaskUrl(uri);

        // Assert
        masked.Should().Be(uri.ToString());
    }

    [Fact]
    public void MaskUrl_WhenUrlHasNoQuery_ShouldReturnUnchanged()
    {
        // Arrange
        var uri = new Uri("https://api.example.com/data");

        // Act
        var masked = _handler.MaskUrl(uri);

        // Assert
        masked.Should().Be(uri.ToString());
    }

    [Fact]
    public void MaskUrl_WhenUrlIsNull_ShouldReturnNullPlaceholder()
    {
        // Act
        var masked = _handler.MaskUrl(null);

        // Assert
        masked.Should().Be("(null)");
    }

    [Theory]
    [InlineData("TOKEN")]
    [InlineData("Token")]
    [InlineData("token")]
    public void MaskUrl_ShouldBeCaseInsensitive(string paramName)
    {
        // Arrange
        var uri = new Uri($"https://api.example.com/data?{paramName}=supersecret");

        // Act
        var masked = _handler.MaskUrl(uri);

        // Assert
        masked.Should().Contain("***REDACTED***");
        masked.Should().NotContain("supersecret");
    }

    [Fact]
    public void MaskUrl_WhenMultipleSensitiveParams_ShouldRedactAll()
    {
        // Arrange
        var uri = new Uri("https://api.example.com/data?apikey=key1&secret=sec1&symbol=MSFT");

        // Act
        var masked = _handler.MaskUrl(uri);

        // Assert
        masked.Should().NotContain("key1");
        masked.Should().NotContain("sec1");
        masked.Should().Contain("symbol=MSFT");
    }
}
