using BF1.ServerAdminTools.Features.Data;
using BF1.ServerAdminTools.Models;
using NexDiscord;

namespace BF1.ServerAdminTools.NexDiscord;

public static partial class SexusBot
{
    public static partial class Live
    {
        public static async void Thread_AutoSusCheck() //threaded
        {
            while (true)
            {
                if (Vari.SexusBot.LiveFunctionsEnabled == false)
                {
                    Thread.Sleep(300000);
                    return;
                }

                if (Vari.AutoRun && !Vari.RuleWindow_Ready)
                {
                    Thread.Sleep(20000); //20 Sec
                    return;
                }

                await Server_Sus_check();

                Thread.Sleep(VariS.LiveDelay_Sus * 1000);
            }
        }

        public static async Task Server_Sus_check()
        {
            Log.I("Starting Server_Sus_Check...");

            string s = $"{Ansi.B.Magenta}Sus Player(s) detected{Ansi.None}:\n\n";

            var header = new StringBuilder();
            var sb_body = new StringBuilder();
            List<PlayerData> playerlist = Vari.Playerlist_All;

            header.Append(String.Format("{0,-16} {1,26} {2,6} {3,5} {4,6} {5,5} {6,5} {7,10}\n\n",
                "Player", "Weapon", "Kills", "KPM", "Acc", "HS", "H/K", "Time"));
            
            try
            {
                foreach (PlayerData p in playerlist.ToList())
                {                    
                    List<WeaponStats> list_ws = await Interaction.CheckSus(p.PersonaId);
                    if (list_ws == null)
                    {
                        continue;
                    }

                    foreach (WeaponStats ws in list_ws)
                    {
                        sb_body.Append(String.Format(Ansi.B.Red + "{0,-16}"+Ansi.None+" {1,26} {2,6} {3,5} {4,6} {5,5} {6,5} {7,10}\n",
                            p.Name, ws.name, ws.kills, ws.killsPerMinute, ws.hitsVShots, ws.headshotsVKills, ws.hitVKills, ws.time));
                    }
                }    
            }
            catch (Exception ex) { Log.Ex(ex); }


            if (sb_body.Length < 1 || sb_body == null)
            {
                Log.I("Server_sus_Check done, no sus found.");
                return;
            }

            await OutCustomAnsi(Ansi.None + s + header + sb_body.ToString(), VariS.channel_admin_discussion);
        }
    }
}

