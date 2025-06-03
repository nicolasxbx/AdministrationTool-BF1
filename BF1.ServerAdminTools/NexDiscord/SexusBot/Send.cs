using Discord;
using Discord.WebSocket;
using NexDiscord;

namespace BF1.ServerAdminTools.NexDiscord;

public static partial class SexusBot
{
    #region Out
    public static async Task Out(string msg)
    {
        try
        {
            if (IsSpam(msg))
            {
                return;
            }
            
            Log.I($"SexusBot Out - Sending: {msg}");
            string msg2 = DWebHooks.TBlock(msg);
            VariS.Message_Last_Sent = await VariS.Current.channel_current.SendMessageAsync(msg2);            
        }
        catch (Exception ex)
        {
            Log.Ex(ex);
        }
    }
    public static async Task OutAnsi(string msg)
    {
        try
        {
            if (IsSpam(msg))
            {
                return;
            }

            Log.I($"SexusBot Out - Sending: {msg}");
            string msg2 = $"ansi\n{Ansi.None}{msg}\n";
            string msg3= DWebHooks.TBlock(msg2);//Tblocks
            VariS.Message_Last_Sent = await VariS.Current.channel_current.SendMessageAsync(msg3);
        }
        catch (Exception ex)
        {
            Log.Ex(ex);
        }
    }
    
    public static async Task OutNoBlock(string msg)
    {
        try
        {
            if (IsSpam(msg))
            {
                return;
            }

            Log.I($"SexusBot OutNoBlock - Sending: {msg}");            
            VariS.Message_Last_Sent = await VariS.Current.channel_current.SendMessageAsync(msg);
        }
        catch (Exception ex)
        {
            Log.Ex(ex);
        }
    }
    #endregion

    #region OutCustom
    public static async Task OutCustom(string msg, ISocketMessageChannel channel)
    {
        try
        {
            if (IsSpam(msg))
            {
                return;
            }

            Log.I($"SexusBot OutCustom - Sending: {msg}");
            string msg2 = DWebHooks.TBlock(msg);
            VariS.Message_Last_Sent = await channel.SendMessageAsync(msg2);
        }
        catch (Exception ex)
        {
            Log.Ex(ex);
        }
    }
    public static async Task OutCustomAnsi(string msg, ISocketMessageChannel channel)
    {
        try
        {
            if (IsSpam(msg))
            {
                return;
            }

            Log.I($"SexusBot Out - Sending: {msg}");
            string msg2 = $"ansi\n{msg}\n";
            string msg3 = DWebHooks.TBlock(msg2);//Tblocks
            VariS.Message_Last_Sent = await channel.SendMessageAsync(msg3);
        }
        catch (Exception ex)
        {
            Log.Ex(ex);
        }
    }
    public static async Task OutCustomNoBlock(string msg, ISocketMessageChannel channel)
    {
        try
        {
            if (IsSpam(msg))
            {
                return;
            }

            VariS.Message_Last_Sent = await channel.SendMessageAsync(msg);
        }
        catch (Exception ex)
        {
            Log.Ex(ex);
        }
    }
    #endregion

    #region misc
    public static async Task OutPermsFail()
    {
        try
        {
            //string msg = DWebHooks.TBlock("You lack permissions to use this command.");
            string msg = DWebHooks.TBlock("Ever heard of permissions? It's great to have em.");

            if (IsSpam(msg))
            {
                return;
            }

            VariS.Message_Last_Sent = await VariS.Current.channel_current.SendMessageAsync(msg);
        }
        catch (Exception ex)
        {
            Log.Ex(ex);
        }
    }
    public static async Task Modify(string msg, ISocketMessageChannel channel, ulong msgID)
    {
        try
        {
            if (IsSpam(msg))
            {
                return;
            }

            await channel.ModifyMessageAsync(msgID, m => m.Content = msg);
        }
        catch (Exception ex)
        {
            Log.Ex(ex);
        }
    }
    public static async Task Modify_Ansi(string msg, ISocketMessageChannel channel, ulong msgID)
    {
        try
        {
            if (IsSpam(msg))
            {
                return;
            }            
            string msg2 = $"ansi\n{Ansi.None}{msg}\n";
            string msg3 = DWebHooks.TBlock(msg2);//Tblocks
            await channel.ModifyMessageAsync(msgID, m => m.Content = msg3);
        }
        catch (Exception ex)
        {
            Log.Ex(ex);
        }
    }
    public static async Task Delete(IMessage message)
    {
        await message.DeleteAsync();
    }
    public static async Task File_Upload(string caption, string filepath, ISocketMessageChannel channel)
    {
        VariS.Message_Image_Last = await channel.SendFileAsync(filepath, caption);
    }
    public static async Task File_Delete_Last()
    {
        await VariS.Message_Image_Last.DeleteAsync();
    }
    #endregion




    private static bool IsSpamOLD(string msg)
    {
        if (msg != VariS.Current.cached_last_msg)
        {
            VariS.Current.cached_last_msg = msg;
            return false;
            //Message is not the same
        }

        //Message is the same, now checking time

        long currenttime = Util.GetUnixFromDate(DateTime.UtcNow);

        
        if (VariS.Message_Last_Sent_Time == 0) 
        {
            VariS.Current.cached_last_msg = msg;
            VariS.Message_Last_Sent_Time = currenttime;
            return false;
            //This is the first message of the day
        }

        if (currenttime >= VariS.Message_Last_Sent_Time + VariS.Spam_Message_Delay_Buffer) //if this message is [Delay_Timeout] seconds away from the last sent message time.
        {
            VariS.Current.cached_last_msg = msg;
            VariS.Message_Last_Sent_Time = currenttime;
            return false;
            //New repeating message is X Seconds after the last msg
        }

        Log.I("SexusBot Repeating msg string detected");
        return true;//Message is the same
    }
    private static bool IsSpam(string msg)
    {
        if (msg == VariS.Current.cached_last_msg || msg == VariS.Current.cached_last_msg2 || msg == VariS.Current.cached_last_msg3)
        {
            VariS.Current.cached_last_msg3 = VariS.Current.cached_last_msg2;
            VariS.Current.cached_last_msg2 = VariS.Current.cached_last_msg;
            VariS.Current.cached_last_msg = msg;

            Log.I("SexusBot Repeating msg string detected");
            return true;
        }
        VariS.Current.cached_last_msg3 = VariS.Current.cached_last_msg2;
        VariS.Current.cached_last_msg2 = VariS.Current.cached_last_msg;
        VariS.Current.cached_last_msg = msg;

        return false;
    }
}