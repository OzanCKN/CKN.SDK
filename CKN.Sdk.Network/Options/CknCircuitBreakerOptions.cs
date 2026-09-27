using System;

namespace CKN.Sdk.Network.Options;

/// <summary>
/// Configures the circuit breaker that trips when a target endpoint repeatedly fails,
/// preventing cascading failures across the application.
/// </summary>
public sealed class CknCircuitBreakerOptions
{
    /// <summary>
    /// Gets or sets the ratio of failed requests (0–1) within the sampling window
    /// that triggers the circuit to open. Defaults to <c>0.5</c> (50 %).
    /// </summary>
    public double FailureRatio { get; set; } = 0.5;

    /// <summary>
    /// Gets or sets the minimum number of requests that must occur within the
    /// sampling window before the failure ratio is evaluated. Defaults to <c>5</c>.
    /// </summary>
    public int MinimumThroughput { get; set; } = 5;

    /// <summary>
    /// Gets or sets the duration of the sampling window. Defaults to <c>30 seconds</c>.
    /// </summary>
    public TimeSpan SamplingDuration { get; set; } = TimeSpan.FromSeconds(30);

    /// <summary>
    /// Gets or sets how long the circuit stays open before transitioning to half-open.
    /// Defaults to <c>30 seconds</c>.
    /// </summary>
    public TimeSpan BreakDuration { get; set; } = TimeSpan.FromSeconds(30);
}
