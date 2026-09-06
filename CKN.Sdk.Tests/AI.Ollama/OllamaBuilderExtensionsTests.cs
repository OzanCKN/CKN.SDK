using System;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace CKN.Sdk.Tests.AI.Ollama;

public class OllamaBuilderExtensionsTests
{
    [Fact]
    public void AddCknOllama_ShouldRegisterChatClient()
    {
        // Arrange
        var services = new ServiceCollection();
        
        // Act
        services.AddCknOllama(options =>
        {
            options.Uri = "http://localhost:11434";
            options.DefaultModel = "llama3";
        });
        
        var serviceProvider = services.BuildServiceProvider();
        var chatClient = serviceProvider.GetService<IChatClient>();

        // Assert
        Assert.NotNull(chatClient);
    }
}
