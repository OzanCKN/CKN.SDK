using System;
using CKN.Sdk.AI.OpenAI;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace CKN.Sdk.Tests.AI.OpenAI;

public class OpenAIServiceCollectionExtensionsTests
{
    [Fact]
    public void AddCknOpenAI_ShouldRegisterIChatClient()
    {
        // Arrange
        var services = new ServiceCollection();
        
        // Act
        services.AddCknOpenAI(options =>
        {
            options.ApiKey = "sk-test123456789";
            options.DefaultModel = "gpt-4o-mini";
        });
        var serviceProvider = services.BuildServiceProvider();
        var client = serviceProvider.GetService<IChatClient>();

        // Assert
        Assert.NotNull(client);
    }
}
