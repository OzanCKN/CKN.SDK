using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Anthropic.SDK;
using Anthropic.SDK.Messaging;
using Microsoft.Extensions.AI;

namespace CKN.Sdk.AI.Anthropic;

/// <summary>
/// A wrapper around AnthropicClient that implements Microsoft.Extensions.AI.IChatClient.
/// </summary>
public sealed class AnthropicChatClient : IChatClient
{
    private readonly AnthropicClient _client;
    private readonly string _defaultModel;

    public AnthropicChatClient(AnthropicClient client, string defaultModel = "claude-3-5-sonnet-20240620")
    {
        _client = client ?? throw new ArgumentNullException(nameof(client));
        _defaultModel = defaultModel;
    }

    public async Task<ChatResponse> GetResponseAsync(IEnumerable<ChatMessage> chatMessages, ChatOptions? options = null, CancellationToken cancellationToken = default)
    {
        var parameters = CreateMessageParameters(chatMessages, options);
        var response = await _client.Messages.GetClaudeMessageAsync(parameters, cancellationToken).ConfigureAwait(false);

        var chatMessage = new ChatMessage(ChatRole.Assistant, response.Content.OfType<global::Anthropic.SDK.Messaging.TextContent>().FirstOrDefault()?.Text);
        
        return new ChatResponse(chatMessage)
        {
            ModelId = response.Model
        };
    }

    public async IAsyncEnumerable<ChatResponseUpdate> GetStreamingResponseAsync(IEnumerable<ChatMessage> chatMessages, ChatOptions? options = null, [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var parameters = CreateMessageParameters(chatMessages, options);
        
        await foreach (var response in _client.Messages.StreamClaudeMessageAsync(parameters, cancellationToken).ConfigureAwait(false))
        {
            if (response.Delta?.Text != null)
            {
                yield return new ChatResponseUpdate
                {
                    Role = ChatRole.Assistant,
                    Contents = new List<AIContent> { new Microsoft.Extensions.AI.TextContent(response.Delta.Text) },
                    ModelId = _defaultModel // Stream updates may not include the model id in Anthropic
                };
            }
        }
    }
    
    public void Dispose()
    {
        // AnthropicClient is typically registered as a singleton and managed by DI
    }

    public object? GetService(Type serviceType, object? serviceKey = null)
    {
        if (serviceType == typeof(AnthropicClient))
        {
            return _client;
        }

        return null;
    }

    private MessageParameters CreateMessageParameters(IEnumerable<ChatMessage> chatMessages, ChatOptions? options)
    {
        var systemMessage = chatMessages.FirstOrDefault(m => m.Role == ChatRole.System);
        
        var messages = chatMessages
            .Where(m => m.Role != ChatRole.System) // Anthropic doesn't allow system role in the messages array
            .Select(m => new Message(
                m.Role == ChatRole.User ? RoleType.User : RoleType.Assistant, 
                m.Text ?? string.Empty))
            .ToList();

        var parameters = new MessageParameters
        {
            Messages = messages,
            Model = options?.ModelId ?? _defaultModel,
            MaxTokens = (int)(options?.MaxOutputTokens ?? 1024),
            Temperature = (decimal?)options?.Temperature,
            TopP = (decimal?)options?.TopP,
        };

        if (systemMessage != null && !string.IsNullOrWhiteSpace(systemMessage.Text))
        {
            parameters.System = new List<SystemMessage> { new SystemMessage(systemMessage.Text) };
        }

        return parameters;
    }
}
