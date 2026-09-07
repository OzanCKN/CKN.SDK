using System.Threading.Tasks;

namespace CKN.Sdk.Messaging.Webhooks;

/// <summary>
/// Defines a contract for receiving and processing external webhooks safely.
/// </summary>
public interface IWebhookReceiver
{
    /// <summary>
    /// Validates the webhook payload against a signature, and if valid, processes it.
    /// </summary>
    /// <param name="payload">The raw JSON payload from the HTTP request body.</param>
    /// <param name="signature">The signature provided in the headers (e.g., X-Hub-Signature-256).</param>
    /// <param name="secret">The secret key used to compute the expected HMAC signature.</param>
    /// <param name="eventName">The name of the event to publish to the EventBus.</param>
    /// <returns>A boolean indicating if the webhook was successfully validated and processed.</returns>
    Task<bool> ReceiveAsync(string payload, string signature, string secret, string eventName);
}
