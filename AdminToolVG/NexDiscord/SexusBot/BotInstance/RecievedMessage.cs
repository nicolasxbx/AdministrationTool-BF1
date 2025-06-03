using Discord.WebSocket;

namespace BF1.ServerAdminTools.NexDiscord;

public static partial class SexusBot
{    
    public static async Task RecievedMessage(SocketMessage sm)
    {        
        if (sm == null)
        {
            return;
        }
        if(string.IsNullOrEmpty(sm.Content))
        {
            return;
        }
        if (sm.Author.IsBot == true)
        {
            return;
        }
        if (sm.Content[0] != '.')
        {
            return;
        }

        Log.I($"SexusBot msg recieved - {sm.Content}");

        ShowMsgInfo(sm);

        //Guild Whitelist Check
        VariS.Current.channel_sgc = sm.Channel as SocketGuildChannel;
        VariS.Current.GuildID = VariS.Current.channel_sgc.Guild.Id;

        if (VariS.sexusbot_whitelisted_guilds.Contains(VariS.Current.GuildID) == false) //Guild Whitelist
        {
            Log.I($"SexusBot: Guild of Message is not Whitelisted ({VariS.Current.GuildID}) !!");
            return;
        }

        //--------------------------- Values -----------------------------

        //SocketMessage
        VariS.Current.sm = sm;
        //SocketUserMessage sum = sm as SocketUserMessage;

        //Message
        VariS.Current.msg = sm.Content;
        try
        {
            VariS.Current.words = sm.Content.Split(' '); //Words of s -> words
        }
        catch
        {
            return;
        }
        //Channel
        VariS.Current.channel_current = sm.Channel;                
        //string channel_name = sm.Channel.Name;

        //User
        //SocketUser s_user = sm.Author;
        VariS.Current.user_sgu = sm.Author as SocketGuildUser;
        VariS.Current.user_socket_roles = VariS.Current.user_sgu.Roles;
        VariS.Current.user_username = sm.Author.Username+""+sm.Author.Id;

        //ulong s_user_id = s_user.Id;
        //string s_user_name = s_user.Username;
        //string s_user_namefull = s_user.ToString();
        //string s_user_number = s_user.Discriminator;

        //------------------ Permissions / Interactions -----------------
        
        PermissionEvaluation(sm.Author.Id);

        await Interaction.IterateCommands();        
    }

    private static void ShowMsgInfo(SocketMessage sm)
    {
        Debug.WriteLine($"");
        Debug.WriteLine($"Message: {sm.Content}");
        Debug.WriteLine($"Author_Username: {sm.Author}");
        Debug.WriteLine($"Channel_Name: {sm.Channel.Name}");
        Debug.WriteLine($"Channel_ID: {sm.Channel.Id}");
        Debug.WriteLine($"Channel: {sm.Channel}");
    }
}

