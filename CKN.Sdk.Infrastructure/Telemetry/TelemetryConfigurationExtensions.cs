using Microsoft.Extensions.DependencyInjection;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace CKN.Sdk.Infrastructure.Telemetry;

/// <summary>
/// Extension methods for configuring OpenTelemetry in CKN services.
/// </summary>
public static class TelemetryConfigurationExtensions
{
    /// <summary>
    /// Adds OpenTelemetry tracing and metrics to the service collection.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="serviceName">The name of the service.</param>
    /// <param name="otlpEndpoint">The optional OTLP exporter endpoint (e.g., http://localhost:4317).</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddCKNTelemetry(this IServiceCollection services, string serviceName, string? otlpEndpoint = null)
    {
        services.AddOpenTelemetry()
            .ConfigureResource(resource => resource
                .AddService(serviceName)
                .AddTelemetrySdk())
            .WithTracing(tracing =>
            {
                tracing.AddAspNetCoreInstrumentation()
                       .AddHttpClientInstrumentation()
                       .AddEntityFrameworkCoreInstrumentation()
                       .AddConsoleExporter();

                if (!string.IsNullOrEmpty(otlpEndpoint))
                {
                    tracing.AddOtlpExporter(opt => opt.Endpoint = new System.Uri(otlpEndpoint));
                }
            })
            .WithMetrics(metrics =>
            {
                metrics.AddAspNetCoreInstrumentation()
                       .AddHttpClientInstrumentation()
                       .AddRuntimeInstrumentation()
                       .AddConsoleExporter();

                if (!string.IsNullOrEmpty(otlpEndpoint))
                {
                    metrics.AddOtlpExporter(opt => opt.Endpoint = new System.Uri(otlpEndpoint));
                }
            });

        return services;
    }
}
