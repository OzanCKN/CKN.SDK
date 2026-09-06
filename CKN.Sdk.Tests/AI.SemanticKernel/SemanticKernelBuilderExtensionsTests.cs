using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.SemanticKernel;
using Xunit;

namespace CKN.Sdk.Tests.AI.SemanticKernel;

public class SemanticKernelBuilderExtensionsTests
{
    [Fact]
    public void AddCknSemanticKernel_ShouldRegisterKernel()
    {
        // Arrange
        var services = new ServiceCollection();
        
        // Act
        services.AddCknSemanticKernel(builder =>
        {
            // Usually we add plugins or AI services here.
            // For example: builder.AddOpenAIChatCompletion(...)
        });
        
        var serviceProvider = services.BuildServiceProvider();
        var kernel = serviceProvider.GetService<Kernel>();

        // Assert
        Assert.NotNull(kernel);
    }
}
