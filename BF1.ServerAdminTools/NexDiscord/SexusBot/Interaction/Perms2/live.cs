using NexDiscord;

namespace BF1.ServerAdminTools.NexDiscord;

public static partial class SexusBot
{
    public static partial class Interaction
    {
        public static async Task live()
        {
            string[] words = VariS.Current.words;
            if (words.Length == 1)
            {
                if (Vari.SexusBot.LiveFunctionsEnabled == false && Vari.SexusBot.IsRunning == true)
                {
                    Vari.SexusBot.LiveFunctionsEnabled = true;
                    await OutAnsi($"{Ansi.Cyan}✅ Turned the live-checks (Scoreboard, Balance, Chat) ON");
                }
                else if (Vari.SexusBot.LiveFunctionsEnabled == true && Vari.SexusBot.IsRunning == true)
                {
                    Vari.SexusBot.LiveFunctionsEnabled = false;
                    await OutAnsi($"{Ansi.Cyan}✅ Turned the live-checks (Scoreboard, Balance, Chat) OFF");
                }
            }
            else if (words.Length >= 2)
            {
                if (words[1] == "autosus" || words[1] == "sus")
                {
                    if (Vari.SexusBot.LiveSusEnabled == false && Vari.SexusBot.IsRunning == true)
                    {
                        Vari.SexusBot.LiveSusEnabled = true;
                        await OutAnsi($"{Ansi.Cyan}✅ Turned automatic sus-checking ON");
                    }
                    else if (Vari.SexusBot.LiveSusEnabled == true && Vari.SexusBot.IsRunning == true)
                    {
                        Vari.SexusBot.LiveSusEnabled = false;
                        await OutAnsi($"{Ansi.Cyan}✅ Turned automatic sus-checking OFF");
                    }
                }
            }
        }
    }
}

