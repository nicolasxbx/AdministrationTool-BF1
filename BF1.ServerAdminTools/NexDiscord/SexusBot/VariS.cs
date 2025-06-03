using Discord;
using Discord.WebSocket;
using OpenAI_API;
using System.Security.Policy;
using OpenAI_API.Chat;
using OpenAI_API.Completions;
using OpenAI_API.Models;

namespace BF1.ServerAdminTools.NexDiscord;

public static partial class VariS
{
    #region key
    public static string Token = ""; // Removed for publishing
    public static string Token2 = ""; // Removed for publishing
    public static string OpenAI_Key = ""; // Removed for publishing
	#endregion
	public static DiscordSocketClient client { get; set; }
    public static SexusBot.BotInstance botinstance { get; set; }
    public static OpenAIAPI OpenAIAPI { get; set; }
    public static Conversation conversation { get; set; }
    public static bool FirstDiscordStart { get; set; } = true;
    public static string bot_game_activity { get; } = "Battlefield 1";
    public static int Spam_Message_Delay_Buffer = 4;
    public static ulong guildid_of_vg_guild { get; } = 947870312572268584;
    public static ulong guildid_of_test2_guild { get; } = 1023632217752215613;
    public static List<ulong> sexusbot_whitelisted_guilds { get;} = new()
        {
            947870312572268584, //VG Server
            1023632217752215613, //test2
            929167104828133396 //test1
        };
    public static double OpenAI_Temperature = 0.5;
    public static ulong msgid_vg_scoreboard0 { get; } = 1026170889726857336;
    public static ulong msgid_vg_scoreboard1 { get; } = 1026170892780322957;
    public static ulong msgid_vg_scoreboard2 { get; } = 1026170893367509022;
    public static int LiveDelay_SB { get; } = 30; //30 sec
    public static int LiveDelay_Balance { get; } = 120; //2 Min
    public static int LiveDelay_Sus { get; } = 300; //5 Min
    public static int LiveDelay_Chat { get; } = 900; //15 Min
    public static bool FirstThreadStart { get; set; } = true;
    public static IMessage Message_Image_Last { get; set; } = null;
    public static IMessage Message_Last_Sent { get; set; } = null;
    public static long Message_Last_Sent_Time { get; set; } = 0;
    public static string chat_function_last_msg { get; set; } = "";
    public static Random rnd { get; set; } = new();
        

    #region Current
    public static class Current
    {
        public static SocketMessage sm { get; set; }
        public static string msg { get; set; }
        public static string[] words { get; set; }
        public static ISocketMessageChannel channel_current { get; set; }
        public static SocketGuildChannel channel_sgc { get; set; }        
        public static ulong GuildID { get; set; }
        public static int PermissionLVL { get; set; } = 0;
        public static SocketGuildUser user_sgu { get; set; }
        public static IReadOnlyCollection<SocketRole> user_socket_roles { get; set; }
        public static string user_username { get; set; } = "";        
        public static string cached_last_msg { get; set; }
        public static string cached_last_msg2 { get; set; }
        public static string cached_last_msg3 { get; set; }
    }
    #endregion
    public static ISocketMessageChannel channel_scoreboard { get; set; }
    public static ISocketMessageChannel channel_admin_discussion { get; set; }
    public static ISocketMessageChannel channel_bot_commands { get; set; }
    public static ISocketMessageChannel channel_test { get; set; }

}