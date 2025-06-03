using BF1.ServerAdminTools.Features.API;
using BF1.ServerAdminTools.Features.Data;
using NexDiscord;

namespace BF1.ServerAdminTools.NexDiscord;

public static partial class SexusBot
{
    public static partial class Interaction
    {
        public static async Task move()
        {
            string[] words = VariS.Current.words;            
            if (words.Length >= 2)
            {
                string name = words[1];
                PlayerData playerdata = FindPlayerInServer(name);
                if (playerdata != null)
                {
                    RespContent result;
                    int endteam = 0;

                    if (playerdata.TeamId == 1)
                    {
                        result = await BF1API.AdminMovePlayer(playerdata.PersonaId, 1);
                        endteam = 2;
                    }
                    else
                    {
                        result = await BF1API.AdminMovePlayer(playerdata.PersonaId, 2);
                        endteam = 1;
                    }
                    if (result.IsSuccess)
                    {
                        await OutAnsi($"{Ansi.Cyan}✅ Player was successfully moved to Team {endteam}");
                    }
                    else
                    {
                        await OutAnsi($"{Ansi.B.Red}❌ Move failed! EA Message:\n {result.Message}");
                        await Out($"His TeamID: {playerdata.TeamId}. endteam: {endteam}");
                    }
                }
                else
                {
                    await OutAnsi($"{Ansi.Red}❌ Player was not found playing on the Server!");
                }
            }
            else
            {
                if (words.Length == 1)
                {
                    await OutAnsi($"{Ansi.Bold}⚠️ Please specify a name");
                }
            }
        }
    }
}

