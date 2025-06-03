using BF1.ServerAdminTools.Features.Data;

namespace BF1.ServerAdminTools.NexDiscord;

public static partial class DWebHooks
{       
    public static async Task LogSexusBot_Kick(BreakRuleInfo info)
    {
        string begin = $"✅KICK (manual)\n";

        string name = $"    Name: {info.Name}\n";
        string kick_reason = $"    Kick Reason: {TBold(info.Reason)}\n";
        string pid = $"    PID: {info.PersonaId}\n";
        string executed_by = $"    Executed by: {VariS.Current.user_username}\n";

        string fullmessage = begin + name + kick_reason + pid + executed_by;
        fullmessage = TBlock(fullmessage);
        await Send_2_Webhook(fullmessage, 1);
        Vari.Webhooks.NexPlayersKicked++;
    }
}