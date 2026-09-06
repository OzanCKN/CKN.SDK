using System;
using System.Text.Json;
using CKN.Sdk.Core.Extensions;
using FluentAssertions;
using Xunit;

namespace CKN.Sdk.Tests.Core.Extensions;

public class MemoryExtensionsTests
{
    private record TestUser(string Name, int Age);

    [Fact]
    public void DeserializeFromSpan_ShouldDeserializeCorrectly()
    {
        // Arrange
        var json = """{"Name":"Ozan","Age":30}""";
        ReadOnlySpan<byte> jsonBytes = System.Text.Encoding.UTF8.GetBytes(json);

        // Act
        var result = jsonBytes.DeserializeFromSpan<TestUser>();

        // Assert
        result.Should().NotBeNull();
        result!.Name.Should().Be("Ozan");
        result.Age.Should().Be(30);
    }

    [Fact]
    public void SplitSpan_ShouldSplitCorrectly_WhenSeparatorExists()
    {
        // Arrange
        ReadOnlySpan<char> span = "hello-world".AsSpan();

        // Act
        span.SplitSpan('-', out var first, out var second);

        // Assert
        first.ToString().Should().Be("hello");
        second.ToString().Should().Be("world");
    }

    [Fact]
    public void SplitSpan_ShouldNotSplit_WhenSeparatorDoesNotExist()
    {
        // Arrange
        ReadOnlySpan<char> span = "helloworld".AsSpan();

        // Act
        span.SplitSpan('-', out var first, out var second);

        // Assert
        first.ToString().Should().Be("helloworld");
        second.IsEmpty.Should().BeTrue();
    }

    [Fact]
    public void ContainsFast_ShouldReturnTrue_WhenCharExists()
    {
        // Arrange
        ReadOnlySpan<char> span = "fast-performance".AsSpan();

        // Act
        var result = span.ContainsFast('-');

        // Assert
        result.Should().BeTrue();
    }
}
