using NexDiscord;

namespace BF1.ServerAdminTools.NexDiscord;

public static partial class SexusBot
{
    public static partial class Interaction
    {
        public static async Task perms()
        {
            await OutAnsi($"Your Permission Level: {Ansi.B.Cyan}{VariS.Current.PermissionLVL}{Ansi.None}\nUserID: {VariS.Current.sm.Author.Id}");
        }
    }
}

