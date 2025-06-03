using BF1.ServerAdminTools.Features.Data;
using BF1.ServerAdminTools.Features.Utils;
using NexDiscord;

namespace BF1.ServerAdminTools.NexDiscord;

public static partial class SexusBot
{
    public static partial class Interaction
    {
        public static async Task search()
        { 
            string wepname = VariS.Current.words[1];
            string results = "";
            foreach (PlayerData p in Vari.Playerlist_All)
            {
                if (p.WeaponS0.Contains(PlayerUtil.GetWeaponChsName(wepname), StringComparison.CurrentCultureIgnoreCase) == true) //primary
                {
                    results += $"{p.Name}: {p.WeaponS0}\n";
                }
                if (p.WeaponS1.Contains(PlayerUtil.GetWeaponChsName(wepname), StringComparison.CurrentCultureIgnoreCase) == true) //secondary
                {
                    results += $"{p.Name}: {p.WeaponS1}\n";
                }
                if (p.WeaponS2.Contains(PlayerUtil.GetWeaponChsName(wepname), StringComparison.CurrentCultureIgnoreCase) == true) //G1
                {
                    results += $"{p.Name}: {p.WeaponS2}\n";
                }
                if (p.WeaponS5.Contains(PlayerUtil.GetWeaponChsName(wepname), StringComparison.CurrentCultureIgnoreCase) == true) //G2
                {
                    results += $"{p.Name}: {p.WeaponS5}\n";
                }
                if (p.WeaponS6.Contains(PlayerUtil.GetWeaponChsName(wepname), StringComparison.CurrentCultureIgnoreCase) == true) //nade
                {
                    results += $"{p.Name}: {p.WeaponS6}\n";
                }
            }
            if (results == "")
            {
                await OutAnsi($"{Ansi.Bold}No reults found.");
            }
            else
            {
                await OutAnsi($"{Ansi.White}Results for '{Ansi.B.Blue}{wepname}{Ansi.None}':\n\n{results}");
            }
        }
    }
}
