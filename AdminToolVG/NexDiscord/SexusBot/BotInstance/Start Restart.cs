using BF1.ServerAdminTools.Common.Helper;

namespace BF1.ServerAdminTools.NexDiscord;

public static partial class SexusBot
{
    public static async Task Toggle()
    {
        if (Vari.SexusBot.IsRunning == true)
        {
            Log.I("SexusBot stopped (toggle)");
            //NotifierHelper.Show(NotifierType.Information, "Discord Bot stopped");            
            await Stop();
        }
        else
        {
            if (!Vari.SexusBot.BotInStartupParams)
            {
                //NotifierHelper.Show(NotifierType.Warning, "Discord Bot not allowed");
                return;
            }

            Log.I("SexusBot Started (toggle)");
            //NotifierHelper.Show(NotifierType.Information, "Discord Bot started");
            VariS.FirstDiscordStart = true;
            await Start();
        }
    }
    public static async Task Start()
    {
        if (!Vari.DiscordMode)
        {
            Log.I("DiscordMode disabled. SexusBot not started");
            return;
        }

        if (Vari.SexusBot.IsRunning)
        {
            Log.I("SexusBot running already. SexusBot not started");
            return;
        }

        VariS.FirstThreadStart = true;
        VariS.botinstance = new();
        VariS.client = new();
        Log.D("Client created");
        Vari.SexusBot.IsRunning = true;
        await VariS.botinstance.RunBot();
    }    
    public static async Task Stop()
    {        
        if (Vari.SexusBot.IsRunning == false)
        {
            Log.I("SexusBot not running, so cannot stop.");
            return;
        }

        Vari.SexusBot.IsRunning = false;
        await VariS.client.StopAsync();
        await VariS.client.LogoutAsync();
        VariS.botinstance = null;
        Log.D("SexusBot Stopped");
    }
    
    /*
    public static void Restart()
    {        
        Thread restart_thread = new Thread(restart_thread_restart)
        {
            IsBackground = true
        };        
        restart_thread.Start();
    }
    public static async void restart_thread_restart()
    {
        Thread.Sleep(1000);

        Log.I("res1");
        await VariS.client.StopAsync();
        Log.I("res2");
        await VariS.client.LogoutAsync();
        Log.I("res3");

        Vari.Sexus.bot_is_running = false;

        Thread.Sleep(1000);

        await Start();
        Log.I("res4");        

        Log.I("Bot restarted.");
        Out("Bot restarted.");
    }*/
}
