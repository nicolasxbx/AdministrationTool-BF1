namespace AdminToolVG;

public static class Prompt
{
    public static string Get(string instruction)
    {
        return AnsiConsole.Prompt(
        new TextPrompt<string>(instruction)
        .PromptStyle("red"));
    }
    public static string GetPW(string instruction)
    {
        return AnsiConsole.Prompt(
        new TextPrompt<string>(instruction)
        .PromptStyle("red")
        .Secret());
    }
    public static bool Confirm(string instruction)
    {
        return AnsiConsole.Confirm(instruction);
    }
    public static void CenterAndOutputText(string text)
    {
        AnsiConsole.Write(new Spectre.Console.Rule(text).NoBorder());
    }
}
