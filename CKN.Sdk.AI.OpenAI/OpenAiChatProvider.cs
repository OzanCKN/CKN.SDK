using System.Threading;
using System.Threading.Tasks;
using CKN.Sdk.AI.Abstractions;
using OpenAI.Chat;

namespace CKN.Sdk.AI.OpenAI;

public class OpenAiChatProvider : ICknAiChatService
{
    private readonly ChatClient _chatClient;
    private readonly string _defaultModel;

    public OpenAiChatProvider(string apiKey, string defaultModel = "gpt-4o")
    {
        _defaultModel = defaultModel;
        _chatClient = new ChatClient(_defaultModel, apiKey);
    }

    public async Task<CknAiResponse> AskAsync(CknAiRequest request, CancellationToken cancellationToken = default)
    {
        var messages = new ChatMessage[]
        {
            new SystemChatMessage(request.SystemPrompt),
            new UserChatMessage(request.UserMessage)
        };

        var options = new ChatCompletionOptions
        {
            Temperature = request.Temperature,
            MaxOutputTokenCount = request.MaxTokens
        };

        var response = await _chatClient.CompleteChatAsync(messages, options, cancellationToken);
        var completion = response.Value;

        return new CknAiResponse
        {
            Content = completion.Content[0].Text,
            TotalTokens = completion.Usage.TotalTokenCount
        };
    }
}
