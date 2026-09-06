using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;

namespace CKN.Sdk.Messaging.RabbitMQ.HealthChecks;

/// <summary>
/// Health check implementation for RabbitMQ to ensure the connection is alive.
/// </summary>
public class RabbitMQHealthCheck : IHealthCheck
{
    private readonly IConnectionFactory _connectionFactory;
    private readonly ILogger<RabbitMQHealthCheck> _logger;

    public RabbitMQHealthCheck(IConnectionFactory connectionFactory, ILogger<RabbitMQHealthCheck> logger)
    {
        _connectionFactory = connectionFactory;
        _logger = logger;
    }

    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        try
        {
            using var connection = await _connectionFactory.CreateConnectionAsync(cancellationToken);
            using var channel = await connection.CreateChannelAsync(cancellationToken: cancellationToken);

            if (connection.IsOpen && channel.IsOpen)
            {
                return HealthCheckResult.Healthy("RabbitMQ connection is healthy.");
            }

            return HealthCheckResult.Unhealthy("RabbitMQ connection or channel is not open.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "RabbitMQ health check failed.");
            return new HealthCheckResult(context.Registration.FailureStatus, exception: ex);
        }
    }
}
