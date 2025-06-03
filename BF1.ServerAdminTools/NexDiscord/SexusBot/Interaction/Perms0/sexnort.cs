using NexDiscord;

namespace BF1.ServerAdminTools.NexDiscord;

public static partial class SexusBot
{
    public static partial class Interaction
    {
        public static async Task sexnort()
        {            
            int i1 = VariS.rnd.Next(1, 30);
            if (i1 == 25)
            {
                await OutAnsi(Ansi.B.Magenta + "💦 💦 💦         Congratulations!         💦 💦 💦");
                await OutAnsi(Ansi.B.Magenta + "💦 💦 💦                                  💦 💦 💦");
                await OutAnsi(Ansi.B.Magenta + "💦 💦 💦                                  💦 💦 💦");
                await OutAnsi(Ansi.B.Magenta + "💦 💦 💦 YOU ARE ELIGIBLE FOR THE SEXNORT 💦 💦 💦");
            }
            else
            {
                await OutAnsi($"{Ansi.White}😞 Currently, You are {Ansi.B.Red}not{Ansi.White} worthy of the sexnort. Try again later.");
            }
        }        
    }
}

