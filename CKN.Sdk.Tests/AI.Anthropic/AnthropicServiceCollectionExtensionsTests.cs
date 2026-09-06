using System;
using Anthropic.SDK;
using CKN.Sdk.AI.Anthropic;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace CKN.Sdk.Tests.AI.Anthropic;

public class AnthropicServiceCollectionExtensionsTests
{
    [Fact]
    public void AddCknAnthropic_ShouldRegisterAnthropicClient()
    {
        // Arrange
        var services = new ServiceCollection();
        
        // Act
        services.AddCknAnthropic(options =>
        {
            options.ApiKey = "sk-ant-test12345";
            options.DefaultModel = "claude-3-opus-20240229";
        });
        var serviceProvider = services.BuildServiceProvider();
        var client = serviceProvider.GetService<AnthropicClient>();

        // Assert
        Assert.NotNull(client);
    }
}
