namespace BF1.ServerAdminTools.NexDiscord;

public static partial class DWebHooks
{
    public static async Task LogMonitoringON()
    {
        if (Vari.Webhooks.Monitoring_first_action == true)
        {            
            await Send_2_Webhook($"🖥 I'm monitoring.", 0);
            Vari.ToolStartDateTime = DateTime.UtcNow;
            Vari.Webhooks.Monitoring_first_action = false;
        }
    }
    public static async Task LogMonitoringOFF()
    {
        try
        {                        
            int uptime_seconds = Convert.ToInt32((DateTime.UtcNow - Vari.ToolStartDateTime).TotalSeconds);
            if (uptime_seconds >= 60 && uptime_seconds < 3600)
            {
                TimeSpan t = TimeSpan.FromSeconds(uptime_seconds);
                int uptimeMinutes = (int)t.TotalMinutes;
                await Send_2_Webhook($"⛔️‍ I stopped. Uptime: {uptimeMinutes} minute(s). {Vari.Webhooks.NexPlayersKicked} Player(s) kicked.", 0);
            }
            else if (uptime_seconds >= 3600)
            {
                TimeSpan t = TimeSpan.FromSeconds(uptime_seconds);
                int uptimeHours = (int)t.TotalHours;
                await Send_2_Webhook($"⛔️‍ I stopped. Uptime: {uptimeHours} hour(s). {Vari.Webhooks.NexPlayersKicked} Player(s) kicked.", 0);
            }
            else
            {
                await Send_2_Webhook($"⛔️‍ I stopped. Uptime: {uptime_seconds} seconds. {Vari.Webhooks.NexPlayersKicked} Player(s) kicked.", 0);
            }
            
            
        }
        catch (Exception ex)
        {
            Log.Ex(ex);
        }
    }
}