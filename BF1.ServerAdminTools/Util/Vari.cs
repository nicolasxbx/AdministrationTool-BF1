using BF1.ServerAdminTools.Features.API.RespJson;
using BF1.ServerAdminTools.Features.Data;
using BF1.ServerAdminTools.Models; //tna

namespace BF1.ServerAdminTools;


public static class Vari //From "Globals"
{
    //Meta
    public static bool DiscordMode { get; } = true;
    public static bool Debug_Start_DiscordBot = false;
    public static string Nexversion { get; set; } = "v9.5D";
    


    public static DateTime ToolStartDateTime { get; set; }
    public static bool AutoRun { get; set; } = false;
    public static bool RuleWindow_Ready { get; set; } = false;
    public static int Sus_minkills { get; } = 150;
    public static int Sus_minacc { get; } = 70;
    public static int Sus_minhs { get; } = 70;


    public static class SexusBot //SexusBot
    {
        public static bool IsRunning { get; set; } = false;
        public static bool BotInStartupParams { get; set; } = false;

        public static bool LiveFunctionsEnabled { get; set; } = false; //to auto start live functions
        public static bool LiveSusEnabled { get; set; } = false; //to auto start live functions
    }
    public static class Webhooks
    {
        public static int NexPlayersKicked { get; set; } = 0;
        public static bool Monitoring_first_action { get; set; } = true;
        public static bool UseCustomWebhooks { get; set; } = false;
        public static string LastSentMessage { get; set; } = string.Empty;
        public static string LastSentMessagePrev { get; set; } = string.Empty;

        public static string Monitoring { get; set; } = string.Empty;
        public static string Kick { get; set; } = string.Empty;
        public static string Ping { get; set; } = string.Empty;
        public static string BFBANWS { get; set; } = string.Empty;
        public static string Balancer { get; set; } = string.Empty;
        public static string Monitoring_VG { get; } = ""; // Removed for publishing
        public static string Kick_VG { get; } = ""; // Removed for publishing

        public static string Ping_VG { get; } = ""; // Removed for publishing

        public static string BFBANWS_VG { get; } = ""; // Removed for publishing
        public static string Balancer_VG { get; } = ""; // Removed for publishing
	}

    //Rest
    public static string WelcomeMessageNex { get; set; } = string.Empty; //tna          
    public static string UserNameOfUser { get; set; } = string.Empty;
    public static string bfbanjsonPrevious { get; set; } = string.Empty;
    public static string gtserverinfojsonPrevious { get; set; } = string.Empty;
    public static string CurrentMapName { get; set; } = string.Empty;
    public static string GameID_VG { get; } = "8315323330831";
    public static string[] NexConquestAssaultMapNames { get;} = { "Caporetto", "River Somme", "Cape Helles", "Zeebrugge", "Heligoland" };


    //Kicking
    public static bool AutoKickBreakPlayer { get; set; } = false;
    public static bool AutoKickPing { get; set; } = false;
    public static bool AutoKickWinSwitching { get; set; } = false;
    public static bool AutoCheckBFBAN { get; set; } = false;
    public static bool AutoKickBFBAN { get; set; } = false;
    public static int PingLimit { get; set; } = 200;
    public static int TicketLimit { get; set; } = 200;

    //Game Data    
    public static PlayerN[] globalplayernarray1 { get; set; }
    public static PlayerN[] globalplayernarray2 { get; set; }
    public static List<PlayerData> Playerlist_All { get; set; }    
    public static List<PlayerData> Playerlist_Team1 { get; set; }
    public static List<PlayerData> Playerlist_Team2 { get; set; }
    public static StatisticData NexStatisticData_Team1 { get; set; }
    public static StatisticData NexStatisticData_Team2 { get; set; }
    public static ServerInfo NexServerinfo { get; set; }
    public static double NexTeamKD1 { get; set; }
    public static double NexTeamKD2 { get; set; }
    public static ServerDetails NexServerDetails { get; set; } = new();
    public static string CurrentServerName { get; set; } = "";

    #region old
    public static string Remid { get; set; } = string.Empty;
    public static string Sid { get; set; } = string.Empty;
    public static bool IsUseMode1 { get; set; } = true;
    public static string SessionId_Mode1 { get; set; } = string.Empty;
    public static string SessionId_Mode2 { get; set; } = string.Empty;
    public static string GameId { get; set; } = string.Empty;
    public static string ServerId { get; set; } = string.Empty;
    public static string PersistedGameId { get; set; } = string.Empty;
    public static bool IsRuleSetRight { get; set; } = false;
    public static List<string> Custom_WeaponList_Team1 { get; set; } = new();
    public static List<string> Custom_WeaponList_Team2 { get; set; } = new();
    public static List<string> Custom_BlackList { get; set; } = new();
    public static List<string> Custom_WhiteList { get; set; } = new();
    public static List<long> Server_AdminList_PID { get; set; } = new();
    public static List<string> Server_AdminList_Name { get; set; } = new();
    public static List<long> Server_VIPList { get; set; } = new();
    public static List<BreakRuleInfo> BreakRuleInfo_PlayerList { get; set; } = new();
    public static List<SpectatorInfo> Server_SpectatorList { get; set; } = new();
    public static bool IsShowCHSWeaponName { get; set; } = true;
    public static string SessionId
    {
        get
        {
            return IsUseMode1 ? SessionId_Mode1 : SessionId_Mode2;
        }
    }
    #endregion


    public static IntPtr NwindowHandle { get; set; }
    public static IntPtr NprocessHandle { get; set; }
    public static int NprocessId { get; set; }
    public static long NpBaseAddress { get; set; }
    public static long NPtrAddress { get; set; }
}
