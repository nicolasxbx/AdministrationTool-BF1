using NLog.Targets.Wrappers;

namespace AdminToolVG;

public static partial class Navigation
{
    public static class Automation
    {
        #region Automation Prompt
        public static async Task AutomationPrompt()
        {
            string[] options = { "[grey58]Return[/]", "", "", "", "", "", "" };

            if (Vari.AutoKickBreakPlayer)
            {
                options[1] = "[lime]Disable AutoKick[/]";
            }
            else
            {
                options[1] = "[red]Enable AutoKick[/]";
            }

            if (Vari.AutoKickWinSwitching)
            {
                options[2] = "[lime]Disable Winswitch AutoKick[/]";
            }
            else
            {
                options[2] = "[red]Enable Winswitch AutoKick[/]";
            }

            if (Vari.AutoKickPing)
            {
                options[3] = "[lime]Disable Ping AutoKick[/]";
            }
            else
            {
                options[3] = "[red]Enable Ping AutoKick[/]";
            }

            if (Vari.AutoSusCheck)
            {
                options[4] = "[lime]Disable AutoSusCheck[/]";
            }
            else
            {
                options[4] = "[red]Enable AutoSusCheck (increased CPU usage)[/]";
            }

            if (Vari.AutoChat)
            {
                options[5] = "[lime]Disable Auto Chatting Rules[/]";
            }
            else
            {
                options[5] = "[red]Enable Auto Chatting Rules[/]";
            }

            if (Vari.AutoBalancer)
            {
                options[6] = "[lime]Disable AutoBalancer[/]";
            }
            else
            {
                options[6] = "[red]Enable AutoBalancer[/]";
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
                case "[lime]Disable AutoKick[/]":
                    await AutomationToggleAutoKick();
                    break;
                case "[red]Enable AutoKick[/]":
                    await AutomationToggleAutoKick();
                    break;

                case "[lime]Disable Winswitch AutoKick[/]":
                    await AutomationToggleAutoKickWS();
                    break;
                case "[red]Enable Winswitch AutoKick[/]":
                    await AutomationToggleAutoKickWS();
                    break;

                case "[lime]Disable Ping AutoKick[/]":
                    await AutomationToggleAutoKickPing();
                    break;
                case "[red]Enable Ping AutoKick[/]":
                    await AutomationToggleAutoKickPing();
                    break;

                case "[lime]Disable AutoSusCheck[/]":
                    await AutomationToggleAutoSusCheck();
                    break;
                case "[red]Enable AutoSusCheck (increased CPU usage)[/]":
                    await AutomationToggleAutoSusCheck();
                    break;

                case "[lime]Disable Auto Chatting Rules[/]":
                    await AutomationToggleAutoChat();
                    break;
                case "[red]Enable Auto Chatting Rules[/]":
                    await AutomationToggleAutoChat();
                    break;

                case "[lime]Disable AutoBalancer[/]":
                    await AutomationToggleAutoBalancer();
                    break;
                case "[red]Enable AutoBalancer[/]":
                    await AutomationToggleAutoBalancer();
                    break;

                case "[grey58]Return[/]":
                    return;
            }
            ClearConsole();
            await AutomationPrompt();
        }
        #endregion

        #region ToggleAutoKick
        static async Task AutomationToggleAutoKick()
        {
            // TURN OFF
            if (Vari.AutoKickBreakPlayer)
            {
                Vari.AutoKickBreakPlayer = false;
                return;
            }

            if (await AuthCheck() == false) return;

            Vari.AutoKickBreakPlayer = true;

            await LogMonitoringCheck();
        }
        #endregion

        #region ToggleAutoKick WS
        static async Task AutomationToggleAutoKickWS()
        {
            // TURN OFF
            if (Vari.AutoKickWinSwitching)
            {
                Vari.AutoKickWinSwitching = false;
                return;
            }

            Vari.AutoKickWinSwitching = true;

            await LogMonitoringCheck();
        }
        #endregion

        #region ToggleAutoKickPing
        static async Task AutomationToggleAutoKickPing()
        {
            // TURN OFF
            if (Vari.AutoKickPing)
            {
                Vari.AutoKickPing = false;
                return;
            }

            if (await AuthCheck() == false) return;

            Vari.AutoKickPing = true;

            Services.TryStartPingService();

            await LogMonitoringCheck();
        }
        #endregion

        #region ToggleAutoSusCheck
        static async Task AutomationToggleAutoSusCheck()
        {
            // TURN OFF
            if (Vari.AutoSusCheck)
            {
                Vari.AutoSusCheck = false;
                return;
            }

            if (await AuthCheck() == false) return;

            Vari.AutoSusCheck = true;

            Services.TryStartAutoSusService();

            await LogMonitoringCheck();
        }
        #endregion

        #region AutoChat
        static async Task AutomationToggleAutoChat()
        {
            // TURN OFF
            if (Vari.AutoChat)
            {
                Vari.AutoChat = false;
                return;
            }

            if (await AuthCheck() == false) return;

            Vari.AutoChat = true;

            Services.TryStartAutoChatService();

            await LogMonitoringCheck();
        }
        #endregion

        #region AutoBalancer
        static async Task AutomationToggleAutoBalancer()
        {
            // TURN OFF
            if (Vari.AutoBalancer)
            {
                Vari.AutoBalancer = false;
                return;
            }

            if (await AuthCheck() == false) return;

            Vari.AutoBalancer = true;

            Services.TryStartAutoBalanceService();

            await LogMonitoringCheck();
        }
        #endregion
    }

    #region Misc
    static async Task<bool> AuthCheck()
    {
        if (string.IsNullOrEmpty(Vari.SessionID))
        {
            Log.CM("SessionID is empty");
            Console.ReadLine();
            return false;
        }

        var result = await BF1API.GetWelcomeMessage();
        if (!result.IsSuccess)
        {
            Log.CM("SessionID is invalid.");
            Console.ReadLine();
            return false;
        }
        Log.CM("\nSessionID is valid.");

        if (string.IsNullOrEmpty(Vari.GameId))
        {
            Log.CM("GameID not found.");
            Console.ReadLine();
            return false;
        }
        Log.CM("GameID is valid.");

        await Services.ServerDetailService.FetchFullServerDetails();
        if (Vari.ServerDetails_AdminList_PID.Count == 0)
        {
            Log.CM("Server Error. Make sure you are Admin on this Server");
            Console.ReadLine();
            return false;
        }
        Log.CM("Serverinfo gathered.");

        if (!Vari.ServerDetails_AdminList_Name.Contains(Vari.CurrentUsername))
        {
            Log.CM($"User {Vari.CurrentUsername} is not Admin on this Server!");
            Console.ReadLine();
            return false;
        }

        return true;
    }

    static async Task LogMonitoringCheck()
    {
        if (Vari.Webhooks.Monitoring_first_action)
        {
            await DWebHooks.LogMonitoringON();
            Vari.Webhooks.Monitoring_first_action = false;
        }
    }
    #endregion
}
