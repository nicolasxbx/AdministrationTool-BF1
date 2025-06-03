using BF1.ServerAdminTools.Features.Data;
using BF1.ServerAdminTools.Features.Utils;
using BF1.ServerAdminTools.Models;

namespace BF1.ServerAdminTools.NexDiscord;

public static partial class DWebHooks
{    
    public static async Task LogOK(BreakRuleInfo info)
    {
        string begin = $"✅KICK\n";
        
        string name = $"    Name: {info.Name}\n";
        string kick_reason = $"    Kick Reason: {TBold(info.Reason)}\n";
        string pid = $"    PID: {info.PersonaId}\n";        
        
        string fullmessage = begin + name + kick_reason + pid;
        fullmessage = TBlock(fullmessage);
        await Send_2_Webhook(fullmessage, 1);
        Vari.Webhooks.NexPlayersKicked++;
    }
    public static async Task LogNO(BreakRuleInfo info)
    {
        string begin = $"❌FAIL\n";
        
        string name = $"    Name: {info.Name}\n";
        string kick_reason = $"    Kick Reason: {TBold(info.Reason)}\n";
        string pid = $"    PID: {info.PersonaId}\n";        
        
        string fullmessage = begin + name + kick_reason + pid;
        fullmessage = TBlock(fullmessage);
        await Send_2_Webhook(fullmessage, 1);
    }
    public static async Task LogPingKick(BreakRuleInfo info)
    {        
        string begin = $"✅KICK\n";

        string name = $"    Name: {info.Name}\n";
        string kick_reason = $"    Kick Reason: {TBold(info.Reason)}\n";
        string pid = $"    PID: {info.PersonaId}\n";
        
        string fullmessage = begin + name + kick_reason + pid;
        fullmessage = TBlock(fullmessage);
        await Send_2_Webhook(fullmessage, 2);
        Vari.Webhooks.NexPlayersKicked++;
    }  
    public static async Task LogWinSwitchDetected(ChangeTeamInfo info)
    {        
        string begin = $"⚠️WS UNKICKED\n";
        
        string name = $"    Name: {info.Name}\n";
        string status = $"    Info: {TBold(info.Status)}\n";
        string scores = $"    Scores: {info.Team1Score} - {info.Team2Score}\n";
        string map = $"    Map: {Vari.CurrentMapName}\n";
        string pid = $"    PID: {info.PersonaId}\n";
        
        string fullmessage = begin + name + status + scores + map + pid;
        fullmessage = TBlock(fullmessage);
        await Send_2_Webhook(fullmessage, 3);
    }
    public static async Task LogWinSwitchKick(ChangeTeamInfo info)
    {        
        string begin = $"✅WS-KICK\n";
        
        string name = $"    Name: {info.Name}\n";
        string kick_reason = $"    Kick Reason: {TBold($"Winswitch {info.Team1Score}-{info.Team2Score}")}\n";
        string status = $"    Info: {info.Status}\n";
        string scores = $"    Scores: {info.Team1Score} - {info.Team2Score}\n";
        string map = $"    Map: {Vari.CurrentMapName}\n";
        string pid = $"    PID: {info.PersonaId}\n";

        string fullmessage = begin + name + status + kick_reason + scores + map + pid;
        fullmessage = TBlock(fullmessage);
        await Send_2_Webhook(fullmessage, 3);
        Vari.Webhooks.NexPlayersKicked++;
    }
    public static async Task LogBFBANDetected(PersonaIDNex pin)
    {        
        string begin = $"⚠️BFBAN-UNKICKED\n";
        
        string name = $"    Name: {pin.originId}\n";
        string url = $"    URL: {TBold(pin.url)}\n";        
        string pid = $"    PID: {pin.originPersonaId}\n";
        
        string fullmessage = begin + name + url + pid;
        fullmessage = TBlock(fullmessage);
        await Send_2_Webhook(fullmessage, 3);
    }
    public static async Task LogBFBANKick(BreakRuleInfo info, PersonaIDNex pin)
    {
        string begin = $"✅BFBAN-KICK\n";
        
        string name = $"    Name: {info.Name}\n";       
        string kick_reason = $"    Kick Reason: {TBold(pin.url)}\n";
        string url = $"    URL: {pin.url}\n";       
        string pid = $"    PID: {info.PersonaId}\n";
        
        string fullmessage = begin + name + kick_reason + url + pid;
        fullmessage = TBlock(fullmessage);
        await Send_2_Webhook(fullmessage, 3);
        Vari.Webhooks.NexPlayersKicked++;
    }
    public static async Task LogBalancer(ChangeTeamInfo info)
    {
        string fullmessage;

        string name = $"    Name: {info.Name}\n";
        //string status = $"    Info: {TBold(info.Status)}\n";
        string scores = $"    Scores: {info.Team1Score} - {info.Team2Score}\n";
        string map = $"    Map: {Vari.CurrentMapName}\n";
        string pid = $"    PID: {info.PersonaId}\n";

        string shortinfo = $" {info.Name} ({info.Team1Score} - {info.Team2Score}) [{Vari.CurrentMapName}]\n";

        if (Vari.ServerDetails_AdminList_PID.Contains(info.PersonaId) == true)
        {

            fullmessage = "(Admin)" + shortinfo;
        }
        else
        {
            if (PlayerUtil.CheckAdminVIP(info.PersonaId, Vari.ServerDetails_VIPList) == "✔")
            {
                fullmessage = "(VIP)" + shortinfo;
            }
            else
            {
                string begin = $"✅Balancer !!\n";
                fullmessage = begin + name + scores + map + pid;
            }
        }

        fullmessage = TBlock(fullmessage);
        await Send_2_Webhook(fullmessage, 4);
    }    
}