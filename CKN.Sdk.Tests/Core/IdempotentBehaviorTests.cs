using System;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using MediatR;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;
using CKN.Sdk.Core.CQRS;
using CKN.Sdk.Core.CQRS.Behaviors;

namespace CKN.Sdk.Tests.Core;

public class IdempotentBehaviorTests
{
    private readonly MemoryDistributedCache _cache;
    private readonly IdempotentBehavior<TestCommand, TestResponse> _sut;

    public IdempotentBehaviorTests()
    {
        _cache = new MemoryDistributedCache(Options.Create(new MemoryDistributedCacheOptions()));
        var loggerMock = new Mock<ILogger<IdempotentBehavior<TestCommand, TestResponse>>>();
        _sut = new IdempotentBehavior<TestCommand, TestResponse>(_cache, loggerMock.Object);
    }

    [Fact]
    public async Task Handle_WhenRequestHasNoIdempotencyKey_ShouldProceedNormally()
    {
        // Arrange
        var request = new TestCommand { IdempotencyKey = null! };
        var nextMock = new Mock<RequestHandlerDelegate<TestResponse>>();
        nextMock.Setup(n => n()).ReturnsAsync(new TestResponse { Success = true });

        // Act
        var result = await _sut.Handle(request, nextMock.Object, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
        nextMock.Verify(n => n(), Times.Once);
    }

    [Fact]
    public async Task Handle_WhenIdempotencyKeyExistsInCache_ShouldThrowConflictException()
    {
        // Arrange
        var request = new TestCommand { IdempotencyKey = Guid.NewGuid().ToString() };
        await _cache.SetStringAsync($"Idempotency:{request.IdempotencyKey}", "Processed");
        var nextMock = new Mock<RequestHandlerDelegate<TestResponse>>();

        // Act
        Func<Task> act = async () => await _sut.Handle(request, nextMock.Object, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage($"Command with Idempotency Key '{request.IdempotencyKey}' was already processed.");
            
        nextMock.Verify(n => n(), Times.Never);
    }

    [Fact]
    public async Task Handle_WhenIdempotencyKeyIsNew_ShouldProceedAndSetCache()
    {
        // Arrange
        var request = new TestCommand { IdempotencyKey = Guid.NewGuid().ToString() };
        var nextMock = new Mock<RequestHandlerDelegate<TestResponse>>();
        nextMock.Setup(n => n()).ReturnsAsync(new TestResponse { Success = true });

        // Act
        var result = await _sut.Handle(request, nextMock.Object, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
        
        nextMock.Verify(n => n(), Times.Once);
        var cached = await _cache.GetStringAsync($"Idempotency:{request.IdempotencyKey}");
        cached.Should().Be("Processed");
    }
}

public class TestCommand : ICommand<TestResponse>, IIdempotentCommand
{
    public string IdempotencyKey { get; set; } = null!;
}

public class TestResponse
{
    public bool Success { get; set; }
}
