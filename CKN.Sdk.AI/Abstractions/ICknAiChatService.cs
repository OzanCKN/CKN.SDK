using System.Threading;
using System.Threading.Tasks;

namespace CKN.Sdk.AI.Abstractions;

/// <summary>
/// Encapsulates a single conversational AI request. All fields are optional
/// except that either <see cref="UserMessage"/> or <see cref="SystemPrompt"/> must be non-empty.
/// </summary>
public class CknAiRequest
{
    /// <summary>
    /// Gets or sets the system-level instruction that shapes the model's behaviour and persona.
    /// </summary>
    public string SystemPrompt { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the user message or prompt to send to the model.
    /// </summary>
    public string UserMessage { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the sampling temperature (0–2). Higher values produce more creative responses;
    /// lower values make responses more deterministic. Defaults to <c>0.7</c>.
    /// </summary>
    public float Temperature { get; set; } = 0.7f;

    /// <summary>
    /// Gets or sets the maximum number of tokens the model may generate. Defaults to <c>1000</c>.
    /// </summary>
    public int MaxTokens { get; set; } = 1000;
}

/// <summary>
/// Represents the AI model's response to a <see cref="CknAiRequest"/>.
/// </summary>
public class CknAiResponse
{
    /// <summary>Gets or sets the generated text content.</summary>
    public string Content { get; set; } = string.Empty;

    /// <summary>Gets or sets the total number of tokens consumed by the request and response.</summary>
    public int TotalTokens { get; set; }
}

/// <summary>
/// Provider-agnostic chat interface for conversational AI.
/// Register a concrete implementation via <c>AddCknOpenAI</c>, <c>AddCknAnthropic</c>, etc.
/// </summary>
public interface ICknAiChatService
{
    /// <summary>
    /// Sends a <see cref="CknAiRequest"/> to the configured AI provider and returns the response.
    /// </summary>
    /// <param name="request">The chat request including prompt, system instructions, and sampling parameters.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A <see cref="CknAiResponse"/> containing the generated content and token usage.</returns>
    Task<CknAiResponse> AskAsync(CknAiRequest request, CancellationToken cancellationToken = default);
}
