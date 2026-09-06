using CKN.Sdk.Core.CQRS;
using FluentAssertions;
using NetArchTest.Rules;
using Xunit;

namespace CKN.Sdk.Tests.Architecture;

public class ArchitectureTests
{
    [Fact]
    public void Core_Should_Not_HaveDependencyOnOtherProjects()
    {
        // Arrange
        var assembly = typeof(IQuery<>).Assembly; // Represents CKN.Sdk.Core

        var otherProjects = new[]
        {
            "CKN.Sdk.Infrastructure",
            "CKN.Sdk.Messaging.RabbitMQ",
            "CKN.Sdk.EntityFramework",
            "CKN.Sdk.Caching.Redis"
        };

        // Act
        var result = Types
            .InAssembly(assembly)
            .ShouldNot()
            .HaveDependencyOnAny(otherProjects)
            .GetResult();

        // Assert
        result.IsSuccessful.Should().BeTrue();
    }
}
