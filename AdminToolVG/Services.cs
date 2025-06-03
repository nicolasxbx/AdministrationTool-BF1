namespace AdminToolVG;

public static class Services
{
    #region StartServices
    public static void StartServices()
    {
        var thread1 = new Thread(UpdateStateLoop)
        {
            IsBackground = true
        };
        thread1.Start();

        var thread2 = new Thread(ServerDetailService.UpdateServerDetailsLoop)
        {
            IsBackground = true
        };
        thread2.Start();

        var thread3 = new Thread(ServerLiveInfoService.UpdateServerLiveDetailsLoop)
        {
            IsBackground = true
        };
        thread3.Start();

        var thread4 = new Thread(ServerPlayerService.UpdatePlayerListLoop)
        {
            IsBackground = true
        };
        thread4.Start();
    }
    static bool pingServiceStarted = false;
    public static void TryStartPingService()
    {
        if (pingServiceStarted)
        {
            return;
        }
        
        var thread5 = new Thread(PingService.UpdatePingLoop)
        {
            IsBackground = true
        };
        thread5.Start();

        pingServiceStarted = true;
    }

    static bool autoSusServiceStarted = false;
    public static void TryStartAutoSusService()
    {
        if (autoSusServiceStarted)
        {
            return;
        }

        var thread6 = new Thread(Thread_AutoSusCheck)
        {
            IsBackground = true
        };
        thread6.Start();

        autoSusServiceStarted = true;
    }

    static bool autoChatServiceStarted = false;
    public static void TryStartAutoChatService()
    {
        if (autoChatServiceStarted)
        {
            return;
        }

        var thread6 = new Thread(Thread_AutoChat)
        {
            IsBackground = true
        };
        thread6.Start();

        autoChatServiceStarted = true;
    }

    static bool autoBalancerServiceStarted = false;
    public static void TryStartAutoBalanceService()
    {
        if (autoBalancerServiceStarted)
        {
            return;
        }

        var thread6 = new Thread(Thread_Balancer)
        {
            IsBackground = true
        };
        thread6.Start();

        autoBalancerServiceStarted = true;
    }
    #endregion

    #region BF1 State Service    
    public static async void UpdateStateLoop() //thread
    {
        while (true)
        {
            if (!ProcessUtil.IsAppRun(CoreUtil.TargetAppName)) //BF1 Closing
            {
                await DWebHooks.LogMonitoringOFF();
                Log.CM("Battlefield closed! Tool is closing....");

                Thread.Sleep(3000);
                Environment.Exit(0);
            }
            Thread.Sleep(1000);
        }
    }
    #endregion

    #region Server Detail Service
    public static class ServerDetailService
    {
        public static async void UpdateServerDetailsLoop()
        {
            while (true)
            {
                if (string.IsNullOrEmpty(Vari.GameId))
                {
                    Vari.ServerDetails = new();

                    Vari.ServerDetails_AdminList_PID.Clear();
                    Vari.ServerDetails_AdminList_Name.Clear();
                    Vari.ServerDetails_VIPList.Clear();
                }
                else
                {
                    await FetchFullServerDetails();
                }

                Thread.Sleep(Vari.Service_Details_Refresh_Delay * 1000);
            }
        }

        public static async Task FetchFullServerDetails()
        {
            // PRE CHECK
            if (string.IsNullOrEmpty(Vari.SessionID)) return;
            if (string.IsNullOrEmpty(Vari.GameId)) return;

            // CLEAR
            Vari.ServerDetails = new();
            Vari.ServerDetails_AdminList_PID.Clear();
            Vari.ServerDetails_AdminList_Name.Clear();
            Vari.ServerDetails_VIPList.Clear();

            Vari.ServerDetails_PersistedGameId = "";

            // GET DATA
            //await BF1API.SetAPILocale();
            var result = await BF1API.GetFullServerDetails();

            if (!result.IsSuccess) return;

            //Parse Json
            var fullServerDetails = JsonUtil.JsonDese<FullServerDetails>(result.Message);
            Vari.FullServerDetails = fullServerDetails;

            Vari.ServerDetails.ServerID = fullServerDetails.result.rspInfo.server.serverId;
            Vari.ServerDetails.ServerGameID = Vari.GameId;
            Vari.ServerDetails_PersistedGameId = fullServerDetails.result.rspInfo.server.persistedGameId;

            Vari.ServerDetails.ServerName = fullServerDetails.result.serverInfo.name;
            Vari.ServerDetails.ServerDescription = fullServerDetails.result.serverInfo.description;            
            
            // SERVER OWNER (+ADD TO ADMIN LIST)
            try
            {
                Vari.ServerDetails.ServerOwnerName = fullServerDetails.result.rspInfo.owner.displayName;
                Vari.ServerDetails.ServerOwnerPersonaId = fullServerDetails.result.rspInfo.owner.personaId;
                Vari.ServerDetails.ServerOwnerImage = fullServerDetails.result.rspInfo.owner.avatar;

                Vari.ServerDetails_AdminList_PID.Add(long.Parse(fullServerDetails.result.rspInfo.owner.personaId));
                Vari.ServerDetails_AdminList_Name.Add(fullServerDetails.result.rspInfo.owner.displayName);
            }
            catch (Exception ex)
            {
                Log.Ex(ex);
            }

            // Admins        
            try
            {                
                foreach (var item in fullServerDetails.result.rspInfo.adminList)
                {
                    Vari.ServerDetails_AdminList_PID.Add(long.Parse(item.personaId));
                    Vari.ServerDetails_AdminList_Name.Add(item.displayName);
                }
            }
            catch (Exception ex)
            {
                Log.Ex(ex);
            }

            // VIPs
            try
            {
                foreach (var item in fullServerDetails.result.rspInfo.vipList)
                {
                    Vari.ServerDetails_VIPList.Add(long.Parse(item.personaId));
                }
            }
            catch (Exception ex)
            {
                Log.Ex(ex);
            }

            // Ban List
            int index;
            try
            {
                index = 1;
                foreach (var item in fullServerDetails.result.rspInfo.bannedList)
                {
                    Vari.ServerDetails_Banlist.Add(new RSPInfo()
                    {
                        Index = index++,
                        avatar = item.avatar,
                        displayName = item.displayName,
                        personaId = item.personaId
                    });
                }
            }
            catch (Exception ex)
            {
                Log.Ex(ex);
            }
        }
    }
    #endregion

    #region Server Live Info Service
    public static class ServerLiveInfoService
    {
        private static ServerInfo _ServerLiveInfo { get; set; } = new();
        private static ServerInfoModel _ServerLiveInfo2 { get; set; } = new();
        public static void UpdateServerLiveDetailsLoop()
        {
            while (true)
            {
                _ServerLiveInfo.Name = Memory.ReadString(Memory.GetBaseAddress() + Offsets.ServerName_Offset, Offsets.ServerName, 64);
                _ServerLiveInfo.Name = string.IsNullOrEmpty(_ServerLiveInfo.Name) ? "Unknown" : _ServerLiveInfo.Name;
                _ServerLiveInfo.MapName = Memory.ReadString(Offsets.OFFSET_CLIENTGAMECONTEXT, Offsets.ServerMapName, 64);
                _ServerLiveInfo.MapName = string.IsNullOrEmpty(_ServerLiveInfo.MapName) ? "Unknown" : _ServerLiveInfo.MapName;
                _ServerLiveInfo.GameMode = Memory.ReadString(Memory.GetBaseAddress() + Offsets.ServerID_Offset, Offsets.ServerGameMode, 64);
                _ServerLiveInfo.GameMode = PlayerUtil.GetGameMode(_ServerLiveInfo.GameMode);
                _ServerLiveInfo.Time = Memory.Read<float>(Memory.GetBaseAddress() + Offsets.ServerTime_Offset, Offsets.ServerTime);

                _ServerLiveInfo2.ServerName = _ServerLiveInfo.Name;
                _ServerLiveInfo2.ServerGameID = _ServerLiveInfo.GameID.ToString();
                _ServerLiveInfo2.ServerMapName = PlayerUtil.GetMapChsName(_ServerLiveInfo.MapName);
                Vari.CurrentMapName = _ServerLiveInfo2.ServerMapName;
                _ServerLiveInfo2.ServerMapImg = PlayerUtil.GetMapPrevImage(_ServerLiveInfo.MapName);

                if (_ServerLiveInfo.MapName == "Unknown" || _ServerLiveInfo2.ServerMapName == "Unknown")
                {
                    _ServerLiveInfo2.ServerGameMode = "Unknown";
                }
                else
                {
                    _ServerLiveInfo2.ServerGameMode = _ServerLiveInfo.GameMode;
                }

                PlayerUtil.GetTeamImage(_ServerLiveInfo.MapName, out _ServerLiveInfo.Team1Img, out _ServerLiveInfo.Team2Img);
                _ServerLiveInfo2.Team1Img = _ServerLiveInfo.Team1Img;
                _ServerLiveInfo2.Team2Img = _ServerLiveInfo.Team2Img;

                _ServerLiveInfo2.ServerTime = PlayerUtil.SecondsToMMSS(_ServerLiveInfo.Time);

                if (_ServerLiveInfo.GameMode == "CQ")
                {

                    _ServerLiveInfo.OffsetTemp = Memory.Read<long>(Memory.GetBaseAddress() + Offsets.ServerScore_Offset, Offsets.ServerScoreTeam);

                    _ServerLiveInfo.MaxScore = Memory.Read<int>(_ServerLiveInfo.OffsetTemp + 0x120);

                    _ServerLiveInfo.Team1Score = Memory.Read<int>(_ServerLiveInfo.OffsetTemp + 0xE8);
                    _ServerLiveInfo.Team2Score = Memory.Read<int>(_ServerLiveInfo.OffsetTemp + 0x118);

                    _ServerLiveInfo.Team1Kill = Memory.Read<int>(_ServerLiveInfo.OffsetTemp + 0x230);
                    _ServerLiveInfo.Team2Kill = Memory.Read<int>(_ServerLiveInfo.OffsetTemp + 0x238);

                    _ServerLiveInfo.Team1Flag = Memory.Read<int>(_ServerLiveInfo.OffsetTemp + 0x250);
                    _ServerLiveInfo.Team2Flag = Memory.Read<int>(_ServerLiveInfo.OffsetTemp + 0x258);

                    _ServerLiveInfo.Team1Score = PlayerUtil.FixedServerScore(_ServerLiveInfo.Team1Score);
                    _ServerLiveInfo.Team2Score = PlayerUtil.FixedServerScore(_ServerLiveInfo.Team2Score);

                    if (_ServerLiveInfo.MaxScore != 0)
                    {
                        var scale = _ServerLiveInfo.MaxScore / 1000.0f;
                        _ServerLiveInfo2.Team1ScoreWidth = PlayerUtil.FixedServerScore(_ServerLiveInfo.Team1Score / (8 * scale));
                        _ServerLiveInfo2.Team2ScoreWidth = PlayerUtil.FixedServerScore(_ServerLiveInfo.Team2Score / (8 * scale));
                    }
                    else
                    {
                        _ServerLiveInfo2.Team1ScoreWidth = 0;
                        _ServerLiveInfo2.Team2ScoreWidth = 0;
                    }

                    _ServerLiveInfo2.Team1Score = _ServerLiveInfo.Team1Score.ToString();
                    _ServerLiveInfo2.Team2Score = _ServerLiveInfo.Team2Score.ToString();

                    _ServerLiveInfo2.Team1Flag = PlayerUtil.FixedServerScore(_ServerLiveInfo.Team1Flag);
                    _ServerLiveInfo2.Team1Kill = PlayerUtil.FixedServerScore(_ServerLiveInfo.Team1Kill);

                    _ServerLiveInfo2.Team2Flag = PlayerUtil.FixedServerScore(_ServerLiveInfo.Team2Flag);
                    _ServerLiveInfo2.Team2Kill = PlayerUtil.FixedServerScore(_ServerLiveInfo.Team2Kill);
                }

                ////////////////////////////////////////////////////////////////////////////////

                // 如果玩家没有进入服务器，要进行一些数据清理
                if (_ServerLiveInfo2.ServerMapName == "Server Map Name")
                {
                    // 清理服务器ID（GameID）
                    _ServerLiveInfo.GameID = 0;
                    Vari.GameId = string.Empty;

                    Vari.ServerDetails_AdminList_PID.Clear();
                    Vari.ServerDetails_AdminList_Name.Clear();
                    Vari.ServerDetails_VIPList.Clear();
                }
                else
                {
                    // 服务器数字ID
                    _ServerLiveInfo.GameID = Memory.Read<long>(Memory.GetBaseAddress() + Offsets.ServerID_Offset, Offsets.ServerGameID);
                    Vari.GameId = _ServerLiveInfo.GameID.ToString();                    
                }

                Vari.ServerLiveInfo = _ServerLiveInfo;
                Vari.ServerLiveInfo2 = _ServerLiveInfo2;


                Thread.Sleep(1000);
            }
        }
    }
    #endregion

    #region Server Player Service
    public static class ServerPlayerService
    {
        private static List<PlayerData> PlayerList_All { get; set; } = new();
        private static List<PlayerData> PlayerList_Team0 { get; set; } = new();
        private static List<PlayerData> PlayerList_Team1 { get; set; } = new();
        private static List<PlayerData> PlayerList_Team2 { get; set; } = new();

        public static List<PlayerData> PlayerDatas_Team1 { get; set; } = new();
        public static List<PlayerData> PlayerDatas_Team2 { get; set; } = new();
        private static List<BreakRuleInfo> Kicking_PlayerList { get; set; } = new();        
        public static PlayerOtherModel PlayerOtherModel { get; set; } = new();
        private static StatisticData _statisticData_Team1 { get; set; } = new();
        private static StatisticData _statisticData_Team2 { get; set; } = new();
        private static int MaxPlayerCount { get; } = 74;

        public static void UpdatePlayerListLoop()
        {
            while (true)
            {
                #region Clear Variables
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

                #endregion

                #region Memory Scan, 74 Times
                var _myBaseAddress = Player.GetLocalPlayer();

                var _myTeamId = Memory.Read<int>(_myBaseAddress + 0x1C34);
                PlayerOtherModel.MySelfTeamID = $"Team ID : {_myTeamId}";

                var _myPlayerName = Memory.ReadString(_myBaseAddress + 0x2156, 64);
                PlayerOtherModel.MySelfName = string.IsNullOrEmpty(_myPlayerName) ? "Player ID : Unknown" : $"Player ID: {_myPlayerName}";

                //////////////////////////////// 玩家数据 ////////////////////////////////

                for (int i = 0; i < MaxPlayerCount; i++)
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

                for (int i = 0; i < MaxPlayerCount; i++)
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
                        PlayerList_All[index].KPM = PlayerUtil.GetPlayerKPM(_kill, PlayerUtil.SecondsToMinute(Vari.ServerLiveInfo.Time));
                    }
                }
                #endregion
                                
                #region What to do with Players
                //Original
                foreach (var item in PlayerList_All)
                {
                    item.Admin = PlayerUtil.CheckAdminVIP(item.PersonaId, Vari.ServerDetails_AdminList_PID);
                    item.VIP = PlayerUtil.CheckAdminVIP(item.PersonaId, Vari.ServerDetails_VIPList);

                    switch (item.TeamId)
                    {
                        case 0:
                            PlayerList_Team0.Add(item);
                            break;
                        case 1:
                            PlayerList_Team1.Add(item);
                            CheckTeam1PlayerIsBreakRule(item);
                            break;
                        case 2:
                            PlayerList_Team2.Add(item);
                            CheckTeam2PlayerIsBreakRule(item);
                            break;
                    }
                }
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

                #endregion

                #region Misc      
                try
                {
                    Vari.ServerLiveInfo2.Team1Info = $" Spawned / Playercount : {_statisticData_Team1.PlayerCount} / {_statisticData_Team1.MaxPlayerCount}  |  Lvl150 : {_statisticData_Team1.Rank150PlayerCount}  |  Kills : {_statisticData_Team1.AllKillCount}  |  Deaths : {_statisticData_Team1.AllDeadCount} | KD : {Vari.NexTeamKD1}";
                    Vari.ServerLiveInfo2.Team2Info = $" Spawned / Playercount : {_statisticData_Team2.PlayerCount} / {_statisticData_Team2.MaxPlayerCount}  |  Lvl150 : {_statisticData_Team2.Rank150PlayerCount}  |  Kills : {_statisticData_Team2.AllKillCount}  |  Deaths : {_statisticData_Team2.AllDeadCount} | KD : {Vari.NexTeamKD2}";
                }
                catch(Exception ex)
                {
                    Log.Ex(ex);
                }

                PlayerOtherModel.ServerPlayerCountInfo = $"ServerPlayerCount : {_statisticData_Team1.MaxPlayerCount + _statisticData_Team2.MaxPlayerCount}";
                #endregion

                KickFlaggedPlayers();

                CheckTeamChanges();

                Vari.Playerlist_All = PlayerList_All;                
                Vari.Playerlist_Team1 = PlayerList_Team1;
                Vari.Playerlist_Team2 = PlayerList_Team2;

                Vari.NexStatisticData_Team1 = _statisticData_Team1;
                Vari.NexStatisticData_Team2 = _statisticData_Team2;

                Vari.NexTeamKD1 = Math.Round(Convert.ToDouble(_statisticData_Team1.AllKillCount) / Convert.ToDouble(_statisticData_Team1.AllDeadCount), 2);
                Vari.NexTeamKD2 = Math.Round(Convert.ToDouble(_statisticData_Team2.AllKillCount) / Convert.ToDouble(_statisticData_Team2.AllDeadCount), 2);

                Thread.Sleep(Vari.Service_Players_Delay * 1000);
            }
        }

        private static void CheckTeam1PlayerIsBreakRule(PlayerData playerData)
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
                for (int i = 0; i < Vari.BannedWeapons_Team1.Count; i++)
                {
                    var item = Vari.BannedWeapons_Team1[i];

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
                for (int i = 0; i < Vari.Custom_Player_BlackList.Count; i++)
                {
                    var item = Vari.Custom_Player_BlackList[i];
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

        private static void CheckTeam2PlayerIsBreakRule(PlayerData playerData)
        {
            int index = Vari.BreakRuleInfo_PlayerList.FindIndex(val => val.PersonaId == playerData.PersonaId);
            if (index == -1)
            {
                //Max Kills
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

                //Max Kills
                if (playerData.Kills > ServerRule.Team2.KDFlag && ServerRule.Team2.KDFlag != 0)
                {
                    // Max KD
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

                // Max KPM
                if (playerData.Kills > ServerRule.Team2.KPMFlag && ServerRule.Team2.KPMFlag != 0)
                {
                    // Max KPM
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

                // Min Rank
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

                // Max Rank
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

                // Banned Weapons
                for (int i = 0; i < Vari.BannedWeapons_Team2.Count; i++)
                {
                    var item = Vari.BannedWeapons_Team2[i];

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
                for (int i = 0; i < Vari.Custom_Player_BlackList.Count; i++)
                {
                    var item = Vari.Custom_Player_BlackList[i];
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

        private static async void KickFlaggedPlayers()
        {
            if (Vari.AutoKickBreakPlayer == false)
            {
                //v4.2 Attempt to debug other rulesets still kicking vg ruleset
                Vari.BreakRuleInfo_PlayerList.Clear();
                Kicking_PlayerList.Clear();

                return;
            }

            for (int i = 0; i < Vari.BreakRuleInfo_PlayerList.Count; i++)
            {
                var item = Vari.BreakRuleInfo_PlayerList[i];
                item.Flag = -1;

                if (Vari.ServerDetails_AdminList_PID.Contains(item.PersonaId)) return;

                if (Vari.Custom_Player_WhiteList.Contains(item.Name)) return;

                int index = Kicking_PlayerList.FindIndex(var => var.PersonaId == item.PersonaId);

                if (index == -1)
                {
                    item.Flag = 0;
                    item.Status = "kicking...";
                    item.Time = DateTime.UtcNow;
                    Kicking_PlayerList.Add(item);

                    //await AutoKickPlayer(item);
                    await Util_BF1.AdminActions.Kick.AutoKick(item, false);
                }
            }

            for (int i = 0; i < Kicking_PlayerList.Count; i++)
            {
                if (Kicking_PlayerList.Count != 0)
                {
                    // If more than 15 seconds, clear the list
                    if (CoreUtil.DiffSeconds(Kicking_PlayerList[i].Time, DateTime.Now) > 10)
                    {
                        Kicking_PlayerList.Clear();
                        break;
                    }
                }

                if (Kicking_PlayerList.Count != 0 && Kicking_PlayerList[i].Flag == 0)
                {
                    // If more than 3 seconds, remove kicking player
                    if (CoreUtil.DiffSeconds(Kicking_PlayerList[i].Time, DateTime.Now) > 3)
                    {
                        Kicking_PlayerList.RemoveAt(i);
                    }
                }

                if (Kicking_PlayerList.Count != 0 && Kicking_PlayerList[i].Flag == 1)
                {
                    // If more than 10 seconds, remove and kick the successful player
                    if (CoreUtil.DiffSeconds(Kicking_PlayerList[i].Time, DateTime.Now) > 10)
                    {
                        Kicking_PlayerList.RemoveAt(i);
                    }
                }

                if (Kicking_PlayerList.Count != 0 && Kicking_PlayerList[i].Flag == 2)
                {
                    // If it takes more than 5 seconds, remove and kick the failed player
                    if (CoreUtil.DiffSeconds(Kicking_PlayerList[i].Time, DateTime.Now) > 5)
                    {
                        Kicking_PlayerList.RemoveAt(i);
                    }
                }
            }
        }        
        private static void CheckTeamChanges()
        {            
            if (string.IsNullOrEmpty(Vari.GameId))
                return;

            if (PlayerList_Team1.Count == 0 && PlayerList_Team2.Count == 0)
                return;
            
            if (PlayerDatas_Team1.Count == 0 && PlayerDatas_Team2.Count == 0)
            {
                PlayerDatas_Team1 = Copy_PlayerDataList(PlayerList_Team1);
                PlayerDatas_Team2 = Copy_PlayerDataList(PlayerList_Team2);

                Vari.PlayerDatas_Team1 = PlayerDatas_Team1;
                Vari.PlayerDatas_Team2 = PlayerDatas_Team2;
                return;
            }
            
            foreach (var item in PlayerDatas_Team1)
            {                
                int index = PlayerList_Team2.FindIndex(var => var.PersonaId == item.PersonaId);
                if (index != -1)
                {
                    try
                    {
                        Util_BF1.HandleTeamChange(new ChangeTeamInfo()
                        {
                            Rank = item.Rank,
                            Name = item.Name,
                            PersonaId = item.PersonaId,
                            Team1Score = Vari.ServerLiveInfo.Team1Score,
                            Team2Score = Vari.ServerLiveInfo.Team2Score,
                            Status = "T1 -> T2",
                            Time = DateTime.Now
                        });
                        break;
                    }
                    catch (Exception ex) { Log.Ex(ex); }

                }
            }
            
            foreach (var item in PlayerDatas_Team2)
            {                
                int index = PlayerList_Team1.FindIndex(var => var.PersonaId == item.PersonaId);
                if (index != -1)
                {
                    try
                    {
                        Util_BF1.HandleTeamChange(new ChangeTeamInfo()
                        {
                            Rank = item.Rank,
                            Name = item.Name,
                            PersonaId = item.PersonaId,
                            Team1Score = Vari.ServerLiveInfo.Team1Score,
                            Team2Score = Vari.ServerLiveInfo.Team2Score,
                            Status = "T1 <- T2",
                            Time = DateTime.Now
                        });
                        break;
                    }
                    catch (Exception ex) { Log.Ex(ex); }
                }
            }

            // 更新保存的数据
            PlayerDatas_Team1 = Copy_PlayerDataList(PlayerList_Team1);
            PlayerDatas_Team2 = Copy_PlayerDataList(PlayerList_Team2);

            Vari.PlayerDatas_Team1 = PlayerDatas_Team1;
            Vari.PlayerDatas_Team2 = PlayerDatas_Team2;
        }
        private static List<PlayerData> Copy_PlayerDataList(List<PlayerData> originalList)
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
    }
    #endregion

    #region Ping Service
    public static class PingService
    {
        public static async void UpdatePingLoop() //tna
        {
            while (true)
            {
                if (Vari.AutoKickPing == true) //nex
                {
                    await Ping_Process_GT_Data(); //nex                
                }
                Thread.Sleep(Vari.Service_Ping_Delay * 1000);
            }
        }
        private static async Task<bool> FetchDataFromGameTools() //tna
        {
            string gametools_serverinfo_json = await HttpHelper.HttpClientGET($"https://api.gametools.network/bf1/players/?gameid={Vari.GameId}");

            //Empty Request
            if (String.IsNullOrEmpty(gametools_serverinfo_json))
            {
                Log.D($"\n❌Get Gametools-Server-Info failed, Gametools is sending an empty request");
                return false;
            }

            //Same Old Request
            if (Vari.gametools_serverinfo_json_previous != string.Empty && gametools_serverinfo_json == Vari.gametools_serverinfo_json_previous)
            {
                return false;
            }

            Vari.gametools_serverinfo_json_previous = gametools_serverinfo_json;

            try
            {
                JsonNode node_full = JsonNode.Parse(gametools_serverinfo_json);
                var options = new JsonSerializerOptions { WriteIndented = true }; //from google

                JsonNode node_teams = node_full!["teams"]!; //JsonArray         
                JsonNode node_team1 = node_teams[0]!; //JsonObject
                JsonNode node_team2 = node_teams[1]!; //JsonObject
                JsonNode node_players1 = node_team1!["players"]!; //JsonArray
                JsonNode node_players2 = node_team2!["players"]!; //JsonArray                                                                  

                PlayerN[] playernarray1 = JsonSerializer.Deserialize<PlayerN[]>(node_players1);
                PlayerN[] playernarray2 = JsonSerializer.Deserialize<PlayerN[]>(node_players2);

                Vari.gametools_playerarray_t1 = playernarray1;
                Vari.gametools_playerarray_t2 = playernarray2;

                return true;
            }
            catch (Exception ex)
            {
                Log.Ex(ex);
                Log.D($"❌Get Gametools-Server-Info failed, Error: '{ex}'");
                return false;
            }            
        }
        private static async Task Ping_Process_GT_Data() //tna
        {
            bool data_fetched = await FetchDataFromGameTools();

            if (!data_fetched)
            {
                return;
            }

            try
            {
                int peoplekickedamount = 0;
                if (Vari.gametools_playerarray_t1[0] != null && Vari.gametools_playerarray_t1 != null)
                {
                    Array.ForEach(Vari.gametools_playerarray_t1, async element => peoplekickedamount += await Ping_Review(element));
                    Array.ForEach(Vari.gametools_playerarray_t2, async element => peoplekickedamount += await Ping_Review(element));
                }
                if (peoplekickedamount > 0)
                {
                    Log.D($"✔Ping-Check finished, {peoplekickedamount} Players kicked.");
                }
                else
                {
                    Log.D($"✔Ping-Check finished, no Players kicked.");
                }
                peoplekickedamount = 0;

            }
            catch (Exception ex)
            {
                Log.Ex(ex);
                Log.D($"❌Ping-Check failed, Error: '{ex}'");
            }
        }
        private static async Task<int> Ping_Review(PlayerN pn)
        {
            int pka = 0;
            if (pn.latency > Vari.PingLimit)
            {
                if (Vari.AutoKickPing)
                {
                    await Util_BF1.AdminActions.Kick.AutoKick(new BreakRuleInfo
                    {
                        Name = pn.name,
                        PersonaId = pn.player_id,
                        Reason = $"Latency exceeded {Vari.PingLimit}. ({pn.latency})"
                    }, true);
                    pka++;
                }
                else
                {

                }                
            }
            return pka;

        }        
    }
    #endregion

    #region AutoSusCheck
    public static async void Thread_AutoSusCheck() //threaded
    {
        while (true)
        {
            if (!Vari.AutoSusCheck)
            {
                Thread.Sleep(30000);
                return;
            }

            await Server_Sus_check();

            Thread.Sleep(300 * 1000); //300 Seconds = 5 Minutes
        }
    }
    public static async Task Server_Sus_check()
    {
        List<PlayerData> playerlist = Vari.Playerlist_All.ToList();

        Vari.SusPlayers.Clear();
        try
        {
            foreach (PlayerData p in playerlist.ToList())
            {
                List<WeaponStats> list_sus_weapons = await Util_BF1.AdminActions.CheckSus(p.PersonaId);
                if (list_sus_weapons == null)
                {
                    continue;
                }

                Vari.SusPlayers.Add(p);               
            }
        }
        catch (Exception ex) { Log.Ex(ex); }
    }
    #endregion

    #region AutoChat
    static async void Thread_AutoChat()
    {
        Thread.Sleep(30000); //30 Second Initial Delay
        while (true)
        {
            if (!Vari.AutoChat)
            {
                Thread.Sleep(30000);
                return;
            }

            Util_BF1.AdminActions.ChatFunction(Vari.CurrentRuleString);
            Thread.Sleep(900000); //15 Min Delay
        }
    }
    #endregion

    #region Balancer
    static async void Thread_Balancer()
    {
        while (true)
        {
            if (!Vari.AutoBalancer)
            {
                Thread.Sleep(30000);
                return;
            }

            //Check Unbalance
            //IF VARI.BALANCER NOT ENABLED RETURN
            try
            {
                if (Vari.ServerLiveInfo == null)
                {
                    Thread.Sleep(2000);
                    continue;
                }

                if (Vari.ServerLiveInfo != null)
                {
                    //Evaluate Stomp
                    bool stomp = Util_BF1.UnbalanceChecker(Vari.ServerLiveInfo.Team1Score, Vari.ServerLiveInfo.Team2Score);
                    if (!stomp || Vari.NexConquestAssaultMapNames.Contains(Vari.CurrentMapName))
                    {
                        Thread.Sleep(360000); //6 Min
                        continue;
                    }

                    // Find out stomping team
                    bool team1_stomping = Vari.ServerLiveInfo.Team1Score > Vari.ServerLiveInfo.Team2Score ? true : false;

                    //Get team list
                    var teamlist = team1_stomping ? Vari.Playerlist_Team1.ToList() : Vari.PlayerDatas_Team2;
                    //Remove Admins/VIPs from List
                    foreach (var item in teamlist)
                    {
                        if (!string.IsNullOrEmpty(item.Admin) || !string.IsNullOrEmpty(item.VIP))
                        {
                            teamlist.Remove(item);
                        }
                    }

                    //Choose random player
                    var Random = new Random();
                    var player = teamlist[Random.Next(0, teamlist.Count)];


                    //Switch Players
                    int teamid = team1_stomping ? 1 : 2;
                    //var result = await BF1API.AdminMovePlayer(player.PersonaId, teamid);
                    Log.D($"WOULDVE KICKED: {player.Name} ({player.PersonaId} from team {teamid})"); //DEBUG
                    Vari.Logs_Switches.Add(new()
                    {
                        DateTime = DateTime.UtcNow,
                        Iteration = 0,
                        Name = player.Name,
                        Status = "AutoBalanced",
                        Map = Vari.CurrentMapName,
                    });
                    Thread.Sleep(600000); //10 Min
                }
            }
            catch (Exception ex)
            {
                Log.Ex(ex);
            }
        }
    }
    #endregion
}