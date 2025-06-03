using NexDiscord;

namespace BF1.ServerAdminTools.NexDiscord;

public static partial class SexusBot
{
    public static partial class Interaction
    {
        public static async Task restart()
        {
            string[] words = VariS.Current.words;
            if (words[0] == ".restart" && VariS.Current.PermissionLVL >= 3)
            {
                if (words.Length == 1) //Discord Bot
                {
                    await OutAnsi($"{Ansi.B.Blue}Restarting the Tool...");
                    await App.RestartNex();
                } 
            }
            if (words[0] == ".restart" && VariS.Current.PermissionLVL < 3)
            {
                await OutPermsFail();
            }
        }
    }
}
