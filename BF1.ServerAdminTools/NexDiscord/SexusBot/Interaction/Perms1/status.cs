using NexDiscord;

namespace BF1.ServerAdminTools.NexDiscord;

public static partial class SexusBot
{
    public static partial class Interaction
    {
        public static async Task status()
        {
            string s = $"Status:\n";
            string s1 = $"1. AutoKickItems: {Ansi.B.White}{Vari.AutoKickBreakPlayer}{Ansi.None}\n";
            string s2 = $"2. AutoKickPing: {Ansi.B.White}{Vari.AutoKickPing}{Ansi.None}\n";
            string s3 = $"3. AutoKickWinSwitching: {Ansi.B.White}{Vari.AutoKickWinSwitching}{Ansi.None}\n";
            string s4 = $"4. AutoCheckBFBAN: {Ansi.B.White}{Vari.AutoCheckBFBAN}{Ansi.None}\n";
            string s5 = $"5. AutoKickBFBAN: {Ansi.B.White}{Vari.AutoKickBFBAN}{Ansi.None}\n";
            string s6 = $"6. Pinglimit: {Ansi.B.White}{Vari.PingLimit}{Ansi.None}\n";
            string s7 = $"7. Ticketlimit: {Ansi.B.White}{Vari.TicketLimit}{Ansi.None}\n";
            await OutAnsi(s + s1 + s2 + s3 + s4 + s5);
        }
    }
}

