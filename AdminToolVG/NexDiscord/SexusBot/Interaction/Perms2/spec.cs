using BF1.ServerAdminTools.Features.Data;
using NexDiscord;
using NStandard;

namespace BF1.ServerAdminTools.NexDiscord;

public static partial class SexusBot
{
    public static partial class Interaction
    {
        public static async Task spec()
        {
            string[] words = VariS.Current.words;
            if (Vari.Server_SpectatorList.IsNull() == false && Vari.Server_SpectatorList.Any() == true)
            {
                string s = $"{Ansi.Bold}Spectators:{Ansi.None}\n";
                int index = 1;
                foreach (SpectatorInfo sp in Vari.Server_SpectatorList)
                {
                    s = s + $"{index}. {Ansi.B.Blue}{sp.Name}{Ansi.None}. (PID: {sp.PersonaId})\n";
                    index++;
                }
                await OutAnsi(s + "\nNote: May also show non-spectating players.");
            }
            else
            {
                await OutAnsi($"{Ansi.B.Red} Serverlist is empty.");
            }
        }
    }
}

