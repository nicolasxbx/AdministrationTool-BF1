using BF1.ServerAdminTools.Models;
using BF1.ServerAdminTools.Models.Score;
using BF1.ServerAdminTools.Windows;
using BF1.ServerAdminTools.Extension;
using BF1.ServerAdminTools.Common.Utils;
using BF1.ServerAdminTools.Common.Helper;
using BF1.ServerAdminTools.Features.API;
using BF1.ServerAdminTools.Features.Core;
using BF1.ServerAdminTools.Features.Data;
using BF1.ServerAdminTools.Features.Utils;
using BF1.ServerAdminTools.NexDiscord;

namespace BF1.ServerAdminTools.Views;


public partial class ScoreView : UserControl
{
    private List<PlayerData> PlayerList_All = new();
    private List<PlayerData> PlayerList_Team0 = new();
    private List<PlayerData> PlayerList_Team1 = new();
    private List<PlayerData> PlayerList_Team2 = new();

    public static List<PlayerData> PlayerDatas_Team1 = new();
    public static List<PlayerData> PlayerDatas_Team2 = new();

    // 正在执行踢人请求的玩家列表，保留指定时间秒数
    private List<BreakRuleInfo> Kicking_PlayerList = new();

    public ServerInfoModel ServerInfoModel { get; set; } = new();
    public PlayerOtherModel PlayerOtherModel { get; set; } = new();

    /// <summary>
    /// 绑定UI队伍1动态数据集合，用于更新DataGrid
    /// </summary>
    public ObservableCollection<PlayerListModel> DataGrid_PlayerList_Team1 { get; set; } = new();
    /// <summary>
    /// 绑定UI队伍2动态数据集合，用于更新DataGrid
    /// </summary>
    public ObservableCollection<PlayerListModel> DataGrid_PlayerList_Team2 { get; set; } = new();

    /// <summary>
    /// 最大玩家数量
    /// </summary>
    private const int MaxPlayer = 74;
    /*
    private struct StatisticData //Moved to "models"
    {
        public int MaxPlayerCount;
        public int PlayerCount;
        public int Rank150PlayerCount;

        public int AllKillCount;
        public int AllDeadCount;
    }*/
    private StatisticData _statisticData_Team1 = new();
    private StatisticData _statisticData_Team2 = new();
    /*
    private struct ServerInfo //Moved to Models
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
    private ServerInfo _serverInfo;
    */
    private ServerInfo _serverInfo = new();

    private struct DataGridSelcContent
    {
        public bool IsOK;
        public int TeamId;
        public int Rank;
        public string Name;
        public long PersonaId;
    }
    private DataGridSelcContent _dataGridSelcContent;

    ///////////////////////////////////////////////////////

    public ScoreView()
    {
        InitializeComponent();
        this.DataContext = this;

        var thread0 = new Thread(UpdateServerInfo)
        {
            IsBackground = true
        };
        thread0.Start();

        var thread1 = new Thread(UpdatePlayerList)
        {
            IsBackground = true
        };
        thread1.Start();
    }

    /// <summary>
    /// 更新服务器信息
    /// </summary>
    private void UpdateServerInfo()
    {
        while (true)
        {
            //////////////////////////////// 服务器数据获取 ////////////////////////////////

            // 服务器名称
            _serverInfo.Name = Memory.ReadString(Memory.GetBaseAddress() + Offsets.ServerName_Offset, Offsets.ServerName, 64);
            _serverInfo.Name = string.IsNullOrEmpty(_serverInfo.Name) ? "Unknown" : _serverInfo.Name;
            // 服务器地图名称
            _serverInfo.MapName = Memory.ReadString(Offsets.OFFSET_CLIENTGAMECONTEXT, Offsets.ServerMapName, 64);
            _serverInfo.MapName = string.IsNullOrEmpty(_serverInfo.MapName) ? "Unknown" : _serverInfo.MapName;
            // 服务器游戏模式
            _serverInfo.GameMode = Memory.ReadString(Memory.GetBaseAddress() + Offsets.ServerID_Offset, Offsets.ServerGameMode, 64);

            // 服务器时间
            _serverInfo.Time = Memory.Read<float>(Memory.GetBaseAddress() + Offsets.ServerTime_Offset, Offsets.ServerTime);

            //////////////////////////////// 服务器数据整理 ////////////////////////////////

            ServerInfoModel.ServerName = _serverInfo.Name;
            ServerInfoModel.ServerGameID = _serverInfo.GameID.ToString();
            ServerInfoModel.ServerMapName = PlayerUtil.GetMapChsName(_serverInfo.MapName);
            Vari.CurrentMapName = ServerInfoModel.ServerMapName;
            ServerInfoModel.ServerMapImg = PlayerUtil.GetMapPrevImage(_serverInfo.MapName);

            _serverInfo.GameMode = PlayerUtil.GetGameMode(_serverInfo.GameMode);
            if (_serverInfo.MapName == "Unknown" || ServerInfoModel.ServerMapName == "Unknown")
                ServerInfoModel.ServerGameMode = "Unknown";
            else
                ServerInfoModel.ServerGameMode = _serverInfo.GameMode;

            PlayerUtil.GetTeamImage(_serverInfo.MapName, out _serverInfo.Team1Img, out _serverInfo.Team2Img);
            ServerInfoModel.Team1Img = _serverInfo.Team1Img;
            ServerInfoModel.Team2Img = _serverInfo.Team2Img;

            ServerInfoModel.ServerTime = PlayerUtil.SecondsToMMSS(_serverInfo.Time);

            // 当服务器模式为征服时，下列数据才有效
            if (_serverInfo.GameMode == "CQ")
            {
                // 比分数据地址
                _serverInfo.OffsetTemp = Memory.Read<long>(Memory.GetBaseAddress() + Offsets.ServerScore_Offset, Offsets.ServerScoreTeam);
                // 最大比分
                _serverInfo.MaxScore = Memory.Read<int>(_serverInfo.OffsetTemp + 0x120);
                // 队伍1、队伍2分数
                _serverInfo.Team1Score = Memory.Read<int>(_serverInfo.OffsetTemp + 0xE8);
                _serverInfo.Team2Score = Memory.Read<int>(_serverInfo.OffsetTemp + 0x118);
                // 队伍1、队伍2从击杀获取得分
                _serverInfo.Team1Kill = Memory.Read<int>(_serverInfo.OffsetTemp + 0x230);
                _serverInfo.Team2Kill = Memory.Read<int>(_serverInfo.OffsetTemp + 0x238);
                // 队伍1、队伍2从旗帜获取得分
                _serverInfo.Team1Flag = Memory.Read<int>(_serverInfo.OffsetTemp + 0x250);
                _serverInfo.Team2Flag = Memory.Read<int>(_serverInfo.OffsetTemp + 0x258);

                //////////////////////////////// 修正服务器得分数据 ////////////////////////////////

                _serverInfo.Team1Score = PlayerUtil.FixedServerScore(_serverInfo.Team1Score);
                _serverInfo.Team2Score = PlayerUtil.FixedServerScore(_serverInfo.Team2Score);

                if (_serverInfo.MaxScore != 0)
                {
                    var scale = _serverInfo.MaxScore / 1000.0f;
                    ServerInfoModel.Team1ScoreWidth = PlayerUtil.FixedServerScore(_serverInfo.Team1Score / (8 * scale));
                    ServerInfoModel.Team2ScoreWidth = PlayerUtil.FixedServerScore(_serverInfo.Team2Score / (8 * scale));
                }
                else
                {
                    ServerInfoModel.Team1ScoreWidth = 0;
                    ServerInfoModel.Team2ScoreWidth = 0;
                }

                ServerInfoModel.Team1Score = _serverInfo.Team1Score.ToString();
                ServerInfoModel.Team2Score = _serverInfo.Team2Score.ToString();

                ServerInfoModel.Team1Flag = PlayerUtil.FixedServerScore(_serverInfo.Team1Flag);
                ServerInfoModel.Team1Kill = PlayerUtil.FixedServerScore(_serverInfo.Team1Kill);

                ServerInfoModel.Team2Flag = PlayerUtil.FixedServerScore(_serverInfo.Team2Flag);
                ServerInfoModel.Team2Kill = PlayerUtil.FixedServerScore(_serverInfo.Team2Kill);
            }

            ////////////////////////////////////////////////////////////////////////////////

            // 如果玩家没有进入服务器，要进行一些数据清理
            if (ServerInfoModel.ServerMapName == "Server Map Name")
            {
                // 清理服务器ID（GameID）
                _serverInfo.GameID = 0;
                Vari.GameId = string.Empty;

                Vari.Server_AdminList_PID.Clear();
                Vari.Server_AdminList_Name.Clear();
                Vari.Server_VIPList.Clear();
            }
            else
            {
                // 服务器数字ID
                _serverInfo.GameID = Memory.Read<long>(Memory.GetBaseAddress() + Offsets.ServerID_Offset, Offsets.ServerGameID);
                Vari.GameId = _serverInfo.GameID.ToString();
            }

            ////////////////////////////////////////////////////////////////////////////////

            Vari.NexServerinfo = _serverInfo;

            Thread.Sleep(1000);
        }
    }

    /// <summary>
    /// 更新玩家列表
    /// </summary>
    private void UpdatePlayerList()
    {
        while (true)
        {
            //////////////////////////////// 数据初始化 ////////////////////////////////

            PlayerList_All.Clear();
            PlayerList_Team0.Clear();
            PlayerList_Team1.Clear();
            PlayerList_Team2.Clear();

            Vari.Server_SpectatorList.Clear();

            var _weaponSlot = new string[8] { "", "", "", "", "", "", "", "" };

            _statisticData_Team1.MaxPlayerCount = 0;
            _statisticData_Team1.PlayerCount = 0;
            _statisticData_Team1.Rank150PlayerCount = 0;
            _statisticData_Team1.AllKillCount = 0;
            _statisticData_Team1.AllDeadCount = 0;

            _statisticData_Team2.MaxPlayerCount = 0;
            _statisticData_Team2.PlayerCount = 0;
            _statisticData_Team2.Rank150PlayerCount = 0;
            _statisticData_Team2.AllKillCount = 0;
            _statisticData_Team2.AllDeadCount = 0;

            Vari.BreakRuleInfo_PlayerList.Clear();

            //////////////////////////////// 自己数据 ////////////////////////////////

            var _myBaseAddress = Player.GetLocalPlayer();

            var _myTeamId = Memory.Read<int>(_myBaseAddress + 0x1C34);
            PlayerOtherModel.MySelfTeamID = $"Team ID : {_myTeamId}";

            var _myPlayerName = Memory.ReadString(_myBaseAddress + 0x2156, 64);
            PlayerOtherModel.MySelfName = string.IsNullOrEmpty(_myPlayerName) ? "Player ID : Unknown" : $"Player ID : {_myPlayerName}";

            //////////////////////////////// 玩家数据 ////////////////////////////////

            for (int i = 0; i < MaxPlayer; i++)
            {
                var _baseAddress = Player.GetPlayerById(i);
                if (!Memory.IsValid(_baseAddress))
                    continue;

                var _mark = Memory.Read<byte>(_baseAddress + 0x1D7C);
                var _teamId = Memory.Read<int>(_baseAddress + 0x1C34);
                var _spectator = Memory.Read<byte>(_baseAddress + 0x1C31);
                var _personaId = Memory.Read<long>(_baseAddress + 0x38);
                var _squadId = Memory.Read<int>(_baseAddress + 0x1E50);
                var _name = Memory.ReadString(_baseAddress + 0x2156, 64);
                if (string.IsNullOrEmpty(_name))
                    continue;

                var _pClientVehicleEntity = Memory.Read<long>(_baseAddress + 0x1D38);
                if (Memory.IsValid(_pClientVehicleEntity))
                {
                    var _pVehicleEntityData = Memory.Read<long>(_pClientVehicleEntity + 0x30);
                    _weaponSlot[0] = Memory.ReadString(_pVehicleEntityData + 0x2F8, new int[] { 0x00 }, 64);

                    for (int j = 1; j < 8; j++)
                    {
                        _weaponSlot[j] = "";
                    }
                }
                else
                {
                    var _pClientSoldierEntity = Memory.Read<long>(_baseAddress + 0x1D48);
                    var _pClientSoldierWeaponComponent = Memory.Read<long>(_pClientSoldierEntity + 0x698);
                    var _m_handler = Memory.Read<long>(_pClientSoldierWeaponComponent + 0x8A8);

                    for (int j = 0; j < 8; j++)
                    {
                        var offset0 = Memory.Read<long>(_m_handler + j * 0x8);

                        offset0 = Memory.Read<long>(offset0 + 0x4A30);
                        offset0 = Memory.Read<long>(offset0 + 0x20);
                        offset0 = Memory.Read<long>(offset0 + 0x38);
                        offset0 = Memory.Read<long>(offset0 + 0x20);

                        _weaponSlot[j] = Memory.ReadString(offset0, 64);
                    }
                }

                var index = PlayerList_All.FindIndex(val => val.Name == _name);
                if (index == -1)
                {
                    PlayerList_All.Add(new PlayerData()
                    {
                        Mark = _mark,
                        TeamId = _teamId,
                        Spectator = _spectator,
                        Clan = PlayerUtil.GetPlayerTargetName(_name, true),
                        Name = PlayerUtil.GetPlayerTargetName(_name, false),
                        PersonaId = _personaId,
                        SquadId = PlayerUtil.GetSquadChsName(_squadId),

                        Rank = 0,
                        Kills = 0,
                        Deaths = 0,
                        Score = 0,

                        KD = 0,
                        KPM = 0,

                        WeaponS0 = _weaponSlot[0],
                        WeaponS1 = _weaponSlot[1],
                        WeaponS2 = _weaponSlot[2],
                        WeaponS3 = _weaponSlot[3],
                        WeaponS4 = _weaponSlot[4],
                        WeaponS5 = _weaponSlot[5],
                        WeaponS6 = _weaponSlot[6],
                        WeaponS7 = _weaponSlot[7],
                    });
                }
            }

            //////////////////////////////// 得分板数据 ////////////////////////////////

            var _pClientScoreBA = Memory.Read<long>(Memory.GetBaseAddress() + 0x39EB8D8);
            _pClientScoreBA = Memory.Read<long>(_pClientScoreBA + 0x68);

            for (int i = 0; i < MaxPlayer; i++)
            {
                _pClientScoreBA = Memory.Read<long>(_pClientScoreBA);
                var _pClientScoreOffset = Memory.Read<long>(_pClientScoreBA + 0x10);
                if (!Memory.IsValid(_pClientScoreBA))
                    continue;

                var _mark = Memory.Read<byte>(_pClientScoreOffset + 0x300);
                var _rank = Memory.Read<int>(_pClientScoreOffset + 0x304);
                if (_rank == 0)
                    continue;
                var _kill = Memory.Read<int>(_pClientScoreOffset + 0x308);
                var _dead = Memory.Read<int>(_pClientScoreOffset + 0x30C);
                var _score = Memory.Read<int>(_pClientScoreOffset + 0x314);

                var index = PlayerList_All.FindIndex(val => val.Mark == _mark);
                if (index != -1)
                {
                    PlayerList_All[index].Rank = _rank;
                    PlayerList_All[index].Kills = _kill;
                    PlayerList_All[index].Deaths = _dead;
                    PlayerList_All[index].Score = _score;
                    PlayerList_All[index].KD = PlayerUtil.GetPlayerKD(_kill, _dead);
                    PlayerList_All[index].KPM = PlayerUtil.GetPlayerKPM(_kill, PlayerUtil.SecondsToMinute(_serverInfo.Time));
                }
            }

            //////////////////////////////// 队伍数据整理 ////////////////////////////////

            //Original
            foreach (var item in PlayerList_All)
            {
                item.Admin = PlayerUtil.CheckAdminVIP(item.PersonaId, Vari.Server_AdminList_PID);
                item.VIP = PlayerUtil.CheckAdminVIP(item.PersonaId, Vari.Server_VIPList);

                switch (item.TeamId)
                {
                    case 0:
                        PlayerList_Team0.Add(item);
                        break;
                    case 1:
                        PlayerList_Team1.Add(item);
                        // 检查队伍1违规玩家
                        CheckTeam1PlayerIsBreakRule(item);
                        break;
                    case 2:
                        PlayerList_Team2.Add(item);
                        // 检查队伍2违规玩家
                        CheckTeam2PlayerIsBreakRule(item);
                        break;
                }
            }
            /* //Failed try
            foreach (var item in PlayerList_All) //tna, got rid of CheckTeamBreakRule-Functions, they are now seperate at the end.
            {
                item.Admin = PlayerUtil.CheckAdminVIP(item.PersonaId, Globals.Server_AdminList_PID);
                item.VIP = PlayerUtil.CheckAdminVIP(item.PersonaId, Globals.Server_VIPList);

                switch (item.TeamId)
                {
                    case 0:
                        PlayerList_Team0.Add(item);
                        break;
                    case 1:
                        PlayerList_Team1.Add(item);
                        // 检查队伍1违规玩家
                        //CheckTeam1PlayerIsBreakRule(item); //nex
                        break;
                    case 2:
                        PlayerList_Team2.Add(item);
                        // 检查队伍2违规玩家
                        //CheckTeam2PlayerIsBreakRule(item); //tna
                        break;
                }
            }*/



            // 观战玩家信息
            foreach (var item in PlayerList_Team0)
            {
                Vari.Server_SpectatorList.Add(new SpectatorInfo()
                {
                    Name = item.Name,
                    PersonaId = item.PersonaId,
                });
            }

            // 队伍1数据统计
            foreach (var item in PlayerList_Team1)
            {
                // 统计当前服务器玩家数量
                if (item.Rank != 0)
                {
                    _statisticData_Team1.MaxPlayerCount++;
                }

                // 统计当前服务器存活玩家数量
                if (item.WeaponS0 != "" ||
                    item.WeaponS1 != "" ||
                    item.WeaponS2 != "" ||
                    item.WeaponS3 != "" ||
                    item.WeaponS4 != "" ||
                    item.WeaponS5 != "" ||
                    item.WeaponS6 != "" ||
                    item.WeaponS7 != "")
                {
                    _statisticData_Team1.PlayerCount++;
                }

                // 统计当前服务器150级玩家数量
                if (item.Rank == 150)
                {
                    _statisticData_Team1.Rank150PlayerCount++;
                }

                // 总击杀数统计
                _statisticData_Team1.AllKillCount += item.Kills;
                // 总死亡数统计
                _statisticData_Team1.AllDeadCount += item.Deaths;
            }

            // 队伍2数据统计
            foreach (var item in PlayerList_Team2)
            {
                // 统计当前服务器玩家数量
                if (item.Rank != 0)
                {
                    _statisticData_Team2.MaxPlayerCount++;
                }

                // 统计当前服务器存活玩家数量
                if (item.WeaponS0 != "" ||
                    item.WeaponS1 != "" ||
                    item.WeaponS2 != "" ||
                    item.WeaponS3 != "" ||
                    item.WeaponS4 != "" ||
                    item.WeaponS5 != "" ||
                    item.WeaponS6 != "" ||
                    item.WeaponS7 != "")
                {
                    _statisticData_Team2.PlayerCount++;
                }

                // 统计当前服务器150级玩家数量
                if (item.Rank == 150)
                {
                    _statisticData_Team2.Rank150PlayerCount++;
                }

                // 总击杀数统计
                _statisticData_Team2.AllKillCount += item.Kills;
                // 总死亡数统计
                _statisticData_Team2.AllDeadCount += item.Deaths;
            }

            // 是否显示中文武器名称
            if (Vari.IsShowCHSWeaponName)
            {
                for (int i = 0; i < PlayerList_Team1.Count; i++)
                {
                    PlayerList_Team1[i].WeaponS0 = PlayerUtil.GetWeaponChsName(PlayerList_Team1[i].WeaponS0);
                    PlayerList_Team1[i].WeaponS1 = PlayerUtil.GetWeaponChsName(PlayerList_Team1[i].WeaponS1);
                    PlayerList_Team1[i].WeaponS2 = PlayerUtil.GetWeaponChsName(PlayerList_Team1[i].WeaponS2);
                    PlayerList_Team1[i].WeaponS3 = PlayerUtil.GetWeaponChsName(PlayerList_Team1[i].WeaponS3);
                    PlayerList_Team1[i].WeaponS4 = PlayerUtil.GetWeaponChsName(PlayerList_Team1[i].WeaponS4);
                    PlayerList_Team1[i].WeaponS5 = PlayerUtil.GetWeaponChsName(PlayerList_Team1[i].WeaponS5);
                    PlayerList_Team1[i].WeaponS6 = PlayerUtil.GetWeaponChsName(PlayerList_Team1[i].WeaponS6);
                    PlayerList_Team1[i].WeaponS7 = PlayerUtil.GetWeaponChsName(PlayerList_Team1[i].WeaponS7);
                }

                for (int i = 0; i < PlayerList_Team2.Count; i++)
                {
                    PlayerList_Team2[i].WeaponS0 = PlayerUtil.GetWeaponChsName(PlayerList_Team2[i].WeaponS0);
                    PlayerList_Team2[i].WeaponS1 = PlayerUtil.GetWeaponChsName(PlayerList_Team2[i].WeaponS1);
                    PlayerList_Team2[i].WeaponS2 = PlayerUtil.GetWeaponChsName(PlayerList_Team2[i].WeaponS2);
                    PlayerList_Team2[i].WeaponS3 = PlayerUtil.GetWeaponChsName(PlayerList_Team2[i].WeaponS3);
                    PlayerList_Team2[i].WeaponS4 = PlayerUtil.GetWeaponChsName(PlayerList_Team2[i].WeaponS4);
                    PlayerList_Team2[i].WeaponS5 = PlayerUtil.GetWeaponChsName(PlayerList_Team2[i].WeaponS5);
                    PlayerList_Team2[i].WeaponS6 = PlayerUtil.GetWeaponChsName(PlayerList_Team2[i].WeaponS6);
                    PlayerList_Team2[i].WeaponS7 = PlayerUtil.GetWeaponChsName(PlayerList_Team2[i].WeaponS7);
                }
            }
            Vari.NexStatisticData_Team1 = _statisticData_Team1;
            Vari.NexStatisticData_Team2 = _statisticData_Team2;


            //////////////////////////////// 统计信息数据 ////////////////////////////////
            Vari.NexTeamKD1 = Math.Round(Convert.ToDouble(_statisticData_Team1.AllKillCount) / Convert.ToDouble(_statisticData_Team1.AllDeadCount), 2);
            Vari.NexTeamKD2 = Math.Round(Convert.ToDouble(_statisticData_Team2.AllKillCount) / Convert.ToDouble(_statisticData_Team2.AllDeadCount), 2);
            //KD's added by TNA

            ServerInfoModel.Team1Info = $" Spawned / Playercount : {_statisticData_Team1.PlayerCount} / {_statisticData_Team1.MaxPlayerCount}  |  Lvl150 : {_statisticData_Team1.Rank150PlayerCount}  |  Kills : {_statisticData_Team1.AllKillCount}  |  Deaths : {_statisticData_Team1.AllDeadCount} | KD : {Vari.NexTeamKD1}";
            ServerInfoModel.Team2Info = $" Spawned / Playercount : {_statisticData_Team2.PlayerCount} / {_statisticData_Team2.MaxPlayerCount}  |  Lvl150 : {_statisticData_Team2.Rank150PlayerCount}  |  Kills : {_statisticData_Team2.AllKillCount}  |  Deaths : {_statisticData_Team2.AllDeadCount} | KD : {Vari.NexTeamKD2}";

            PlayerOtherModel.ServerPlayerCountInfo = $"ServerPlayerCount : {_statisticData_Team1.MaxPlayerCount + _statisticData_Team2.MaxPlayerCount}";

            ////////////////////////////////////////////////////////////////////////////////

            this.Dispatcher.BeginInvoke(() =>
            {
                UpdateDataGridTeam1();
                DataGrid_PlayerList_Team1.Sort();
            });

            this.Dispatcher.BeginInvoke(() =>
            {
                UpdateDataGridTeam2();
                DataGrid_PlayerList_Team2.Sort();
            });

            ////////////////////////////////////////////////////////////////////////////////

            // 自动踢出违规玩家
            AutoKickBreakPlayer();

            ////////////////////////////////////////////////////////////////////////////////

            // 检测换边玩家
            CheckPlayerChangeTeam();

            ////////////////////////////////////////////////////////////////////////////////

            /* //failed try
            if (PlayerList_All.Equals(Globals.NexPlayerList_AllPrevious) || Globals.NexPlayerList_AllPrevious == null) //nex
            {
                foreach (var item in PlayerList_All)
                {
                    //item.Admin = PlayerUtil.CheckAdminVIP(item.PersonaId, Globals.Server_AdminList_PID);
                    //item.VIP = PlayerUtil.CheckAdminVIP(item.PersonaId, Globals.Server_VIPList);

                    switch (item.TeamId)
                    {
                        case 0:
                            //PlayerList_Team0.Add(item);
                            break;
                        case 1:
                            //PlayerList_Team1.Add(item);
                            // 检查队伍1违规玩家
                            CheckTeam1PlayerIsBreakRule(item);
                            break;
                        case 2:
                            //PlayerList_Team2.Add(item);
                            // 检查队伍2违规玩家
                            CheckTeam2PlayerIsBreakRule(item);
                            break;
                    }
                    //NotifierHelper.Show(NotifierType.None, "NEW");
                    LoggerHelper.Info($"NEW");
                }
            }            
            else
            {
                LoggerHelper.Info($"OLD");
            }
            Globals.NexPlayerList_AllPrevious = PlayerList_All;*/

            Vari.Playerlist_All = PlayerList_All;
            Vari.Playerlist_Team1 = PlayerDatas_Team1;
            Vari.Playerlist_Team2 = PlayerList_Team2;
            
            Thread.Sleep(8000); //was 2000
        }
    }

    /// <summary>
    /// 动态更新 DataGrid 队伍1
    /// </summary>
    private void UpdateDataGridTeam1()
    {
        if (PlayerList_Team1.Count == 0 && DataGrid_PlayerList_Team1.Count != 0)
        {
            DataGrid_PlayerList_Team1.Clear();
        }

        if (PlayerList_Team1.Count != 0)
        {
            // 更新DataGrid中现有的玩家数据，并把DataGrid中已经不在服务器的玩家清除
            for (int i = 0; i < DataGrid_PlayerList_Team1.Count; i++)
            {
                int index = PlayerList_Team1.FindIndex(val => val.Name == DataGrid_PlayerList_Team1[i].Name);
                if (index != -1)
                {
                    DataGrid_PlayerList_Team1[i].Rank = PlayerList_Team1[index].Rank;
                    DataGrid_PlayerList_Team1[i].Clan = PlayerList_Team1[index].Clan;
                    DataGrid_PlayerList_Team1[i].Admin = PlayerList_Team1[index].Admin;
                    DataGrid_PlayerList_Team1[i].VIP = PlayerList_Team1[index].VIP;
                    DataGrid_PlayerList_Team1[i].SquadId = PlayerList_Team1[index].SquadId;
                    DataGrid_PlayerList_Team1[i].Kill = PlayerList_Team1[index].Kills;
                    DataGrid_PlayerList_Team1[i].Dead = PlayerList_Team1[index].Deaths;
                    DataGrid_PlayerList_Team1[i].KD = PlayerList_Team1[index].KD.ToString("0.00");
                    DataGrid_PlayerList_Team1[i].KPM = PlayerList_Team1[index].KPM.ToString("0.00");
                    DataGrid_PlayerList_Team1[i].Score = PlayerList_Team1[index].Score;
                    DataGrid_PlayerList_Team1[i].WeaponS0 = PlayerList_Team1[index].WeaponS0;
                    DataGrid_PlayerList_Team1[i].WeaponS1 = PlayerList_Team1[index].WeaponS1;
                    DataGrid_PlayerList_Team1[i].WeaponS2 = PlayerList_Team1[index].WeaponS2;
                    DataGrid_PlayerList_Team1[i].WeaponS3 = PlayerList_Team1[index].WeaponS3;
                    DataGrid_PlayerList_Team1[i].WeaponS4 = PlayerList_Team1[index].WeaponS4;
                    DataGrid_PlayerList_Team1[i].WeaponS5 = PlayerList_Team1[index].WeaponS5;
                    DataGrid_PlayerList_Team1[i].WeaponS6 = PlayerList_Team1[index].WeaponS6;
                    DataGrid_PlayerList_Team1[i].WeaponS7 = PlayerList_Team1[index].WeaponS7;
                }
                else
                {
                    DataGrid_PlayerList_Team1.RemoveAt(i);
                }
            }

            // 增加DataGrid没有的玩家数据
            for (int i = 0; i < PlayerList_Team1.Count; i++)
            {
                int index = DataGrid_PlayerList_Team1.ToList().FindIndex(val => val.Name == PlayerList_Team1[i].Name);
                if (index == -1)
                {
                    DataGrid_PlayerList_Team1.Add(new PlayerListModel()
                    {
                        Rank = PlayerList_Team1[i].Rank,
                        Clan = PlayerList_Team1[i].Clan,
                        Name = PlayerList_Team1[i].Name,
                        PersonaId = PlayerList_Team1[i].PersonaId,
                        Admin = PlayerList_Team1[i].Admin,
                        VIP = PlayerList_Team1[i].VIP,
                        SquadId = PlayerList_Team1[i].SquadId,
                        Kill = PlayerList_Team1[i].Kills,
                        Dead = PlayerList_Team1[i].Deaths,
                        KD = PlayerList_Team1[i].KD.ToString("0.00"),
                        KPM = PlayerList_Team1[i].KPM.ToString("0.00"),
                        Score = PlayerList_Team1[i].Score,
                        WeaponS0 = PlayerList_Team1[i].WeaponS0,
                        WeaponS1 = PlayerList_Team1[i].WeaponS1,
                        WeaponS2 = PlayerList_Team1[i].WeaponS2,
                        WeaponS3 = PlayerList_Team1[i].WeaponS3,
                        WeaponS4 = PlayerList_Team1[i].WeaponS4,
                        WeaponS5 = PlayerList_Team1[i].WeaponS5,
                        WeaponS6 = PlayerList_Team1[i].WeaponS6,
                        WeaponS7 = PlayerList_Team1[i].WeaponS7
                    });

                }
            }

            // 修正序号
            for (int i = 0; i < DataGrid_PlayerList_Team1.Count; i++)
            {
                DataGrid_PlayerList_Team1[i].Index = i + 1;
            }
        }
    }

    /// <summary>
    /// 动态更新 DataGrid 队伍2
    /// </summary>
    private void UpdateDataGridTeam2()
    {
        if (PlayerList_Team2.Count == 0 && DataGrid_PlayerList_Team2.Count != 0)
        {
            DataGrid_PlayerList_Team2.Clear();
        }

        if (PlayerList_Team2.Count != 0)
        {
            // 更新DataGrid中现有的玩家数据，并把DataGrid中已经不在服务器的玩家清除
            for (int i = 0; i < DataGrid_PlayerList_Team2.Count; i++)
            {
                int index = PlayerList_Team2.FindIndex(val => val.Name == DataGrid_PlayerList_Team2[i].Name);
                if (index != -1)
                {
                    DataGrid_PlayerList_Team2[i].Rank = PlayerList_Team2[index].Rank;
                    DataGrid_PlayerList_Team2[i].Clan = PlayerList_Team2[index].Clan;
                    DataGrid_PlayerList_Team2[i].Admin = PlayerList_Team2[index].Admin;
                    DataGrid_PlayerList_Team2[i].VIP = PlayerList_Team2[index].VIP;
                    DataGrid_PlayerList_Team2[i].SquadId = PlayerList_Team2[index].SquadId;
                    DataGrid_PlayerList_Team2[i].Kill = PlayerList_Team2[index].Kills;
                    DataGrid_PlayerList_Team2[i].Dead = PlayerList_Team2[index].Deaths;
                    DataGrid_PlayerList_Team2[i].KD = PlayerList_Team2[index].KD.ToString("0.00");
                    DataGrid_PlayerList_Team2[i].KPM = PlayerList_Team2[index].KPM.ToString("0.00");
                    DataGrid_PlayerList_Team2[i].Score = PlayerList_Team2[index].Score;
                    DataGrid_PlayerList_Team2[i].WeaponS0 = PlayerList_Team2[index].WeaponS0;
                    DataGrid_PlayerList_Team2[i].WeaponS1 = PlayerList_Team2[index].WeaponS1;
                    DataGrid_PlayerList_Team2[i].WeaponS2 = PlayerList_Team2[index].WeaponS2;
                    DataGrid_PlayerList_Team2[i].WeaponS3 = PlayerList_Team2[index].WeaponS3;
                    DataGrid_PlayerList_Team2[i].WeaponS4 = PlayerList_Team2[index].WeaponS4;
                    DataGrid_PlayerList_Team2[i].WeaponS5 = PlayerList_Team2[index].WeaponS5;
                    DataGrid_PlayerList_Team2[i].WeaponS6 = PlayerList_Team2[index].WeaponS6;
                    DataGrid_PlayerList_Team2[i].WeaponS7 = PlayerList_Team2[index].WeaponS7;
                }
                else
                {
                    DataGrid_PlayerList_Team2.RemoveAt(i);
                }
            }

            // 增加DataGrid没有的玩家数据
            for (int i = 0; i < PlayerList_Team2.Count; i++)
            {
                int index = DataGrid_PlayerList_Team2.ToList().FindIndex(val => val.Name == PlayerList_Team2[i].Name);
                if (index == -1)
                {
                    DataGrid_PlayerList_Team2.Add(new PlayerListModel()
                    {
                        Rank = PlayerList_Team2[i].Rank,
                        Clan = PlayerList_Team2[i].Clan,
                        Name = PlayerList_Team2[i].Name,
                        PersonaId = PlayerList_Team2[i].PersonaId,
                        Admin = PlayerList_Team2[i].Admin,
                        VIP = PlayerList_Team2[i].VIP,
                        SquadId = PlayerList_Team2[i].SquadId,
                        Kill = PlayerList_Team2[i].Kills,
                        Dead = PlayerList_Team2[i].Deaths,
                        KD = PlayerList_Team2[i].KD.ToString("0.00"),
                        KPM = PlayerList_Team2[i].KPM.ToString("0.00"),
                        Score = PlayerList_Team2[i].Score,
                        WeaponS0 = PlayerList_Team2[i].WeaponS0,
                        WeaponS1 = PlayerList_Team2[i].WeaponS1,
                        WeaponS2 = PlayerList_Team2[i].WeaponS2,
                        WeaponS3 = PlayerList_Team2[i].WeaponS3,
                        WeaponS4 = PlayerList_Team2[i].WeaponS4,
                        WeaponS5 = PlayerList_Team2[i].WeaponS5,
                        WeaponS6 = PlayerList_Team2[i].WeaponS6,
                        WeaponS7 = PlayerList_Team2[i].WeaponS7
                    });
                }
            }

            // 修正序号
            for (int i = 0; i < DataGrid_PlayerList_Team2.Count; i++)
            {
                DataGrid_PlayerList_Team2[i].Index = i + 1;
            }
        }
    }

    #region 检查违规
    /// <summary>
    /// 检查队伍1玩家是否违规
    /// </summary>
    /// <param name="playerData"></param>
    private void CheckTeam1PlayerIsBreakRule(PlayerData playerData)
    {
        int index = Vari.BreakRuleInfo_PlayerList.FindIndex(val => val.PersonaId == playerData.PersonaId);
        if (index == -1)
        {
            // 限制玩家击杀
            if (playerData.Kills > ServerRule.Team1.MaxKill && ServerRule.Team1.MaxKill != 0)
            {
                Vari.BreakRuleInfo_PlayerList.Add(new BreakRuleInfo
                {
                    Name = playerData.Name,
                    PersonaId = playerData.PersonaId,
                    Reason = $"Kill Limit {ServerRule.Team1.MaxKill:0}"
                });

                return;
            }

            // 计算玩家KD最低击杀数
            if (playerData.Kills > ServerRule.Team1.KDFlag && ServerRule.Team1.KDFlag != 0)
            {
                // 限制玩家KD
                if (playerData.KD > ServerRule.Team1.MaxKD && ServerRule.Team1.MaxKD != 0.00f)
                {
                    Vari.BreakRuleInfo_PlayerList.Add(new BreakRuleInfo
                    {
                        Name = playerData.Name,
                        PersonaId = playerData.PersonaId,
                        Reason = $"KD Limit {ServerRule.Team1.MaxKD:0.00}"
                    });
                }

                return;
            }

            // 计算玩家KPM比条件
            if (playerData.Kills > ServerRule.Team1.KPMFlag && ServerRule.Team1.KPMFlag != 0)
            {
                // 限制玩家KPM
                if (playerData.KPM > ServerRule.Team1.MaxKPM && ServerRule.Team1.MaxKPM != 0.00f)
                {
                    Vari.BreakRuleInfo_PlayerList.Add(new BreakRuleInfo
                    {
                        Name = playerData.Name,
                        PersonaId = playerData.PersonaId,
                        Reason = $"KPM Limit {ServerRule.Team1.MaxKPM:0.00}"
                    });
                }

                return;
            }

            // 限制玩家最低等级
            if (playerData.Rank < ServerRule.Team1.MinRank && ServerRule.Team1.MinRank != 0 && playerData.Rank != 0)
            {
                Vari.BreakRuleInfo_PlayerList.Add(new BreakRuleInfo
                {
                    Name = playerData.Name,
                    PersonaId = playerData.PersonaId,
                    Reason = $"Min Rank Limit {ServerRule.Team1.MinRank:0}"
                });

                return;
            }

            // 限制玩家最高等级
            if (playerData.Rank > ServerRule.Team1.MaxRank && ServerRule.Team1.MaxRank != 0 && playerData.Rank != 0)
            {
                Vari.BreakRuleInfo_PlayerList.Add(new BreakRuleInfo
                {
                    Name = playerData.Name,
                    PersonaId = playerData.PersonaId,
                    Reason = $"Max Rank Limit {ServerRule.Team1.MaxRank:0}"
                });

                return;
            }

            // 从武器规则里遍历限制武器名称
            for (int i = 0; i < Vari.Custom_WeaponList_Team1.Count; i++)
            {
                var item = Vari.Custom_WeaponList_Team1[i];

                // K 弹
                if (item == "_KBullet")
                {
                    if (playerData.WeaponS0.Contains("_KBullet") ||
                        playerData.WeaponS1.Contains("_KBullet") ||
                        playerData.WeaponS2.Contains("_KBullet") ||
                        playerData.WeaponS3.Contains("_KBullet") ||
                        playerData.WeaponS4.Contains("_KBullet") ||
                        playerData.WeaponS5.Contains("_KBullet") ||
                        playerData.WeaponS6.Contains("_KBullet") ||
                        playerData.WeaponS7.Contains("_KBullet"))
                    {
                        Vari.BreakRuleInfo_PlayerList.Add(new BreakRuleInfo
                        {
                            Name = playerData.Name,
                            PersonaId = playerData.PersonaId,
                            Reason = $"Read Rules, No 'K Bullet'"
                        });

                        return;
                    }
                }

                // 步枪手榴弹（破片）
                if (item == "_RGL_Frag")
                {
                    if (playerData.WeaponS0.Contains("_RGL_Frag") ||
                        playerData.WeaponS1.Contains("_RGL_Frag") ||
                        playerData.WeaponS2.Contains("_RGL_Frag") ||
                        playerData.WeaponS3.Contains("_RGL_Frag") ||
                        playerData.WeaponS4.Contains("_RGL_Frag") ||
                        playerData.WeaponS5.Contains("_RGL_Frag") ||
                        playerData.WeaponS6.Contains("_RGL_Frag") ||
                        playerData.WeaponS7.Contains("_RGL_Frag"))
                    {
                        Vari.BreakRuleInfo_PlayerList.Add(new BreakRuleInfo
                        {
                            Name = playerData.Name,
                            PersonaId = playerData.PersonaId,
                            Reason = "No 'Rifle Grenade Frag'"
                        });

                        return;
                    }
                }

                // 步枪手榴弹（烟雾）
                if (item == "_RGL_Smoke")
                {
                    if (playerData.WeaponS0.Contains("_RGL_Smoke") ||
                        playerData.WeaponS1.Contains("_RGL_Smoke") ||
                        playerData.WeaponS2.Contains("_RGL_Smoke") ||
                        playerData.WeaponS3.Contains("_RGL_Smoke") ||
                        playerData.WeaponS4.Contains("_RGL_Smoke") ||
                        playerData.WeaponS5.Contains("_RGL_Smoke") ||
                        playerData.WeaponS6.Contains("_RGL_Smoke") ||
                        playerData.WeaponS7.Contains("_RGL_Smoke"))
                    {
                        Vari.BreakRuleInfo_PlayerList.Add(new BreakRuleInfo
                        {
                            Name = playerData.Name,
                            PersonaId = playerData.PersonaId,
                            Reason = "No 'Rifle Grenade Smoke'"
                        });

                        return;
                    }
                }

                // 步枪手榴弹（高爆）
                if (item == "_RGL_HE")
                {
                    if (playerData.WeaponS0.Contains("_RGL_HE") ||
                        playerData.WeaponS1.Contains("_RGL_HE") ||
                        playerData.WeaponS2.Contains("_RGL_HE") ||
                        playerData.WeaponS3.Contains("_RGL_HE") ||
                        playerData.WeaponS4.Contains("_RGL_HE") ||
                        playerData.WeaponS5.Contains("_RGL_HE") ||
                        playerData.WeaponS6.Contains("_RGL_HE") ||
                        playerData.WeaponS7.Contains("_RGL_HE"))
                    {
                        Vari.BreakRuleInfo_PlayerList.Add(new BreakRuleInfo
                        {
                            Name = playerData.Name,
                            PersonaId = playerData.PersonaId,
                            Reason = "No 'Rifle Grenade HE'"
                        });

                        return;
                    }
                }

                if (playerData.WeaponS0 == item ||
                    playerData.WeaponS1 == item ||
                    playerData.WeaponS2 == item ||
                    playerData.WeaponS3 == item ||
                    playerData.WeaponS4 == item ||
                    playerData.WeaponS5 == item ||
                    playerData.WeaponS6 == item ||
                    playerData.WeaponS7 == item)
                {
                    Vari.BreakRuleInfo_PlayerList.Add(new BreakRuleInfo
                    {
                        Name = playerData.Name,
                        PersonaId = playerData.PersonaId,
                        Reason = $"Read Rules, No '{PlayerUtil.GetWeaponShortTxt(item)}'"
                    });

                    return;
                }
            }

            // 黑名单
            for (int i = 0; i < Vari.Custom_BlackList.Count; i++)
            {
                var item = Vari.Custom_BlackList[i];
                if (playerData.Name == item)
                {
                    Vari.BreakRuleInfo_PlayerList.Add(new BreakRuleInfo
                    {
                        Name = playerData.Name,
                        PersonaId = playerData.PersonaId,
                        Reason = "Server Black List"
                    });

                    return;
                }
            }
        }
    }

    /// <summary>
    /// 检查队伍2玩家是否违规
    /// </summary>
    /// <param name="playerData"></param>
    private void CheckTeam2PlayerIsBreakRule(PlayerData playerData)
    {
        int index = Vari.BreakRuleInfo_PlayerList.FindIndex(val => val.PersonaId == playerData.PersonaId);
        if (index == -1)
        {
            // 限制玩家击杀
            if (playerData.Kills > ServerRule.Team2.MaxKill && ServerRule.Team2.MaxKill != 0)
            {
                Vari.BreakRuleInfo_PlayerList.Add(new BreakRuleInfo
                {
                    Name = playerData.Name,
                    PersonaId = playerData.PersonaId,
                    Reason = $"Kill Limit {ServerRule.Team2.MaxKill:0}"
                });

                return;
            }

            // 计算玩家KD最低击杀数
            if (playerData.Kills > ServerRule.Team2.KDFlag && ServerRule.Team2.KDFlag != 0)
            {
                // 限制玩家KD
                if (playerData.KD > ServerRule.Team2.MaxKD && ServerRule.Team2.MaxKD != 0.00f)
                {
                    Vari.BreakRuleInfo_PlayerList.Add(new BreakRuleInfo
                    {
                        Name = playerData.Name,
                        PersonaId = playerData.PersonaId,
                        Reason = $"KD Limit {ServerRule.Team2.MaxKD:0.00}"
                    });
                }

                return;
            }

            // 计算玩家KPM比条件
            if (playerData.Kills > ServerRule.Team2.KPMFlag && ServerRule.Team2.KPMFlag != 0)
            {
                // 限制玩家KPM
                if (playerData.KPM > ServerRule.Team2.MaxKPM && ServerRule.Team2.MaxKPM != 0.00f)
                {
                    Vari.BreakRuleInfo_PlayerList.Add(new BreakRuleInfo
                    {
                        Name = playerData.Name,
                        PersonaId = playerData.PersonaId,
                        Reason = $"KPM Limit {ServerRule.Team2.MaxKPM:0.00}"
                    });
                }

                return;
            }

            // 限制玩家最低等级
            if (playerData.Rank < ServerRule.Team2.MinRank && ServerRule.Team2.MinRank != 0 && playerData.Rank != 0)
            {
                Vari.BreakRuleInfo_PlayerList.Add(new BreakRuleInfo
                {
                    Name = playerData.Name,
                    PersonaId = playerData.PersonaId,
                    Reason = $"Min Rank Limit {ServerRule.Team2.MinRank:0}"
                });

                return;
            }

            // 限制玩家最高等级
            if (playerData.Rank > ServerRule.Team2.MaxRank && ServerRule.Team2.MaxRank != 0 && playerData.Rank != 0)
            {
                Vari.BreakRuleInfo_PlayerList.Add(new BreakRuleInfo
                {
                    Name = playerData.Name,
                    PersonaId = playerData.PersonaId,
                    Reason = $"Max Rank Limit {ServerRule.Team2.MaxRank:0}"
                });

                return;
            }

            // 从武器规则里遍历限制武器名称
            for (int i = 0; i < Vari.Custom_WeaponList_Team2.Count; i++)
            {
                var item = Vari.Custom_WeaponList_Team2[i];

                // K 弹
                if (item == "_KBullet")
                {
                    if (playerData.WeaponS0.Contains("_KBullet") ||
                        playerData.WeaponS1.Contains("_KBullet") ||
                        playerData.WeaponS2.Contains("_KBullet") ||
                        playerData.WeaponS3.Contains("_KBullet") ||
                        playerData.WeaponS4.Contains("_KBullet") ||
                        playerData.WeaponS5.Contains("_KBullet") ||
                        playerData.WeaponS6.Contains("_KBullet") ||
                        playerData.WeaponS7.Contains("_KBullet"))
                    {
                        Vari.BreakRuleInfo_PlayerList.Add(new BreakRuleInfo
                        {
                            Name = playerData.Name,
                            PersonaId = playerData.PersonaId,
                            Reason = $"Read Rules, No 'K Bullet'"
                        });

                        return;
                    }
                }

                // 步枪手榴弹（破片）
                if (item == "_RGL_Frag")
                {
                    if (playerData.WeaponS0.Contains("_RGL_Frag") ||
                        playerData.WeaponS1.Contains("_RGL_Frag") ||
                        playerData.WeaponS2.Contains("_RGL_Frag") ||
                        playerData.WeaponS3.Contains("_RGL_Frag") ||
                        playerData.WeaponS4.Contains("_RGL_Frag") ||
                        playerData.WeaponS5.Contains("_RGL_Frag") ||
                        playerData.WeaponS6.Contains("_RGL_Frag") ||
                        playerData.WeaponS7.Contains("_RGL_Frag"))
                    {
                        Vari.BreakRuleInfo_PlayerList.Add(new BreakRuleInfo
                        {
                            Name = playerData.Name,
                            PersonaId = playerData.PersonaId,
                            Reason = "No 'Rifle Grenade Frag'"
                        });

                        return;
                    }
                }

                // 步枪手榴弹（烟雾）
                if (item == "_RGL_Smoke")
                {
                    if (playerData.WeaponS0.Contains("_RGL_Smoke") ||
                        playerData.WeaponS1.Contains("_RGL_Smoke") ||
                        playerData.WeaponS2.Contains("_RGL_Smoke") ||
                        playerData.WeaponS3.Contains("_RGL_Smoke") ||
                        playerData.WeaponS4.Contains("_RGL_Smoke") ||
                        playerData.WeaponS5.Contains("_RGL_Smoke") ||
                        playerData.WeaponS6.Contains("_RGL_Smoke") ||
                        playerData.WeaponS7.Contains("_RGL_Smoke"))
                    {
                        Vari.BreakRuleInfo_PlayerList.Add(new BreakRuleInfo
                        {
                            Name = playerData.Name,
                            PersonaId = playerData.PersonaId,
                            Reason = "No 'Rifle Grenade Smoke'"
                        });

                        return;
                    }
                }

                // 步枪手榴弹（高爆）
                if (item == "_RGL_HE")
                {
                    if (playerData.WeaponS0.Contains("_RGL_HE") ||
                        playerData.WeaponS1.Contains("_RGL_HE") ||
                        playerData.WeaponS2.Contains("_RGL_HE") ||
                        playerData.WeaponS3.Contains("_RGL_HE") ||
                        playerData.WeaponS4.Contains("_RGL_HE") ||
                        playerData.WeaponS5.Contains("_RGL_HE") ||
                        playerData.WeaponS6.Contains("_RGL_HE") ||
                        playerData.WeaponS7.Contains("_RGL_HE"))
                    {
                        Vari.BreakRuleInfo_PlayerList.Add(new BreakRuleInfo
                        {
                            Name = playerData.Name,
                            PersonaId = playerData.PersonaId,
                            Reason = "No 'Rifle Grenade HE'"
                        });

                        return;
                    }
                }

                if (playerData.WeaponS0 == item ||
                    playerData.WeaponS1 == item ||
                    playerData.WeaponS2 == item ||
                    playerData.WeaponS3 == item ||
                    playerData.WeaponS4 == item ||
                    playerData.WeaponS5 == item ||
                    playerData.WeaponS6 == item ||
                    playerData.WeaponS7 == item)
                {
                    Vari.BreakRuleInfo_PlayerList.Add(new BreakRuleInfo
                    {
                        Name = playerData.Name,
                        PersonaId = playerData.PersonaId,
                        Reason = $"Read Rules, No '{PlayerUtil.GetWeaponShortTxt(item)}'"
                    });

                    return;
                }
            }

            // 黑名单
            for (int i = 0; i < Vari.Custom_BlackList.Count; i++)
            {
                var item = Vari.Custom_BlackList[i];
                if (playerData.Name == item)
                {
                    Vari.BreakRuleInfo_PlayerList.Add(new BreakRuleInfo
                    {
                        Name = playerData.Name,
                        PersonaId = playerData.PersonaId,
                        Reason = "Server Black List"
                    });

                    return;
                }
            }
        }
    }

    /// <summary>
    /// 自动踢出违规玩家
    /// </summary>
    private async void AutoKickBreakPlayer()
    {
        // 自动踢出违规玩家开关
        if (Vari.AutoKickBreakPlayer)
        {
            // 遍历违规玩家列表
            for (int i = 0; i < Vari.BreakRuleInfo_PlayerList.Count; i++)
            {
                var item = Vari.BreakRuleInfo_PlayerList[i];
                item.Flag = -1;

                // 跳过管理员
                if (!Vari.Server_AdminList_PID.Contains(item.PersonaId))
                {
                    // 跳过白名单玩家
                    if (!Vari.Custom_WhiteList.Contains(item.Name))
                    {
                        // 先检查踢出玩家是否在 正在踢人 列表中
                        int index = Kicking_PlayerList.FindIndex(var => var.PersonaId == item.PersonaId);
                        if (index == -1)
                        {
                            // 该玩家不在 正在踢人 列表中
                            item.Flag = 0;
                            item.Status = "正在踢人中...";
                            item.Time = DateTime.Now;
                            Kicking_PlayerList.Add(item);

                            // 执行踢人请求
                            await AutoKickPlayer(item);
                        }
                    }
                }
            }

            for (int i = 0; i < Kicking_PlayerList.Count; i++)
            {
                if (Kicking_PlayerList.Count != 0)
                {
                    // 如果超过15秒，清空列表
                    if (CoreUtil.DiffSeconds(Kicking_PlayerList[i].Time, DateTime.Now) > 10)
                    {
                        Kicking_PlayerList.Clear();
                        break;
                    }
                }

                if (Kicking_PlayerList.Count != 0 && Kicking_PlayerList[i].Flag == 0)
                {
                    // 如果超过3秒，移除 正在踢人 玩家
                    if (CoreUtil.DiffSeconds(Kicking_PlayerList[i].Time, DateTime.Now) > 3)
                    {
                        Kicking_PlayerList.RemoveAt(i);
                    }
                }

                if (Kicking_PlayerList.Count != 0 && Kicking_PlayerList[i].Flag == 1)
                {
                    // 如果超过10秒，移除 踢出成功 玩家
                    if (CoreUtil.DiffSeconds(Kicking_PlayerList[i].Time, DateTime.Now) > 10)
                    {
                        Kicking_PlayerList.RemoveAt(i);
                    }
                }

                if (Kicking_PlayerList.Count != 0 && Kicking_PlayerList[i].Flag == 2)
                {
                    // 如果超过5秒，移除 踢出失败 玩家
                    if (CoreUtil.DiffSeconds(Kicking_PlayerList[i].Time, DateTime.Now) > 5)
                    {
                        Kicking_PlayerList.RemoveAt(i);
                    }
                }
            }
        }
    }

    /// <summary>
    /// 自动踢出玩家
    /// </summary>
    /// <param name="info"></param>
    private async Task AutoKickPlayer(BreakRuleInfo info)
    {
        var result = await BF1API.AdminKickPlayer(info.PersonaId, info.Reason);

        if (result.IsSuccess)
        {
            info.Flag = 1;
            info.Status = "kicked";
            info.Time = DateTime.Now;

            LogView._dAddKickOKLog(info);
            await DWebHooks.LogOK(info);
        }
        else
        {
            info.Flag = 2;
            info.Status = "not kicked " + result.Message;
            info.Time = DateTime.Now;

            LogView._dAddKickNOLog(info);
            await DWebHooks.LogNO(info);
        }

    }
    #endregion

    /// <summary>
    /// 手动踢出玩家
    /// </summary>
    /// <param name="reason"></param>
    private async Task KickPlayer(string reason)
    {
        if (!string.IsNullOrEmpty(Vari.SessionId))
        {
            if (_dataGridSelcContent.IsOK)
            {
                NotifierHelper.Show(NotifierType.Information, $"Kicking Player {_dataGridSelcContent.Name} ...");

                var result = await BF1API.AdminKickPlayer(_dataGridSelcContent.PersonaId, reason);
                if (result.IsSuccess)
                {
                    NotifierHelper.Show(NotifierType.Success, $"Playerkick {_dataGridSelcContent.Name} Success  |  Time: {result.ExecTime:0.00} seconds");
                }
                else
                {
                    NotifierHelper.Show(NotifierType.Error, $"Playerkick {_dataGridSelcContent.Name} Fail {result.Message}  | Time: {result.ExecTime:0.00} seconds");
                }
            }
            else
            {
                NotifierHelper.Show(NotifierType.Warning, "Please select the correct player");
            }
        }
        else
        {
            NotifierHelper.Show(NotifierType.Warning, "Please get the player SessionID first");
        }
    }

    /// <summary>
    /// 检测换边玩家
    /// </summary>
    private void CheckPlayerChangeTeam()
    {
        // 如果玩家没有进入服务器，不检测换边情况
        if (string.IsNullOrEmpty(Vari.GameId))
            return;

        // 如果双方玩家人数都为0，不检测换边情况
        if (PlayerList_Team1.Count == 0 && PlayerList_Team2.Count == 0)
            return;

        // 第一次初始化
        if (PlayerDatas_Team1.Count == 0 && PlayerDatas_Team2.Count == 0)
        {
            PlayerDatas_Team1 = CopyList(PlayerList_Team1);            
            PlayerDatas_Team2 = CopyList(PlayerList_Team2);
            return;
        }

        // 变量保存的队伍1玩家列表
        foreach (var item in PlayerDatas_Team1)
        {
            // 查询这个玩家是否在目前的队伍2中
            int index = PlayerList_Team2.FindIndex(var => var.PersonaId == item.PersonaId);
            if (index != -1)
            {
                try
                {
                    LogView._dAddChangeTeamInfo(new ChangeTeamInfo()
                    {
                        Rank = item.Rank,
                        Name = item.Name,
                        PersonaId = item.PersonaId,
                        Team1Score = _serverInfo.Team1Score,
                        Team2Score = _serverInfo.Team2Score,
                        Status = "T1 -> T2",
                        Time = DateTime.Now
                    });
                    break;
                }
                catch (Exception ex) { Log.Ex(ex); }
                
            }
        }

        // 变量保存的队伍2玩家列表
        foreach (var item in PlayerDatas_Team2)
        {
            // 查询这个玩家是否在目前的队伍1中
            int index = PlayerList_Team1.FindIndex(var => var.PersonaId == item.PersonaId);
            if (index != -1)
            {
                try
                {
                    LogView._dAddChangeTeamInfo(new ChangeTeamInfo()
                    {
                        Rank = item.Rank,
                        Name = item.Name,
                        PersonaId = item.PersonaId,
                        Team1Score = _serverInfo.Team1Score,
                        Team2Score = _serverInfo.Team2Score,
                        Status = "T1 <- T2",
                        Time = DateTime.Now
                    });
                    break;
                }
                catch (Exception ex) { Log.Ex(ex); }                
            }
        }

        // 更新保存的数据
        PlayerDatas_Team1 = CopyList(PlayerList_Team1);
        PlayerDatas_Team2 = CopyList(PlayerList_Team2);
    }

    /// <summary>
    /// List深复制
    /// </summary>
    /// <param name="originalList"></param>
    /// <returns></returns>
    private List<PlayerData> CopyList(List<PlayerData> originalList)
    {
        List<PlayerData> list = new();
        foreach (var item in originalList)
        {
            PlayerData data = new()
            {
                Rank = item.Rank,
                Name = item.Name,
                PersonaId = item.PersonaId
            };
            list.Add(data);
        }
        return list;
    }

    #region 右键菜单事件
    /// <summary>
    /// 右键菜单 踢出玩家 - 自定义理由
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void MenuItem_Admin_KickPlayer_Custom_Click(object sender, RoutedEventArgs e)
    {
        if (!string.IsNullOrEmpty(Vari.SessionId))
        {
            if (_dataGridSelcContent.IsOK)
            {
                var customKickWindow = new CustomKickWindow(_dataGridSelcContent.Name, _dataGridSelcContent.PersonaId)
                {
                    Owner = MainWindow.ThisMainWindow
                };
                customKickWindow.ShowDialog();
            }
            else
            {
                NotifierHelper.Show(NotifierType.Warning, "Please select the correct player");
            }
        }
        else
        {
            NotifierHelper.Show(NotifierType.Warning, "Please get the player SessionID first");
        }
    }

    /// <summary>
    /// 右键菜单 踢出玩家 - 攻击性行为
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private async void MenuItem_Admin_KickPlayer_OffensiveBehavior_Click(object sender, RoutedEventArgs e)
    {
        await KickPlayer("OFFENSIVEBEHAVIOR");
    }

    /// <summary>
    /// 右键菜单 踢出玩家 - 延迟
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private async void MenuItem_Admin_KickPlayer_Latency_Click(object sender, RoutedEventArgs e)
    {
        await KickPlayer("LATENCY");
    }

    /// <summary>
    /// 右键菜单 踢出玩家 - 违反规则
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private async void MenuItem_Admin_KickPlayer_RuleViolation_Click(object sender, RoutedEventArgs e)
    {
        await KickPlayer("RULEVIOLATION");
    }

    /// <summary>
    /// 右键菜单 踢出玩家 - 其他
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private async void MenuItem_Admin_KickPlayer_General_Click(object sender, RoutedEventArgs e)
    {
        await KickPlayer("GENERAL");
    }

    /// <summary>
    /// 右键菜单 更换玩家队伍
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private async void MenuItem_Admin_ChangePlayerTeam_Click(object sender, RoutedEventArgs e)
    {
        if (!string.IsNullOrEmpty(Vari.SessionId))
        {
            if (_dataGridSelcContent.IsOK)
            {
                NotifierHelper.Show(NotifierType.Information, $"Changing teams of player {_dataGridSelcContent.Name}...");

                var result = await BF1API.AdminMovePlayer(_dataGridSelcContent.PersonaId, _dataGridSelcContent.TeamId);
                if (result.IsSuccess)
                {
                    NotifierHelper.Show(NotifierType.Success, $"Changed teams of player {_dataGridSelcContent.Name}   |  Time: {result.ExecTime:0.00} s");
                }
                else
                {
                    NotifierHelper.Show(NotifierType.Error, $"Failed to change teams of player {_dataGridSelcContent.Name} Msg: {result.Message}  |  Time: {result.ExecTime:0.00} s");
                }
            }
            else
            {
                NotifierHelper.Show(NotifierType.Warning, "Please select the correct player, the operation is canceled");
            }
        }
        else
        {
            NotifierHelper.Show(NotifierType.Warning, "Please obtain the player's SessionID before performing this operation");
        }
    }

    private async void MenuItem_CopyPlayerName_Click(object sender, RoutedEventArgs e)
    {
        long personaId = _dataGridSelcContent.PersonaId;
        var result = await BF1API.GetPersonasByIds(personaId);
        string id;
        if (result.IsSuccess)
        {
            try //tna
            {
                Debug.WriteLine(result.Message);
                JsonNode jNode = JsonNode.Parse(result.Message);
                id = jNode["result"]![$"{personaId}"]!["nucleusId"].GetValue<string>();

                Clipboard.SetText(id);

                NotifierHelper.Show(NotifierType.Success, $"Copied ID {id} successfully");
            }
            catch (Exception ex) //tna
            {
                Log.Ex(ex);
                NotifierHelper.Show(NotifierType.Information, "Failed to copy ID.");
            }
        }
    }

    /// <summary>
    /// 复制玩家数字ID
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void MenuItem_CopyPlayerName_PID_Click(object sender, RoutedEventArgs e)
    {
        if (_dataGridSelcContent.IsOK)
        {
            Clipboard.SetText(_dataGridSelcContent.PersonaId.ToString());
            NotifierHelper.Show(NotifierType.Success, $"Copied PID {_dataGridSelcContent.PersonaId} successfully");
        }
        else
        {
            NotifierHelper.Show(NotifierType.Warning, "Please select the correct player, the operation is canceled");
        }
    }

    /// <summary>
    /// 查询玩家战绩
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void MenuItem_QueryPlayerRecord_Click(object sender, RoutedEventArgs e)
    {
        if (_dataGridSelcContent.IsOK)
        {
            var queryRecordWindow = new QueryRecordWindow(_dataGridSelcContent.Name, _dataGridSelcContent.PersonaId, _dataGridSelcContent.Rank);
            queryRecordWindow.Show();
        }
        else
        {
            NotifierHelper.Show(NotifierType.Warning, "Please select the correct player, the operation is canceled");
        }
    }

    /// <summary>
    /// 查询玩家战绩（BT）
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void MenuItem_QueryPlayerRecordWeb_BT_Click(object sender, RoutedEventArgs e)
    {
        if (_dataGridSelcContent.IsOK)
        {
            string playerName = _dataGridSelcContent.Name;

            ProcessUtil.OpenLink(@"https://battlefieldtracker.com/bf1/profile/pc/" + playerName);
            NotifierHelper.Show(NotifierType.Success, $"BT Query of（{_dataGridSelcContent.Name}）successful.");
        }
        else
        {
            NotifierHelper.Show(NotifierType.Warning, "Please select the correct player, the operation is canceled");
        }
    }

    /// <summary>
    /// 查询玩家战绩（GT）
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void MenuItem_QueryPlayerRecordWeb_GT_Click(object sender, RoutedEventArgs e)
    {
        if (_dataGridSelcContent.IsOK)
        {
            string playerName = _dataGridSelcContent.Name;

            ProcessUtil.OpenLink(@"https://gametools.network/stats/pc/name/" + playerName + "?game=bf1");
            NotifierHelper.Show(NotifierType.Success, $"GT Query of（{_dataGridSelcContent.Name}）successful.");
        }
        else
        {
            NotifierHelper.Show(NotifierType.Warning, "Please select the correct player, the operation is canceled");
        }
    }

    /// <summary>
    /// 清理得分板标题排序
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void MenuItem_ClearScoreSort_Click(object sender, RoutedEventArgs e)
    {
        Dispatcher.BeginInvoke(()=>
        {
            CollectionViewSource.GetDefaultView(DataGrid_Team1.ItemsSource).SortDescriptions.Clear();
            CollectionViewSource.GetDefaultView(DataGrid_Team2.ItemsSource).SortDescriptions.Clear();

            NotifierHelper.Show(NotifierType.Success, "Cleared Scoreboard Sorting successfully");
        });
    }

    /// <summary>
    /// 显示中文武器名称（参考）
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void MenuItem_ShowWeaponNameZHCN_Click(object sender, RoutedEventArgs e)
    {
        MenuItem item = sender as MenuItem;
        if (item != null)
        {
            if (item.IsChecked)
            {
                Vari.IsShowCHSWeaponName = true;
                NotifierHelper.Show(NotifierType.Success, $"Weapon Name Mode switched");
            }
            else
            {
                Vari.IsShowCHSWeaponName = false;
                NotifierHelper.Show(NotifierType.Success, $"Weapon Name Mode switched");
            }
        }
    }
    #endregion

    #region DataGrid相关方法
    private void DataGrid_Team1_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        var item = DataGrid_Team1.SelectedItem as PlayerListModel;
        if (item != null)
        {
            _dataGridSelcContent.IsOK = true;
            _dataGridSelcContent.TeamId = 1;
            _dataGridSelcContent.Rank = item.Rank;
            _dataGridSelcContent.Name = item.Name;
            _dataGridSelcContent.PersonaId = item.PersonaId;
        }
        else
        {
            _dataGridSelcContent.IsOK = false;
            _dataGridSelcContent.TeamId = -1;
            _dataGridSelcContent.Rank = -1;
            _dataGridSelcContent.Name = string.Empty;
            _dataGridSelcContent.PersonaId = -1;
        }

        Update_DateGrid_Selection();
    }

    private void DataGrid_Team2_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        var item = DataGrid_Team2.SelectedItem as PlayerListModel;
        if (item != null)
        {
            _dataGridSelcContent.IsOK = true;
            _dataGridSelcContent.TeamId = 2;
            _dataGridSelcContent.Rank = item.Rank;
            _dataGridSelcContent.Name = item.Name;
            _dataGridSelcContent.PersonaId = item.PersonaId;
        }
        else
        {
            _dataGridSelcContent.IsOK = false;
            _dataGridSelcContent.TeamId = -1;
            _dataGridSelcContent.Rank = -1;
            _dataGridSelcContent.Name = string.Empty;
            _dataGridSelcContent.PersonaId = -1;
        }

        Update_DateGrid_Selection();
    }

    private void Update_DateGrid_Selection()
    {
        var sb = new StringBuilder();

        if (_dataGridSelcContent.IsOK)
        {
            sb.Append($"Player ID : {_dataGridSelcContent.Name}");
            sb.Append($" | TeamID : {_dataGridSelcContent.TeamId}");
            sb.Append($" | LVL : {_dataGridSelcContent.Rank}");
            sb.Append($" | Date : {DateTime.Now}");
        }
        else
        {
            sb.Append($"No Players selected");
            sb.Append($" | Date : {DateTime.Now}");
        }

        TextBlock_DataGridSelectionContent.Text = sb.ToString();
    }
    #endregion
}
