using System;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using CKN.Sdk.Core.Events;

namespace CKN.Sdk.Messaging.Webhooks;

/// <summary>
/// A default implementation for receiving webhooks and bridging them to the EventBus.
/// </summary>
public class WebhookReceiver : IWebhookReceiver
{
    private readonly IEventBus _eventBus;
    private readonly ILogger<WebhookReceiver> _logger;

    public WebhookReceiver(IEventBus eventBus, ILogger<WebhookReceiver> logger)
    {
        _eventBus = eventBus;
        _logger = logger;
    }

    public async Task<bool> ReceiveAsync(string payload, string signature, string secret, string eventName)
    {
        if (string.IsNullOrWhiteSpace(payload) || string.IsNullOrWhiteSpace(signature) || string.IsNullOrWhiteSpace(secret))
        {
            _logger.LogWarning("Webhook payload, signature, or secret is missing.");
            return false;
        }

        // Validate HMAC SHA256 Signature
        var isValid = ValidateSignature(payload, signature, secret);
        if (!isValid)
        {
            _logger.LogWarning("Webhook signature validation failed for event: {EventName}", eventName);
            return false;
        }

        // Bridge to EventBus
        var integrationEvent = new WebhookIntegrationEvent(payload, eventName);
        
        try
        {
            await _eventBus.PublishAsync(integrationEvent);
            _logger.LogInformation("Webhook successfully bridged to EventBus as {EventName}", eventName);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to publish webhook to EventBus for event: {EventName}", eventName);
            return false;
        }
    }

    private bool ValidateSignature(string payload, string signature, string secret)
    {
        // Many webhook providers prefix the signature with algorithm name, e.g., 'sha256=...'
        var actualSignature = signature;
        if (signature.StartsWith("sha256=", StringComparison.OrdinalIgnoreCase))
        {
            actualSignature = signature.Substring(7);
        }

        using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(secret));
        var hashBytes = hmac.ComputeHash(Encoding.UTF8.GetBytes(payload));
        var computedSignature = BitConverter.ToString(hashBytes).Replace("-", "").ToLowerInvariant();

        // Use constant time comparison to avoid timing attacks
        return CryptographicOperations.FixedTimeEquals(
            Encoding.UTF8.GetBytes(computedSignature),
            Encoding.UTF8.GetBytes(actualSignature.ToLowerInvariant()));
    }
}
