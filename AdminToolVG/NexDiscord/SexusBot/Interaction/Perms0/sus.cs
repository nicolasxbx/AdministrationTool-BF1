using BF1.ServerAdminTools.Common.Utils;
using BF1.ServerAdminTools.Features.API;
using BF1.ServerAdminTools.Features.API.RespJson;
using BF1.ServerAdminTools.Features.Data;
using BF1.ServerAdminTools.Features.Utils;
using BF1.ServerAdminTools.Models;
using NexDiscord;

namespace BF1.ServerAdminTools.NexDiscord;

public static partial class SexusBot
{
    public static partial class Interaction
    {
        public static async Task sus()
        {
            string[] words = VariS.Current.words;
            if (words.Length >= 2)
            {
                string name = words[1];
                try
                {
                    PlayerData p = FindPlayerInServer(name);
                    if (p == null)
                    {
                        await OutAnsi($"Player was {Ansi.Red}not{Ansi.None} found on the server.");
                    }
                    else
                    {
                        List<WeaponStats> result = await Util_BF1.AdminActions.CheckSus(p.PersonaId);

                        if (result == null)
                        {
                            await OutAnsi($"{Ansi.Bold}No{Ansi.None} suspicious weapons found for {Ansi.B.Blue}{name}{Ansi.None}.");
                        }
                        else
                        {
                            try
                            {
                                var header = new StringBuilder();
                                var sb = new StringBuilder();
                                header.Append(String.Format("{0,-26} {1,6} {2,5} {3,6} {4,5} {5,5} {6,10}\n\n", "Weapon", "Kills", "KPM", "Acc", "HS", "H/K", "Time"));
                                foreach (WeaponStats ws in result)
                                {
                                    sb.Append(String.Format("{0,-26} {1,6} {2,5} {3,6} {4,5} {5,5} {6,10}\n", ws.name, ws.kills, ws.killsPerMinute, ws.hitsVShots, ws.headshotsVKills, ws.hitVKills, ws.time));
                                }
                                await OutAnsi($"{Ansi.Bold}Suspicious{Ansi.None} weapons for {Ansi.B.Blue}{name}{Ansi.None}:\n\n{header}{sb}");
                            }
                            catch (Exception ex) { Log.Ex(ex); }
                        }
                    }
                }
                catch (Exception ex) { Log.Ex(ex); }
            }
            else
            {
                await OutAnsi($"Syntax: {Ansi.B.Magenta}.sus{Ansi.None} <name>");
            }
        }
    }
}

