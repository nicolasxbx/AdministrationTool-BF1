namespace BF1.ServerAdminTools.NexDiscord;
public static partial class DWebHooks
{
    
    private static string Botname()
    {
        if (Vari.UserNameOfUser == null || Vari.UserNameOfUser == "")
        {
            if (Vari.SexusBot.IsRunning == true)
            {
                return "SexusBot";
            }
            else
            {
                return "No Name (Unknown)";
            }
        }
        else
        {
            return Vari.UserNameOfUser;
        }

    }
    internal static string TBlock(string s)
    {
        return "```" + s + "```";
    }
    private static string TBold(string s)
    {
        //return "**" + s + "**";
        return s;
    }
}
