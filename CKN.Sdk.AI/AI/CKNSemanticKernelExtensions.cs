using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.SemanticKernel;
using System;
using System.Net.Http;

namespace CKN.Sdk.AI;

public static class CKNSemanticKernelExtensions
{
    public static IServiceCollection AddCKNSemanticKernel(this IServiceCollection services, IConfiguration configuration)
    {
        var kernelBuilder = services.AddKernel();

        bool useLocalLlm = configuration.GetValue<bool>("FeatureManagement:UseLocalLLM", true);
        bool useCloudLlm = configuration.GetValue<bool>("FeatureManagement:UseCloudLLM", false);

        if (useCloudLlm)
        {
            var provider = configuration["CloudLLM:Provider"];
            if (provider == "Gemini")
            {
                var chatModel = configuration["CloudLLM:ChatModel"] ?? "gemini-3.5-flash-lite";
                var apiKey = configuration["CloudLLM:ApiKey"] ?? "dummy-gemini-key";
                if (string.IsNullOrWhiteSpace(apiKey))
                {
                    apiKey = "dummy-gemini-key";
                }

                var httpClient = new HttpClient { Timeout = TimeSpan.FromMinutes(5) };

#pragma warning disable SKEXP0070 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
                kernelBuilder.AddGoogleAIGeminiChatCompletion(
                    modelId: chatModel,
                    apiKey: apiKey,
                    httpClient: httpClient
                );

                // Add text embedding generation for semantic search/memory features
                kernelBuilder.AddGoogleAIEmbeddingGeneration(
                    modelId: "gemini-embedding-2", // Standard Gemini embedding model
                    apiKey: apiKey,
                    httpClient: httpClient
                );
#pragma warning restore SKEXP0070
            }
        }
        else if (useLocalLlm)
        {
            // Use local Ollama
            var ollamaEndpoint = configuration["Ollama:Endpoint"] ?? "http://localhost:11434/v1";
            var httpClient = new HttpClient { BaseAddress = new Uri(ollamaEndpoint) };
            var chatModel = configuration["Ollama:ChatModel"] ?? "llama3";
            var embedModel = configuration["Ollama:EmbeddingModel"] ?? "mxbai-embed-large";
            var apiKey = configuration["Ollama:ApiKey"] ?? "dummy-key";

#pragma warning disable SKEXP0010
            kernelBuilder.AddOpenAIChatCompletion(
                modelId: chatModel,
                apiKey: apiKey,
                httpClient: httpClient
            );
            kernelBuilder.AddOpenAITextEmbeddingGeneration(
                modelId: embedModel,
                apiKey: apiKey,
                httpClient: httpClient
            );
#pragma warning restore SKEXP0010
        }
        else
        {
            // Use Azure OpenAI (or OpenAI) depending on what is configured
            var endpoint = configuration["AzureOpenAI:Endpoint"];
            var apiKey = configuration["AzureOpenAI:ApiKey"];
            var chatDeployment = configuration["AzureOpenAI:ChatDeploymentName"] ?? "gpt-4o";
            var embedDeployment = configuration["AzureOpenAI:EmbeddingDeploymentName"];

            if (!string.IsNullOrEmpty(endpoint) && endpoint != "https://dummy-endpoint.opckn.azure.com/" && !string.IsNullOrEmpty(apiKey))
            {
#pragma warning disable SKEXP0010
                kernelBuilder.AddAzureOpenAIChatCompletion(
                    deploymentName: chatDeployment,
                    endpoint: endpoint,
                    apiKey: apiKey
                );

                if (!string.IsNullOrEmpty(embedDeployment))
                {
                    kernelBuilder.AddAzureOpenAITextEmbeddingGeneration(
                        deploymentName: embedDeployment,
                        endpoint: endpoint,
                        apiKey: apiKey
                    );
                }
#pragma warning restore SKEXP0010
            }
            else
            {
                // Fallback to plain OpenAI
                var oaiModel = configuration["OpenAI:ChatModel"] ?? "gpt-4o";
                var oaiEmbedModel = configuration["OpenAI:EmbeddingModel"] ?? "text-embedding-3-small";
                var oaiKey = configuration["OpenAI:ApiKey"] ?? "dummy-key";
                var oaiEndpoint = configuration["OpenAI:Endpoint"];

                HttpClient? oaiClient = !string.IsNullOrEmpty(oaiEndpoint)
                    ? new HttpClient { BaseAddress = new Uri(oaiEndpoint) }
                    : null;

#pragma warning disable SKEXP0010
                kernelBuilder.AddOpenAIChatCompletion(
                    modelId: oaiModel,
                    apiKey: oaiKey,
                    httpClient: oaiClient
                );

                kernelBuilder.AddOpenAITextEmbeddingGeneration(
                    modelId: oaiEmbedModel,
                    apiKey: oaiKey,
                    httpClient: oaiClient
                );
#pragma warning restore SKEXP0010
            }
        }

        return services;
    }
}
