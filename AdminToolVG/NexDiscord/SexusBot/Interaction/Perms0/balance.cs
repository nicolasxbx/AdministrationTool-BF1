using NexDiscord;
using System;

namespace BF1.ServerAdminTools.NexDiscord;

public static partial class SexusBot
{
    public static partial class Interaction
    {
        public static async Task balance() //debug
        {
            if (Vari.ServerLiveInfo == null || Vari.NexStatisticData_Team1 == null)
            {
                await OutAnsi(Ansi.Bold+"Match is currently finished or match stats are not retrievable.");
                return;
            }

            //SCORE
            string score = $"({Vari.ServerLiveInfo.Team1Score}-{Vari.ServerLiveInfo.Team2Score})";

            //STRENGTH
            var strengths = Util_BF1.StrengthCalculation();
            int strength_t1 = strengths.Item1;
            int strength_t2 = strengths.Item2;
            int strength_diff = strength_t1 - strength_t2;
            string strength;
            if (strength_diff > 150)
            {
                if (strength_diff > 250)
                {
                    strength = Ansi.None + $"Players of {Ansi.B.Blue}Team 1{Ansi.None} are much stronger.";
                }
                else
                {
                    strength = Ansi.None + $"Players of {Ansi.B.Blue}Team 1{Ansi.None} are stronger.";
                }                
            }
            else if(strength_diff < -150)
            {
                if (strength_diff < -250)
                {
                    strength = Ansi.None + $"Players of {Ansi.B.Red}Team 2{Ansi.None} are {Ansi.Bold}much stronger.";
                }
                else
                {
                    strength = Ansi.None + $"Players of {Ansi.B.Red}Team 2{Ansi.None} are {Ansi.Bold}stronger.";
                }
            }
            else
            {
                strength = $"{Ansi.Bold}Teams have equal strength ({Ansi.Bold}{strength_t1}{Ansi.None}-{Ansi.Bold}{strength_t2}{Ansi.None}).";
            }

            //WIN PREDICTION
            int winprediction_t1 = Util_BF1.WinPercentageCalculation(strength_t1, strength_t2);
            string winprediction_s;
            if (winprediction_t1 >= 50)
            {
                winprediction_s = $"{Ansi.Blue}Team 1{Ansi.None} is predicted to win by {Ansi.B.Cyan}{winprediction_t1}%{Ansi.None}.";
            }
            else
            {
                winprediction_s = $"{Ansi.Red}Team 2{Ansi.None} is predicted to win by {Ansi.B.Cyan}{100 - winprediction_t1}%.{Ansi.None}";
            }

            //CONCLUSION
            string conclusion = "";
            bool unbalanced = Util_BF1.UnbalanceChecker(Vari.ServerLiveInfo.Team1Score, Vari.ServerLiveInfo.Team2Score);
            int maxscore = Math.Max(Vari.ServerLiveInfo.Team1Score, Vari.ServerLiveInfo.Team2Score);
            int minscore = Math.Min(Vari.ServerLiveInfo.Team1Score, Vari.ServerLiveInfo.Team2Score);

            bool team1stronger = (strength_t1 > strength_t2);                        

            if (unbalanced == false)
            {
                if (maxscore >= 650)
                {
                    conclusion = "It's too late to balance this match.";
                }
                else if (minscore <= 150)
                {
                    if (strength_diff > 250 || strength_diff < -250)
                    {
                        if (team1stronger == true)
                        {
                            conclusion = "Map just started, but Players of Team 1 are much stronger.";
                        }
                        else
                        {
                            conclusion = "Map just started, but Players of Team 2 are much stronger.";
                        }                        
                    }
                    else
                    {
                        conclusion = "No balancing needed for now.";
                    }
                }
                else
                {
                    conclusion = "No balancing needed for now.";
                }
            }
            else
            {
                if(Vari.ServerLiveInfo.Team1Score > Vari.ServerLiveInfo.Team2Score)
                {
                    conclusion = "Team 2 needs help!";
                }
                else
                {
                    conclusion = "Team 1 needs help!";
                }
            }

            string s = $"{score}\n{strength}\n{winprediction_s}\n\n{Ansi.B.White}{conclusion}";
            await OutAnsi(Ansi.None+s);

            //await SendFile("caption", @"C:\AMStronic\test.jpg", GetChannels("general", VariS.guildid_of_test2_guild));
        }        
    }
}

