using System.Collections.Specialized;

namespace BF1.ServerAdminTools.NexDiscord;
public static partial class DWebHooks
{    
    private static async Task Send_2_Webhook(string msg, int type) // 0 = Monitor, 1 = Kick, 2 = Ping, 3 = BFBAN, 4 = Balancer
    {
        if (Vari.ServerDetails.ServerName.Contains("[VG]"))
        {
            if (type == 0)
            {
                await Send_3_Botname(msg, Vari.Webhooks.Monitoring_VG);
            }
            else if (type == 1)
            {
                await Send_3_Botname(msg, Vari.Webhooks.Kick_VG);
            }
            else if (type == 2)
            {
                await Send_3_Botname(msg, Vari.Webhooks.Ping_VG);
            }
            else if (type == 3)
            {
                await Send_3_Botname(msg, Vari.Webhooks.BFBANWS_VG);
            }
            else if (type == 4)
            {
                await Send_3_Botname(msg, Vari.Webhooks.Balancer_VG);
            }
        }
        else
        {
            if (type == 0)
            {
                await Send_3_Botname(msg, FileConfig.CurrentConfig.WebhookConfig.monitoring);
            }
            else if (type == 1)
            {
                await Send_3_Botname(msg, FileConfig.CurrentConfig.WebhookConfig.itemkicks);
            }
            else if (type == 2)
            {
                await Send_3_Botname(msg, FileConfig.CurrentConfig.WebhookConfig.pingkicks);
            }
            else if (type == 3)
            {
                await Send_3_Botname(msg, FileConfig.CurrentConfig.WebhookConfig.winswitch_bfban);
            }
            else if (type == 4)
            {
                await Send_3_Botname(msg, FileConfig.CurrentConfig.WebhookConfig.balancers);
            }
        }
    }
    private static async Task Send_3_Botname(string msg, string wh)
    {
        if (string.IsNullOrEmpty(wh)) return;

        if (wh.Length < 50) return;

        try
        {
            await Send_4(wh,
            msg,
            Botname()
            );
        }
        catch (Exception ex)
        {
            Log.Ex(ex);
        }
    }
    private static async Task Send_4(string url, string message, string botname)
    {
        if (message != Vari.Webhooks.LastSentMessage && message != Vari.Webhooks.LastSentMessagePrev)
        {
            try
            {
                await Send_5_Post(url, new NameValueCollection() {
                {
                   "username",
                   botname
                },
                {
                   "content",
                   message
                }
                });
            }
            catch (Exception ex)
            {
                Log.Ex(ex);
            }
        }
        else
        {
            Log.Ex("Repeating Webhook message detected.");
        }

        Vari.Webhooks.LastSentMessagePrev = Vari.Webhooks.LastSentMessage;
        Vari.Webhooks.LastSentMessage = message;
    }
    private static async Task<byte[]> Send_5_Post(string uri, NameValueCollection pair)
    {
        try
        {            
            using (WebClient wc = new WebClient())
            {
                return await wc.UploadValuesTaskAsync(uri, pair);
            }
        }
        catch
        {
            Log.Ex("Failed to send Webhook. (Spammed?)");
            return null;
        }
    }
}
