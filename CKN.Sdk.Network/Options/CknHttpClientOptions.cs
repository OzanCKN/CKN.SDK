using System;
using System.Collections.Generic;
using CKN.Sdk.Network.Auth;

namespace CKN.Sdk.Network.Options;

/// <summary>
/// All configuration for a single named or typed CKN HTTP client.
/// Pass an instance to <c>AddCknHttpClient&lt;T&gt;</c> via the <c>Action&lt;CknHttpClientOptions&gt;</c> delegate.
/// </summary>
public sealed class CknHttpClientOptions
{
    /// <summary>
    /// Gets or sets the base address of the remote API.
    /// All relative URLs in <c>GetJsonAsync</c> are resolved against this value.
    /// </summary>
    public string? BaseAddress { get; set; }

    /// <summary>
    /// Gets or sets the per-request timeout. Defaults to <c>10 seconds</c>.
    /// </summary>
    public TimeSpan Timeout { get; set; } = TimeSpan.FromSeconds(10);

    /// <summary>
    /// Gets or sets headers added to every outgoing request.
    /// Common use: <c>User-Agent</c>, <c>Accept</c>, custom tracing headers.
    /// </summary>
    public Dictionary<string, string> DefaultHeaders { get; set; } = [];

    /// <summary>
    /// Gets or sets the authentication strategy. Set to one of:
    /// <see cref="ApiKeyHeaderAuthStrategy"/>, <see cref="ApiKeyQueryAuthStrategy"/>, or <see cref="BearerAuthStrategy"/>.
    /// <c>null</c> means no authentication is added automatically.
    /// </summary>
    public CknAuthStrategy? Auth { get; set; }

    /// <summary>Gets or sets retry policy options. See <see cref="CknRetryOptions"/>.</summary>
    public CknRetryOptions Retry { get; set; } = new();

    /// <summary>Gets or sets circuit-breaker options. See <see cref="CknCircuitBreakerOptions"/>.</summary>
    public CknCircuitBreakerOptions CircuitBreaker { get; set; } = new();

    /// <summary>
    /// Gets or sets client-side rate limiter options. <c>null</c> disables rate limiting.
    /// Use this to prevent exceeding an external API's request quota.
    /// </summary>
    public CknRateLimiterOptions? RateLimit { get; set; }

    /// <summary>
    /// Gets or sets the maximum number of requests that run in parallel inside
    /// <c>BatchAsync</c>. Defaults to <c>4</c>.
    /// </summary>
    public int MaxDegreeOfParallelism { get; set; } = 4;

    /// <summary>
    /// Gets or sets query-parameter names whose values are redacted in structured logs.
    /// Comparison is case-insensitive. Defaults to common API key parameter names.
    /// </summary>
    public IReadOnlyList<string> SensitiveQueryParams { get; set; } =
        ["token", "apikey", "api_key", "key", "secret", "password", "access_token"];
}
