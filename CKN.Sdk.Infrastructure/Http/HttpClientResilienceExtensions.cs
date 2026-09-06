using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Http.Resilience;
using Polly;

namespace CKN.Sdk.Infrastructure.Http;

/// <summary>
/// Extension methods for configuring resilient HTTP clients using Polly.
/// </summary>
public static class HttpClientResilienceExtensions
{
    /// <summary>
    /// Adds default resilience policies (Retry, Circuit Breaker, Timeout) to the HttpClient.
    /// </summary>
    public static IHttpClientBuilder AddEnterpriseResilience(this IHttpClientBuilder builder)
    {
        builder.AddStandardResilienceHandler(options =>
        {
            options.Retry.MaxRetryAttempts = 3;
            options.Retry.Delay = TimeSpan.FromSeconds(2);
            options.Retry.BackoffType = DelayBackoffType.Exponential;

            options.CircuitBreaker.FailureRatio = 0.5;
            options.CircuitBreaker.SamplingDuration = TimeSpan.FromSeconds(10);
            options.CircuitBreaker.MinimumThroughput = 5;
            options.CircuitBreaker.BreakDuration = TimeSpan.FromSeconds(30);

            options.AttemptTimeout.Timeout = TimeSpan.FromSeconds(10);
        });

        return builder;
    }
}
