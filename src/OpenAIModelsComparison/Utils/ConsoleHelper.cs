using OpenAIModelsComparison.Models;
using Spectre.Console;

namespace OpenAIModelsComparison.Utils;

internal class ConsoleHelper
{
    public static void CreateHeader()
    {
        AnsiConsole.Clear();

        Grid grid = new();
        grid.AddColumn();
        grid.AddRow(new FigletText("OpenAI Models").Centered().Color(Color.Red));
        grid.AddRow(Align.Center(new Panel("[red]Sample by Thomas Sebastian Jensen ([link]https://www.tsjdev-apps.de[/])[/]")));

        AnsiConsole.Write(grid);
        AnsiConsole.WriteLine();
    }

    public static List<string> GetSelection(
        string[] options)
    {
        CreateHeader();

        List<string> models = AnsiConsole.Prompt(
            new MultiSelectionPrompt<string>()
                .Title("Please select from the [yellow]options[/].")
                .Required()
                .PageSize(10)
                .InstructionsText(
                    "[grey](Press [yellow]<space>[/] to toggle your " +
                    "selection and [yellow]<enter>[/] to accept)[/]")
                .AddChoices(options));

        return models;
    }

    public static string GetString(
        string prompt)
    {
        CreateHeader();

        return AnsiConsole.Prompt(
            new TextPrompt<string>(prompt)
            .PromptStyle("white")
            .ValidationErrorMessage("[red]Invalid prompt[/]")
            .Validate(prompt =>
            {
                if (prompt.Length < 3)
                {
                    return ValidationResult.Error("[red]Value too short[/]");
                }

                if (prompt.Length > 200)
                {
                    return ValidationResult.Error("[red]Value too long[/]");
                }

                return ValidationResult.Success();
            }));
    }

    public static void CreateOutputInfo(
        ICollection<ModelResponse> modelResponses)
    {
        AnsiConsole.WriteLine();
        AnsiConsole.WriteLine();

        Table table = new();
        table.Border(TableBorder.Ascii);
        table.Expand();

        table.AddColumn("Model Name");
        table.AddColumn(new TableColumn("Prompt Tokens").Centered());
        table.AddColumn(new TableColumn("Completions Tokens").Centered());
        table.AddColumn(new TableColumn("Duration").Centered());

        foreach (ModelResponse modelResponse in modelResponses)
        {
            table.AddRow(
                modelResponse.DeploymentName,
                modelResponse.PromptTokens.ToString(),
                modelResponse.CompletionTokens.ToString(),
                modelResponse.Duration.ToString());
        }

        AnsiConsole.Write(table);
        AnsiConsole.WriteLine();
    }
}
