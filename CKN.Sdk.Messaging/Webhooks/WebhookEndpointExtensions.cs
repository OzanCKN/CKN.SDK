using System.Text.Json;
using System.Threading.Tasks;
using CKN.Sdk.Core.Events;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace CKN.Sdk.Messaging.Webhooks;

/// <summary>
/// Provides extension methods for mapping webhook endpoints that directly publish to the EventBus.
/// </summary>
public static class WebhookEndpointExtensions
{
    /// <summary>
    /// Maps a POST endpoint that deserializes the incoming webhook payload and publishes it as an integration event.
    /// </summary>
    /// <typeparam name="TIntegrationEvent">The type of the integration event.</typeparam>
    /// <param name="endpoints">The endpoint route builder.</param>
    /// <param name="pattern">The route pattern.</param>
    /// <returns>The endpoint convention builder.</returns>
    public static IEndpointConventionBuilder MapCknWebhook<TIntegrationEvent>(this IEndpointRouteBuilder endpoints, string pattern)
        where TIntegrationEvent : IIntegrationEvent
    {
        return endpoints.MapPost(pattern, async (HttpContext context, IEventBus eventBus) =>
        {
            var payload = await JsonSerializer.DeserializeAsync<TIntegrationEvent>(context.Request.Body);
            
            if (payload == null)
            {
                return Results.BadRequest("Invalid webhook payload.");
            }

            await eventBus.PublishAsync(payload);
            return Results.Ok();
        });
    }
}
