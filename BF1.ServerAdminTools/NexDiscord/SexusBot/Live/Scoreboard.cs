using BF1.ServerAdminTools.Features.Utils;
using NexDiscord;

namespace BF1.ServerAdminTools.NexDiscord;

public static partial class SexusBot
{
    public static partial class Live
    {
        public static async void Thread_Scoreboard_Channel() //threaded
        {
            while (true)
            {
                if (Vari.SexusBot.LiveFunctionsEnabled == false)
                {
                    Thread.Sleep(300000);
                    return;
                }

                if (Vari.CurrentMapName == null || Vari.NexServerinfo == null || Vari.NexServerDetails == null)
                {
                    await Modify_Ansi(Ansi.Bold + "Loading...", VariS.channel_scoreboard, VariS.msgid_vg_scoreboard0);
                    await Modify_Ansi(Ansi.Bold + "Loading...", VariS.channel_scoreboard, VariS.msgid_vg_scoreboard1);
                    await Modify_Ansi(Ansi.Bold + "Loading...", VariS.channel_scoreboard, VariS.msgid_vg_scoreboard2);

                    Thread.Sleep(VariS.LiveDelay_SB * 1000);
                    return;
                }

                var result = Scoreboard_Create();

                try
                {
                    await Modify_Ansi(result.s_info, VariS.channel_scoreboard, VariS.msgid_vg_scoreboard0);
                    await Modify_Ansi(result.s_t1, VariS.channel_scoreboard, VariS.msgid_vg_scoreboard1);
                    await Modify_Ansi(result.s_t2, VariS.channel_scoreboard, VariS.msgid_vg_scoreboard2);
                }
                catch (Exception ex)
                {
                    Log.Ex(ex);
                }

                Thread.Sleep(VariS.LiveDelay_SB * 1000);
            }
        }

        public static (string s_info, string s_t1, string s_t2) Scoreboard_Create()
        {
            string msg_info = $"🕑 Timestamp: {Ansi.B.Cyan}{DateTime.UtcNow.ToString("HH:mm:ss")}{Ansi.None}";
            string msg_team1 = "";
            string msg_team2 = "";


            StringBuilder sb = new();
            string winprediction_s = "";
            string strength_score = "";

            try
            {
                msg_info += $"\n\n🗺 {Ansi.B.Magenta}{Vari.CurrentMapName}{Ansi.None} (🕑{PlayerUtil.SecondsToMMSS(Vari.NexServerinfo.Time)})\n\n\n";

                string spawned1 = $"{Vari.NexStatisticData_Team1.PlayerCount}/{Vari.NexStatisticData_Team1.MaxPlayerCount}";
                string spawned2 = $"{Vari.NexStatisticData_Team2.PlayerCount}/{Vari.NexStatisticData_Team2.MaxPlayerCount}";

                sb.Append(String.Format("{0,-7}   {1,-5}     {2,-5}     "+Ansi.B.White+"{3,-5}   "+Ansi.None+"{4, 10}\n", 
                    "", "Kill-", "Flag-", "Total", "Spawned in"));
                sb.Append(String.Format("{0,-7}   {1,-5}     {2,-5}     "+Ansi.B.White+"{3,-5}   "+Ansi.None+"{4, 10}\n\n", 
                    "", "score", "score", $"Score", "/ of Total"));
                sb.Append(String.Format(Ansi.B.Blue+"{0,-7}"+Ansi.None+"   {1,5}     {2,5}     "+Ansi.B.White+ "{3,5}   "+Ansi.None+"{4, 10}\n", 
                    "Team 1:", Vari.NexServerinfo.Team1Kill, Vari.NexServerinfo.Team1Flag, Vari.NexServerinfo.Team1Score, spawned1));
                sb.Append(String.Format(Ansi.B.Red+"{0,-7}"+Ansi.None+"   {1,5}     {2,5}     "+Ansi.B.White+ "{3,5}   "+Ansi.None+"{4, 10}\n\n\n", 
                    "Team 2:", Vari.NexServerinfo.Team2Kill, Vari.NexServerinfo.Team2Flag, Vari.NexServerinfo.Team2Score, spawned2));

                sb.Append(String.Format("{0,-7}   {1,5}     {2,6}     {3,4}   {4, 10}\n\n", 
                    "", "Kills", "Deaths", "K/D", "LVL 150s"));
                sb.Append(String.Format(Ansi.B.Blue+"{0,-7}"+Ansi.None+"   {1,5}     {2,6}     {3,4}   {4, 10}\n", 
                    "Team 1:", Vari.NexStatisticData_Team1.AllKillCount, Vari.NexStatisticData_Team1.AllDeadCount, Vari.NexTeamKD1, Vari.NexStatisticData_Team1.Rank150PlayerCount));
                sb.Append(String.Format(Ansi.B.Red+"{0,-7}"+Ansi.None+"   {1,5}     {2,6}     {3,4}   {4, 10}\n\n\n\n", 
                    "Team 2:", Vari.NexStatisticData_Team2.AllKillCount, Vari.NexStatisticData_Team2.AllDeadCount, Vari.NexTeamKD2, Vari.NexStatisticData_Team2.Rank150PlayerCount));

                var strengths = Util_BF1.StrengthCalculation();

                int strength_t1 = strengths.Item1;
                int strength_t2 = strengths.Item2;

                strength_score = string.Format($"{Ansi.White}Strength Evaluation out of 1000:     "+Ansi.B.Magenta+"{0,3} "+Ansi.None+"<-> "+Ansi.B.Magenta+"{1,3}"+Ansi.None+"\n\n", strength_t1, strength_t2);

                int winprediction_t1 = Util_BF1.WinPercentageCalculation(strength_t1, strength_t2);

                if (winprediction_t1 >= 50)
                {
                    winprediction_s = $"{Ansi.White}Team 1 is predicted to win by:              {Ansi.B.Cyan}{winprediction_t1} {Ansi.None}%";
                }
                else
                {
                    winprediction_s = $"{Ansi.White}Team 2 is predicted to win by:              {Ansi.B.Cyan}{100 - winprediction_t1} {Ansi.None}%";
                }

                msg_info = Ansi.None + msg_info + sb.ToString() + strength_score + winprediction_s;
                msg_team1 = Ansi.B.Blue + "Team 1:\n\n" + Ansi.White +SexusBot.Interaction.CreateScoreboardText(1);
                msg_team2 = Ansi.B.Red + "Team 2:\n\n" + Ansi.White +SexusBot.Interaction.CreateScoreboardText(2);
                //msg_team1 = DiscNex.TBlock("Currently disabled because of Discord Rate Limitations :(");
                //msg_team2 = DiscNex.TBlock("Currently disabled because of Discord Rate Limitations :(");
            }
            catch (Exception ex) { Log.Ex(ex); }

            return (msg_info, msg_team1, msg_team2);
        }
    }
}

