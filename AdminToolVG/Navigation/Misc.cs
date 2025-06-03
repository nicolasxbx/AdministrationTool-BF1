namespace AdminToolVG;

public static partial class Navigation
{    
    public static async Task MiscMenu()
    {
        Console.Clear();
        Log.CM("Misc\n");

        var selection = AnsiConsole.Prompt(new SelectionPrompt<string>()
            .HighlightStyle(new Spectre.Console.Style(Spectre.Console.Color.White))
            .AddChoices(new[] { "[grey58]Return[/]", "Switch yourself (ALT+P Ingame)", "Configure Discord Logging", "Toggle Audio" }));

        switch(selection)
        {            
            case "Switch yourself (ALT+P Ingame)":
                await Util_BF1.AdminActions.SwitchLocalPlayer_AwaitFreeSlot();
                break;
            case "Configure Discord Logging":
                WebhookMenu();
                break;
            case "Toggle Audio":
                if (FileConfig.CurrentConfig.AudioDisabled)
                {
                    FileConfig.CurrentConfig.AudioDisabled = false;
                }
                else
                {
                    FileConfig.CurrentConfig.AudioDisabled = true;
                }
                FileConfig.CurrentConfig.Save();
                break;
        }
    }
    public static void SupportMe()
    {        
        ClearConsole();
        Util.OpenLink("https://www.buymeacoffee.com/nex1");
        Log.CM("[gold1]This tool is fully free, thank you.[/]");        
        Console.ReadLine();
        Log.CM("[gold1]https://www.buymeacoffee.com/nex1[/]");
        Console.ReadLine();
    }
    static void WebhookMenu()
    {
        ClearConsole();
        Log.C("Discords Webhooks are formatted as shown: 'https://discord.com/api/webhooks/.../...'\n");

        var selection = AnsiConsole.Prompt(new SelectionPrompt<string>().AddChoices(new[]
            { "[grey58]Return[/]", "Show current Webhooks", "Monitoring", "Item Kicks", "Ping Kicks", "Winswitches/BFBANS", "Webhook Balancers" }));

        ClearConsole();
        switch (selection)
        {
            case "Show current Webhooks":
                Log.C("Current Webhook Configurations:");
                Log.C($"Webhook Monitoring: {FileConfig.CurrentConfig.WebhookConfig.monitoring}");
                Log.C($"Webhook Item Kicks: {FileConfig.CurrentConfig.WebhookConfig.itemkicks}");
                Log.C($"Webhook Ping Kicks: {FileConfig.CurrentConfig.WebhookConfig.pingkicks}");
                Log.C($"Webhook Winswitches/BFBANS: {FileConfig.CurrentConfig.WebhookConfig.winswitch_bfban}");
                Log.C($"Webhook Webhook Balancers: {FileConfig.CurrentConfig.WebhookConfig.balancers}");
                Console.ReadLine();
                break;
            case "Monitoring":
                FileConfig.CurrentConfig.WebhookConfig.monitoring = AnsiConsole.Ask<string>("Paste Webhook:");
                break;
            case "Item Kicks":
                FileConfig.CurrentConfig.WebhookConfig.itemkicks = AnsiConsole.Ask<string>("Paste Webhook:");
                break;
            case "Ping Kicks":
                FileConfig.CurrentConfig.WebhookConfig.pingkicks = AnsiConsole.Ask<string>("Paste Webhook:");
                break;
            case "Winswitches/BFBANS":
                FileConfig.CurrentConfig.WebhookConfig.winswitch_bfban = AnsiConsole.Ask<string>("Paste Webhook:");
                break;
            case "Webhook Balancers":
                FileConfig.CurrentConfig.WebhookConfig.balancers = AnsiConsole.Ask<string>("Paste Webhook:");
                break;
            case "[grey58]Return[/]":
                return;
        }

        FileConfig.CurrentConfig.Save();
        WebhookMenu();
    }
}