using Azure;
using Azure.AI.OpenAI;
using OpenAIModelsComparison.Models;
using OpenAIModelsComparison.Utils;
using Spectre.Console;
using System.Diagnostics;

// Create header
ConsoleHelper.CreateHeader();

// Get OpenAI Key
string openAIKey =
    ConsoleHelper.GetString($"Please insert your [yellow]OpenAI[/] API key:");

List<string> selectedModels =
    ConsoleHelper.GetSelection(
        [Statics.GPT35TurboKey, Statics.GPT4Key,
        Statics.GPT4TurboKey, Statics.GPT4oKey]);

// Create OpenAI client
OpenAIClient client = new(openAIKey);

// Create header
ConsoleHelper.CreateHeader();

// Create ChatCompletionsOptions
List<ChatCompletionsOptions> chatCompletionsOptions = [];
foreach (string model in selectedModels)
{
    chatCompletionsOptions.Add(CreateChatCompletionsOptions(model));
}

while (true)
{
    AnsiConsole.WriteLine();
    AnsiConsole.MarkupLine("[green]USER:[/]");

    string? userMessage = Console.ReadLine();

    List<ModelResponse> modelResponses = [];
    foreach (ChatCompletionsOptions options in chatCompletionsOptions)
    {
        options.Messages.Add(new ChatRequestUserMessage(userMessage));
        ModelResponse response = await HandleRequest(options);
        modelResponses.Add(response);
    }

    ConsoleHelper.CreateOutputInfo(modelResponses);
}

async Task<ModelResponse> HandleRequest(ChatCompletionsOptions options)
{
    AnsiConsole.WriteLine();
    AnsiConsole.MarkupLine($"[green]{options.DeploymentName}:[/]");

    Stopwatch stopwatch = new();
    stopwatch.Start();

    Response<ChatCompletions> chatCompletionsResponse =
        await client.GetChatCompletionsAsync(options);

    stopwatch.Stop();

    string messageConent =
        chatCompletionsResponse.Value.Choices[0].Message.Content;

    AnsiConsole.WriteLine(
        messageConent);

    options.Messages.Add(
        new ChatRequestAssistantMessage(
            messageConent));

    CompletionsUsage usageInfo = chatCompletionsResponse.Value.Usage;

    return new ModelResponse(
        options.DeploymentName,
        usageInfo.PromptTokens,
        usageInfo.CompletionTokens,
        stopwatch.Elapsed);
}

static ChatCompletionsOptions CreateChatCompletionsOptions(
    string deploymentName)
{
    ChatCompletionsOptions chatCompletionsOptions = new()
    {
        MaxTokens = 1000,
        Temperature = 0.7f,
        DeploymentName = deploymentName,
    };

    chatCompletionsOptions.Messages.Add(new ChatRequestSystemMessage(
        "You are a helpful AI assistant."));

    return chatCompletionsOptions;
}