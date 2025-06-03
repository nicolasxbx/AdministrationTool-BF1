using BF1.ServerAdminTools.Features.API;
using BF1.ServerAdminTools.Features.Data;
using NexDiscord;

namespace BF1.ServerAdminTools.NexDiscord;

public static partial class SexusBot
{
    public static partial class Interaction
    {
        public static async Task kick()
        {
            string[] words = VariS.Current.words;
            string msg = VariS.Current.msg;
            if (words.Length >= 3)
            {
                string name = words[1];
                string reason = msg.Substring(msg.IndexOf(name) + name.Length + 1); //Finds the reason
                if (reason.Length <= 35)
                {
                    await Kick1_CheckServerList(name, reason);
                }
                else
                {
                    await OutAnsi($"{Ansi.Red}⚠️ Please keep the reason below 36 Characters.");
                }
            }
            else
            {
                if (words.Length == 2)
                {
                    await OutAnsi($"{Ansi.Red}⚠️ Please specify a reason");
                }
                if (words.Length == 1)
                {
                    await OutAnsi($"{Ansi.Red}⚠️ Please specify a name and reason");
                }
            }            
        }
        private static async Task Kick1_CheckServerList(string name, string reason)
        {            
            try
            {
                PlayerData playerdata = FindPlayerInServer(name);
                if (playerdata != null)
                {
                    BreakRuleInfo info = new BreakRuleInfo
                    {
                        Name = playerdata.Name,
                        PersonaId = playerdata.PersonaId,
                        Reason = reason,
                        Status = ""
                    };
                    await Kick2_Check_Admin_And_Kick(info);
                }
                else
                {
                    await OutAnsi($"{Ansi.Red}❌ Player was not found playing on the Server!");
                }                
            }
            catch (Exception ex)
            {
                Log.Ex(ex);
                await OutAnsi($"{Ansi.Red}❌ Error while trying to fetch player. Info:\n{ex.Message}");
            }
        }
        private static async Task Kick2_Check_Admin_And_Kick(BreakRuleInfo info)
        {
            if (IsNotAdminOrWhitelisted(info.Name, info.PersonaId) == true)
            {
                try
                {
                    RespContent result = await BF1API.AdminKickPlayer(info.PersonaId, info.Reason); // TESTING PURPOSES
                    //RespContent result = new();
                    result.IsSuccess = true;

                    if (result.IsSuccess)
                    {
                        info.Status = "kicked out";
                        await OutAnsi($"{Ansi.B.Green}✅ Kicked\nPlayer: {Ansi.B.Blue}{info.Name}{Ansi.None}\nReason: {info.Reason}\nPID: {info.PersonaId}");
                        await DWebHooks.LogSexusBot_Kick(info);
                    }
                    else
                    {
                        info.Status = "Kick failed, " + result.Message;
                        await OutAnsi($"{Ansi.Red}❌ Kick failed! EA Message:\n {result.Message}");
                    }
                }
                catch (Exception ex)
                {
                    Log.Ex(ex);
                    await OutAnsi($"{Ansi.Red}❌ Error while trying to kick Player. Info:\n{ex.Message}");
                }
            }
            else
            {
                await OutAnsi($"{Ansi.Red}❌ {info.Name} is Admin or Whitelisted");
            }
        }
    }
}

