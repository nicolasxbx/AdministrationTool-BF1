namespace BF1.ServerAdminTools.Models;

public class StatisticData //private struct from ScoreView, taken and pasted.
{
    public int MaxPlayerCount;
    public int PlayerCount;
    public int Rank150PlayerCount;

    public int AllKillCount;
    public int AllDeadCount;
}

public class ServerInfo //Moved to Models
{
    public long OffsetTemp;

    public string Name;
    public long GameID;
    public float Time;

    public string GameMode;
    public string MapName;

    public int MaxScore;

    public int Team1Score;
    public int Team2Score;

    public int Team1Kill;
    public int Team2Kill;

    public int Team1Flag;
    public int Team2Flag;

    public string Team1Img;
    public string Team2Img;
}

public class WeaponStats
{
    public string name { get; set; }
    public string imageUrl { get; set; }
    public string star { get; set; }

    public int kills { get; set; }
    public string killsPerMinute { get; set; }

    public int headshots { get; set; }
    public string headshotsVKills { get; set; }

    public int shots { get; set; }
    public int hits { get; set; }
    public string hitsVShots { get; set; }

    public string hitVKills { get; set; }
    public string time { get; set; }
}

public class Map
{
    public string mapPrettyName { get; set; }
    public string mapImage { get; set; }
    public string modePrettyName { get; set; }
}

public class RSPInfo
{
    public int Index { get; set; }
    public string platform { get; set; }
    public string nucleusId { get; set; }
    public string personaId { get; set; }
    public string platformId { get; set; }
    public string displayName { get; set; }
    public string avatar { get; set; }
    public string accountId { get; set; }
}

public struct DataGridSelcContent
{
    public bool IsOK;
    public int TeamId;
    public int Rank;
    public string Name;
    public long PersonaId;
}

public class NexLogKickObject
{
    public DateTime DateTime { get; set; }
    public int Iteration { get; set; }
    public string Name { get; set; }    
    public string Reason { get; set; }
    public bool Fail { get; set; }
    public long PID { get; set; }
}

public class NexLogSwitchObject
{
    public DateTime DateTime { get; set; }
    public int Iteration { get; set; }
    public string Name { get; set; }    
    public string Status { get; set; }
    public string Map { get; set; }
    public long PID { get; set; }
}
