using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;

namespace CKN.Sdk.Core.CQRS.Behaviors;

/// <summary>
/// Defines a command that requires idempotency to prevent duplicate executions.
/// </summary>
public interface IIdempotentCommand
{
    /// <summary>
    /// Gets the unique idempotency key for this command.
    /// </summary>
    string IdempotencyKey { get; }
}

/// <summary>
/// A pipeline behavior that ensures commands with an Idempotency-Key are only executed once.
/// </summary>
public class IdempotentBehavior<TRequest, TResponse>(
    IDistributedCache cache,
    ILogger<IdempotentBehavior<TRequest, TResponse>> logger) 
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse> // Intercepts any MediatR request
{
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        if (request is IIdempotentCommand idempotentCommand && !string.IsNullOrWhiteSpace(idempotentCommand.IdempotencyKey))
        {
            var cacheKey = $"Idempotency:{idempotentCommand.IdempotencyKey}";
            var cachedResult = await cache.GetStringAsync(cacheKey, cancellationToken);
            
            if (!string.IsNullOrEmpty(cachedResult))
            {
                logger.LogWarning("Duplicate execution prevented for IdempotencyKey: {IdempotencyKey}", idempotentCommand.IdempotencyKey);
                throw new InvalidOperationException($"Command with Idempotency Key '{idempotentCommand.IdempotencyKey}' was already processed.");
            }

            var response = await next();
            
            // Save to cache indicating it was processed, expire after 24 hours
            await cache.SetStringAsync(cacheKey, "Processed", new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(24)
            }, cancellationToken);

            return response;
        }

        return await next();
    }
}
