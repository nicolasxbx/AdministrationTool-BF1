using Discord;
using Discord.WebSocket;

namespace BF1.ServerAdminTools.NexDiscord;

public static partial class SexusBot
{
    public class BotInstance
    {        
        public async Task RunBot()
        {
            if (VariS.FirstDiscordStart == false)
            {
                Log.I("Second DiscordBot Start stopped.");
                return;
            }

            /*
            if (Vari.Debug_Start_DiscordBot)
            {
                VariS.Token = VariS.Token2;
            }
            */

            var _config = new DiscordSocketConfig { MessageCacheSize = 10, GatewayIntents = GatewayIntents.All }; //new config
            VariS.client = new DiscordSocketClient(_config);

            await VariS.client.LoginAsync(TokenType.Bot, VariS.Token);
            await VariS.client.StartAsync();

            VariS.client.Ready += BotIsReady;
            VariS.client.Log += BotLogged;

            await Task.Delay(-1);
                        
            VariS.FirstDiscordStart = false;

            Log.I("BotInstance created");
        }   


        public async Task BotIsReady()
        {
            if (VariS.FirstDiscordStart == false)
            {
                Log.I("Second DiscordBot BOTISREADY stopped.");
                return;
            }

            Log.I("SexusBot is Ready");
            
            await VariS.client.SetGameAsync(VariS.bot_game_activity); // GAME STATUS
            
            VariS.client.MessageReceived += HandleMessageAsync;

            InitOpenAIAPI();

            if (Vari.Debug_Start_DiscordBot)
            {
                return;
            }

            InitChannels();
            Live.Start_Live_Checks();
            VariS.FirstDiscordStart = false;
        }        

        public async Task HandleMessageAsync(SocketMessage socketmessage)
        {            
            await RecievedMessage(socketmessage);
        }

        public static void InitChannels()
        {            
            try
            {
                VariS.channel_scoreboard = GetChannels("live-scoreboard", VariS.guildid_of_vg_guild);
                VariS.channel_admin_discussion = GetChannels("admin-discussion", VariS.guildid_of_vg_guild);
                VariS.channel_bot_commands = GetChannels("bot-commands", VariS.guildid_of_vg_guild);
                VariS.channel_test = GetChannels("test", 1023632217752215613); //test 2 server
            }
            catch (Exception ex)
            {
                Log.Ex(ex);
            }
        }       
        
        public Task BotLogged(LogMessage message)
        {
            Log.I("SexusBot BotLog: " + message);
            return Task.CompletedTask;
        }        
        public void InitOpenAIAPI()
        {
            try
            {
                VariS.OpenAIAPI = new(VariS.OpenAI_Key);
            }
            catch (Exception ex) 
            { 
                Log.Ex(ex, "OpenAI Init");
            }            
        }
    }
}
