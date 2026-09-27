using System;
using System.Collections.Generic;

namespace CKN.Sdk.Network.Options;

/// <summary>
/// Configures the retry policy applied to outgoing HTTP requests.
/// Retries are triggered for transient errors: network failures, 408, 429, and 5xx responses.
/// </summary>
public sealed class CknRetryOptions
{
    /// <summary>
    /// Gets or sets the maximum number of retry attempts (not counting the initial attempt).
    /// Defaults to <c>3</c>.
    /// </summary>
    public int MaxAttempts { get; set; } = 3;

    /// <summary>
    /// Gets or sets the base delay between retries. With exponential back-off this value doubles each attempt.
    /// Defaults to <c>2 seconds</c>.
    /// </summary>
    public TimeSpan BaseDelay { get; set; } = TimeSpan.FromSeconds(2);

    /// <summary>
    /// Gets or sets whether to add random jitter to the retry delay to prevent thundering-herd scenarios.
    /// Defaults to <c>true</c>.
    /// </summary>
    public bool UseJitter { get; set; } = true;

    /// <summary>
    /// Gets or sets additional HTTP status codes beyond the defaults (408, 429, 5xx)
    /// that should trigger a retry.
    /// </summary>
    public IReadOnlyList<int> AdditionalRetryStatusCodes { get; set; } = [];
}
