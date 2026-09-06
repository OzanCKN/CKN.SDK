namespace CKN.Sdk.Messaging.RabbitMQ;

/// <summary>
/// Configuration options for the RabbitMQ event bus provider.
/// </summary>
public class RabbitMQOptions
{
    /// <summary>
    /// Gets or sets the host name. Default is "localhost".
    /// </summary>
    public string HostName { get; set; } = "localhost";

    /// <summary>
    /// Gets or sets the port. Default is 5672.
    /// </summary>
    public int Port { get; set; } = 5672;

    /// <summary>
    /// Gets or sets the username. Default is "guest".
    /// </summary>
    public string UserName { get; set; } = "guest";

    /// <summary>
    /// Gets or sets the password. Default is "guest".
    /// </summary>
    public string Password { get; set; } = "guest";
    
    /// <summary>
    /// Gets or sets the queue name for this application to consume messages from.
    /// </summary>
    public string QueueName { get; set; } = "ckn.default.queue";
}
