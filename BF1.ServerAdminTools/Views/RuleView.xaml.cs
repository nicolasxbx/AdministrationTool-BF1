using BF1.ServerAdminTools.Models.Rule;
using BF1.ServerAdminTools.Common.Utils;
using BF1.ServerAdminTools.Common.Helper;
using BF1.ServerAdminTools.Features.Data;
using BF1.ServerAdminTools.Features.Config;
using BF1.ServerAdminTools.Features.Client;
using BF1.ServerAdminTools.Features.API;
using BF1.ServerAdminTools.Features.API.RespJson;

using BF1.ServerAdminTools.Models;
using BF1.ServerAdminTools.NexDiscord;
using NexDiscord;
using Discord;

namespace BF1.ServerAdminTools.Views;

/// <summary>
/// RuleView.xaml 的交互逻辑
/// </summary>
public partial class RuleView : UserControl
{
    private List<RuleConfig> RuleConfigs { get; set; } = new();

    public RuleTeamModel RuleTeam1Model { get; set; } = new();
    public RuleTeamModel RuleTeam2Model { get; set; } = new();

    public ObservableCollection<RuleWeaponModel> DataGrid_RuleWeaponModels { get; set; } = new();
    public ObservableCollection<string> ComboBox_ConfigNames { get; set; } = new();

    //tna
    public Thread thread2Ping { get; set; }
    public Thread thread3BFBAN { get; set; }
    public Thread thread4AutoRun { get; set; }

    /// <summary>
    /// 是否已经执行
    /// </summary>
    private bool isHasBeenExec = false;
    /// <summary>
    /// 是否执行应用规则
    /// </summary>
    private bool isApplyRule = false;

    public RuleView()
    {
        InitializeComponent();
        this.DataContext = this;
        MainWindow.ClosingDisposeEvent += MainWindow_ClosingDisposeEvent;        

        // 添加武器信息列表
        foreach (var item in WeaponData.AllWeaponInfo)
        {
            DataGrid_RuleWeaponModels.Add(new RuleWeaponModel()
            {
                Class = item.Class,
                Name = item.Chinese,
                English = item.English,
                Team1 = false,
                Team2 = false
            });
        }

        var thread0 = new Thread(AutoKickLifeBreakPlayer)
        {
            IsBackground = true
        };
        thread0.Start();

        var thread1 = new Thread(CheckState)
        {
            IsBackground = true
        };
        thread1.Start();

        var thread2Ping = new Thread(NexPing0AutoCheck) //tna
        {
            IsBackground = true
        };
        thread2Ping.Start();

        var thread3BFBAN = new Thread(NexBFBAN0AutoCheck) //tna
        {
            IsBackground = true
        };
        thread3BFBAN.Start();

        var thread4AutoRun = new Thread(NexStartUpFileCheck)
        {
            IsBackground = true
        };
        thread4AutoRun.Start();

        if (!File.Exists(FileUtil.F_Rule_Path))
        {
            for (int i = 0; i < 10; i++)
            {
                RuleConfigs.Add(new RuleConfig()
                {
                    RuleName = $"CustomRule {i}",
                    RuleInfos = new RuleConfig.RuleInfo()
                    {
                        Team1Normal = new RuleConfig.RuleInfo.Normal()
                        {
                            MaxKill = 0,
                            KDFlag = 0,
                            MaxKD = 0.00f,
                            KPMFlag = 0,
                            MaxKPM = 0.00f,
                            MinRank = 0,
                            MaxRank = 0,
                            LifeMaxKD = 0.00f,
                            LifeMaxKPM = 0.00f,
                            LifeMaxWeaponStar = 0,
                            LifeMaxVehicleStar = 0
                        },
                        Team2Normal = new RuleConfig.RuleInfo.Normal()
                        {
                            MaxKill = 0,
                            KDFlag = 0,
                            MaxKD = 0.00f,
                            KPMFlag = 0,
                            MaxKPM = 0.00f,
                            MinRank = 0,
                            MaxRank = 0,
                            LifeMaxKD = 0.00f,
                            LifeMaxKPM = 0.00f,
                            LifeMaxWeaponStar = 0,
                            LifeMaxVehicleStar = 0
                        },
                        Team1Weapon = new List<string>() { },
                        Team2Weapon = new List<string>() { },
                        BlackList = new List<string>() { },
                        WhiteList = new List<string>() { }
                    }
                });
            }

            File.WriteAllText(FileUtil.F_Rule_Path, JsonUtil.JsonSeri(RuleConfigs));
        }

        if (File.Exists(FileUtil.F_Rule_Path))
        {
            using (var streamReader = new StreamReader(FileUtil.F_Rule_Path))
            {
                RuleConfigs = JsonUtil.JsonDese<List<RuleConfig>>(streamReader.ReadToEnd());

                foreach (var item in RuleConfigs)
                {
                    ComboBox_ConfigNames.Add(item.RuleName);
                }

                ApplyRuleByIndex(0);
            }
        }
        Thread.Sleep(4500);
        NexVerifySessionIDStartup();        
    }

    private void MainWindow_ClosingDisposeEvent()
    {
        File.WriteAllText(FileUtil.F_Rule_Path, JsonUtil.JsonSeri(RuleConfigs));
    }

    /// <summary>
    /// 应用规则
    /// </summary>
    /// <param name="index"></param>
    private void ApplyRuleByIndex(int index)
    {
        var rule = RuleConfigs[index].RuleInfos;

        RuleTeam1Model.MaxKill = rule.Team1Normal.MaxKill;
        RuleTeam1Model.KDFlag = rule.Team1Normal.KDFlag;
        RuleTeam1Model.MaxKD = rule.Team1Normal.MaxKD;
        RuleTeam1Model.KPMFlag = rule.Team1Normal.KPMFlag;
        RuleTeam1Model.MaxKPM = rule.Team1Normal.MaxKPM;
        RuleTeam1Model.MinRank = rule.Team1Normal.MinRank;
        RuleTeam1Model.MaxRank = rule.Team1Normal.MaxRank;
        RuleTeam1Model.LifeMaxKD = rule.Team1Normal.LifeMaxKD;
        RuleTeam1Model.LifeMaxKPM = rule.Team1Normal.LifeMaxKPM;
        RuleTeam1Model.LifeMaxWeaponStar = rule.Team1Normal.LifeMaxWeaponStar;
        RuleTeam1Model.LifeMaxVehicleStar = rule.Team1Normal.LifeMaxVehicleStar;

        RuleTeam2Model.MaxKill = rule.Team2Normal.MaxKill;
        RuleTeam2Model.KDFlag = rule.Team2Normal.KDFlag;
        RuleTeam2Model.MaxKD = rule.Team2Normal.MaxKD;
        RuleTeam2Model.KPMFlag = rule.Team2Normal.KPMFlag;
        RuleTeam2Model.MaxKPM = rule.Team2Normal.MaxKPM;
        RuleTeam2Model.MinRank = rule.Team2Normal.MinRank;
        RuleTeam2Model.MaxRank = rule.Team2Normal.MaxRank;
        RuleTeam2Model.LifeMaxKD = rule.Team2Normal.LifeMaxKD;
        RuleTeam2Model.LifeMaxKPM = rule.Team2Normal.LifeMaxKPM;
        RuleTeam2Model.LifeMaxWeaponStar = rule.Team2Normal.LifeMaxWeaponStar;
        RuleTeam2Model.LifeMaxVehicleStar = rule.Team2Normal.LifeMaxVehicleStar;

        ListBox_Custom_BlackList.Items.Clear();
        foreach (var item in rule.BlackList)
        {
            ListBox_Custom_BlackList.Items.Add(item);
        }

        ListBox_Custom_WhiteList.Items.Clear();
        foreach (var item in rule.WhiteList)
        {
            ListBox_Custom_WhiteList.Items.Add(item);
        }

        for (int i = 0; i < DataGrid_RuleWeaponModels.Count; i++)
        {
            var item = DataGrid_RuleWeaponModels[i];

            var v1 = rule.Team1Weapon.IndexOf(item.English);
            if (v1 != -1)
                item.Team1 = true;
            else
                item.Team1 = false;

            var v2 = rule.Team2Weapon.IndexOf(item.English);
            if (v2 != -1)
                item.Team2 = true;
            else
                item.Team2 = false;
        }
    }

    /// <summary>
    /// 保存规则
    /// </summary>
    /// <param name="index"></param>
    private void SaveRuleByIndex(int index)
    {
        var rule = RuleConfigs[index].RuleInfos;

        rule.Team1Normal.MaxKill = RuleTeam1Model.MaxKill;
        rule.Team1Normal.KDFlag = RuleTeam1Model.KDFlag;
        rule.Team1Normal.MaxKD = RuleTeam1Model.MaxKD;
        rule.Team1Normal.KPMFlag = RuleTeam1Model.KPMFlag;
        rule.Team1Normal.MaxKPM = RuleTeam1Model.MaxKPM;
        rule.Team1Normal.MinRank = RuleTeam1Model.MinRank;
        rule.Team1Normal.MaxRank = RuleTeam1Model.MaxRank;
        rule.Team1Normal.LifeMaxKD = RuleTeam1Model.LifeMaxKD;
        rule.Team1Normal.LifeMaxKPM = RuleTeam1Model.LifeMaxKPM;
        rule.Team1Normal.LifeMaxWeaponStar = RuleTeam1Model.LifeMaxWeaponStar;
        rule.Team1Normal.LifeMaxVehicleStar = RuleTeam1Model.LifeMaxVehicleStar;

        rule.Team2Normal.MaxKill = RuleTeam2Model.MaxKill;
        rule.Team2Normal.KDFlag = RuleTeam2Model.KDFlag;
        rule.Team2Normal.MaxKD = RuleTeam2Model.MaxKD;
        rule.Team2Normal.KPMFlag = RuleTeam2Model.KPMFlag;
        rule.Team2Normal.MaxKPM = RuleTeam2Model.MaxKPM;
        rule.Team2Normal.MinRank = RuleTeam2Model.MinRank;
        rule.Team2Normal.MaxRank = RuleTeam2Model.MaxRank;
        rule.Team2Normal.LifeMaxKD = RuleTeam2Model.LifeMaxKD;
        rule.Team2Normal.LifeMaxKPM = RuleTeam2Model.LifeMaxKPM;
        rule.Team2Normal.LifeMaxWeaponStar = RuleTeam2Model.LifeMaxWeaponStar;
        rule.Team2Normal.LifeMaxVehicleStar = RuleTeam2Model.LifeMaxVehicleStar;

        rule.BlackList.Clear();
        foreach (string item in ListBox_Custom_BlackList.Items)
        {
            rule.BlackList.Add(item);
        }

        rule.WhiteList.Clear();
        foreach (string item in ListBox_Custom_WhiteList.Items)
        {
            rule.WhiteList.Add(item);
        }

        rule.Team1Weapon.Clear();
        rule.Team2Weapon.Clear();
        for (int i = 0; i < DataGrid_RuleWeaponModels.Count; i++)
        {
            var item = DataGrid_RuleWeaponModels[i];
            if (item.Team1)
                rule.Team1Weapon.Add(item.English);

            if (item.Team2)
                rule.Team2Weapon.Add(item.English);
        }
    }

    ////////////////////////////////////////////////////////////////////////////////

    private void CheckState()
    {
        while (true)
        {
            if (string.IsNullOrEmpty(Vari.GameId))
            {
                if (!isHasBeenExec)
                {
                    this.Dispatcher.BeginInvoke(() =>
                    {
                        /*
                        if (ToggleButton_RunAutoKick.IsChecked == true)
                        {
                            ToggleButton_RunAutoKick.IsChecked = false;
                            Globals.AutoKickBreakPlayer = false;
                            CheckBox_Ping.IsChecked = false;//tna
                            Globals.AutoKickPing = false; //tna
                        }*/

                        
                    });

                    isHasBeenExec = true;
                }
            }
            Thread.Sleep(5000); //tna it was 1000
        }
    }

    ////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// 自动踢出生涯违规玩家
    /// </summary>
    private void AutoKickLifeBreakPlayer()
    {
        while (true)
        {
            // 自动踢出违规玩家
            if (Vari.AutoKickBreakPlayer)
            {
                var team1Player = JsonSerializer.Deserialize<List<PlayerData>>(JsonSerializer.Serialize(ScoreView.PlayerDatas_Team1));
                var team2Player = JsonSerializer.Deserialize<List<PlayerData>>(JsonSerializer.Serialize(ScoreView.PlayerDatas_Team2));

                foreach (var item in team1Player)
                {
                    CheckBreakLifePlayerTeam1(item);
                }

                foreach (var item in team2Player)
                {
                    CheckBreakLifePlayerTeam2(item);
                }
            }
            else
            {
                try
                {
                    this.Dispatcher.Invoke(() =>
                    {
                        ToggleButton_RunAutoKick.IsChecked = false;
                    });                    
                }
                catch (Exception ex)
                {
                    Log.Ex(ex);
                }
            }

            Thread.Sleep(5000);
        }
    }

    /// <summary>
    /// 检查队伍1生涯违规玩家
    /// </summary>
    /// <param name="data"></param>
    private async void CheckBreakLifePlayerTeam1(PlayerData data)
    {
        // 跳过管理员
        if(NexBoolPlayerIsNotAdminOrWhitelisted(data.Name, data.PersonaId) == false)
        {
            return;
        }

        // 限制玩家生涯KD、KPM
        if (ServerRule.Team1.LifeMaxKD != 0 || ServerRule.Team1.LifeMaxKPM != 0)
        {
            var result = await BF1API.DetailedStatsByPersonaId(data.PersonaId);
            if (result.IsSuccess)
            {
                var detailedStats = JsonUtil.JsonDese<DetailedStats>(result.Message);

                // 拿到该玩家的生涯数据
                int kills = detailedStats.result.basicStats.kills;
                int deaths = detailedStats.result.basicStats.deaths;

                float kd = (float)Math.Round((double)kills / deaths, 2);
                float kpm = detailedStats.result.basicStats.kpm;

                if (ServerRule.Team1.LifeMaxKD != 0 && kd > ServerRule.Team1.LifeMaxKD)
                {
                    AutoKickPlayer(new BreakRuleInfo
                    {
                        Name = data.Name,
                        PersonaId = data.PersonaId,
                        Reason = $"Life KD Limit {ServerRule.Team1.LifeMaxKD:0.00}"
                    });

                    return;
                }

                if (ServerRule.Team1.LifeMaxKPM != 0 && kpm > ServerRule.Team1.LifeMaxKPM)
                {
                    AutoKickPlayer(new BreakRuleInfo
                    {
                        Name = data.Name,
                        PersonaId = data.PersonaId,
                        Reason = $"Life KPM Limit {ServerRule.Team1.LifeMaxKPM:0.00}"
                    });

                    return;
                }                
            }
        }

        // 限制玩家武器最高星数
        if (ServerRule.Team1.LifeMaxWeaponStar != 0)
        {
            var result = await BF1API.GetWeaponsByPersonaId(data.PersonaId);
            if (result.IsSuccess)
            {
                var getWeapons = JsonUtil.JsonDese<GetWeapons>(result.Message);

                var weaponStar = new List<int>();
                foreach (var res in getWeapons.result)
                {
                    foreach (var wea in res.weapons)
                    {
                        if (wea.stats.values.kills == 0)
                            continue;

                        weaponStar.Add((int)wea.stats.values.kills);
                    }
                }

                if (weaponStar.Count == 0)
                    return;

                weaponStar.Sort((x, y) => -x.CompareTo(y));

                if (weaponStar[0] / 100 > ServerRule.Team1.LifeMaxWeaponStar)
                {
                    AutoKickPlayer(new BreakRuleInfo
                    {
                        Name = data.Name,
                        PersonaId = data.PersonaId,
                        Reason = $"Life Weapon Star Limit {ServerRule.Team1.LifeMaxWeaponStar:0}"
                    });

                    return;
                }
            }
        }

        // 限制玩家载具最高星数
        if (ServerRule.Team1.LifeMaxVehicleStar != 0)
        {
            var result = await BF1API.GetVehiclesByPersonaId(data.PersonaId);
            if (result.IsSuccess)
            {
                var getVehicles = JsonUtil.JsonDese<GetVehicles>(result.Message);

                var vehicleStar = new List<int>();
                foreach (var res in getVehicles.result)
                {
                    foreach (var veh in res.vehicles)
                    {
                        if (veh.stats.values.kills == 0)
                            continue;

                        vehicleStar.Add((int)veh.stats.values.kills);
                    }
                }

                if (vehicleStar.Count == 0)
                    return;

                vehicleStar.Sort((x, y) => -x.CompareTo(y));

                if (vehicleStar[0] / 100 > ServerRule.Team1.LifeMaxVehicleStar)
                {
                    AutoKickPlayer(new BreakRuleInfo
                    {
                        Name = data.Name,
                        PersonaId = data.PersonaId,
                        Reason = $"Life Vehicle Star Limit {ServerRule.Team1.LifeMaxVehicleStar:0}"
                    });

                    return;
                }
            }
        }
    }

    /// <summary>
    /// 检查队伍2生涯违规玩家
    /// </summary>
    /// <param name="data"></param>
    private async void CheckBreakLifePlayerTeam2(PlayerData data)
    {
        // 跳过管理员
        if (NexBoolPlayerIsNotAdminOrWhitelisted(data.Name, data.PersonaId) == false)
        {
            return;
        }

        // 限制玩家生涯KD、KPM
        if (ServerRule.Team2.LifeMaxKD != 0 || ServerRule.Team2.LifeMaxKPM != 0)
        {
            var result = await BF1API.DetailedStatsByPersonaId(data.PersonaId);
            if (result.IsSuccess)
            {
                var detailedStats = JsonUtil.JsonDese<DetailedStats>(result.Message);

                // 拿到该玩家的生涯数据
                int kills = detailedStats.result.basicStats.kills;
                int deaths = detailedStats.result.basicStats.deaths;

                float kd = (float)Math.Round((double)kills / deaths, 2);
                float kpm = detailedStats.result.basicStats.kpm;

                if (ServerRule.Team2.LifeMaxKD != 0 && kd > ServerRule.Team2.LifeMaxKD)
                {
                    AutoKickPlayer(new BreakRuleInfo
                    {
                        Name = data.Name,
                        PersonaId = data.PersonaId,
                        Reason = $"Life KD Limit {ServerRule.Team2.LifeMaxKD:0.00}"
                    });

                    return;
                }

                if (ServerRule.Team2.LifeMaxKPM != 0 && kpm > ServerRule.Team2.LifeMaxKPM)
                {
                    AutoKickPlayer(new BreakRuleInfo
                    {
                        Name = data.Name,
                        PersonaId = data.PersonaId,
                        Reason = $"Life KPM Limit {ServerRule.Team2.LifeMaxKPM:0.00}"
                    });

                    return;
                }
            }
        }

        // 限制玩家武器最高星数
        if (ServerRule.Team2.LifeMaxWeaponStar != 0)
        {
            var result = await BF1API.GetWeaponsByPersonaId(data.PersonaId);
            if (result.IsSuccess)
            {
                var getWeapons = JsonUtil.JsonDese<GetWeapons>(result.Message);

                var weaponStar = new List<int>();
                foreach (var res in getWeapons.result)
                {
                    foreach (var wea in res.weapons)
                    {
                        if (wea.stats.values.kills == 0)
                            continue;

                        weaponStar.Add((int)wea.stats.values.kills);
                    }
                }

                if (weaponStar.Count == 0)
                    return;

                weaponStar.Sort((x, y) => -x.CompareTo(y));

                if (weaponStar[0] / 100 > ServerRule.Team2.LifeMaxWeaponStar)
                {
                    AutoKickPlayer(new BreakRuleInfo
                    {
                        Name = data.Name,
                        PersonaId = data.PersonaId,
                        Reason = $"Life Weapon Star Limit {ServerRule.Team2.LifeMaxWeaponStar:0}"
                    });

                    return;
                }
            }
        }

        // 限制玩家载具最高星数
        if (ServerRule.Team2.LifeMaxVehicleStar != 0)
        {
            var result = await BF1API.GetVehiclesByPersonaId(data.PersonaId);
            if (result.IsSuccess)
            {
                var getVehicles = JsonUtil.JsonDese<GetVehicles>(result.Message);

                var vehicleStar = new List<int>();
                foreach (var res in getVehicles.result)
                {
                    foreach (var veh in res.vehicles)
                    {
                        if (veh.stats.values.kills == 0)
                            continue;

                        vehicleStar.Add((int)veh.stats.values.kills);
                    }
                }

                if (vehicleStar.Count == 0)
                    return;

                vehicleStar.Sort((x, y) => -x.CompareTo(y));

                if (vehicleStar[0] / 100 > ServerRule.Team2.LifeMaxVehicleStar)
                {
                    AutoKickPlayer(new BreakRuleInfo
                    {
                        Name = data.Name,
                        PersonaId = data.PersonaId,
                        Reason = $"Life Vehicle Star Limit {ServerRule.Team2.LifeMaxVehicleStar:0}"
                    });

                    return;
                }
            }
        }
    }

    /// <summary>
    /// 自动踢出普通违规玩家
    /// </summary>
    /// <param name="info"></param>
    private async void AutoKickPlayer(BreakRuleInfo info)
    {
        var result = await BF1API.AdminKickPlayer(info.PersonaId, info.Reason);
        if (result.IsSuccess)
        {
            info.Status = "kicked out";
            info.Time = DateTime.Now;
            LogView._dAddKickOKLog(info);
            await DWebHooks.LogOK(info);
        }
        else
        {
            info.Status = "Kick failed " + result.Message;
            info.Time = DateTime.Now;
            LogView._dAddKickNOLog(info);
            await DWebHooks.LogNO(info);
        }
    }

    ////////////////////////////////////////////////////////////////////////////////

    private void AppendLog(string msg)
    {
        try
        {
            this.Dispatcher.Invoke(() =>
            {
                TextBox_RuleLog.AppendText(msg + "\n");
            });            
        }
        catch (Exception ex)
        {
            Log.Ex(ex);
        }
    }

    private void Button_ApplyRule_Click(object sender, RoutedEventArgs e) //moved code to NexApplyRules(
    {
        AudioUtil.ClickSound();
        NexApplyRules();        
    }

    private void NexApplyRules() //tna
    {
        TextBox_RuleLog.Clear();

        AppendLog("===== Operation time =====");
        AppendLog("");
        AppendLog($"{DateTime.Now:yyyy/MM/dd HH:mm:ss}");
        AppendLog("");

        ServerRule.Team1.MaxKill = RuleTeam1Model.MaxKill;
        ServerRule.Team1.KDFlag = RuleTeam1Model.KDFlag;
        ServerRule.Team1.MaxKD = RuleTeam1Model.MaxKD;
        ServerRule.Team1.KPMFlag = RuleTeam1Model.KPMFlag;
        ServerRule.Team1.MaxKPM = RuleTeam1Model.MaxKPM;
        ServerRule.Team1.MinRank = RuleTeam1Model.MinRank;
        ServerRule.Team1.MaxRank = RuleTeam1Model.MaxRank;

        ServerRule.Team2.MaxKill = RuleTeam2Model.MaxKill;
        ServerRule.Team2.KDFlag = RuleTeam2Model.KDFlag;
        ServerRule.Team2.MaxKD = RuleTeam2Model.MaxKD;
        ServerRule.Team2.KPMFlag = RuleTeam2Model.KPMFlag;
        ServerRule.Team2.MaxKPM = RuleTeam2Model.MaxKPM;
        ServerRule.Team2.MinRank = RuleTeam2Model.MinRank;
        ServerRule.Team2.MaxRank = RuleTeam2Model.MaxRank;

        ServerRule.Team1.LifeMaxKD = RuleTeam1Model.LifeMaxKD;
        ServerRule.Team1.LifeMaxKPM = RuleTeam1Model.LifeMaxKPM;
        ServerRule.Team1.LifeMaxWeaponStar = RuleTeam1Model.LifeMaxWeaponStar;
        ServerRule.Team1.LifeMaxVehicleStar = RuleTeam1Model.LifeMaxVehicleStar;

        ServerRule.Team2.LifeMaxKD = RuleTeam2Model.LifeMaxKD;
        ServerRule.Team2.LifeMaxKPM = RuleTeam2Model.LifeMaxKPM;
        ServerRule.Team2.LifeMaxWeaponStar = RuleTeam2Model.LifeMaxWeaponStar;
        ServerRule.Team2.LifeMaxVehicleStar = RuleTeam2Model.LifeMaxVehicleStar;

        /////////////////////////////////////////////////////////////////////////////

        if (ServerRule.Team1.MinRank >= ServerRule.Team1.MaxRank && ServerRule.Team1.MinRank != 0 && ServerRule.Team1.MaxRank != 0)
        {
            Vari.IsRuleSetRight = false;
            isApplyRule = false;

            AppendLog($"Team 1 Restriction Level rules are set incorrectly");
            AppendLog("");

            NotifierHelper.Show(NotifierType.Warning, $"Team 1 Restriction Level rules are set incorrectly");

            return;
        }

        if (ServerRule.Team2.MinRank >= ServerRule.Team2.MaxRank && ServerRule.Team2.MinRank != 0 && ServerRule.Team2.MaxRank != 0)
        {
            Vari.IsRuleSetRight = false;
            isApplyRule = false;

            AppendLog($"Team 1 Restriction Level rules are set incorrectly");
            AppendLog("");

            NotifierHelper.Show(NotifierType.Warning, $"Team 2 Restriction level rules are not set correctly");

            return;
        }

        /////////////////////////////////////////////////////////////////////////////

        // 清空限制武器列表
        Vari.Custom_WeaponList_Team1.Clear();
        Vari.Custom_WeaponList_Team2.Clear();
        // 添加自定义限制武器
        foreach (var item in DataGrid_RuleWeaponModels)
        {
            if (item.Team1)
            {
                Vari.Custom_WeaponList_Team1.Add(item.English);
            }

            if (item.Team2)
            {
                Vari.Custom_WeaponList_Team2.Add(item.English);
            }
        }

        // 清空黑名单列表
        Vari.Custom_BlackList.Clear();
        // 添加自定义黑名单列表
        foreach (var item in ListBox_Custom_BlackList.Items)
        {
            Vari.Custom_BlackList.Add(item as string);
        }

        // 清空白名单列表
        Vari.Custom_WhiteList.Clear();
        // 添加自定义白名单列表
        foreach (var item in ListBox_Custom_WhiteList.Items)
        {
            Vari.Custom_WhiteList.Add(item as string);
        }

        if (ToggleButton_RunAutoKick.IsChecked == true)
        {
            ToggleButton_RunAutoKick.IsChecked = false;
            //Globals.AutoKickBreakPlayer = false;
        }

        Vari.IsRuleSetRight = true;
        isApplyRule = true;

        AppendLog($"Successfully submitted the current rules, please restart the automatic kick function");
        AppendLog("");

        //NotifierHelper.Show(NotifierType.Success, $"The current rule is applied successfully, please click <Query current rule> to check whether the rule is correct");

        Task.Run(() =>
        {
            Task.Delay(30000).Wait();

            isApplyRule = false;
        });
    }

    private void Button_QueryRule_Click(object sender, RoutedEventArgs e)
    {
        AudioUtil.ClickSound();

        TextBox_RuleLog.Clear();

        AppendLog("===== Query Time =====");
        AppendLog("");
        AppendLog($"{DateTime.Now:yyyy/MM/dd HH:mm:ss}");
        AppendLog("");

        AppendLog("==== Team 1 ====");
        AppendLog("");
        AppendLog($"MaxKill : {ServerRule.Team1.MaxKill}");

        AppendLog($"KDFlag : {ServerRule.Team1.KDFlag}");
        AppendLog($"MaxKD : {ServerRule.Team1.MaxKD}");

        AppendLog($"KPMFlag : {ServerRule.Team1.KPMFlag}");
        AppendLog($"MaxKPM : {ServerRule.Team1.MaxKPM}");

        AppendLog($"MinRank : {ServerRule.Team1.MinRank}");
        AppendLog($"MaxRank : {ServerRule.Team1.MaxRank}");

        AppendLog($"LifeMaxKD : {ServerRule.Team1.LifeMaxKD}");
        AppendLog($"LifeMaxKPM : {ServerRule.Team1.LifeMaxKPM}");

        AppendLog($"LifeMaxWeaponStar : {ServerRule.Team1.LifeMaxWeaponStar}");
        AppendLog($"LifeMaxVehicleStar : {ServerRule.Team1.LifeMaxVehicleStar}");
        AppendLog("");

        AppendLog("==== Team 2 ====");
        AppendLog("");
        AppendLog($"MaxKill : {ServerRule.Team2.MaxKill}");

        AppendLog($"KDFlag : {ServerRule.Team2.KDFlag}");
        AppendLog($"MaxKD : {ServerRule.Team2.MaxKD}");

        AppendLog($"KPMFlag : {ServerRule.Team2.KPMFlag}");
        AppendLog($"MaxKPM : {ServerRule.Team2.MaxKPM}");

        AppendLog($"MinRank : {ServerRule.Team2.MinRank}");
        AppendLog($"MaxRank : {ServerRule.Team2.MaxRank}");

        AppendLog($"LifeMaxKD : {ServerRule.Team2.LifeMaxKD}");
        AppendLog($"LifeMaxKPM : {ServerRule.Team2.LifeMaxKPM}");

        AppendLog($"LifeMaxWeaponStar : {ServerRule.Team2.LifeMaxWeaponStar}");
        AppendLog($"LifeMaxVehicleStar : {ServerRule.Team2.LifeMaxVehicleStar}");

        AppendLog("\n");

        AppendLog($"========== Team 1 Banned Weapons List ==========");
        AppendLog("");
        foreach (var item in Vari.Custom_WeaponList_Team1)
        {
            AppendLog($"weapon name {Vari.Custom_WeaponList_Team1.IndexOf(item) + 1} : {item}");
        }
        AppendLog("\n");

        AppendLog($"========== Team 2 Banned Weapons List ==========");
        AppendLog("");
        foreach (var item in Vari.Custom_WeaponList_Team2)
        {
            AppendLog($"weapon name {Vari.Custom_WeaponList_Team2.IndexOf(item) + 1} : {item}");
        }
        AppendLog("\n");

        AppendLog($"========== Blacklist ==========");
        AppendLog("");
        foreach (var item in Vari.Custom_BlackList)
        {
            AppendLog($"player ID {Vari.Custom_BlackList.IndexOf(item) + 1} : {item}");
        }
        AppendLog("\n");

        AppendLog($"========== Whitelist ==========");
        AppendLog("");
        foreach (var item in Vari.Custom_WhiteList)
        {
            AppendLog($"playerID {Vari.Custom_WhiteList.IndexOf(item) + 1} : {item}");
        }
        AppendLog("\n");

        NotifierHelper.Show(NotifierType.Success, $"The query of the current rules is successful, please click <Check the offending players> to test whether it is correct");
    }
    
    private async void Button_Discord_Click(object sender, RoutedEventArgs e)
    {
        AudioUtil.ClickSound();
        await SexusBot.Toggle();
    }

    private void Button_Add_BlackList_Click(object sender, RoutedEventArgs e)
    {
        AudioUtil.ClickSound();

        if (TextBox_BlackList_PlayerName.Text != "")
        {
            bool isContains = false;

            foreach (var item in ListBox_Custom_BlackList.Items)
            {
                if ((item as string) == TextBox_BlackList_PlayerName.Text)
                {
                    isContains = true;
                }
            }

            if (!isContains)
            {
                ListBox_Custom_BlackList.Items.Add(TextBox_BlackList_PlayerName.Text);

                NotifierHelper.Show(NotifierType.Success, $"Added {TextBox_BlackList_PlayerName.Text} to the blacklistsss successfully");
                TextBox_BlackList_PlayerName.Text = "";
            }
            else
            {
                NotifierHelper.Show(NotifierType.Warning, $"Addes {TextBox_BlackList_PlayerName.Text} to the blacklistsss unsuccessfully");
                TextBox_BlackList_PlayerName.Text = "";
            }
        }
        else
        {
            NotifierHelper.Show(NotifierType.Warning, $"The ID of the player to be added to the blacklist is empty, and the add operation is canceled");
        }
    }

    private void Button_Remove_BlackList_Click(object sender, RoutedEventArgs e)
    {
        AudioUtil.ClickSound();

        if (ListBox_Custom_BlackList.SelectedIndex != -1)
        {
            NotifierHelper.Show(NotifierType.Success, $"Remove from blacklist（{ListBox_Custom_BlackList.SelectedItem}）success");
            ListBox_Custom_BlackList.Items.Remove(ListBox_Custom_BlackList.SelectedItem);
        }
        else
        {
            NotifierHelper.Show(NotifierType.Warning, $"Please select the player ID you want to delete correctly or the custom blacklist list is empty, the delete operation is canceled");
        }
    }

    private void Button_Clear_BlackList_Click(object sender, RoutedEventArgs e)
    {
        AudioUtil.ClickSound();

        // 清空黑名单列表
        Vari.Custom_BlackList.Clear();
        ListBox_Custom_BlackList.Items.Clear();

        NotifierHelper.Show(NotifierType.Success, $"Clear the blacklist list successfully");
    }

    private void Button_Add_WhiteList_Click(object sender, RoutedEventArgs e)
    {
        AudioUtil.ClickSound();

        if (TextBox_WhiteList_PlayerName.Text != "")
        {
            bool isContains = false;

            foreach (var item in ListBox_Custom_WhiteList.Items)
            {
                if ((item as string) == TextBox_WhiteList_PlayerName.Text)
                {
                    isContains = true;
                }
            }

            if (!isContains)
            {
                ListBox_Custom_WhiteList.Items.Add(TextBox_WhiteList_PlayerName.Text);

                NotifierHelper.Show(NotifierType.Success, $"Added {TextBox_WhiteList_PlayerName.Text} to the Whitelist successfully");

                TextBox_WhiteList_PlayerName.Text = "";
            }
            else
            {
                NotifierHelper.Show(NotifierType.Warning, $"Added {TextBox_WhiteList_PlayerName.Text} It already exists, please don't add it again");
                TextBox_WhiteList_PlayerName.Text = "";
            }
        }
        else
        {
            NotifierHelper.Show(NotifierType.Warning, $"The ID of the player to be added to the whitelist is empty, and the add operation is canceled");
        }
    }

    private void Button_Remove_WhiteList_Click(object sender, RoutedEventArgs e)
    {
        AudioUtil.ClickSound();

        if (ListBox_Custom_WhiteList.SelectedIndex != -1)
        {
            NotifierHelper.Show(NotifierType.Success, $"Remove from whitelist（{ListBox_Custom_WhiteList.SelectedItem}）successfully");
            ListBox_Custom_WhiteList.Items.Remove(ListBox_Custom_WhiteList.SelectedItem);
        }
        else
        {
            NotifierHelper.Show(NotifierType.Warning, $"Please select the player ID you want to delete correctly or the custom whitelist list is empty, the delete operation is canceled");
        }
    }

    private void Button_Clear_WhiteList_Click(object sender, RoutedEventArgs e)
    {
        AudioUtil.ClickSound();

        // 清空白名单列表
        Vari.Custom_WhiteList.Clear();
        ListBox_Custom_WhiteList.Items.Clear();

        NotifierHelper.Show(NotifierType.Success, $"Clear the blank list list successfully");
    }

    /// <summary>
    /// 检查自动踢人环境是否合格
    /// </summary>
    /// <returns></returns>
    private async Task<bool> CheckKickEnv()
    {
        TextBox_RuleLog.Clear();

        //NotifierHelper.Show(NotifierType.Information, $"Checking environment...");

        AppendLog("===== Date =====");
        AppendLog("");
        AppendLog($"{DateTime.Now:yyyy/MM/dd HH:mm:ss}");

        AppendLog("");
        AppendLog("Checking if rules are applied...");
        NexApplyRuleAutomatic();
        AppendLog("✔ Rules applied correctly");
        /*
        if (!isApplyRule)
        {
            AppendLog("❌ The player did not apply the rules correctly, please Apply rules again");
            NotifierHelper.Show(NotifierType.Warning, $"Environment check failed, operation canceled");
            return false;
        }
        else
        {
            AppendLog("✔ Rules applied correctly");
        }*/

        AppendLog("");
        AppendLog("Checking if SessionId is correct...");
        if (string.IsNullOrEmpty(Vari.SessionId))
        {
            AppendLog("❌ The SessionId is empty, please obtain the SessionId first, and the operation is canceled");
            NotifierHelper.Show(NotifierType.Warning, $"The SessionId is empty");
            return false;
        }
        else
        {
            AppendLog("✔ SessionId is correct");
        }

        AppendLog("");
        AppendLog("Checking if SessionId is valid...");
        var result = await BF1API.GetWelcomeMessage();
        if (!result.IsSuccess)
        {
            AppendLog("❌ The SessionId has expired, please refresh the SessionId and the operation will be canceled");
            NotifierHelper.Show(NotifierType.Warning, $"The SessionId has expired");
            return false;
        }
        else
        {
            AppendLog("✔ SessionId is valid");
        }

        AppendLog("");
        AppendLog("Checking if GameID is correct...");
        if (string.IsNullOrEmpty(Vari.GameId))
        {
            AppendLog("❌ GameId is empty, please enter the server first and cancel the operation");
            NotifierHelper.Show(NotifierType.Warning, $"❌ GameId is empty, please enter the server first and cancel the operation");
            return false;
        }
        else
        {
            AppendLog("✔ GameID is correct");
        }

        AppendLog("");
        AppendLog("Checking if server administrator list is correct...");
        if (Vari.Server_AdminList_PID.Count == 0)
        {
            AppendLog("❌ Fail. Make sure you are in the server. (One that's not a DICE Offical one).");
            AppendLog("❌ Tip: Go to 'Server' and press 'Get Current Server Details'");            
            NotifierHelper.Show(NotifierType.Warning, $"Environment check failed, operation canceled");
            return false;
        }
        else
        {
            AppendLog("✔ Server administrator list check correct");
        }

        /*AppendLog("");
        AppendLog("Checking if current Server is Whitelisted to use new functions....");
        if (Vari.NexGlobalWhiteListServers.Contains(Vari.GameId))
        {            
            AppendLog("✔Server is whitelisted to use all functions!");
        }
        else
        {            
            AppendLog("❌Server is not whitelisted to use the three new functions below!");
            AppendLog("All old functions will still work though :)");
            AppendLog($"Gameid: {Vari.GameId}");
            NotifierHelper.Show(NotifierType.Warning, $"❌Server is not whitelisted to use the three new functions below!");
        }*/

        AppendLog("");        
        string welcomeMsg = Vari.WelcomeMessageNex;
        string[] splitwords = welcomeMsg.Split(' ');
        string lastword = splitwords[splitwords.Length - 1];
        string username = lastword.Remove(lastword.Length - 1);
        Vari.UserNameOfUser = username;
        AppendLog($"Checking if the player '{username}' is admin for the current server...");        
        if (Vari.Server_AdminList_Name.Contains(Vari.UserNameOfUser))
        {
            AppendLog("✔ Confirmed, player is in Admin List of current Server");            
        }
        else
        {
            AppendLog("❌ The player is not admin of current server. Go to 'Server' and press 'Get Current Server Details");            
            NotifierHelper.Show(NotifierType.Warning, "❌ The player is not admin of current server. Go to 'Server' and press 'Get Current Server Details");
            return false;
        }

        await DWebHooks.LogMonitoringON();
        return true;
    }

    // 开启自动踢人
    private async void ToggleButton_RunAutoKick_Click(object sender, RoutedEventArgs e)
    {
        if (ToggleButton_RunAutoKick.IsChecked == true)
        {
            // 检查自动踢人环境
            ToggleButton_RunAutoKick.IsChecked = true;
            try //tna
            {
                ToggleButton_RunAutoKick.IsChecked = true;
                if (await CheckKickEnv())
                {
                    ToggleButton_RunAutoKick.IsChecked = true;
                    isHasBeenExec = false;
                    Vari.AutoKickBreakPlayer = true;                    
                    NotifierHelper.Show(NotifierType.Success, $"Automatic kick is turned on successfully");
                    AudioUtil.ClickSound();
                }
                else
                {
                    Vari.AutoKickBreakPlayer = false;
                    ToggleButton_RunAutoKick.IsChecked = false;
                }
            }
            catch (Exception ex) //tna
            {
                Log.Ex(ex);
                Vari.AutoKickBreakPlayer = false;
                ToggleButton_RunAutoKick.IsChecked = false;
            }
            
        }
        else
        {
            Vari.AutoKickBreakPlayer = false;
            ToggleButton_RunAutoKick.IsChecked = false;
            NotifierHelper.Show(NotifierType.Success, $"Automatically kick people off");
        }
    }

    private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
    {
        ProcessUtil.OpenLink(e.Uri.OriginalString);
        e.Handled = true;
    }

    private void Button_OpenConfigurationFolder_Click(object sender, RoutedEventArgs e)
    {
        AudioUtil.ClickSound();

        ProcessUtil.OpenLink(FileUtil.Default_Path);
    }

    

    private async void Button_CheckKickEnv_Click(object sender, RoutedEventArgs e)
    {
        AudioUtil.ClickSound();        
        if (await CheckKickEnv())
        {
            AppendLog("");
            AppendLog("After the environmental inspection is completed, the automatic kick can be turned on");

            NotifierHelper.Show(NotifierType.Success, $"After the environmental inspection is completed, the automatic kick can be turned on");
        }
    }

    private void Button_ReNameRule_Click(object sender, RoutedEventArgs e)
    {
        AudioUtil.ClickSound();

        var name = TextBox_ReNameRule.Text.Trim();
        if (string.IsNullOrEmpty(name))
            return;

        var index = ComboBox_CustomConfigName.SelectedIndex;
        if (index == -1)
            return;

        ComboBox_ConfigNames[index] = name;
        RuleConfigs[index].RuleName = name;

        ComboBox_CustomConfigName.SelectedIndex = index;
    }

    private void Button_SaveCurrentRule_Click(object sender, RoutedEventArgs e)
    {
        AudioUtil.ClickSound();

        var index = ComboBox_CustomConfigName.SelectedIndex;
        if (index == -1)
            return;

        SaveRuleByIndex(index);
    }

    private void ComboBox_CustomConfigName_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        var index = ComboBox_CustomConfigName.SelectedIndex;
        if (index == -1)
            return;

        ApplyRuleByIndex(index);
    }

    private async void Button_ManualKickBreakRulePlayer_Click(object sender, RoutedEventArgs e)
    {
        AudioUtil.ClickSound();

        // 检查自动踢人环境
        if (await CheckKickEnv())
        {
            AppendLog("");
            AppendLog("After the environment check is completed, the manual kicking operation is successful.Please check the log for the execution result.");

            for (int i = 0; i < Vari.BreakRuleInfo_PlayerList.Count; i++)
            {
                await ManualKickPlayer(Vari.BreakRuleInfo_PlayerList[i]);
            }

            var team1Player = JsonSerializer.Deserialize<List<PlayerData>>(JsonSerializer.Serialize(ScoreView.PlayerDatas_Team1));
            var team2Player = JsonSerializer.Deserialize<List<PlayerData>>(JsonSerializer.Serialize(ScoreView.PlayerDatas_Team2));

            foreach (var item in team1Player)
            {
                CheckBreakLifePlayerTeam1(item);
            }

            foreach (var item in team2Player)
            {
                CheckBreakLifePlayerTeam2(item);
            }

            /*if (Vari.NexGlobalWhiteListServers.Contains(Vari.GameId) == true) //tna
            {
                NexPing1Initiate();
                NexBFBAN1Scan();
                NotifierHelper.Show(NotifierType.Success, "✔Manual Kick successfull.");
            }
            else
            {
                NotifierHelper.Show(NotifierType.Success, "✔Default Manual Kick successfull. Choose a whitelisted Server to execute Ping and BFBAN Scan!!");
            }*/

            await NexPing1Initiate();
            NexBFBAN1Scan();
            NotifierHelper.Show(NotifierType.Success, "✔Manual Kick successfull.");
        }
    }

    private async Task ManualKickPlayer(BreakRuleInfo info)
    {
        if (NexBoolPlayerIsNotAdminOrWhitelisted(info.Name, info.PersonaId) == true) //nex
        {
            var result = await BF1API.AdminKickPlayer(info.PersonaId, info.Reason);

            if (result.IsSuccess)
            {
                info.Status = "kicked out";
                LogView._dAddKickOKLog(info);
                await DWebHooks.LogOK(info);
            }
            else
            {
                info.Status = "Kick failed " + result.Message;
                LogView._dAddKickNOLog(info);
                await DWebHooks.LogNO(info);
            }
        }
        else //nex
        {
            BreakRuleInfo info2 = info;
            info2.Reason = info.Reason + " (ADMIN OR WHITELISTED)";
            LogView._dAddKickNOLog(info2);
        }
    }


































    //----------------------------------------------------------------------- TNA ------------------------------------------------------------------------------------------------------------------------------

    private bool NexApplyRuleAutomatic() //tna
    {
        if (isApplyRule == false)
        {
            NexApplyRules();
        }
        return isApplyRule;
    }
    private async void NexVerifySessionIDStartup() //cant be task bc constructor calls this
    {
        if (!string.IsNullOrEmpty(Vari.SessionId))
        {
            await BF1API.SetAPILocale();
            var result = await BF1API.GetWelcomeMessage();
            if (result.IsSuccess)
            {
                var welcomeMsg = JsonUtil.JsonDese<WelcomeMsg>(result.Message);
                var msg = welcomeMsg.result.firstMessage;
                Vari.WelcomeMessageNex = msg;
            }
        }
    }
    private bool NexBoolPlayerIsNotAdminOrWhitelisted(string name, long pid)
    {
        try
        {
            if (!Vari.Server_AdminList_PID.Contains(pid))
            {
                if (!Vari.Custom_WhiteList.Contains(name))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }
        catch
        {
            return false;
        }        
    }
    private void NexUnixTimeDifferenceGameTools(string unixstring, bool isforPing)
    {
        double timestampD = Double.Parse(unixstring);
        DateTime dateTime0 = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
        DateTime dateTimeOfEvent = dateTime0.AddSeconds(timestampD).ToLocalTime();
        double timedifferencelong = (DateTime.Now - dateTimeOfEvent).TotalSeconds;
        //string dateTimeTime = dateTimeOld.ToLongTimeString();
        int timedifferenceintSeconds = Convert.ToInt32(timedifferencelong);
        if (timedifferenceintSeconds > 60)
        {
            TimeSpan t = TimeSpan.FromSeconds(timedifferenceintSeconds);
            int timedifferenceintMinutes = t.Minutes;
            if (isforPing == true)
            {
                NotifierHelper.Show(NotifierType.None, $"❌Ping-Check cancelled, Gametools is sending an old request from {timedifferenceintMinutes} minute(s) ago.");
            }
            else
            {
                NotifierHelper.Show(NotifierType.None, $"❌BFBAN-Check cancelled, Gametools is sending an old request from {timedifferenceintMinutes} minute(s) ago.");
            }
        }
        else if (timedifferenceintSeconds > 3600)
        {
            TimeSpan t = TimeSpan.FromSeconds(timedifferenceintSeconds);
            int timedifferenceintHours = t.Hours;
            if (isforPing == true)
            {
                NotifierHelper.Show(NotifierType.None, $"❌Ping-Check cancelled, Gametools is sending an old request from {timedifferenceintHours} hour(s) ago.");
            }
            else
            {
                NotifierHelper.Show(NotifierType.None, $"❌BFBAN-Check cancelled, Gametools is sending an old request from {timedifferenceintHours} hour(s) ago.");
            }
        }
        else
        {
            if (isforPing == true)
            {
                NotifierHelper.Show(NotifierType.None, $"❌Ping-Check cancelled, Gametools is sending an old request from {timedifferenceintSeconds} seconds ago.");
            }
            else
            {
                NotifierHelper.Show(NotifierType.None, $"❌BFBAN-Check cancelled, Gametools is sending an old request from {timedifferenceintSeconds} seconds ago.");
            }
        }
    }




    private async Task<bool> NexGetGametoolsServerInfoJson(bool isforPing) //tna
    {
        string gtserverinfojson = await HttpHelper.HttpClientGET($"https://api.gametools.network/bf1/players/?gameid={Vari.GameId}");

        if (String.IsNullOrEmpty(gtserverinfojson) == false)
        {
            if (String.IsNullOrEmpty(Vari.gtserverinfojsonPrevious) == true || (String.IsNullOrEmpty(Vari.gtserverinfojsonPrevious) == false && Vari.gtserverinfojsonPrevious != gtserverinfojson))
            {
                try
                {
                    if (String.IsNullOrEmpty(Vari.gtserverinfojsonPrevious) == true)
                    {
                        Vari.gtserverinfojsonPrevious = gtserverinfojson;
                    }
                    
                    JsonNode node_full = JsonNode.Parse(gtserverinfojson);
                    var options = new JsonSerializerOptions { WriteIndented = true }; //from google

                    JsonNode node_teams = node_full!["teams"]!; //JsonArray         
                    JsonNode node_team1 = node_teams[0]!; //JsonObject
                    JsonNode node_team2 = node_teams[1]!; //JsonObject
                    JsonNode node_players1 = node_team1!["players"]!; //JsonArray
                    JsonNode node_players2 = node_team2!["players"]!; //JsonArray                                                                  

                    PlayerN[] playernarray1 = JsonSerializer.Deserialize<PlayerN[]>(node_players1);
                    PlayerN[] playernarray2 = JsonSerializer.Deserialize<PlayerN[]>(node_players2);

                    Vari.globalplayernarray1 = playernarray1;
                    Vari.globalplayernarray2 = playernarray2;
                    return true;
                }
                catch (Exception ex)
                {
                    Log.Ex(ex);
                    //NotifierHelper.Show(NotifierType.Information, ex.Message+" NexPing1Initiate");
                    AppendLog($"❌Get Gametools-Server-Info failed, Error: '{ex}'");
                    NotifierHelper.Show(NotifierType.Information, $"\n❌Request failed, Error: '{ex}'");
                    return false;
                }
            }
            else
            {
                JsonNode node_full = JsonNode.Parse(gtserverinfojson);
                var options = new JsonSerializerOptions { WriteIndented = true }; //from google

                JsonNode node_unixtimestamp = node_full!["update_timestamp"]!;
                NexUnixTimeDifferenceGameTools(node_unixtimestamp.ToString(), isforPing);                                
                return false;
            }
        }
        else
        {
            AppendLog($"\n❌Get Gametools-Server-Info failed, Gametools is sending an empty request");
            NotifierHelper.Show(NotifierType.None, $"❌Request cancelled, Gametools is sending an empty request.");
            return false;
        }
    }








    //PING-SCANNING + KICK
    private async void NexPing0AutoCheck() //tna
    {
        while (true)
        {
            if (Vari.AutoKickPing == true) //nex
            {
                await NexPing1Initiate(); //nex                
            }            
            Thread.Sleep(60000);
        }
    }
    private async Task NexPing1Initiate() //tna
    {        
        string gtserverinfojson = await HttpHelper.HttpClientGET($"https://api.gametools.network/bf1/players/?gameid={Vari.GameId}");

        bool noerror = await NexGetGametoolsServerInfoJson(true);
        if (noerror == true)
        {
            try
            {
                int peoplekickedamount = 0;
                if (Vari.globalplayernarray1[0] != null && Vari.globalplayernarray1 != null)
                {
                    Array.ForEach(Vari.globalplayernarray1, element => peoplekickedamount += NexPing2ReviewPing(element));
                    Array.ForEach(Vari.globalplayernarray2, element => peoplekickedamount += NexPing2ReviewPing(element));
                }                
                if(peoplekickedamount>0)
                {
                    NotifierHelper.Show(NotifierType.Success, $"✔Ping-Check finished, {peoplekickedamount} Players kicked.");
                }
                else
                {
                    NotifierHelper.Show(NotifierType.None, $"✔Ping-Check finished, no Players kicked.");
                }                
                peoplekickedamount = 0;

            }
            catch (Exception ex)
            {
                Log.Ex(ex);
                AppendLog($"\n❌Ping-Check failed, Error: '{ex}'");
                NotifierHelper.Show(NotifierType.Information, $"❌Ping-Check failed, Error: '{ex}'");
            }
        }
    }
    private int NexPing2ReviewPing(PlayerN pn) //has to be void
    {
        int pka = 0;
        if (pn.latency > Vari.PingLimit)
        {
            pka++;
            NexPing3KickPlayer(new BreakRuleInfo
            {
                Name = pn.name,
                PersonaId = pn.player_id,
                Reason = $"Latency exceeded {Vari.PingLimit}. ({pn.latency})"
            });
        }
        return pka;

    }
    private async void NexPing3KickPlayer(BreakRuleInfo info) //has to be void
    {                
        if (NexBoolPlayerIsNotAdminOrWhitelisted(info.Name, info.PersonaId) == true)
        {
            var result = await BF1API.AdminKickPlayer(info.PersonaId, info.Reason);

            if (result.IsSuccess)
            {
                info.Status = "kicked out";
                LogView._dAddKickOKLog(info);
                await DWebHooks.LogPingKick(info);
            }
            else
            {
                info.Status = "Kick failed " + result.Message;
                LogView._dAddKickNOLog(info);                
            }
        }
        else
        {
            BreakRuleInfo info2 = info;
            info2.Reason = info.Reason + " (ADMIN OR WHITELISTED)";
            LogView._dAddKickNOLog(info2);
        }
    }
    







    //BFBAN-SCANNING + KICK
    private void NexBFBAN0AutoCheck() //tna
    {
        while (true)
        {            
            if (Vari.AutoCheckBFBAN == true) //nex
            {
                NexBFBAN1Scan(); //nex                
            }
            Thread.Sleep(60000);
        }
    }
    private async void NexBFBAN1Scan() //tna
    {
        string pids = "";
        bool noerror = await NexGetGametoolsServerInfoJson(false);
        if (noerror == true)
        {
            if (Vari.globalplayernarray1 != null || Vari.globalplayernarray2 != null)
            {
                Array.ForEach(Vari.globalplayernarray1, element => pids = pids + element.player_id.ToString() + "%2C");
                Array.ForEach(Vari.globalplayernarray2, element => pids = pids + element.player_id.ToString() + "%2C");

                //AppendLog($"\npids Length: {pids.Length}");
                int amountofplayers = 0;
                Array.ForEach(Vari.globalplayernarray1, element => amountofplayers++);
                Array.ForEach(Vari.globalplayernarray1, element => amountofplayers++);
                //AppendLog($"Amount of Players: {amountofplayers}");
                amountofplayers = 0;

                string bfbanjson = await HttpHelper.HttpClientGET($"https://api.gametools.network/bfban/checkban?personaids={pids}");
                if (bfbanjson != null)
                {
                    if (Vari.bfbanjsonPrevious == null || (Vari.bfbanjsonPrevious != null && Vari.bfbanjsonPrevious != bfbanjson))
                    {
                        try
                        {
                            if (Vari.bfbanjsonPrevious == null)
                            {
                                Vari.bfbanjsonPrevious = bfbanjson;
                            }

                            JsonNode node_full = JsonNode.Parse(bfbanjson);
                            var options = new JsonSerializerOptions { WriteIndented = true };

                            JsonObject node_personaidsobject = node_full!["personaids"]!.AsObject();
                            using var stream = new MemoryStream();
                            using var writer = new Utf8JsonWriter(stream);
                            node_personaidsobject.WriteTo(writer);
                            writer.Flush();
                            personaids personaids1 = JsonSerializer.Deserialize<personaids>(stream.ToArray());

                            int bfbannedplayersdetected = 0;
                            Array.ForEach(Vari.globalplayernarray1, element => bfbannedplayersdetected += NexBFBAN2HackerCheck(personaids1![element.player_id.ToString()]));
                            
                            if (Vari.AutoKickBFBAN==true)
                            {
                                if (bfbannedplayersdetected > 0)
                                {

                                    NotifierHelper.Show(NotifierType.Success, $"✔BFBAN-Scan finished. {bfbannedplayersdetected} Players kicked.");
                                }
                                else
                                {
                                    NotifierHelper.Show(NotifierType.None, $"✔BFBAN-Scan finished. no Players kicked.");
                                }
                            }
                            else
                            {
                                if (bfbannedplayersdetected > 0)
                                {

                                    NotifierHelper.Show(NotifierType.Success, $"✔BFBAN-Scan finished. {bfbannedplayersdetected} Players detected.");
                                }
                                else
                                {
                                    NotifierHelper.Show(NotifierType.None, $"✔BFBAN-Scan finished. no Players detected.");
                                }
                            }

                            

                            bfbannedplayersdetected = 0;
                        }
                        catch (Exception ex)
                        {
                            Log.Ex(ex);
                            AppendLog($"\n❌BFBAN-Check failed, Error: '{ex}'");
                        }
                    }
                    else if (Vari.bfbanjsonPrevious == bfbanjson)
                    {
                        AppendLog($"\n❌BFBAN-Check cancelled. (Gametools is sending an old BFBAN-Request)");
                        NotifierHelper.Show(NotifierType.Information, $"❌BFBAN-Check cancelled. (Gametools is sending an old BFBAN-Request)");
                    }
                }
                else
                {
                    AppendLog($"\n❌BFBAN-Check cancelled. (Gametools is sending an empty BFBAN-request)");
                    NotifierHelper.Show(NotifierType.Information, $"❌BFBAN-Check cancelled. (Gametools is sending an empty BFBAN-request)");
                }
            }
            else
            {
                AppendLog($"\n❌BFBAN-Check cancelled. (Playerlist is empty, just try again.)");
                NotifierHelper.Show(NotifierType.Information, $"❌BFBAN-Check cancelled. (Playerlist is empty, just try again.)");
            }
        }     
        //string bfbanjson = await HttpHelper.HttpClientGET($"https://api.gametools.network/bfban/checkban?personaids=814373198%2C884387651%2C844851406");
        //AppendLog($"{personaids1!["814373198"].hacker.ToString()}");s            
        //JsonNode output = personaidarray[0].hacker.ToString();
        //AppendLog($"{output.GetType()}\n\n{output}");            
    }
    private int NexBFBAN2HackerCheck(PersonaIDNex pidn) //cant be async
    {
        int pba = 0;
        if (pidn.hacker == true && Vari.Custom_WhiteList.Contains(pidn.originId) == false)
        {
            pba++;
            if (Vari.AutoKickBFBAN == true)
            {
                NexBFBan3KickPlayer(new BreakRuleInfo
                {
                    Name = pidn.originId,
                    PersonaId = long.Parse(pidn.originPersonaId),
                    Reason = $"BFBAN detected ({pidn.originUserId})."
                }, pidn);
            }
            else
            {
                LogView._dAddBFBanLog(pidn);
                AppendLog("\n!! BFBAN Detected, check Logs !!");
                DWebHooks.LogBFBANDetected(pidn);
            }
        }
        return pba;
    }
    private async Task NexBFBan3KickPlayer(BreakRuleInfo info, PersonaIDNex pidn)
    {
        if (NexBoolPlayerIsNotAdminOrWhitelisted(info.Name, info.PersonaId) == true)
        {
            var result = await BF1API.AdminKickPlayer(info.PersonaId, info.Reason);

            if (result.IsSuccess)
            {
                info.Status = "kicked out";
                LogView._dAddKickOKLogBFBAN(info, pidn);
                await DWebHooks.LogBFBANKick(info, pidn);
            }
            else
            {
                info.Status = "Kick failed " + result.Message;
                LogView._dAddKickNOLogBFBAN(info, pidn);                
            }
        }
        else
        {
            BreakRuleInfo info2 = info;
            info2.Reason = info.Reason + " (ADMIN OR WHITELISTED)";
            LogView._dAddKickNOLogBFBAN(info2, pidn);
        }
    }
    





    


    private async void CheckBox_Ping_Click(object sender, RoutedEventArgs e) //tna
    {
        AudioUtil.ClickSound();        
        if (CheckBox_Ping.IsChecked == true)
        {
            //if (Vari.NexGlobalWhiteListServers.Contains(Vari.GameId))
            try
            {
                if (await CheckKickEnv())
                {
                    Vari.AutoKickPing = true;
                    /*
                    if (Vari.NexThread2IntState == 0)
                    {
                        Vari.NexThreadStateStart = thread2Ping.ThreadState;
                        Vari.NexThread2IntState++;
                    }
                    if (thread2Ping.ThreadState == Vari.NexThreadStateStart)
                    {
                        //thread2Ping.Start();                            
                    }
                    */
                    NotifierHelper.Show(NotifierType.Success, $"Ping-Auto-Kicking enabled.");
                }
                else
                {
                    CheckBox_Ping.IsChecked = false;
                }
            }
            catch
            {
                NotifierHelper.Show(NotifierType.Error, "Make sure to Auth yourself in 'Auth'.");
                CheckBox_Ping.IsChecked = false;
                NexVerifySessionIDStartup();
            }
        }
        else
        {
            Vari.AutoKickPing = false;            
            NotifierHelper.Show(NotifierType.Success, $"Ping-Auto-Kicking disabled.");
        }
    }
    private async void CheckBox_WinSwitching_Click(object sender, RoutedEventArgs e) //tna
    {
        AudioUtil.ClickSound();
        if (CheckBox_WinSwitching.IsChecked == true)
        {
            //if (Vari.NexGlobalWhiteListServers.Contains(Vari.GameId))
            try
            {
                if (await CheckKickEnv())
                {
                    Vari.AutoKickWinSwitching = true;
                    NotifierHelper.Show(NotifierType.Success, $"WinSwitching-Auto-Kicking enabled.");
                }
                else
                {
                    CheckBox_WinSwitching.IsChecked = false;
                }
            }
            catch
            {
                NotifierHelper.Show(NotifierType.Error, "Make sure to Auth yourself in 'Auth'.");
                CheckBox_WinSwitching.IsChecked = false;
                NexVerifySessionIDStartup();
            }
        }
        else
        {
            Vari.AutoKickWinSwitching = false;
            NotifierHelper.Show(NotifierType.Success, $"Winswitch-Auto-Kicking disabled.");
        }
    }
    private async void CheckBox_BFBAN_Click(object sender, RoutedEventArgs e) //tna
    {
        AudioUtil.ClickSound();
        if (CheckBox_BFBAN.IsChecked == true)
        {
            //if (Vari.NexGlobalWhiteListServers.Contains(Vari.GameId))
            try
            {
                if (await CheckKickEnv())
                {
                    Vari.AutoCheckBFBAN = true;
                    /*
                    if (Vari.NexThread3IntState == 0)
                    {
                        Vari.NexThreadStateStart = thread3BFBAN.ThreadState;
                        Vari.NexThread3IntState++;
                    }

                    if (thread3BFBAN.ThreadState == Vari.NexThreadStateStart)
                    {
                        //thread3BFBAN.Start();                            
                    }
                    */
                    NotifierHelper.Show(NotifierType.Success, $"BFBAN Auto-Checking enabled.");
                }
                else
                {
                    CheckBox_BFBAN.IsChecked = false;
                }
            }
            catch
            {
                NotifierHelper.Show(NotifierType.Error, "Make sure to Auth yourself in 'Auth'.");
                CheckBox_BFBAN.IsChecked = false;
                NexVerifySessionIDStartup();
            }
        }
        else
        {
            Vari.AutoCheckBFBAN = false;
            NotifierHelper.Show(NotifierType.Success, $"BFBAN Auto-Checking disabled.");
        }
    }
    private async void CheckBox_BFBANKick_Click(object sender, RoutedEventArgs e) //tna
    {
        AudioUtil.ClickSound();
        if (CheckBox_BFBANKick.IsChecked == true)        
        {
            //if (Vari.NexGlobalWhiteListServers.Contains(Vari.GameId))
            if (Vari.AutoCheckBFBAN == true)
            {
                try
                {
                    if (await CheckKickEnv())
                    {
                        Vari.AutoKickBFBAN = true;
                        NotifierHelper.Show(NotifierType.Success, $"BFBAN Auto-Kicking enabled.");
                    }
                    else
                    {
                        CheckBox_BFBANKick.IsChecked = false;
                    }
                }
                catch
                {
                    NotifierHelper.Show(NotifierType.Error, "Make sure to Auth yourself in 'Auth'.");
                    CheckBox_BFBANKick.IsChecked = false;
                    NexVerifySessionIDStartup();
                }
            }
            else
            {
                Vari.AutoKickBFBAN = false;
                CheckBox_BFBANKick.IsChecked = false;
                NotifierHelper.Show(NotifierType.Error, $"Auto-Check for BFBANs need to be checked.");
            }
        }
        else
        {
            Vari.AutoKickBFBAN = false;            
            NotifierHelper.Show(NotifierType.Success, $"BFBAN Auto-Kicking disabled.");
        }
    }




    
    private void NexStartUpFileCheck()
    {        
        if (File.Exists(FileUtil.F_NexAutoRun_Path) == true) //nex
        {
            try
            {
                StreamReader sr = new StreamReader(FileUtil.F_NexAutoRun_Path);
                string s1 = sr.ReadLine();                             
                sr.Close();
                Debug.WriteLine("Startup Read2 done. Result: " + s1);
                if (s1 == "true")
                {
                    Vari.AutoRun = true;
                }                
            }
            catch (Exception ex)
            {
                Log.Ex(ex);
            }
        }
        else //File doesn't exist.
        {
            try
            {
                StreamWriter sw = new StreamWriter(FileUtil.F_NexAutoRun_Path);
                sw.WriteLine("false");
                Debug.WriteLine("Write1 'false' done");
                sw.Close();
            }
            catch (Exception ex)
            {
                Log.Ex(ex);
            }
        }

        if (File.Exists(FileUtil.F_NexWebhook_Path) == true) //nex
        {
            try
            {
                StreamReader sr = new StreamReader(FileUtil.F_NexWebhook_Path);
                Vari.Webhooks.Monitoring = sr.ReadLine();
                Vari.Webhooks.Kick = sr.ReadLine();
                Vari.Webhooks.Ping = sr.ReadLine();
                Vari.Webhooks.BFBANWS = sr.ReadLine();
                Vari.Webhooks.Balancer = sr.ReadLine();
                sr.Close();
            }
            catch (Exception ex) { Log.Ex(ex); }
        }
        else //File doesn't exist.
        {
            try
            {
                StreamWriter sw = new StreamWriter(FileUtil.F_NexWebhook_Path);
                sw.WriteLine("Insert WebhookMonitoring here");
                sw.WriteLine("Insert WebhookKick here");
                sw.WriteLine("Insert WebhookPing here");
                sw.WriteLine("Insert WebhookBFBANWS here");
                sw.WriteLine("Insert WebhookBalancer here");
                Debug.WriteLine("Write2 Webhooks done");
                sw.Close();
            }
            catch { Debug.WriteLine("Write1 FAIL"); }
        }

        Thread.Sleep(6500);
        NexCheckBoxes();        
    }    
    public void NexCheckBoxes()
    {
        if (Vari.AutoRun == true)
        {
            try
            {
                Thread.Sleep(1500);
                /*
                this.Dispatcher.BeginInvoke(() =>
                {
                    CheckBox_Ping.IsChecked = true;
                    CheckBox_Ping_Click(null, null);
                });
                */
                Thread.Sleep(1000);
                this.Dispatcher.BeginInvoke(() =>
                {
                    CheckBox_WinSwitching.IsChecked = true;
                    CheckBox_WinSwitching_Click(null, null);
                });
                /*
                Thread.Sleep(1000);
                this.Dispatcher.BeginInvoke(() =>
                {
                    CheckBox_BFBAN.IsChecked = true;
                    CheckBox_BFBAN_Click(null, null);
                });
                */
                /*
                Thread.Sleep(1000);
                this.Dispatcher.BeginInvoke(() =>
                {
                    CheckBox_BFBANKick.IsChecked = true;
                    CheckBox_BFBANKick_Click(null, null);
                });*/
                Thread.Sleep(1000);
                this.Dispatcher.BeginInvoke(() =>
                {
                    ToggleButton_RunAutoKick.IsChecked = true;
                    ToggleButton_RunAutoKick_Click(null, null);
                });
                Vari.RuleWindow_Ready = true;
            }
            catch (Exception ex){ Log.Ex(ex); }            
        }
    }



    private async void Button_Test_Click(object sender, RoutedEventArgs e) //tna
    {

        //Log.I($"Current ServerID: {Vari.GameId}, Saved VG ServerID: {Vari.GameID_VG}");
        //Debug.WriteLine($"Current ServerID: {Vari.GameId}, Saved VG ServerID: {Vari.GameID_VG}");


        //Log.I($"Servername: {Vari.CurrentServerName}");
        //Debug.WriteLine($"Servername: {Vari.CurrentServerName}");

        long personaId = 1460455160;
        string id = "";
        string id2 = "";

        var result = await BF1API.GetPersonasByIds(personaId);
        Debug.WriteLine(result.IsSuccess + " " + result.Message);

        if (result.IsSuccess)
        {
            try //tna
            {
                JsonNode jNode = JsonNode.Parse(result.Message);
                id = jNode["result"]![$"{personaId}"]![$"nucleusId"].GetValue<string>();
                id2 = jNode["result"]![$"{personaId}"]!["platformId"].GetValue<string>();
            }
            catch (Exception ex) //tna
            {
                Log.Ex(ex);                
            }
        }

        Debug.WriteLine("id: " + id + " pid: " + id2);
    }
}
