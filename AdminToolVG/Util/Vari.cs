namespace AdminToolVG;

public static class Vari //From "Globals"
{
    //Meta
    public static string Nexversion { get; } = "v5.1";                
    public static int Sus_minkills { get; } = 150;
    public static int Sus_minacc { get; } = 70;
    public static int Sus_minhs { get; } = 70;
    public static bool DiscordMode { get; } = true;
    public static bool Debug_Start_DiscordBot { get; } = false;
    public static string CurrentRuleString { get; set; } = "";
    public static bool CurrentlyInStatusMode { get; set; } = false;
    public static string VGRuleString { get; } = "RULES: NO SMG08, ARTYTRUCK, HEAVY BOMBER AND RIFLE NADES. NO POLITICS, WINSWITCHING OR GRIEFING. MAX 200 Ping. [VG] Vanguard are recruiting - Report at discord.gg/VGBF1.";
    public static string[] NexConquestAssaultMapNames { get; } = { "Caporetto", "River Somme", "Cape Helles", "Zeebrugge", "Heligoland" };

    // SessionID + Username
    public static string SessionID { get; set; } = "";
    public static string WelcomeMessage { get; set; } = string.Empty; //tna          
    public static string CurrentUsername { get; set; } = "<username>";

    //CONSOLE
    public static bool FirstStartMenuDisplay { get; set; } = true;
    public static bool CustomRules { get; set; } = false;
    public static List<NexLogKickObject> Logs_Kicks { get; set; } = new();
    public static List<NexLogSwitchObject> Logs_Switches { get; set; } = new();    

    //Kicking
    public static ObservableCollection<RuleWeaponModel> WeaponItemsAndBanFlags { get; set; } = new();
    public static List<string> BannedWeapons_Team1 { get; set; } = new();
    public static List<string> BannedWeapons_Team2 { get; set; } = new();
    public static bool AutoKickBreakPlayer { get; set; } = false;
    public static bool AutoKickPing { get; set; } = false;
    public static bool AutoKickWinSwitching { get; set; } = false;
    public static bool AutoCheckBFBAN { get; set; } = false;
    public static bool AutoKickBFBAN { get; set; } = false;
    public static bool AutoSusCheck { get; set; } = false;
    public static bool AutoChat { get; set; } = false;
    public static bool AutoBalancer { get; set; } = false;
    public static int PingLimit { get; set; } = 200;
    public static int TicketLimit { get; set; } = 200;    
    public static int OnlyKickWhenPlayerCountAbove { get; set; } = 50; //0 = off
    public static List<string> Custom_Player_BlackList { get; set; } = new();
    public static List<string> Custom_Player_WhiteList { get; set; } = new();
    public static Ruleset CurrentlyAppliedRuleset { get; set; } = new();
    public static string RuleSetConfigFilePath { get; } = FileUtil.Default_Path + @"\bannedweapons.cfg";    
    public static List<PlayerData> SusPlayers { get; set; } = new();

    #region Services
    // SERVICE ServerDetails
    public static int Service_Details_Refresh_Delay { get; } = 1;
    public static DetailModel ServerDetails { get; set; } = new();
    public static FullServerDetails FullServerDetails { get; set; } = new();
    public static GetServerDetailsBody FullServerDetails2 { get; set; } = new();
    public static string ServerDetails_PersistedGameId { get; set; } = string.Empty;
    public static List<long> ServerDetails_AdminList_PID { get; set; } = new();
    public static List<string> ServerDetails_AdminList_Name { get; set; } = new();
    public static List<long> ServerDetails_VIPList { get; set; } = new();
    public static List<RSPInfo> ServerDetails_Banlist { get; set; } = new();

    // SERVICE ServerLiveInfo
    public static ServerInfo ServerLiveInfo { get; set; }
    public static ServerInfoModel ServerLiveInfo2 { get; set; }

    // SERVICE ServerPlayers
    public static int Service_Players_Delay { get; } = 8;
    public static string CurrentMapName { get; set; } = string.Empty;
    public static string bfbanjsonPrevious { get; set; } = string.Empty;    
    public static List<PlayerData> Playerlist_All { get; set; }
    public static List<PlayerData> Playerlist_Team1 { get; set; }
    public static List<PlayerData> Playerlist_Team2 { get; set; }
    public static List<PlayerData> PlayerDatas_Team1 { get; set; } = new();
    public static List<PlayerData> PlayerDatas_Team2 { get; set; } = new();
    public static StatisticData NexStatisticData_Team1 { get; set; }
    public static StatisticData NexStatisticData_Team2 { get; set; }
    public static double NexTeamKD1 { get; set; }
    public static double NexTeamKD2 { get; set; }

    // SERVICE Ping
    public static int Service_Ping_Delay { get; } = 60;
    public static string gametools_serverinfo_json_previous { get; set; } = string.Empty;
    public static PlayerN[] gametools_playerarray_t1 { get; set; }
    public static PlayerN[] gametools_playerarray_t2 { get; set; }
    #endregion

    // WEBHOOKS
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
    public static DateTime ToolStartDateTime { get; set; }

    // SEXUSBOT     
    public static class SexusBot
    {
        public static bool IsRunning { get; set; } = false;
        public static bool BotInStartupParams { get; set; } = false;
        public static bool LiveSusEnabled { get; set; } = false; //to auto start live functions
    }
    public static bool AutoRun { get; set; } = false;
    public static bool RuleWindow_Ready { get; set; } = false;
    public static string HWID { get; set; } = string.Empty;
    public static bool HWID_Authd { get; set; } = false;
    public static bool SexusBotLiveFunctionsEnabled { get; set; } = false;
    public static bool B { get; set; } = false;
    public static bool C { get; set; } = false;


    #region Unsorted
    public static ServerDetails NexServerDetails { get; set; } = new();
    public static string GameId { get; set; } = string.Empty;    
    public static List<BreakRuleInfo> BreakRuleInfo_PlayerList { get; set; } = new();
    public static List<SpectatorInfo> Server_SpectatorList { get; set; } = new();
    #endregion


    #region adress details
    public static IntPtr NwindowHandle { get; set; }
    public static IntPtr NprocessHandle { get; set; }
    public static int NprocessId { get; set; }
    public static long NpBaseAddress { get; set; }
    public static long NPtrAddress { get; set; }
    #endregion
}
