namespace BF1.ServerAdminTools.Features.Client;

public static class ModeData
{
    public struct ModeName
    {
        public string English;
        public string Chinese;
        public string Mark;
    }

    /// <summary>
    /// 地图模式数据
    /// </summary>
    public readonly static List<ModeName> AllModeInfo = new()
    {
        new() { English="ID_M_GAMEMODE_ZONECONTROL", Chinese="ZC", Mark="ZoneControl0" },
        new() { English="ID_M_GAMEMODE_AIRASSAULT", Chinese="AA", Mark="AirAssault0" },
        new() { English="ID_M_GAMEMODE_TUGOFWAR", Chinese="FL", Mark="TugOfWar0" },
        new() { English="ID_M_GAMEMODE_DOMINATION", Chinese="Dom", Mark="Domination0" },
        new() { English="ID_M_GAMEMODE_BREAKTHROUGH", Chinese="SO", Mark="Breakthrough0" },
        new() { English="ID_M_GAMEMODE_RUSH", Chinese="突袭", Mark="Rush0" },
        new() { English="ID_M_GAMEMODE_TEAMDEATHMATCH", Chinese="TDM", Mark="TeamDeathMatch0" },
        new() { English="ID_M_GAMEMODE_BREAKTHROUGHLARGE", Chinese="OP", Mark="BreakthroughLarge0" },
        new() { English="ID_M_GAMEMODE_POSSESSION", Chinese="WP", Mark="Possession0" },
        new() { English="ID_M_GAMEMODE_CONQUEST", Chinese="CQ", Mark="Conquest0" }
    };
}
