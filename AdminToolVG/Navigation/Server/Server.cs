namespace AdminToolVG;

public static partial class Navigation
{
    #region ServerPrompt
    static async Task ServerPrompt()
    {
        // TEAM SELECTION
        SelectionPrompt<string> prompt_team = new();
        prompt_team.AddChoice("[grey58]Return[/]");
        prompt_team.AddChoice("[dodgerblue2]Team 1[/]");
        prompt_team.AddChoice("[red]Team 2[/]");
        //"Chat", "Maps", "Scoreboard", 
        prompt_team.AddChoice("[lightseagreen]Maps[/]");
        prompt_team.AddChoice("[orangered1]Chat[/]");
        prompt_team.HighlightStyle = new Spectre.Console.Style(Spectre.Console.Color.Green3_1);

        string selection_team = AnsiConsole.Prompt(prompt_team);
        PlaySound();

        List<string> list_players = new();

        if (selection_team == "[dodgerblue2]Team 1[/]")
        {
            foreach (var item in Vari.Playerlist_Team1)
            {
                list_players.Add(item.Name);
            }
            await ServerActionsPromptPlayerSelection(list_players);
            Console.ReadLine();
        }
        else if (selection_team == "[red]Team 2[/]")
        {
            foreach (var item in Vari.Playerlist_Team2)
            {
                list_players.Add(item.Name);
            }
            await ServerActionsPromptPlayerSelection(list_players, true);
            Console.ReadLine();
        }
        else if (selection_team == "[lightseagreen]Maps[/]")
        {
            await MapPresets.Menu();
        }
        else if (selection_team == "[orangered1]Chat[/]")
        {
            ChatPrompt();
        }
    }
    #endregion

    #region Server Actions
    static async Task ServerActionsPromptPlayerSelection(List<string> list_players, bool team2 = false)
    {
        //Sort Playerlist
        list_players.Sort();

        //Prompt
        SelectionPrompt<string> prompt_players = new();
        prompt_players.AddChoice("Return");
        prompt_players.AddChoices(list_players);        
        if (team2)
        {
            prompt_players.HighlightStyle = new Spectre.Console.Style(Spectre.Console.Color.Red);
        }
        else
        {
            prompt_players.HighlightStyle = new Spectre.Console.Style(Spectre.Console.Color.Blue);
        }
        prompt_players.PageSize = 25;

        string selection_player = AnsiConsole.Prompt(prompt_players);
        PlaySound();

        if (selection_player == "Return")
        {
            await ServerPrompt();
        }
        else
        {
            await ServerActionsPromptActionSelection(selection_player, list_players, team2);
        }
    }
    static async Task ServerActionsPromptActionSelection(string selected_player, List<string> list_players, bool team2)
    {        
        SelectionPrompt<string> prompt_action = new();
        prompt_action.AddChoice("Return");
        prompt_action.AddChoice("Kick");
        prompt_action.AddChoice("Move");
        prompt_action.AddChoice("Ban");
        prompt_action.AddChoice("Fetch ID");
        if (team2)
        {
            prompt_action.HighlightStyle = new Spectre.Console.Style(Spectre.Console.Color.Red);
        }
        else
        {
            prompt_action.HighlightStyle = new Spectre.Console.Style(Spectre.Console.Color.Blue);
        }
        string selection_action = AnsiConsole.Prompt(prompt_action);
        PlaySound();

        if (selection_action == "Return")
        {
            await ServerActionsPromptPlayerSelection(list_players);
        }
        else if (selection_action == "Kick")
        {
            var reason = AnsiConsole.Ask<string>("Reason to kick?");
            var result = await Util_BF1.AdminActions.Kick.FindAndManualKick(selected_player, reason);

            if (result.Item1)
            {
                Log.CM("Player kicked successfully. (According to EA)");
            }
            else
            {
                Log.CM($"Error. ({result.Item2})");
            }
        }
        else if (selection_action == "Move")
        {
            var result = await Util_BF1.AdminActions.TryMovePlayer(selected_player);

            if (result.Item1)
            {
                Log.CM("Player moved successfully.");
            }
            else
            {
                Log.CM($"Error. ({result.Item2})");
            }
        }
        else if (selection_action == "Ban")
        {
            var success = await Util_BF1.AdminActions.BanPlayer(selected_player);

            if (success)
            {
                Log.CM($"'{selected_player}' [red]banned[/]. \nIf they are still in the server, you may need to kick them.");
                Console.ReadLine();
            }
            else
            {
                Log.CM("[red]EA Error. Maybe the ban-slots are full?[/]");
                Console.ReadLine();
            }
        }
        else if (selection_action == "Fetch ID")
        {
            var player = Vari.Playerlist_All.Find(x => x.Name == selected_player);

            if(player is null || player.PersonaId is 0)
            {
                Log.CM($"Error fetching player from server.");                
                return;
            }

            string id = await Util_BF1.AdminActions.GetIDFromPID(player.PersonaId);

            if(!string.IsNullOrEmpty(id))
            {
                Log.CM($"Player PID: {player.PersonaId}\nPlayer ID: {id}");                         
            }
            else
            {
                Log.CM("EA Server error.");                
            }
        }
    }
    #endregion    
}
