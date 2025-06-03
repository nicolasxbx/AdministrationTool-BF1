using Discord.WebSocket;

namespace BF1.ServerAdminTools.NexDiscord;

public static partial class SexusBot
{
    public static void PermissionEvaluation(ulong userid) //0=All 1=Clan 2=ComAdmin 3=Admin //4=me
    {
        VariS.Current.PermissionLVL = 0;
        if (UserHasRoleName("Friend") == true)
        {
            VariS.Current.PermissionLVL = 0;
        }
        if (UserHasRoleName("Guard") == true)
        {
            VariS.Current.PermissionLVL = 1;
        }
        if (UserHasRoleName("Community Admin") == true)
        {
            VariS.Current.PermissionLVL = 2;
        }
        if (UserHasRoleName("Admin") == true)
        {
            VariS.Current.PermissionLVL = 3;                      
        }    
    }

    public static bool IsNotAdminOrWhitelisted(string name, long pid)
    {
        try
        {
            if (Vari.Server_AdminList_PID.Contains(pid) || !Vari.Custom_WhiteList.Contains(name))
            {
                return false; //Is Admin or Whitelisted
            }
            else
            {
                return true; //Is Not.
            }
        }
        catch
        {
            return false; //Error occured.
        }
    }
    public static bool? IsAdmin(long pid)
    {
        try
        {
            if (Vari.Server_AdminList_PID.Contains(pid))
            {
                return true; //Is Admin
            }
            else
            {
                return false; //Is Not.
            }
        }
        catch
        {
            return null; //Error occured.
        }
    }

    public static bool UserHasRoleName(string rolename)
    { 
        foreach (SocketRole r in VariS.Current.user_socket_roles)
        {
            if (r.Name == rolename)
            {
                return true;
            }            
        }
        return false;
    }
    public static bool UserRolesContainName(string rolename)
    {
        foreach (SocketRole r in VariS.Current.user_socket_roles)
        {
            if (r.Name.Contains(rolename))
            {
                return true;
            }
        }
        return false;
    }
    public static bool UserHasRoleID(ulong roleid)
    {
        foreach (SocketRole r in VariS.Current.user_socket_roles)
        {
            if (r.Id == roleid)
            {
                return true;
            }
        }
        return false;
    }    
    public static ISocketMessageChannel GetChannels(string name, ulong guildID)
    {
        var channel = VariS.client.GetGuild(guildID).Channels.SingleOrDefault(x => x.Name == name);
        return channel as ISocketMessageChannel;
    }    
}

