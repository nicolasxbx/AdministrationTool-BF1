using BF1.ServerAdminTools;
using Spectre.Console;
using System.Drawing;

namespace AdminToolVG;

public static partial class Navigation
{
    static int Refresh_Delay { get; } = 1000;

    private static bool isPaused = false;
    private static bool shouldExit = false;
    private static bool threadActive = false;
    private static int scroll_depth = 0;
    public static void LiveDashboard()
    {
        Console.CursorVisible = false;
        isPaused = false;

        Thread updateScreenThread = new Thread(ThreadLoop_UpdateScreen)
        {
            IsBackground = true,
        };
        if (!threadActive)
        {
            shouldExit = false;
            updateScreenThread.Start();
            threadActive = true;
        }
        else
        {
            Log.D("ALREADY ACTIVE");
            shouldExit = true;
            while (true)
            {
                if (!threadActive)
                {
                    shouldExit = false;
                    updateScreenThread.Start();
                    threadActive = true;
                    break;
                }
            }
        }

        while (threadActive)
        {
            // Check if the user wants to pause or enter the menu
            ConsoleKeyInfo keyInfo = Console.ReadKey(true);
            if (keyInfo.Key == ConsoleKey.Spacebar)
            {
                // Toggle the pause state
                isPaused = !isPaused;     
                
                if (isPaused)
                {
                    Log.CM("[red]Paused.[/]");
                }
            }
            else if (keyInfo.Key == ConsoleKey.Enter)
            {
                shouldExit = true;
                break;
            }
            else if (keyInfo.Key == ConsoleKey.UpArrow && isPaused)
            {
                scroll_depth -= 30;
                if (scroll_depth < 0) scroll_depth = 0;
                Console.SetCursorPosition(0, scroll_depth);
            }
            else if (keyInfo.Key == ConsoleKey.DownArrow && isPaused)
            {
                scroll_depth += 30;                
                Console.SetCursorPosition(0, scroll_depth);
            }           
        }
        Console.CursorVisible = true;
    }
    static void ThreadLoop_UpdateScreen()
    {
        while (true)
        {
            threadActive = true;            

            if (isPaused)
            {
                Thread.Sleep(Refresh_Delay);
                continue;
            }
            else
            {
                if (shouldExit)
                {
                    threadActive = false;
                    break;
                }
                ShowDashboardScreen();
                Thread.Sleep(Refresh_Delay);
            }
        }
    }
    static void ShowDashboardScreen()
    {
        Console.Clear();
        Prompt.CenterAndOutputText("[grey58]Press Spacebar to Pause.[/]");


        string playercount = (Vari.Playerlist_Team1.Count + Vari.Playerlist_Team2.Count).ToString();
        Prompt.CenterAndOutputText($"[white]{Vari.CurrentMapName} ({Vari.ServerLiveInfo.Team1Score}-{Vari.ServerLiveInfo.Team2Score}) - {playercount} Players.[/]");
        Log.C("");

        ShowRules();
        ShowStatus();
        ShowSusPlayers();
        ShowLogs();

        Console.SetCursorPosition(0, 0);
        scroll_depth = 0;
    }

    static void ShowRules()
    {
        string status = "";
        string color = "[deeppink1_1]";
        if (Vari.CustomRules)
        {
            color = "[bold yellow]";
            status += $"{color}{Vari.CurrentlyAppliedRuleset.ConfigName}[/]";
        }
        else
        {
            status += $"{color}VG Rules[/]";
        }
        Prompt.CenterAndOutputText(status);
        Prompt.CenterAndOutputText($"[white]{Vari.CurrentlyAppliedRuleset.Pinglimit}ms[/] Pinglimit.");
        Prompt.CenterAndOutputText($"Minimum [white]{Vari.CurrentlyAppliedRuleset.KickAbovePlayerCount}[/] players.");

        string weapon_string = "";
        bool firstweapon = true;
        foreach (string weapon in Vari.CurrentlyAppliedRuleset.Weapons)
        {
            if (!firstweapon)
            {
                weapon_string += ", ";
            }
            else
            {
                firstweapon = false;
            }
            weapon_string += $"{PlayerUtil.GetWeaponShortTxt(weapon)}";
        }
        Prompt.CenterAndOutputText($"[blue]{weapon_string}[/]");
    }
    static void ShowStatus()
    {        
        string enabled_functions = string.Empty;
        if (Vari.AutoKickBreakPlayer) enabled_functions += "[lime]AutoKick[/]";        
        if (Vari.AutoKickWinSwitching)
        {
            if (enabled_functions != string.Empty) enabled_functions += ", ";
            enabled_functions += "[lime]AutoWinswitchKick[/]";
        }        
        if (Vari.AutoKickPing)
        {
            if (enabled_functions != string.Empty) enabled_functions += ", ";
            enabled_functions += "[lime]AutoPingKick[/]";
        }
        if (Vari.AutoSusCheck)
        {
            if (enabled_functions != string.Empty) enabled_functions += ", ";
            enabled_functions += "[lime]AutoSusCheck[/]";
        }

        if (enabled_functions != string.Empty)
        {
            enabled_functions = enabled_functions + " [grey58]enabled.[/]";
            Log.C("");
            Prompt.CenterAndOutputText(enabled_functions);
        }
        else
        {
            Log.C("");
        }
    }

    static void ShowSusPlayers()
    {
        if (Vari.SusPlayers.Count > 0)
        {
            string sus_players = string.Empty;
            foreach (var player in Vari.SusPlayers)
            {
                if (sus_players != string.Empty)
                {
                    sus_players += ", ";
                }
                sus_players += player.Name;
            }

            Log.C("");
            Prompt.CenterAndOutputText($"[red]SUS PLAYER(S) DETECTED:{sus_players}[/]");
        }
    }

    static void ShowLogs()
    {
        Log.C("");
        var table_logs = new Table().Title("Kicked Players:").Centered();
        table_logs.AddColumn(new TableColumn("#").Centered());
        table_logs.AddColumn(new TableColumn("Name").Centered());
        table_logs.AddColumn(new TableColumn("Reason").Centered());
        table_logs.AddColumn(new TableColumn("Time UTC").Centered());

        for (int i = Vari.Logs_Kicks.Count - 1; i >= 0; i--) //Reverse Iteration
        {
            var item = Vari.Logs_Kicks[i];
            string name = item.Name;
            string reason = item.Reason;
            if (item.Fail)
            {
                name = $"[red]{item.Name}[/]";
                reason = $"[red]{item.Reason}[/]";
            }
            table_logs.AddRow(item.Iteration.ToString(), name, reason, item.DateTime.ToShortTimeString());
        }

        var table_switches = new Table().Title("Team Switches:").Centered();
        table_switches.AddColumn(new TableColumn("#").Centered());
        table_switches.AddColumn(new TableColumn("Name").Centered());
        table_switches.AddColumn(new TableColumn("Info").Centered());
        table_switches.AddColumn(new TableColumn("Map").Centered());
        table_switches.AddColumn(new TableColumn("Time UTC").Centered());

        for (int i = Vari.Logs_Switches.Count - 1; i >= 0; i--) //Reverse Iteration
        {
            var item = Vari.Logs_Switches[i];
            table_switches.AddRow(item.Iteration.ToString(), item.Name, item.Status, item.Map, item.DateTime.ToShortTimeString());
        }

        if (Vari.Logs_Kicks.Count > 0)
        {
            AnsiConsole.Write(table_logs);            
        }
        else
        {
            Prompt.CenterAndOutputText("[grey58]No registered kicks.[/]");            
        }
        Log.C("");

        if (Vari.Logs_Switches.Count > 0)
        {
            Log.C("");
            AnsiConsole.Write(table_switches);
        }
        else
        {
            Prompt.CenterAndOutputText("[grey58]No registered Team Switches.[/]");
        }

        //Prompt.CenterAndOutputText(grid);
        Log.D(Vari.Logs_Switches.Count().ToString());
    }    
}
