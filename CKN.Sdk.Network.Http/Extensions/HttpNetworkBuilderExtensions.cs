using System;
using CKN.Sdk.Network.Abstractions;
using CKN.Sdk.Network.Auth;
using CKN.Sdk.Network.Http.DelegatingHandlers;
using CKN.Sdk.Network.Http.Services;
using CKN.Sdk.Network.Options;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Http.Resilience;
using Microsoft.Extensions.Logging;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Extension methods that register the <c>HttpClient</c>-based provider for <see cref="ICknNetworkBuilder"/>.
/// </summary>
public static class HttpNetworkBuilderExtensions
{
    /// <summary>
    /// Registers a typed HTTP client backed by <see cref="System.Net.Http.HttpClient"/> with resilience,
    /// authentication, rate limiting, and sensitive-parameter masking pre-configured.
    /// </summary>
    /// <typeparam name="TClient">
    /// The typed client class. Must derive from <see cref="CknHttpClientBase"/>.
    /// </typeparam>
    /// <param name="builder">The CKN network builder.</param>
    /// <param name="configure">A delegate that configures <see cref="CknHttpClientOptions"/>.</param>
    /// <returns>An <see cref="IHttpClientBuilder"/> for additional custom configuration.</returns>
    /// <example>
    /// <code>
    /// services.AddCknNetwork(net =>
    ///     net.AddCknHttpClient&lt;FinnhubProvider&gt;(opt =>
    ///     {
    ///         opt.BaseAddress = "https://finnhub.io/api/v1/";
    ///         opt.Auth = new ApiKeyQueryAuthStrategy("token", config["Finnhub:Key"]!);
    ///         opt.RateLimit = new CknRateLimiterOptions { RequestsPerPeriod = 1 };
    ///     }));
    /// </code>
    /// </example>
    public static IHttpClientBuilder AddCknHttpClient<TClient>(
        this ICknNetworkBuilder builder,
        Action<CknHttpClientOptions> configure)
        where TClient : CknHttpClientBase
    {
        var clientName = typeof(TClient).Name;
        var options = new CknHttpClientOptions();
        configure(options);

        builder.Services.Configure<CknHttpClientOptions>(clientName, configure);

        var httpClientBuilder = builder.Services
            .AddHttpClient<TClient>(client =>
            {
                if (!string.IsNullOrWhiteSpace(options.BaseAddress))
                    client.BaseAddress = new Uri(options.BaseAddress);

                client.Timeout = options.Timeout;

                foreach (var (name, value) in options.DefaultHeaders)
                    client.DefaultRequestHeaders.TryAddWithoutValidation(name, value);
            });

        // 1. Sensitive param masking + logging (outermost handler)
        httpClientBuilder.AddHttpMessageHandler(sp =>
            new SensitiveQueryMaskingHandler(
                options.SensitiveQueryParams,
                sp.GetRequiredService<ILoggerFactory>().CreateLogger(clientName),
                clientName));

        // 2. Authentication handler
        if (options.Auth is ApiKeyQueryAuthStrategy queryAuth)
            httpClientBuilder.AddHttpMessageHandler(() =>
                new ApiKeyQueryAuthHandler(queryAuth.ParameterName, queryAuth.Value));
        else if (options.Auth is ApiKeyHeaderAuthStrategy headerAuth)
            httpClientBuilder.AddHttpMessageHandler(() =>
                new ApiKeyHeaderAuthHandler(headerAuth.HeaderName, headerAuth.Value));
        else if (options.Auth is BearerAuthStrategy bearerAuth)
            httpClientBuilder.AddHttpMessageHandler(() =>
                new BearerAuthHandler(bearerAuth.Token));

        // 3. Client-side rate limiter
        if (options.RateLimit is not null)
            httpClientBuilder.AddHttpMessageHandler(() =>
                new RateLimiterHandler(options.RateLimit));

        // 4. Polly resilience (retry, circuit breaker, timeout)
        httpClientBuilder.AddStandardResilienceHandler(resilienceOptions =>
        {
            resilienceOptions.Retry.MaxRetryAttempts = options.Retry.MaxAttempts;
            resilienceOptions.Retry.Delay = options.Retry.BaseDelay;
            resilienceOptions.Retry.UseJitter = options.Retry.UseJitter;

            resilienceOptions.CircuitBreaker.FailureRatio = options.CircuitBreaker.FailureRatio;
            resilienceOptions.CircuitBreaker.MinimumThroughput = options.CircuitBreaker.MinimumThroughput;
            resilienceOptions.CircuitBreaker.SamplingDuration = options.CircuitBreaker.SamplingDuration;
            resilienceOptions.CircuitBreaker.BreakDuration = options.CircuitBreaker.BreakDuration;

            resilienceOptions.AttemptTimeout.Timeout = options.Timeout;
        });

        return httpClientBuilder;
    }
}
