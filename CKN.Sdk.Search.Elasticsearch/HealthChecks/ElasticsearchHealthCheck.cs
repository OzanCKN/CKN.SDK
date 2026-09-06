using System;
using System.Threading;
using System.Threading.Tasks;
using Elastic.Clients.Elasticsearch;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging;

namespace CKN.Sdk.Search.Elasticsearch.HealthChecks;

/// <summary>
/// Health check implementation for Elasticsearch to ensure the cluster is responsive.
/// </summary>
public class ElasticsearchHealthCheck : IHealthCheck
{
    private readonly ElasticsearchClient _client;
    private readonly ILogger<ElasticsearchHealthCheck> _logger;

    public ElasticsearchHealthCheck(ElasticsearchClient client, ILogger<ElasticsearchHealthCheck> logger)
    {
        _client = client;
        _logger = logger;
    }

    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await _client.PingAsync(cancellationToken);

            if (response.IsValidResponse)
            {
                return HealthCheckResult.Healthy("Elasticsearch cluster is healthy.");
            }

            return HealthCheckResult.Unhealthy($"Elasticsearch ping failed. Reason: {response.DebugInformation}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Elasticsearch health check failed.");
            return new HealthCheckResult(context.Registration.FailureStatus, exception: ex);
        }
    }
}
