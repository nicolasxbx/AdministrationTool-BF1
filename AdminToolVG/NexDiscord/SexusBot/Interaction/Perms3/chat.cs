using BF1.ServerAdminTools.Common.Utils;
using BF1.ServerAdminTools.Features.Chat;
using BF1.ServerAdminTools.Features.Core;
using BF1.ServerAdminTools.Features.Utils;
using NexDiscord;

namespace BF1.ServerAdminTools.NexDiscord;

public static partial class SexusBot
{
    public static partial class Interaction
    {
        public static async Task chat()
        {
            string[] words = VariS.Current.words;
            string msg0 = VariS.Current.msg;
            if (words.Length >= 2)
            {
                string chatmsg = "";
                if (words[1] == "rules")
                {
                    chatmsg = "[VG] Vanguard are recruiting - join us at discord.gg/VGBF1 - RULES: NO SMG08, ARTYTRUCK AND HEAVY BOMBER, MAX 200 PING.";
                }
                else
                {
                    chatmsg = msg0.Substring(6);
                }

                if (VariS.chat_function_last_msg == chatmsg) //SPAM
                {
                    VariS.chat_function_last_msg = chatmsg;
                    return;
                }
                else //NOT SPAM
                {
                    VariS.chat_function_last_msg = chatmsg;
                }

                bool b = Util_BF1.AdminActions.ChatFunction(chatmsg);
                if (b == true)
                {
                    await OutAnsi($"{Ansi.Cyan}✅ Message successfully sent to the ingame chat!");
                }
                else
                {
                    await OutAnsi($"{Ansi.Red}❌ An error occured. Check if the host's game is in borderless mode.");
                }
            }
        }
    }
}

