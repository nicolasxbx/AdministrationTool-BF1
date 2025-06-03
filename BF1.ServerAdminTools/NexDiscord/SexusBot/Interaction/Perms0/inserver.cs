using BF1.ServerAdminTools.Features.Data;
using NexDiscord;
using NStandard;

namespace BF1.ServerAdminTools.NexDiscord;

public static partial class SexusBot
{
    public static partial class Interaction
    {
        public static async Task inserver()
        {
            string[] words = VariS.Current.words;
            if (words.Length == 2)
            {
                try
                {
                    PlayerData player = FindPlayerInServer(words[1]);
                    if (player != null)
                    {
                        //string s0 = $": {}\n";
                        string s = $"{Ansi.B.Cyan}✅ Player found!{Ansi.None}\n\n";
                        string s2 = $"Name: {Ansi.B.White}{player.Name}{Ansi.None}\n";
                        string s3 = $"PID: {player.PersonaId}\n";                            
                        string s4 = $"Rank: {player.Rank}\n";
                        string s5 = "";
                        if (player.SquadId.IsNullOrEmpty() == false)
                        {
                            s5 = $"Clantag: {player.Clan}\n";
                        }
                        string s6 = $"Squad: {player.SquadId}\n\n";

                        string s7 = $"Kills: {player.Kills}\n";
                        string s8 = $"Deaths: {player.Deaths}\n";
                        string s9 = $"Score: {player.Score}\n";
                        string s10 = $"KPM: {player.KPM}\n\n";

                        string s11 = $"Guns: {player.WeaponS0}, {player.WeaponS1}\n";
                        string s12 = $"Gadgets: {player.WeaponS2}, {player.WeaponS5}, {player.WeaponS6}\n\n";

                        string s99 = $"{DateTime.Now}";
                        await OutAnsi(s + s2 + s3 + s4 + s5 + s6 + s7 + s8 + s9 + s10 + s11 + s12 + s99);
                    }
                    else
                    {
                        await OutAnsi($"{Ansi.Bold}❌ Player was not found playing on the Server!");
                    }
                }
                catch (Exception ex)
                {
                    Log.Ex(ex);
                    await OutAnsi($"{Ansi.B.Red}❌ Error while trying to fetch player. Info:\n{ex.Message}");
                }
            }
            else
            {
                await Out("Syntax: " + ".inserver <playername>");
            }
        }

        public static PlayerData FindPlayerInServer(string name)
        {
            try
            {
                foreach (PlayerData p in Vari.Playerlist_All)
                {
                    if (p.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase) == true)
                    {
                        return p;
                    }
                }
                return null;
            }
            catch (Exception ex)
            {
                Log.Ex(ex);
                return null;
            }
        }
    }
}

