using System.Linq;
using FluentAssertions;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace CKN.Sdk.Tests.AI;

public class AiClientRegistrationTests
{
    [Fact]
    public void AddCknAnthropic_ShouldRegister_IChatClient()
    {
        // Arrange
        var services = new ServiceCollection();
        
        // Act
        services.AddCknAnthropic(options =>
        {
            options.ApiKey = "test-key";
            options.DefaultModel = "test-model";
        });
        
        var serviceProvider = services.BuildServiceProvider();

        // Assert
        var chatClient = serviceProvider.GetService<IChatClient>();
        chatClient.Should().NotBeNull();
        chatClient.GetType().Name.Should().Be("AnthropicChatClient");
    }

    [Fact]
    public void AddCknGemini_ShouldRegister_IChatClient()
    {
        // Arrange
        var services = new ServiceCollection();
        
        // Act
        services.AddCknGemini(options =>
        {
            options.ApiKey = "test-key";
            options.DefaultModel = "test-model";
        });
        
        var serviceProvider = services.BuildServiceProvider();

        // Assert
        var chatClient = serviceProvider.GetService<IChatClient>();
        chatClient.Should().NotBeNull();
        
        // AsChatClient creates a wrapper around IChatCompletionService
        var chatClientType = chatClient!.GetType();
        chatClientType.FullName.Should().Contain("ChatCompletionServiceChatClient");
    }
}
