namespace CKN.Sdk.AI.Anthropic;

/// <summary>
/// Configuration options for the Anthropic AI provider.
/// </summary>
public class AnthropicOptions
{
    /// <summary>
    /// Gets or sets the API key for Anthropic.
    /// </summary>
    public string ApiKey { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the default model to use (e.g., "claude-3-opus-20240229").
    /// </summary>
    public string DefaultModel { get; set; } = "claude-3-opus-20240229";
}
