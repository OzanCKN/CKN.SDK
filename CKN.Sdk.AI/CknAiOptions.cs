using System;
using CKN.Sdk.AI.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace CKN.Sdk.AI;

public enum AiProviderType
{
    OpenAi,
    Gemini,
    Claude
}

public class CknAiOptions
{
    public AiProviderType Provider { get; set; } = AiProviderType.OpenAi;
    public string ApiKey { get; set; } = string.Empty;
    public string DefaultModel { get; set; } = "gpt-4o";
}
