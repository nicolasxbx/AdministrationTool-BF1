using NexDiscord;

namespace BF1.ServerAdminTools.NexDiscord;

public static partial class SexusBot
{
    public static partial class Interaction
    {
        public static async Task help()
        {
            string s = $"{Ansi.Bold}Available commands for you:{Ansi.None}\n";

            //Everyone
            string ai = ".ai <prompt> (Conversational Chatbot like ChatGPT)\n";
            string ai2 = ".ai2 <prompt> (Unbiased Chatbot-Query)\n";
            string ai3 = ".ai3 <prompt> (Fast, uncensored but weak TextBot-Query)\n";
            string test = ".test\n";
            string sexnort = ".sexnort\n";
            string team = ".team <1/2>\n";
            string inserver = ".inserver <playername>\n";
            string search = ".search <weaponname>\n";
            string sus = ".sus <playername>\n";
            string summary = ".summary\n";
            if (VariS.Current.PermissionLVL >= 0)
            {
                s += ai; 
                s += ai2; 
                s += ai3;
                s += test;
                s += sexnort;
                s += team;
                s += inserver;
                s += search;
                s += sus;
                s+= summary;
            }

            //Guard
            string status = ".status\n";
            string kick = ".kick <playername> <reason>\n";
            string move = ".move <playername>\n";

            if (VariS.Current.PermissionLVL >= 1)
            {
                s += Ansi.Cyan;
                s += status;
                s += kick;
                s += move;
            }

            //Comm Admins
            string spec = ".spec\n";
            string features = ".features <on/off> [feature-number 1-5]\n  or .features <value 100-500> <feature-number 6-7>\n";
            
            string live = ".live [sus]\n";

            if (VariS.Current.PermissionLVL >= 2) // Comm Admin
            {
                s += Ansi.Blue;
                s += spec;
                s += features;
                
                s += live;
            }

            //Admins

            string chat = ".chat <rules> or .chat <YOURMESSAGE>\n";
            string restart = ".restart\n";
            if (VariS.Current.PermissionLVL >= 3) // Admin
            {
                s += Ansi.Magenta;
                s += chat;
                s += restart;
            }

            await OutAnsi(s);
        }
    }
}

