namespace CKN.Sdk.AI.Ollama;

/// <summary>
/// Configuration options for the Ollama provider.
/// </summary>
public class OllamaOptions
{
    /// <summary>
    /// Gets or sets the base URI for the Ollama instance (e.g. http://localhost:11434).
    /// </summary>
    public string Uri { get; set; } = "http://localhost:11434";

    /// <summary>
    /// Gets or sets the default model to use (e.g. llama3).
    /// </summary>
    public string DefaultModel { get; set; } = "llama3";
}
