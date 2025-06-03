using LinkDotNet.NUniqueHardwareID;

namespace AdminToolVG;

public static partial class Navigation
{    
    public static async Task SexusNavAuth()
    {
        //CHECK IF AUTHENTICATED IN CONFIG
        if (FileConfig.CurrentConfig.Auth_ID == Vari.HWID)
        {
            Vari.HWID_Authd = true;
        }
        //Attempt to Authenticate User
        else if (Vari.ServerDetails.ServerName.Contains("[VG]") && Vari.ServerDetails_AdminList_Name.Contains(Vari.CurrentUsername))
        {
            Vari.HWID_Authd = true;
            FileConfig.CurrentConfig.Auth_ID = Vari.HWID;
            FileConfig.CurrentConfig.Save();
        }

        //Already Authenticated
        if (Vari.HWID_Authd)
        {
            await SexusNav();
            return;
        }

        Log.C("Join the VG Server to authenticate yourself, then come back here.\n");
        Console.ReadLine();
    }

    public static async Task SexusNav() //Chat, Balance, Scoreboard, Sus
    {
        Log.C("Disclaimer: These checks will use more cpu-resources.\n");
        string[] options = { "[grey58]Return[/]", "", "", "" };

        if (Vari.SexusBotLiveFunctionsEnabled)
        {
            options[1] = "[lime]Disable Live Functions[/]";
        }
        else
        {
            options[1] = "[red]Enable Live Functions[/]";
        }

        SelectionPrompt<string> p = new();
        p.AddChoices(options);
        p.HighlightStyle = new Spectre.Console.Style(Spectre.Console.Color.Aqua);
        p.WrapAround = true;

        selection = AnsiConsole.Prompt(p);
        PlaySound();

        ClearConsole();

        switch (selection)
        {
            case "[lime]Disable Live Functions[/]":
                ToggleLiveFunctions();
                break;
            case "[red]Enable Live Functions[/]":
                ToggleLiveFunctions();
                break;
            case "[grey58]Return[/]":
                return;            
        }

        ClearConsole();
        await SexusNav();
    }

    static void ToggleLiveFunctions()
    {
        // TURN OFF
        if (Vari.SexusBotLiveFunctionsEnabled)
        {
            Vari.SexusBotLiveFunctionsEnabled = false;
            return;
        }        

        Log.C("Starting Bot...");

        Thread t_bot = new Thread(StartBot)
        {
            IsBackground = true,
        };
        t_bot.Start();

        Log.C("Starting Service...");

        Thread t_balance = new Thread(SexusBot.Live.Thread_Balance)
        {
            IsBackground = true,
        };
        t_balance.Start();        

        Thread t_sus = new Thread(SexusBot.Live.Thread_AutoSusCheck)
        {
            IsBackground = true,
        };
        t_sus.Start();

        Thread t_chat = new Thread(SexusBot.Live.Thread_ChatRules)
        {
            IsBackground = true,
        };
        //t_chat.Start();

        Thread t_scoreboard = new Thread(SexusBot.Live.Thread_Scoreboard_Channel)
        {
            IsBackground = true,
        };
        t_scoreboard.Start();

        Vari.SexusBotLiveFunctionsEnabled = true;

        Log.D("Started Services");
    }

    public static async void StartBot()
    {
        if (Vari.SexusBot.IsRunning == false)
        {
            await SexusBot.Start();
        }
    }
}