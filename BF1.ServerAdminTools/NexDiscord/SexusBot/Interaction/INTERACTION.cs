using System;
using System.Collections.Generic;
using System.Text;

namespace BF1.ServerAdminTools.NexDiscord;

public static partial class SexusBot
{
    public static partial class Interaction
    {
        
        public static async Task IterateCommands()
        {
            switch (VariS.Current.words[0])
            {
                case ".ai":
                    await Interaction.ai();
                    break;
                case ".ai2":
                    await Interaction.ai2();
                    break;
                case ".ai3":
                    await Interaction.ai3();
                    break;
                case ".balance":
                    await Interaction.balance();
                    break;
                case ".debug":
                    await Interaction.debug();
                    break;
                case ".help":
                    await Interaction.help();
                    break;
                case ".inserver":
                    await Interaction.inserver();
                    break;
                case ".perms":
                    await Interaction.perms();
                    break;
                case ".search":
                    await Interaction.search();
                    break;
                case ".sexnort":
                    await Interaction.sexnort();
                    break;                
                case ".sus":
                    await Interaction.sus();
                    break;
                case ".team":
                    await Interaction.team();
                    break;
                case ".test":
                    await Interaction.test();
                    break;
                case ".uptime":
                    await Interaction.uptime();
                    break;
            }

            //if (VariS.Current.PermissionLVL < 1) return;
            switch (VariS.Current.words[0]) //perms 1
            {
                case ".kick":
                    if (await Permitted(1) == false) return;
                    await Interaction.kick();
                    break;
                case ".move":
                    if (await Permitted(1) == false) return;
                    await Interaction.move();
                    break;
                case ".status":
                    if (await Permitted(1) == false) return;
                    await Interaction.status();
                    break;
            }

            //if (VariS.Current.PermissionLVL < 2) return;
            switch (VariS.Current.words[0]) //perms 2
            {                
                case ".features":
                    if (await Permitted(2) == false) return;
                    await Interaction.features();
                    break;
                case ".live":
                    if (await Permitted(2) == false) return;
                    await Interaction.live();
                    break;
                case ".spec":
                    if (await Permitted(2) == false) return;
                    await Interaction.spec();
                    break;
                
            }
            //if (VariS.Current.PermissionLVL < 3) return;
            switch (VariS.Current.words[0]) //perms 3
            {
                case ".chat":
                    if (await Permitted(3) == false) return;
                    await Interaction.chat();
                    break;
                case ".restart":
                    if (await Permitted(3) == false) return;
                    await Interaction.restart();
                    break;
            }
        }
        public static async Task<bool> Permitted(int perms)
        {
            if (VariS.Current.PermissionLVL < perms)
            {
                await OutPermsFail();
                return false;
            }
            return true;
        }
        public static async Task IterateCommandsOLD()
        {
            //For Everyone
            await Interaction.help();
            await Interaction.test();
            await Interaction.debug();
            await Interaction.perms();
            await Interaction.sexnort();
            await Interaction.team();
            await Interaction.inserver();
            await Interaction.search();
            await Interaction.sus();

            //Perms 1 Guard
            await Interaction.status();
            await Interaction.kick();
            await Interaction.move();

            //Perms 2 Comm Admin
            await Interaction.spec();
            await Interaction.chat();
            await Interaction.live();

            //Perms 3 Admin
            await Interaction.features();
            await Interaction.restart();
        }
    }
}
