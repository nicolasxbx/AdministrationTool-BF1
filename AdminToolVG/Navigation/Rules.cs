namespace AdminToolVG;

public static partial class Navigation
{
    public static class RulePrompts
    {
        #region RulePromptSelection
        public static void RulePromptSelection()
        {
            ClearConsole();

            SelectionPrompt<string> p = new();
            p.HighlightStyle = new Spectre.Console.Style(Spectre.Console.Color.DeepSkyBlue3);
            p.WrapAround = true;

            p.AddChoice("[grey58]Return[/]");

            p.AddChoice("Apply Ruleset");

            p.AddChoice("[green]Create Ruleset[/]");

            if (FileConfig.CurrentConfig.WeaponConfigs.Count > 0)
            {
                p.AddChoice("[darkred_1]Delete Ruleset[/]");
            }

            string selection = AnsiConsole.Prompt(p);
            PlaySound();

            if (selection == "Apply Ruleset")
            {
                ApplyRuleSetMenu();
            }            
            else if (selection == "[green]Create Ruleset[/]")
            {
                AddRulesetPrompt();
            }
            else if (selection == "[darkred_1]Delete Ruleset[/]")
            {
                DeleteRulesetPrompt();
            }
        }
        #endregion

        #region ApplyRuleSetMenu
        static void ApplyRuleSetMenu()
        {
            SelectionPrompt<string> p = new();
            p.HighlightStyle = new Spectre.Console.Style(Spectre.Console.Color.DeepSkyBlue3);
            p.WrapAround = true;
            p.AddChoice("[grey58]Return[/]");
            p.AddChoice("[deeppink4]Apply Default VG Ruleset[/]");

            foreach (var item in FileConfig.CurrentConfig.WeaponConfigs)
            {
                p.AddChoice(item.ConfigName);
            }

            string selected_ruleset_name = AnsiConsole.Prompt(p);
            PlaySound();

            if (selected_ruleset_name == "[grey58]Return[/]")
            {
                RulePromptSelection();
                return;
            }
            else if (selected_ruleset_name == "[deeppink4]Apply Default VG Ruleset[/]")
            {
                SetInitialRules();
            }

            var selected_ruleset = FileConfig.CurrentConfig.WeaponConfigs.Find(x => x.ConfigName == selected_ruleset_name);

            if (selected_ruleset == null || selected_ruleset.ConfigName != selected_ruleset_name)
            {
                Log.CM($"[red]Failed to apply {selected_ruleset_name}[/]");
                Console.ReadLine();
                return;
            }

            ApplyRuleset(selected_ruleset);

            //Log.CM($"[green]Applied {selected_ruleset_name} successfully.[/]");

            Vari.CustomRules = true;
        }
        public static void SetInitialRules()
        {
            Ruleset ruleset = new()
            {
                ConfigName = "VG Ruleset",
                Pinglimit = 200,
                KickAbovePlayerCount = 50,
                RulesChat = Vari.VGRuleString,
                Weapons = new()
            {
                "U_MaximSMG", "U_MaximSMG_Wep_Accuracy",
                "ID_P_VNAME_ILYAMUROMETS", "ID_P_VNAME_ARTILLERYTRUCK",
                "_RGL_Frag", "_RGL_Smoke", "_RGL_HE",
            },
            };

            ApplyRuleset(ruleset);

            Vari.CustomRules = false;
        }
        #endregion

        #region ApplyRuleset
        public static void ApplyRuleset(Ruleset ruleset)
        {
            Vari.AutoKickBreakPlayer = false;
            Vari.AutoKickPing = false;

            Vari.PingLimit = ruleset.Pinglimit;
            Vari.OnlyKickWhenPlayerCountAbove = ruleset.KickAbovePlayerCount;
            Vari.CurrentRuleString = ruleset.RulesChat;

            foreach (var item in Vari.WeaponItemsAndBanFlags) //v4.3 Fix
            {
                item.Team1 = false;
                item.Team2 = false;
            }

            foreach (var item in ruleset.Weapons)
            {                
                var _itemInList = Vari.WeaponItemsAndBanFlags.Where(x => x.English == item);    
                
                if (_itemInList.Count() < 1) continue;
                var item3 = _itemInList.First();
                
                item3.Team1 = true;
                item3.Team2 = true;
            }

            Vari.BannedWeapons_Team1.Clear();
            Vari.BannedWeapons_Team2.Clear();

            foreach (var item in Vari.WeaponItemsAndBanFlags)
            {
                if (item.Team1)
                {
                    Vari.BannedWeapons_Team1.Add(item.English);
                }

                if (item.Team2)
                {
                    Vari.BannedWeapons_Team2.Add(item.English);
                }
            }

            Vari.CurrentlyAppliedRuleset = ruleset;
        }        
        #endregion

        #region AddRulesetPrompt
        static void AddRulesetPrompt()
        {
            string configname = string.Empty;

            while (true)
            {
                configname = AnsiConsole.Ask<string>("Name of Ruleset:");

                if (!FileConfig.CurrentConfig.WeaponConfigs.Any(x => x.ConfigName == configname))
                {
                    break;
                }

                Log.CM("Name exists already!");
            }
            Log.C("");

            int pinglimit = AnsiConsole.Ask<int>("Pinglimit (default 200):");
            Log.C("");

            int kickaboveplayercount = AnsiConsole.Ask<int>("Stop kicking when Playercount below (default 50):");
            Log.C("");

            string rulechat = AnsiConsole.Ask<string>("(Optional) Serverrules/Servertext to output in chat:");
            Log.C("");

            MultiSelectionPrompt<string> msprompt = new()
            {
                Title = "Select items to ban from server:\nPress Space to select.",
                PageSize = 20,
                WrapAround = true,       
                HighlightStyle = new Spectre.Console.Style(Spectre.Console.Color.DeepSkyBlue3),
            };

            Dictionary<string, List<string>> dict = new();

            foreach (var item in Vari.WeaponItemsAndBanFlags)
            {
                if (dict.ContainsKey(item.Class))
                {
                    dict.TryGetValue(item.Class, out var list);
                    list!.Add(item.Name);
                }
                else
                {
                    List<string> list = new()
                {
                    item.Name
                };
                    dict.Add(item.Class, list);
                }
            }

            foreach (var item in dict)
            {
                msprompt.AddChoiceGroup(item.Key, item.Value);
            }

            var selected_items = AnsiConsole.Prompt(msprompt);
            PlaySound();

            //v4.4 FIX
            List<string> selected_items_name_formatted = new();
            foreach (var item in selected_items)
            {
                var result = Vari.WeaponItemsAndBanFlags.First(x => x.Name == item);
                selected_items_name_formatted.Add(result.English);
            }

            FileConfig.CurrentConfig.WeaponConfigs.Add(new Ruleset
            {
                ConfigName = configname,
                Weapons = selected_items_name_formatted,
                Pinglimit = pinglimit,
                KickAbovePlayerCount = kickaboveplayercount,
                RulesChat = rulechat,
            });
            FileConfig.CurrentConfig.Save();

            Log.CM($"\n[green]Added {configname} successfully.[/]");
            Console.ReadLine();

            RulePromptSelection();
        }
        #endregion

        #region DeleteRulesetPrompt
        static void DeleteRulesetPrompt()
        {
            SelectionPrompt<string> p = new();
            p.HighlightStyle = new Spectre.Console.Style(Spectre.Console.Color.Red);
            p.WrapAround = true;

            p.AddChoice("[grey58]Return[/]");
            foreach (var item in FileConfig.CurrentConfig.WeaponConfigs)
            {
                p.AddChoice(item.ConfigName);
            }

            string selection_del = AnsiConsole.Prompt(p);
            PlaySound();

            if (selection_del == "[grey58]Return[/]")
            {
                RulePromptSelection();
                return;
            }

            if (DeleteRuleset(selection_del))
            {
                Log.CM($"[green]Deleted {selection_del} successfully.[/]");
            }
            else
            {
                Log.CM($"[red]Failed to delete {selection_del}[/]");
            }

            Console.ReadLine();
            RulePromptSelection();
        }
        static bool DeleteRuleset(string name)
        {
            var result = FileConfig.CurrentConfig.WeaponConfigs.Find(x => x.ConfigName == name);

            if (result == null || result.ConfigName != name)
            {
                return false;
            }

            var success = FileConfig.CurrentConfig.WeaponConfigs.Remove(result);

            if (!success)
            {
                return false;
            }

            FileConfig.CurrentConfig.Save();
            return true;
        }
        #endregion
    }
}
