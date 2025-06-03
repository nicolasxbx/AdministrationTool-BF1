namespace AdminToolVG;

public static partial class Navigation
{
    #region Scoreboard
    static async Task ScoreboardPrompt()
    {
        int width_current = Console.WindowWidth;
        int height_current = Console.WindowHeight;

        var width_new = 152;
        var height_new = 40;

        if (width_new > Console.LargestWindowWidth)
        {
            width_new = Console.LargestWindowWidth;
        }
        if (height_new > Console.LargestWindowHeight)
        {
            height_new = Console.LargestWindowHeight;
        }

        if (width_new < width_current)
        {
            width_new = width_current;
        }
        if (height_new < height_current)
        {
            height_new = height_current;
        }

        Console.SetWindowSize(width_new, height_new);

        Console.Clear();

        while (true)
        {
            int index1 = 1;
            int index2 = 1;

            List<string> sp_ID = new();
            List<string> sp_ID2 = new();
            List<int> sp_Rank = new();
            List<int> sp_Rank2 = new();
            List<string> sp_name = new();
            List<string> sp_name2 = new();
            List<int> sp_K = new();
            List<int> sp_K2 = new();
            List<int> sp_D = new();
            List<int> sp_D2 = new();
            List<int> sp_Score = new();
            List<int> sp_Score2 = new();
            List<long> sp_pid = new();
            List<long> sp_pid2 = new();

            try
            {
                if (Vari.Playerlist_All!.Any() && Vari.Playerlist_All != null)
                {
                    List<PlayerData> Playerlist_All_Sorted = Vari.Playerlist_All.OrderByDescending(o => o.Score).ToList();
                    foreach (PlayerData p in Playerlist_All_Sorted)
                    {
                        if (p.TeamId == 1)
                        {
                            sp_ID.Add($"{index1}.");
                            sp_Rank.Add(p.Rank);
                            sp_name.Add(p.Name);
                            sp_K.Add(p.Kills);
                            sp_D.Add(p.Deaths);
                            sp_Score.Add(p.Score);
                            sp_pid.Add(p.PersonaId);
                            index1++;
                        }

                        if (p.TeamId == 2)
                        {
                            sp_ID2.Add($"{index2}.");
                            sp_Rank2.Add(p.Rank);
                            sp_name2.Add(p.Name);
                            sp_K2.Add(p.Kills);
                            sp_D2.Add(p.Deaths);
                            sp_Score2.Add(p.Score);
                            sp_pid2.Add(p.PersonaId);
                            index2++;
                        }
                    }
                }
                else
                {

                }
            }
            catch (Exception ex)
            {
                Log.Ex(ex);
            }

            var table = new Table();            
            table.AddColumn("ID");
            table.AddColumn("Rank");
            table.AddColumn("Name");
            table.AddColumn("Kills");
            table.AddColumn("Deaths");
            table.AddColumn("Score");
            table.AddColumn("PID");
            table.AddColumn(" ");
            table.AddColumn("ID");
            table.AddColumn("Rank");
            table.AddColumn("Name");
            table.AddColumn("Kills");
            table.AddColumn("Deaths");
            table.AddColumn("Score");
            table.AddColumn("PID");

            Console.Clear();
            table.Rows.Clear();
            table.Border(TableBorder.HeavyEdge);
            
            int j = 0;
            foreach (var item in sp_ID)
            {
                try
                {
                    table.AddRow(
                    $"[white]{sp_ID[j]}[/]", $"[grey50]{sp_Rank[j]}[/]", $"[deepskyblue1]{sp_name[j]}[/]", $"[dodgerblue3]{sp_K[j]}[/]", $"[dodgerblue3]{sp_D[j]}[/]", sp_Score[j].ToString(), $"[grey30]{sp_pid[j]}[/]",
                    " ",
                    $"[white]{sp_ID2[j]}[/]", $"[grey50]{sp_Rank2[j]}[/]", $"[red]{sp_name2[j]}[/]", $"[darkred_1]{sp_K2[j]}[/]", $"[darkred_1]{sp_D2[j]}[/]", sp_Score2[j].ToString(), $"[grey30]{sp_pid2[j]}[/]"
                    );
                    j++;
                }
                catch
                {
                    table.AddRow("", "", "", "", "", "", "", "", "", "", "", "", "", "", "");
                    j++;
                }
                
            }

            AnsiConsole.Write(table);
            Console.WriteLine();

            // Ask for the user's favorite fruit
            var selection = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                .AddChoices(new[] { "Refresh", "Return"})
                );
            PlaySound();

            if (selection == "Return")
            {
                Console.SetWindowSize(width_current, height_current);
                break;
            }
        }
    }    
    #endregion
}
