using System;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.Elasticsearch;

namespace CKN.Sdk.Infrastructure.Logging;

public static class SerilogConfigurationExtensions
{
    /// <summary>
    /// Configures Serilog with Console and Elasticsearch sinks.
    /// Also applies CorrelationId and PII masking automatically.
    /// </summary>
    public static IHostBuilder AddCknElasticLogging(this IHostBuilder builder, string elasticUri, string applicationName)
    {
        return builder.UseSerilog((context, loggerConfiguration) =>
        {
            loggerConfiguration
                .MinimumLevel.Information()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                .MinimumLevel.Override("Microsoft.Hosting.Lifetime", LogEventLevel.Information)
                .Enrich.FromLogContext()
                .Enrich.WithProperty("Application", applicationName)
                .Enrich.WithMachineName()
                .Enrich.WithEnvironmentName()
                .WriteTo.Console()
                .WriteTo.Elasticsearch(new ElasticsearchSinkOptions(new Uri(elasticUri))
                {
                    AutoRegisterTemplate = true,
                    IndexFormat = $"{applicationName.ToLower().Replace(".", "-")}-logs-{{0:yyyy.MM.dd}}",
                    NumberOfReplicas = 1,
                    NumberOfShards = 2
                });
        });
    }
}
