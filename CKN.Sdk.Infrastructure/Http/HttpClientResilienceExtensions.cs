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
    /// Adds default resilience policies (Retry, Circuit Breaker, Timeout, Rate Limiting) to the HttpClient.
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
            // Madde 13: 429 Rate Limiting is built-in by AddStandardResilienceHandler
        });

        return builder;
    }

    /// <summary>
    /// Adds a graceful degradation (Fallback) policy to return a default response when the service fails.
    /// </summary>
    public static IHttpClientBuilder AddGracefulDegradation<TFallback>(this IHttpClientBuilder builder, TFallback fallbackValue)
    {
        builder.AddResilienceHandler("GracefulDegradation", pipeline =>
        {
            pipeline.AddFallback(new Polly.Fallback.FallbackStrategyOptions<System.Net.Http.HttpResponseMessage>
            {
                ShouldHandle = new PredicateBuilder<System.Net.Http.HttpResponseMessage>()
                    .Handle<Exception>()
                    .HandleResult(r => !r.IsSuccessStatusCode),
                FallbackAction = args => 
                {
                    var response = new System.Net.Http.HttpResponseMessage(System.Net.HttpStatusCode.OK)
                    {
                        Content = new System.Net.Http.StringContent(System.Text.Json.JsonSerializer.Serialize(fallbackValue), System.Text.Encoding.UTF8, "application/json")
                    };
                    return System.Threading.Tasks.ValueTask.FromResult(Polly.Outcome.FromResult(response));
                }
            });
        });

        return builder;
    }
}
