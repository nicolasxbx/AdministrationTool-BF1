namespace BF1.ServerAdminTools.NexDiscord;

public static partial class SexusBot
{
    public static partial class Live
    {
        public static void Start_Live_Checks()
        {
            if (VariS.FirstThreadStart == false)
            {
                Log.I("Cancelled Thread Starts because not first instance");
                return;
            }

            var thread_sb = new Thread(Live.Thread_Scoreboard_Channel)
            {
                IsBackground = true,
                Name = "Scoreboard-Refresh",
            };
            var thread_cb = new Thread(Live.Thread_Balance)
            {
                IsBackground = true,
                Name = "CheckBalance",
            };
            var thread_sus = new Thread(Live.Thread_AutoSusCheck)
            {
                IsBackground = true,
                Name = "Auto-Server-Sus-Check",
            };
            var thread_chat = new Thread(Live.Thread_ChatRules)
            {
                IsBackground = true,
                Name = "Auto-Chat-Rules",
            };

            thread_sb.Start();
            thread_cb.Start();
            thread_sus.Start();
            thread_chat.Start();

            VariS.FirstThreadStart = false;
        }
    }
}

