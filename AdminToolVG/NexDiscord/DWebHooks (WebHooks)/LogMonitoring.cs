namespace BF1.ServerAdminTools.NexDiscord;

public static partial class DWebHooks
{
    public static async Task LogMonitoringON()
    {
        if (Vari.Webhooks.Monitoring_first_action == true)
        {            
            await Send_2_Webhook($"🖥 I'm monitoring.", 0);
            Vari.ToolStartDateTime = DateTime.Now;
            Vari.Webhooks.Monitoring_first_action = false;
        }
    }
    public static async Task LogMonitoringOFF()
    {
        try
        {                        
            int uptime = Convert.ToInt32((DateTime.Now - Vari.ToolStartDateTime).TotalSeconds);
            if (uptime >= 60 && uptime < 3600)
            {
                TimeSpan t = TimeSpan.FromSeconds(uptime);
                int uptimeMinutes = (int)t.TotalMinutes;
                await Send_2_Webhook($"⛔️‍ I stopped. Uptime: {uptimeMinutes} minute(s). {Vari.Webhooks.NexPlayersKicked} Player(s) kicked.", 0);
            }
            else if (uptime >= 3600)
            {
                TimeSpan t = TimeSpan.FromSeconds(uptime);
                int uptimeHours = (int)t.TotalHours;
                await Send_2_Webhook($"⛔️‍ I stopped. Uptime: {uptimeHours} hour(s). {Vari.Webhooks.NexPlayersKicked} Player(s) kicked.", 0);
            }
            else
            {
                await Send_2_Webhook($"⛔️‍ I stopped. Uptime: {uptime} seconds. {Vari.Webhooks.NexPlayersKicked} Player(s) kicked.", 0);
            }
            
            
        }
        catch (Exception ex)
        {
            Log.Ex(ex);
        }
    }
}