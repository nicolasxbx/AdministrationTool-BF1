using BF1.ServerAdminTools.Features.Data;
using NexDiscord;

namespace BF1.ServerAdminTools.NexDiscord;

public static partial class SexusBot
{
    public static partial class Interaction
    {
        public static async Task team()
        {
            string[] words = VariS.Current.words;
            if (words[1] == "1" || words[1] == "2")
            {
                int team = Int32.Parse(words[1]);

                string sb2 = CreateScoreboardText(team);

                if (sb2 != null)
                {
                    await OutAnsi(Ansi.White+sb2.ToString());
                }
                else
                {
                    await OutAnsi($"{Ansi.B.Red}⚠️ An Error occured.");
                }

            }
            if (words.Length == 1)
            {
                await OutAnsi($"Syntax: .team {Ansi.Bold}<1/2>");
            }
        }

        public static string CreateScoreboardText(int team)
        {
            int index1 = 1;            

            List<string> sp_ID = new();
            List<int> sp_Rank = new();
            List<string> sp_name = new();
            List<int> sp_K = new();
            List<int> sp_D = new();
            List<int> sp_Score = new();

            List<long> sp_pid = new();

            try
            {
                if (Vari.Playerlist_All.Any() && Vari.Playerlist_All != null)
                {
                    List<PlayerData> Playerlist_All_Sorted = Vari.Playerlist_All.OrderByDescending(o => o.Score).ToList();
                    foreach (PlayerData p in Playerlist_All_Sorted)
                    {
                        if (p.TeamId == team)
                        {
                            sp_ID.Add($"{index1}.");
                            sp_Rank.Add(p.Rank);
                            sp_name.Add(p.Name);
                            sp_K.Add(p.Kills);
                            sp_D.Add(p.Deaths);
                            sp_Score.Add(p.Score);
                            sp_pid.Add(p.PersonaId);
                            index1++;
                        }
                    }
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                Log.Ex(ex);
            }

            string[] sp_ID_a = sp_ID.ToArray();
            int[] sp_Rank_a = sp_Rank.ToArray();
            string[] sp_name_a = sp_name.ToArray();
            int[] sp_K_a = sp_K.ToArray();
            int[] sp_D_a = sp_D.ToArray();
            int[] sp_Score_a = sp_Score.ToArray();
            long[] sp_pid_a = sp_pid.ToArray();

            var sb = new StringBuilder();

            sb.Append(String.Format("{0,-3}    {1,3}    {2,-16}  {3, 3}  {4, 3}     {5, 5}\n\n",
                "ID", "LVL", "Name", "K", "D", "Score"));

            string color_admin;
            string color_admin2;
            for (int index = 0; index < sp_ID_a.Length; index++)
            {
                if (IsAdmin(sp_pid_a[index]) == true)
                {
                    color_admin = Ansi.B.Red;
                    color_admin2 = Ansi.White;
                }
                else
                {
                    color_admin = "";
                    color_admin2 = "";
                }

                sb.Append(String.Format("{0,-3}    {1,3}    "+color_admin+"{2,-16}"+color_admin2+"  {3, 3}  {4, 3}     {5, 5}\n", 
                sp_ID_a[index], sp_Rank_a[index], sp_name_a[index], sp_K_a[index], sp_D_a[index], sp_Score_a[index]));
            }

            return sb.ToString();
        }
    }
}

