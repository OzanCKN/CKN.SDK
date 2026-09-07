using System;
using CKN.Sdk.Core.Events;

namespace CKN.Sdk.Messaging.Webhooks;

/// <summary>
/// A generic integration event representing an external webhook payload.
/// </summary>
public record WebhookIntegrationEvent : IntegrationEvent
{
    public string Payload { get; }
    public string ProviderEventName { get; }

    public WebhookIntegrationEvent(string payload, string providerEventName)
    {
        Payload = payload;
        ProviderEventName = providerEventName;
    }
}
