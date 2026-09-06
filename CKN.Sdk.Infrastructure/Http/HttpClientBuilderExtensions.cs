using System;
using Microsoft.Extensions.DependencyInjection;

namespace CKN.Sdk.Infrastructure.Http;

/// <summary>
/// Extension methods for configuring resilient HTTP clients.
/// </summary>
public static class HttpClientBuilderExtensions
{
    /// <summary>
    /// Adds standard resilience policies (Retry, Circuit Breaker, Timeout, Rate Limiting) to an HttpClient.
    /// This uses the official Microsoft.Extensions.Http.Resilience package to ensure high availability.
    /// </summary>
    public static IHttpClientBuilder AddCknResilienceHandler(this IHttpClientBuilder builder)
    {
        builder.AddStandardResilienceHandler(options => 
        {
            // Configure exponential backoff retry (Wait 2s, 4s, 8s...)
            options.Retry.MaxRetryAttempts = 3;
            options.Retry.Delay = TimeSpan.FromSeconds(2);
            options.Retry.UseJitter = true;
            
            // Configure circuit breaker to prevent cascading failures
            options.CircuitBreaker.FailureRatio = 0.5;
            options.CircuitBreaker.SamplingDuration = TimeSpan.FromSeconds(30);
            options.CircuitBreaker.MinimumThroughput = 5;
            options.CircuitBreaker.BreakDuration = TimeSpan.FromSeconds(30);
            
            // Timeout and RateLimiting are automatically configured with safe defaults
        });
        
        return builder;
    }
}
