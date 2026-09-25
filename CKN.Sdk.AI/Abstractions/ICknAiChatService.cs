using System.Threading;
using System.Threading.Tasks;

namespace CKN.Sdk.AI.Abstractions;

public class CknAiRequest
{
    public string SystemPrompt { get; set; } = string.Empty;
    public string UserMessage { get; set; } = string.Empty;
    public float Temperature { get; set; } = 0.7f;
    public int MaxTokens { get; set; } = 1000;
}

public class CknAiResponse
{
    public string Content { get; set; } = string.Empty;
    public int TotalTokens { get; set; }
}

public interface ICknAiChatService
{
    Task<CknAiResponse> AskAsync(CknAiRequest request, CancellationToken cancellationToken = default);
}
