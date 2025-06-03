namespace BF1.ServerAdminTools;

public static class Util_BF1
{
    public static bool UnbalanceChecker(int t1, int t2)
    {
        int scorediff = t1 - t2;
        //int minscore;
        int maxscore = Math.Max(t1, t2);        

        if (maxscore <= 150 || maxscore >= 650)
        {
            return false;
        }

        if (maxscore <= 300)
        {
            if (scorediff >= 100 || scorediff <= -100)
            {
                return true;
            }
        }

        if (maxscore <= 400)
        {
            if (scorediff >= 150 || scorediff <= -150)
            {
                return true;
            }
        }

        if (maxscore <= 500)
        {
            if (scorediff >= 200 || scorediff <= -200)
            {
                return true;
            }
        }

        if (maxscore <= 650)
        {
            if (scorediff >= 250 || scorediff <= -250)
            {
                return true;
            }
        }

        return false;
    }

    public static (int, int) StrengthCalculation()
    {
        if (Vari.NexServerinfo == null || Vari.NexServerDetails == null || Vari.NexStatisticData_Team1 == null)
        {
            return (-1, -1);
        }

        int strength_t1 = 0;
        int strength_t2 = 0;

        strength_t1 = (int)(Vari.NexStatisticData_Team1.Rank150PlayerCount * 60 + Vari.NexTeamKD1 * 150);
        strength_t2 = (int)(Vari.NexStatisticData_Team2.Rank150PlayerCount * 60 + Vari.NexTeamKD2 * 150);

        if (strength_t1 >= 1000)
        {
            strength_t1 = 999;
        }
        if (strength_t2 >= 1000)
        {
            strength_t2 = 999;
        }

        //Trying to stop weird overflow where it shows the max int decimal
        if (strength_t1 > 1000 || strength_t2 > 1000 || strength_t1 < -1000 || strength_t2 < -1000)
        {
            strength_t1 = 0;
            strength_t2 = 0;
        }

        return (strength_t1, strength_t2);
    }
    public static int WinPercentageCalculation(int s1, int s2)
    {
        if (Vari.NexServerinfo == null)
        {
            return -1;
        }

        int scorediff = (Vari.NexServerinfo.Team1Score - Vari.NexServerinfo.Team2Score);

        int winprediction_t1 = (int)(scorediff * 0.08) + 50; //625 Diff = 99% Win Chance

        //Factor in 5% of Strength-Difference.
        winprediction_t1 += Convert.ToInt32((s1 - s2) * 0.05);

        if (winprediction_t1 >= 100)
        {
            winprediction_t1 = 99;
        }
        if (winprediction_t1 <= 0)
        {
            winprediction_t1 = 1;
        }

        return winprediction_t1;
    }
}
