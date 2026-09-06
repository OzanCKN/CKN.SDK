namespace CKN.Sdk.Messaging.Kafka;

/// <summary>
/// Configuration options for the Kafka event bus provider.
/// </summary>
public class KafkaOptions
{
    /// <summary>
    /// Gets or sets the bootstrap servers (e.g., "localhost:9092").
    /// </summary>
    public string BootstrapServers { get; set; } = "localhost:9092";

    /// <summary>
    /// Gets or sets the consumer group id.
    /// </summary>
    public string GroupId { get; set; } = "ckn-default-group";
}
