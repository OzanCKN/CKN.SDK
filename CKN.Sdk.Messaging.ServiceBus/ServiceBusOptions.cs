namespace CKN.Sdk.Messaging.ServiceBus;

/// <summary>
/// Configuration options for Azure Service Bus provider.
/// </summary>
public class ServiceBusOptions
{
    /// <summary>
    /// Gets or sets the Azure Service Bus connection string.
    /// </summary>
    public string ConnectionString { get; set; } = string.Empty;
}
