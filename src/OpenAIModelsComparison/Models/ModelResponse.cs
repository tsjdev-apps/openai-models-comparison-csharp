namespace OpenAIModelsComparison.Models;

internal record ModelResponse(
    string DeploymentName,
    int PromptTokens,
    int CompletionTokens,
    TimeSpan Duration);
