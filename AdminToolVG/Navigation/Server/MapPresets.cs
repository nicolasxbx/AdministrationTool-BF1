using LinkDotNet.NUniqueHardwareID;

namespace AdminToolVG;

public static partial class Navigation
{
    public static class MapPresets
    {
        #region Menu
        public static async Task Menu()
        {
            Log.C("Loading...");

            var settings_current = await Util_BF1.AdminActions.Preset_GetSettings();

            if (settings_current == null) //Try again
            {
                settings_current = await Util_BF1.AdminActions.Preset_GetSettings();
            }

            Console.Clear();
            if (settings_current != null)
            {
                if (settings_current.result.mapRotations[0].maps.Count > 0)
                {
                    string maps = string.Empty;
                    foreach (var map in settings_current.result.mapRotations[0].maps)
                    {
                        if (maps != string.Empty)
                        {
                            maps += ", ";
                        }
                        maps += MapData.MapDictNex[map.mapName];
                    }
                    Log.C($"Current Mappool: {maps}");
                }
            }

            //SELECTION
            SelectionPrompt<string> prompt_preset = new();

            prompt_preset.AddChoice("[grey58]Return[/]");

            prompt_preset.AddChoice("[deeppink1_1]Force Switch current map[/]");

            if (FileConfig.CurrentConfig.MapPresets.Count > 0)
            {
                prompt_preset.AddChoice("[red]Apply Preset[/]");
            }

            prompt_preset.AddChoice("[dodgerblue2]Add Preset[/]");

            if (FileConfig.CurrentConfig.MapPresets.Count > 0)
            {
                prompt_preset.AddChoice("[darkred_1]Delete Preset[/]");
            }

            prompt_preset.AddChoice("Save current active Preset");

            prompt_preset.HighlightStyle = new Spectre.Console.Style(Spectre.Console.Color.LightSeaGreen);
            string selected_option = AnsiConsole.Prompt(prompt_preset);
            PlaySound();

            if (selected_option == "[red]Apply Preset[/]")
            {
                await Apply();
            }
            else if (selected_option == "[dodgerblue2]Add Preset[/]")
            {
                await Add();
            }
            else if (selected_option == "[darkred_1]Delete Preset[/]")
            {
                await Delete();
            }
            else if (selected_option == "Save current active Preset")
            {
                await AddDefault();
            }
            else if (selected_option == "[deeppink1_1]Force Switch current map[/]")
            {
                await SwitchMap();
            }
        }
        #endregion

        #region Switch Current Map
        static async Task SwitchMap()
        {
            Log.C("");

            // GET SETTINGS
            GetServerDetailsBody? settings_current;
            while (true)
            {
                settings_current = await Util_BF1.AdminActions.Preset_GetSettings();

                if (settings_current != null) break;

                Log.C("EA Error.\n");
                SelectionPrompt<string> prompt_retry = new();
                prompt_retry.HighlightStyle = new Spectre.Console.Style(Spectre.Console.Color.LightSeaGreen);
                prompt_retry.AddChoice("Return");
                prompt_retry.AddChoice("Retry");

                var selection_retry = AnsiConsole.Prompt(prompt_retry);
                PlaySound();
                if (selection_retry == "Return") return;
            }

            //Add Prompt Options
            ClearConsole();
            SelectionPrompt<string> msprompt = new()
            {
                Title = "Select Maps:\nPress Space to select.",
                PageSize = 20,
            };
            msprompt.WrapAround = true;
            msprompt.AddChoice("return");

            //Index Prompt selections
            Dictionary<string, int> Index_Dict = new();
            int index = 0;
            foreach (var item in settings_current.result.mapRotations[0].maps)
            {
                string fullmapname = MapData.MapDictNex[item.mapName];
                Index_Dict[fullmapname] = index;
                msprompt.AddChoice(fullmapname);
                index++;
            }

            var selected_choice = AnsiConsole.Prompt(msprompt);
            PlaySound();
            if (selected_choice == "return")
            {
                return;
            }

            //Change Map

            Log.D(selected_choice);
            int levelindex = Index_Dict[selected_choice];
            Log.D(levelindex.ToString());

            string persistedgameid = settings_current.result.server.persistedGameId;
            Log.D(persistedgameid);

            var result = await BF1API.ChangeServerMap(persistedgameid, levelindex);
            Log.D(result.Message);
        }
        #endregion

        #region Apply Preset
        static async Task Apply()
        {
            // GET SETTINGS
            GetServerDetailsBody? settings_current;
            while (true)
            {
                settings_current = await Util_BF1.AdminActions.Preset_GetSettings();

                if (settings_current != null) break;

                Log.C("EA Error.\n");
                SelectionPrompt<string> prompt_retry = new();
                prompt_retry.HighlightStyle = new Spectre.Console.Style(Spectre.Console.Color.LightSeaGreen);
                prompt_retry.AddChoice("Return");
                prompt_retry.AddChoice("Retry");

                var selection_retry = AnsiConsole.Prompt(prompt_retry);
                PlaySound();
                if (selection_retry == "Return") return;
            }

            ClearConsole();

            //SELECTION
            SelectionPrompt<string> prompt_preset = new()
            {
                Title = "Choose",
            };
            prompt_preset.AddChoice("[grey58]Return[/]");
            foreach (var item in FileConfig.CurrentConfig.MapPresets)
            {
                prompt_preset.AddChoice(item.name);
            }
            prompt_preset.HighlightStyle = new Spectre.Console.Style(Spectre.Console.Color.LightSeaGreen);
            string selected_option = AnsiConsole.Prompt(prompt_preset);
            PlaySound();

            if (selected_option == "[grey58]Return[/]")
            {
                return;
            }

            MapPreset? selection = FileConfig.CurrentConfig.MapPresets.Find(x => x.name == selected_option);

            if (selection == null)
            {
                Log.C("Error.");
                Console.ReadLine();
                return;
            }

            foreach (var item in selection.maps)
            {
                Log.D(item.mapName + " " + item.gameMode);
            }

            var result = await Util_BF1.AdminActions.Preset_UpdateMapRotation(settings_current!, selection.maps);

            //result will always say not success, but is success            
            Log.C("Success.");
            Console.ReadLine();

            await MapPresets.Menu();
        }
        #endregion

        #region Add Preset
        static async Task Add()
        {
            string name = AnsiConsole.Ask<string>("Name of Preset:");
            Log.C("");

            MultiSelectionPrompt<string> msprompt = new()
            {
                Title = "Select Maps:\nPress Space to select.",
                PageSize = 20,
                WrapAround = true,
                HighlightStyle = new Spectre.Console.Style(Spectre.Console.Color.LightSeaGreen)
            };

            foreach (var item in MapData.MapDictNex)
            {
                msprompt.AddChoice(item.Value);
            }

            var selected_items = AnsiConsole.Prompt(msprompt);
            PlaySound();
            List<MapsItem> maps = new();
            foreach (var item in selected_items)
            {
                maps.Add(new MapsItem
                {
                    mapName = MapData.MapDictNex.FirstOrDefault(x => x.Value == item).Key,
                    gameMode = "CQ0",
                });
            }

            //Save Item
            FileConfig.CurrentConfig.MapPresets.Add(new MapPreset
            {
                name = name,
                maps = maps,
            });
            FileConfig.CurrentConfig.Save();

            Log.CM($"\n[green]Added {name} successfully.[/]");
            Console.ReadLine();

            await MapPresets.Menu();
        }
        #endregion

        #region Delete Preset
        static async Task Delete()
        {
            //SELECTION
            SelectionPrompt<string> prompt_preset = new();
            prompt_preset.AddChoice("[grey58]Return[/]");
            foreach (var item in FileConfig.CurrentConfig.MapPresets)
            {
                prompt_preset.AddChoice(item.name);
            }
            prompt_preset.HighlightStyle = new Spectre.Console.Style(Spectre.Console.Color.Red);
            string selected_option = AnsiConsole.Prompt(prompt_preset);
            PlaySound();

            if (selected_option == "[grey58]Return[/]")
            {
                return;
            }

            MapPreset? selection = FileConfig.CurrentConfig.MapPresets.Find(x => x.name == selected_option);

            if (selection == null)
            {
                Log.C("Error.");
                Console.ReadLine();
                return;
            }

            FileConfig.CurrentConfig.MapPresets.Remove(selection);
            FileConfig.CurrentConfig.Save();

            Log.CM($"[green]Deleted {selected_option} successfully.[/]");
            Console.ReadLine();

            await MapPresets.Menu();
        }
        #endregion

        #region Add Default
        public static async Task AddDefault()
        {
            string default_name = "Saved Default";

            Log.C("");

            // GET SETTINGS
            GetServerDetailsBody? settings_current;
            while (true)
            {
                settings_current = await Util_BF1.AdminActions.Preset_GetSettings();

                if (settings_current != null) break;

                Log.C("EA Error.\n");
                SelectionPrompt<string> prompt_retry = new();
                prompt_retry.HighlightStyle = new Spectre.Console.Style(Spectre.Console.Color.LightSeaGreen);
                prompt_retry.AddChoice("Return");
                prompt_retry.AddChoice("Retry");

                var selection_retry = AnsiConsole.Prompt(prompt_retry);
                PlaySound();
                if (selection_retry == "Return") return;
            }

            MapPreset? a = FileConfig.CurrentConfig.MapPresets.Find(x => x.name == default_name);

            if (a == null) //Doesn't exist yet
            {
                FileConfig.CurrentConfig.MapPresets.Add(new MapPreset
                {
                    name = default_name,
                    maps = settings_current.result.mapRotations[0].maps,
                });
            }
            else //Already exists
            {
                a.maps = settings_current.result.mapRotations[0].maps;
            }

            FileConfig.CurrentConfig.Save();

            await MapPresets.Menu();
        }
        #endregion

        #region Print
        static void PrintSettings(GetServerDetailsBody body)
        {
            Log.D($"settings.name: {body.result.serverSettings.name}");
            Log.D($"settings.description: {body.result.serverSettings.description}");
            Log.D($"settings.message: {body.result.serverSettings.message}");
            Log.D($"settings.password: {body.result.serverSettings.password}");
            Log.D($"settings.mapRotationId:  {body.result.serverSettings.mapRotationId}");
            Log.D($"settings.bannerUrl: {body.result.serverSettings.bannerUrl}");
            Log.D($"settings.customGameSettings: {body.result.serverSettings.customGameSettings}");
            Log.D($"---");
            Log.D($"maprotation.mapRotationId: {body.result.mapRotations[0].mapRotationId}");
            Log.D($"maprotation.name: {body.result.mapRotations[0].name}");
            Log.D($"maprotation.description: {body.result.mapRotations[0].description}");
            Log.D($"maprotation.mod: {body.result.mapRotations[0].mod}");
            Log.D($"maprotation.rotationType: {body.result.mapRotations[0].rotationType}");

            foreach (var map in body.result.mapRotations[0].maps)
            {
                Log.D($"Map: {map.mapName}, Mode: {map.gameMode}");
            }
        }
        #endregion
    }
}