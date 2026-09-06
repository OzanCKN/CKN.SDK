namespace CKN.Sdk.AI.OpenAI;

/// <summary>
/// Configuration options for the OpenAI AI provider.
/// </summary>
public class OpenAIOptions
{
    /// <summary>
    /// Gets or sets the API key for OpenAI.
    /// </summary>
    public string ApiKey { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the default model to use (e.g., "gpt-4o-mini").
    /// </summary>
    public string DefaultModel { get; set; } = "gpt-4o-mini";
}
