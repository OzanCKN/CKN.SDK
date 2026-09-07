using System;
using Microsoft.Extensions.DependencyInjection;
using Polly;
using Polly.Fallback;

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

    /// <summary>
    /// Adds a graceful degradation (fallback) policy to an HttpClient.
    /// If the HTTP call fails completely, the provided fallback value will be returned instead of throwing an exception.
    /// </summary>
    public static IHttpClientBuilder AddCknFallbackHandler(this IHttpClientBuilder builder, System.Net.Http.HttpResponseMessage fallbackResponse)
    {
        builder.AddResilienceHandler("ckn-fallback", pipelineBuilder => 
        {
            pipelineBuilder.AddFallback(new Polly.Fallback.FallbackStrategyOptions<System.Net.Http.HttpResponseMessage>
            {
                FallbackAction = _ => new System.Threading.Tasks.ValueTask<Polly.Outcome<System.Net.Http.HttpResponseMessage>>(
                    Polly.Outcome.FromResult(fallbackResponse)),
                ShouldHandle = arguments => new System.Threading.Tasks.ValueTask<bool>(
                    arguments.Outcome.Exception != null || (arguments.Outcome.Result != null && !arguments.Outcome.Result.IsSuccessStatusCode))
            });
        });
        
        return builder;
    }
}
