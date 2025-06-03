namespace BF1.ServerAdminTools.NexDiscord;

public static partial class SexusBot
{
    public static partial class Live
    {
        public static void Thread_ChatRules() //Thread
        {
            /*
            string rules = "RULES: NO SMG08, ARTYTRUCK OR HEAVY BOMBER. MAX 200 PING. ";
            string rules2 = "ALSO: NO POLITCS, DISCRIMINATION, GRIEFING (TRAIN/VEHICLE-WASTING) AND BALLROOM ROOF CAMPING";

            string discord = "Stuck in Queue? Kicked for Admin? VIP is only 1 Euro monthly. Join us -> discord.gg/VGBF1";
            string discord2 = "VIP is 1 euro. Instant serverjoin and immunity to random slot-kicking! discord.gg/VGBF1";

            string recruit = "Join our VG-Friendship, and become [VG] member today! -> discord.gg/VGBF1";
            string recruit2 = "YOU are welcomed into [VG]. JOIN us today! -> discord.gg/VGBF1";

            */            
            while (true)
            {
                if (!Vari.SexusBotLiveFunctionsEnabled || !Vari.SexusBot.IsRunning)
                {
                    Thread.Sleep(30000);
                    return;
                }

                Thread.Sleep(10000); //Initial Delay
                /*
                Send(rules);
                Send(rules2);
                Send(discord);
                Send(recruit);

                Send(rules);
                Send(rules2);
                Send(discord2);
                Send(recruit2);
                */

                Send(Vari.CurrentRuleString);
                Thread.Sleep(900000); //15 Min
            }
        }

        private static void Send(string s)
        {
            if (Vari.SexusBotLiveFunctionsEnabled == false)
            {
                return;
            }

            Util_BF1.AdminActions.ChatFunction(s);
            Thread.Sleep(VariS.LiveDelay_Chat * 1000);
        }
    }
}

