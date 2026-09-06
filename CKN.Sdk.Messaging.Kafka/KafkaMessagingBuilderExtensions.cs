using System;
using CKN.Sdk.Core.Events;
using CKN.Sdk.Messaging.Extensions;
using CKN.Sdk.Messaging.Kafka;
using Confluent.Kafka;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection;

public static class KafkaMessagingBuilderExtensions
{
    /// <summary>
    /// Configures the messaging bus to use Apache Kafka as the transport provider.
    /// </summary>
    public static ICknMessagingBuilder UseKafka(this ICknMessagingBuilder builder, Action<KafkaOptions>? configureOptions = null)
    {
        var options = new KafkaOptions();
        configureOptions?.Invoke(options);
        
        builder.Services.Configure<KafkaOptions>(opt => 
        {
            opt.BootstrapServers = options.BootstrapServers;
            opt.GroupId = options.GroupId;
        });

        builder.Services.TryAddSingleton<IProducer<Null, string>>(sp =>
        {
            var config = new ProducerConfig { BootstrapServers = options.BootstrapServers };
            return new ProducerBuilder<Null, string>(config).Build();
        });

        builder.Services.TryAddSingleton<IEventBus, KafkaEventBus>();

        return builder;
    }
}
