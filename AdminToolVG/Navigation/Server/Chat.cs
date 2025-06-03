namespace AdminToolVG;

public static partial class Navigation
{
    #region Chat
    static void ChatPrompt()
    {
        Log.CM("[bold red]Try the Hotkey Alt+O when ingame![/]\n");
        Log.C($"Current Ruletext: \n{Vari.CurrentRuleString}\n");

        string selection = AnsiConsole.Prompt(
           new SelectionPrompt<string>()
           .AddChoices(new[] { "[grey58]Return[/]", "Send Ruletext (ALT+O Ingame)", "Send Custom Message", })
           .HighlightStyle(new Spectre.Console.Style(Spectre.Console.Color.OrangeRed1))
           );
        PlaySound();

        if (selection == "Send Ruletext (ALT+O Ingame)")
        {
            string chatmsg = Vari.CurrentRuleString;            
            CustomChat(chatmsg);
        }
        else if (selection == "Send Custom Message")
        {
            var chatmsg = AnsiConsole.Ask<string>("Message:");            
            CustomChat(chatmsg);
        }        
    }
    static void CustomChat(string chatmsg)
    {
        PlaySound();
        if (Util_BF1.AdminActions.ChatFunction(chatmsg))
        {
            Log.CM("Message sent.");
            Log.I($"Message sent: {chatmsg}");
        }
        else
        {
            Log.CM("Error. Try Borderless Mode.");
        }

        Console.ReadLine();
    }
    #endregion
}
