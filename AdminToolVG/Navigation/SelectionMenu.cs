using NAudio.Wave;

namespace AdminToolVG;

public static partial class Navigation
{
    static string[] mainmenuoptions { get; } = { "[deeppink1_1]Live Dashboard[/]", "[aqua]Automation[/]", "[deepskyblue3]Rules[/]", "[green3_1]Server[/]", "Misc", "[gold1]Support me[/]" };
        
    internal static void ClearConsole()
    {
        Console.Clear();
        Log.C("");
    }

    #region SelectionMenu    
    static string selection = "";
    internal static async Task SelectionMenu()
    {
        ClearConsole();        

        if (Vari.FirstStartMenuDisplay)
        {            
            Vari.FirstStartMenuDisplay = false;
        }        

        SelectionPrompt<string> p = new();
        p.AddChoices(mainmenuoptions);
        p.HighlightStyle = new Spectre.Console.Style(Spectre.Console.Color.White);
        p.WrapAround = true;

        selection = AnsiConsole.Prompt(p);
        PlaySound();

        await SelectionNavigation();
    }    
    #endregion

    #region SelectionNavigation
    static async Task SelectionNavigation()
    {
        ClearConsole();

        switch (selection)
        {
            case "[deeppink1_1]Live Dashboard[/]":
                LiveDashboard();
                break;
            case "[aqua]Automation[/]":
                await Automation.AutomationPrompt();
                break;
            case "[deepskyblue3]Rules[/]":
                RulePrompts.RulePromptSelection();
                break;
            case "[green3_1]Server[/]":
                await ServerPrompt();
                break;
            case "Misc":
                await MiscMenu();
                break;
            case "[gold1]Support me[/]":
                SupportMe();
                break;
        }        
    }
    #endregion   

    public static void PlaySound()
    {
        if (FileConfig.CurrentConfig.AudioDisabled) return;

        Thread t = new Thread(PlaySound3)
        {
            IsBackground = true,
        };
        t.Start();
    }
    static void PlaySound3()
    {
        var player = new System.Media.SoundPlayer();
        player.Stream = Properties.Resources.sound;
        player.Play();
    }
}
