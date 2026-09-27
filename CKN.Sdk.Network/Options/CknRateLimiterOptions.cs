using System;

namespace CKN.Sdk.Network.Options;

/// <summary>
/// Configures the client-side token-bucket rate limiter applied before each outgoing request.
/// Use this to prevent exceeding the external API's rate limit quota.
/// </summary>
public sealed class CknRateLimiterOptions
{
    /// <summary>
    /// Gets or sets the maximum number of requests allowed per <see cref="Period"/>.
    /// Defaults to <c>10</c>.
    /// </summary>
    public int RequestsPerPeriod { get; set; } = 10;

    /// <summary>
    /// Gets or sets the replenishment period. Defaults to <c>1 second</c>.
    /// </summary>
    public TimeSpan Period { get; set; } = TimeSpan.FromSeconds(1);

    /// <summary>
    /// Gets or sets the maximum number of requests that may queue while waiting for a token.
    /// Requests beyond this limit receive an immediate failure result.
    /// Defaults to <c>100</c>.
    /// </summary>
    public int QueueLimit { get; set; } = 100;
}
