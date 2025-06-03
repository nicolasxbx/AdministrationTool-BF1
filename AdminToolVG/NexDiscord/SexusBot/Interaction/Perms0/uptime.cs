using NexDiscord;

namespace BF1.ServerAdminTools.NexDiscord;

public static partial class SexusBot
{
    public static partial class Interaction
    {
        public static async Task uptime()
        {
            int uptime = Convert.ToInt32((DateTime.Now - Vari.ToolStartDateTime).TotalSeconds);
            if (uptime >= 60 && uptime < 3600)
            {
                TimeSpan t = TimeSpan.FromSeconds(uptime);
                int uptimeMinutes = (int)t.TotalMinutes;
                await OutAnsi($"Uptime: {Ansi.B.Blue}{uptimeMinutes} minute(s){Ansi.None}.");
            }
            else if (uptime >= 3600)
            {
                TimeSpan t = TimeSpan.FromSeconds(uptime);
                int uptimeHours = (int)t.TotalHours;
                await OutAnsi($"Uptime: {Ansi.B.Blue}{uptimeHours} hour(s){Ansi.None}.");
            }
            else
            {
                await OutAnsi($"Uptime: {Ansi.B.Blue}{uptime} seconds{Ansi.None}.");
            }
        }
    }
}

