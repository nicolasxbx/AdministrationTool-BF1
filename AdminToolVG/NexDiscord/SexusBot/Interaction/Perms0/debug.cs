namespace BF1.ServerAdminTools.NexDiscord;

public static partial class SexusBot
{
    public static partial class Interaction
    {
        public static async Task debug() //debug
        {
            if (!Vari.Debug_Start_DiscordBot)
            {
                return;
            }

            //await SendFile("caption", @"C:\AMStronic\test.jpg", GetChannels("general", VariS.guildid_of_test2_guild));
        }
        public static async void Debug_Thread()
        {
            for (int i = 0; i < 10; i++)
            {
                Debug.WriteLine("Debug Uploading file");
                if (VariS.Message_Image_Last != null)
                {
                    await File_Delete_Last();
                }
                await File_Upload("caption", @"C:\AMStronic\test.jpg", GetChannels("general", VariS.guildid_of_test2_guild));
                Thread.Sleep(3000);
            }
        }
    }
}

