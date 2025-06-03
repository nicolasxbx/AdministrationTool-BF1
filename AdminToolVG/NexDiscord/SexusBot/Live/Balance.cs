using NexDiscord;

namespace BF1.ServerAdminTools.NexDiscord;

public static partial class SexusBot
{
    public static partial class Live
    {
        public static async void Thread_Balance() //threaded
        {
            while (true)
            {
                if (!Vari.SexusBotLiveFunctionsEnabled || !Vari.SexusBot.IsRunning)
                {
                    Thread.Sleep(30000);
                    return;
                }

                try
                {
                    if (Vari.ServerLiveInfo != null)
                    {
                        bool stomp = Util_BF1.UnbalanceChecker(Vari.ServerLiveInfo.Team1Score, Vari.ServerLiveInfo.Team2Score);

                        if (stomp == true && Vari.NexConquestAssaultMapNames.Contains(Vari.CurrentMapName) == false)
                        {                            
                            await OutCustomAnsi($"{Ansi.B.Red}Balancers needed. {Ansi.None}({Vari.ServerLiveInfo.Team1Score}-{Vari.ServerLiveInfo.Team2Score}) [{Ansi.B.Blue}{Vari.CurrentMapName}{Ansi.None}]", VariS.channel_bot_commands);
                            Thread.Sleep(360000); //6 Min
                        }
                    }
                }
                catch (Exception ex)
                {
                    Log.Ex(ex);
                }

                Thread.Sleep(VariS.LiveDelay_Balance * 1000);
            }
        }
    }
}

