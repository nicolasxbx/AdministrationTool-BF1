using BF1.ServerAdminTools.Common.Utils;
using BF1.ServerAdminTools.Features.API;
using BF1.ServerAdminTools.Features.API.RespJson;
using NexDiscord;

namespace BF1.ServerAdminTools.NexDiscord;

public static partial class SexusBot
{
    public static partial class Interaction
    {
        public static async Task features()
        {
            string[] words = VariS.Current.words;
            bool finished = false;
            if (words.Length == 2)
            {
                if (words[1] == "on")
                {
                    await NexVerifySessionIDStartup2();

                    Vari.AutoKickBreakPlayer = Vari.AutoKickPing = Vari.AutoKickWinSwitching = Vari.AutoCheckBFBAN = Vari.AutoKickBFBAN = true;
                    await OutAnsi($"✅ Turned all Features {Ansi.Cyan}ON");
                    finished = true;
                }
                if (words[1] == "off")
                {
                    Vari.AutoKickBreakPlayer = Vari.AutoKickPing = Vari.AutoKickWinSwitching = Vari.AutoCheckBFBAN = Vari.AutoKickBFBAN = false;
                    await OutAnsi($"✅ Turned all Features {Ansi.Red}OFF");
                    finished = true;
                }
            }
            if (words.Length == 3)
            {
                bool b = false;
                bool is_on_or_off = false;
                if (words[1] == "on")
                {
                    b = true;
                    is_on_or_off = true;
                }
                if (words[1] == "off")
                {
                    b = false;
                    is_on_or_off = true;
                }
                switch (words[2])
                {
                    case "1":
                        Vari.AutoKickBreakPlayer = b;
                        await OutAnsi($"{Ansi.Cyan}✅ AutoKickItems set to: {Vari.AutoKickBreakPlayer}\n");
                        finished = true;
                        break;
                    case "2":
                        Vari.AutoKickPing = b;
                        await OutAnsi($"{Ansi.Cyan}✅ AutoKickPing set to: {Vari.AutoKickPing}\n");
                        finished = true;
                        break;
                    case "3":
                        Vari.AutoKickWinSwitching = b;
                        await OutAnsi($"{Ansi.Cyan}✅ AutoKickWinSwitching set to: {Vari.AutoKickWinSwitching}\n");
                        finished = true;
                        break;
                    case "4":
                        Vari.AutoCheckBFBAN = b;
                        await OutAnsi($"{Ansi.Cyan}✅ AutoCheckBFBAN set to: {Vari.AutoCheckBFBAN}\n");
                        finished = true;
                        break;
                    case "5":
                        Vari.AutoKickBFBAN = b;
                        await OutAnsi($"{Ansi.Cyan}✅ AutoKickBFBAN set to: {Vari.AutoKickBFBAN}\n");
                        finished = true;
                        break;
                }
                if (is_on_or_off == false)
                {
                    int i = 200;
                    try
                    {
                        i = Int32.Parse(words[1]);
                        if (words[2] == "6" && i >= 100 && i <= 500)
                        {
                            Vari.PingLimit = i;
                            await OutAnsi($"{Ansi.Cyan} ✅ PingLimit set to {Vari.PingLimit}\n");
                            finished = true;
                        }
                        if (words[2] == "7" && i >= 100 && i <= 500)
                        {
                            Vari.PingLimit = i;
                            await OutAnsi($"{Ansi.Cyan} ✅ TicketLimit set to {Vari.TicketLimit}\n");
                            finished = true;
                        }
                    }
                    catch
                    {
                        finished = false;
                    }
                }
            }

            if (finished == false)
            {
                await OutAnsi($"{Ansi.Bold}⚠Syntax: " + ".features <on/off> [feature-number 1-5]\nor .features <value 100-500> <feature-number 6-7>");
            }
        }

        private static async Task NexVerifySessionIDStartup2() //tna
        {
            if (!string.IsNullOrEmpty(Vari.SessionID))
            {
                await BF1API.SetAPILocale();
                var result = await BF1API.GetWelcomeMessage();
                if (result.IsSuccess)
                {
                    var welcomeMsg = JsonUtil.JsonDese<WelcomeMsg>(result.Message);
                    var msg = welcomeMsg.result.firstMessage;
                    Vari.WelcomeMessage = msg;
                }
            }
        }        
    }
}

