using BF1.ServerAdminTools.Common.Helper;
using BF1.ServerAdminTools.Features.Data;
using BF1.ServerAdminTools.Features.API;
using BF1.ServerAdminTools.Models;
using BF1.ServerAdminTools.NexDiscord;

namespace BF1.ServerAdminTools.Views;

/// <summary>
/// LogView.xaml 的交互逻辑
/// </summary>
public partial class LogView : UserControl
{
    public static Action<BreakRuleInfo> _dAddKickOKLog;
    public static Action<BreakRuleInfo> _dAddKickNOLog;
    public static Action<ChangeTeamInfo> _dAddChangeTeamInfo;

    public static Action<PersonaIDNex> _dAddBFBanLog; //tna
    public static Action<BreakRuleInfo, PersonaIDNex> _dAddKickOKLogBFBAN; //tna
    public static Action<BreakRuleInfo, PersonaIDNex> _dAddKickNOLogBFBAN; //tna

    public LogView()
    {
        InitializeComponent();
        this.DataContext = this;
        MainWindow.ClosingDisposeEvent += MainWindow_ClosingDisposeEvent;

        _dAddKickOKLog = AddKickOKLog;
        _dAddKickNOLog = AddKickNOLog;
        _dAddChangeTeamInfo = AddChangeTeamLog;

        _dAddBFBanLog = AddBFBanTeamLog;//tna
        _dAddKickOKLogBFBAN = AddKickOKLogBFBAN; //tna
        _dAddKickNOLogBFBAN = AddKickNOLogBFBAN; //tna
    }

    private void MainWindow_ClosingDisposeEvent()
    {

    }

    /////////////////////////////////////////////////////

    private void AppendKickOKLog(string msg)
    {
        TextBox_KickOKLog.AppendText(msg + "\n");
    }

    private void AppendKickNOLog(string msg)
    {
        TextBox_KickNOLog.AppendText(msg + "\n");
    }

    private void AppendChangeTeamLog(string msg)
    {
        TextBox_ChangeTeamLog.AppendText(msg + "\n");
    }

    /////////////////////////////////////////////////////

    /// <summary>
    /// 追加踢人成功日志
    /// </summary>
    /// <param name="info"></param>
    private void AddKickOKLog(BreakRuleInfo info)
    {
        this.Dispatcher.Invoke(() =>
        {
            if (TextBox_KickOKLog.LineCount >= 1000)
                TextBox_KickOKLog.Clear();
            
            AppendKickOKLog($"Name: {info.Name}");
            AppendKickOKLog($"Kick Reason: {info.Reason}");
            AppendKickOKLog($"          PID: {info.PersonaId}");
            AppendKickOKLog($"          Time: {DateTime.Now}\n");
            //AppendKickOKLog($"Status: kicked\n");
            //AppendKickOKLog($"Status: {info.Status}\n");

            SQLiteHelper.AddLog2SQLite("kick_ok", info);
        });
    }   


    /// <summary>
    /// 追加踢人失败日志
    /// </summary>
    /// <param name="info"></param>
    private void AddKickNOLog(BreakRuleInfo info)
    {
        this.Dispatcher.Invoke(() =>
        {
            if (TextBox_KickNOLog.LineCount >= 1000)
                TextBox_KickNOLog.Clear();
            
            /*if (Vari.NexGlobalWhiteListServers.Contains(Vari.GameId) == false) //tna
            {
                info.Reason = info.Reason + " +(Server is not Whitelisted for new functions)";
            }*/
            
            if (!Vari.Server_AdminList_Name.Contains(Vari.UserNameOfUser))
            {
                info.Reason = info.Reason + " +[User not Admin on Server]";
            }
            Vari.AutoKickBreakPlayer = false;

            AppendKickNOLog($"Name: {info.Name}");
            AppendKickNOLog($"Kick Reason: {info.Reason}");
            AppendKickNOLog($"      PID: {info.PersonaId}");            
            AppendKickNOLog($"      Time: {DateTime.Now}\n");            

            SQLiteHelper.AddLog2SQLite("kick_no", info);            
        });
    }
    /// <summary>
    /// 追加更换队伍日志
    /// </summary>
    /// <param name="info"></param>
    private async void AddChangeTeamLog(ChangeTeamInfo info) //cant be task
    {
        await this.Dispatcher.Invoke(async () =>
        {
            if (TextBox_ChangeTeamLog.LineCount >= 1000)
                TextBox_ChangeTeamLog.Clear();
            
            AppendChangeTeamLog($"Name: {info.Name}");
            await NexWinswitch1Check(info); //tna  
            AppendChangeTeamLog($"Scores: {info.Team1Score} - {info.Team2Score}");
            AppendChangeTeamLog($"Map: {Vari.CurrentMapName}");
            AppendChangeTeamLog($"      LVL: {info.Rank}");
            AppendChangeTeamLog($"      PID: {info.PersonaId}");
            AppendChangeTeamLog($"      Time: {DateTime.Now}\n");
            
            info.Status = NexDBLogsWinswitchBalance(info); //Adds to the status in the SQLDB if it was a win or balance switch
            string st = info.Status;
            if (st[0] == 'B') //Check if first letter of the Status is B => Is it a balanceswitcher
            {
                SQLiteHelper.AddLog2SQLiteBalancersNex(info);
                await NexBalancerDetected(info);
            }

            SQLiteHelper.AddLog2SQLite(info);            
        });
        RobotView._dSendChangeTeamInfo(info);
    }   

    private void AddBFBanTeamLog(PersonaIDNex pin) //tna
    {
        this.Dispatcher.Invoke(() =>
        {
            if (TextBox_ChangeTeamLog.LineCount >= 1000)
                TextBox_ChangeTeamLog.Clear();

            AppendChangeTeamLog($"Name: {pin.originId}");
            AppendChangeTeamLog($"Info: BFBAN DETECTED !!");
            AppendChangeTeamLog($"      URL: {pin.url}");            
            AppendChangeTeamLog($"      ID: {pin.originUserId}");
            AppendChangeTeamLog($"      PID: {pin.originPersonaId}");
            AppendChangeTeamLog($"      Time: {DateTime.Now}\n");
        });
    }

    private void AddKickOKLogBFBAN(BreakRuleInfo info, PersonaIDNex pidn)
    {
        this.Dispatcher.Invoke(() =>
        {
            if (TextBox_KickOKLog.LineCount >= 1000)
                TextBox_KickOKLog.Clear();

            AppendKickOKLog($"Name: {info.Name}");
            AppendKickOKLog($"Kick Reason: {info.Reason}");
            AppendKickOKLog($"      URL: {pidn.url}");            
            AppendKickOKLog($"      PID: {pidn.originPersonaId}");
            AppendKickOKLog($"      Time: {DateTime.Now}\n");           

            SQLiteHelper.AddLog2SQLite("kick_ok", info);            
        });
    }

    private void AddKickNOLogBFBAN(BreakRuleInfo info, PersonaIDNex pidn)
    {
        this.Dispatcher.Invoke(() =>
        {
            if (TextBox_KickNOLog.LineCount >= 1000)
                TextBox_KickNOLog.Clear();

            /*if (Vari.NexGlobalWhiteListServers.Contains(Vari.GameId) == false) //tna
            {
                info.Reason = info.Reason + " +(Server is not Whitelisted for new functions)";
            }*/
            if (!Vari.Server_AdminList_Name.Contains(Vari.UserNameOfUser))
            {
                info.Reason = info.Reason + " +[User not Admin on Server]";
            }
            Vari.AutoCheckBFBAN = false;
            Vari.AutoKickBFBAN = false;

            AppendKickNOLog($"Name: {info.Name}");
            AppendKickNOLog($"Kick Reason: {info.Reason}");
            AppendKickNOLog($"      URL: {pidn.url}");            
            AppendKickNOLog($"      PID: {pidn.originPersonaId}");
            AppendKickNOLog($"      Time: {DateTime.Now}\n"); 

            SQLiteHelper.AddLog2SQLite("kick_no", info);            
        });
    }


    private void MenuItem_ClearKickOKLog_Click(object sender, RoutedEventArgs e)
    {
        TextBox_KickOKLog.Clear();
        NotifierHelper.Show(NotifierType.Success, "Cleared Successfully");
    }

    private void MenuItem_ClearKickNOLog_Click(object sender, RoutedEventArgs e)
    {
        TextBox_KickNOLog.Clear();
        NotifierHelper.Show(NotifierType.Success, "Cleared Successfully");
    }

    private void MenuItem_ClearChangeTeamLog_Click(object sender, RoutedEventArgs e)
    {
        TextBox_ChangeTeamLog.Clear();
        NotifierHelper.Show(NotifierType.Success, "Cleared Successfully");
    }

    private string NexDBLogsWinswitchBalance(ChangeTeamInfo info)
    {
        string s = "ERROR";
        string s2 = $"({info.Team1Score}-{info.Team2Score}) [{Vari.CurrentMapName}]";
        string s3 = info.Status + $" ({info.Team1Score}-{info.Team2Score}) [{Vari.CurrentMapName}]";
        int t1 = info.Team1Score;
        int t2 = info.Team2Score;
        int t1_L = t1 + Vari.TicketLimit;
        int t2_L = t2 + Vari.TicketLimit;

        int stomp = 0;
        

        if (t1_L < t2)
        {
            stomp = 2; //team 2 is stomping
        }
        if (t1 > t2_L)
        {
            stomp = 1; //team 1 is stomping
        }

        if (info.Status == "T1 -> T2") //To Team 2
        {
            if (stomp == 2)
            {
                s = "Winswitch to T2! " + s2;
            }
            else if (stomp == 1)
            {
                s = "Balanced to T2! " + s2;
                
            }
            else
            {
                return s3;
            }
        }
        else if (info.Status == "T1 <- T2") //To Team 1
        {
            if (stomp == 1)
            {
                s = "Winswitch to T1! " + s2;
            }
            else if (stomp == 2)
            {
                s = "Balanced to T1! " + s2;
                
            }
            else
            {
                return s3;
            }
        }

        if (Vari.NexConquestAssaultMapNames.Contains(Vari.CurrentMapName) == true && (info.Team1Score < 300 || info.Team2Score < 300)) //No Winswitch if its Conquest assault
        {
            return s3;
        }

        return s;
    }

    private async Task NexWinswitch1Check(ChangeTeamInfo info) //tna
    {
        if (info.Status == "T1 -> T2" && (info.Team1Score + Vari.TicketLimit) < info.Team2Score)
        {
            await NexWinswitch2Detected(info);
        }
        else if (info.Status == "T1 <- T2" && info.Team1Score > (info.Team2Score + Vari.TicketLimit))
        {
            await NexWinswitch2Detected(info);
        }
        else
        {
            AppendChangeTeamLog($"Info: {info.Status}");
        }
    }
    private async Task NexWinswitch2Detected(ChangeTeamInfo info) //tna
    {
                
        if (Vari.AutoKickWinSwitching == true)
        {
            if (Vari.NexConquestAssaultMapNames.Contains(Vari.CurrentMapName) != true) //If map is not conquest assault
            {
                AppendChangeTeamLog($"Info: WINSWITCH DETECTED ({info.Status}) !!!");
                await NexWinswitch3KickPlayer(new BreakRuleInfo
                {
                    Name = info.Name,
                    PersonaId = info.PersonaId,
                    Reason = $"Winswitch {info.Team1Score}-{info.Team2Score}"
                }, info);
            }
            else //If map is Conquest Assault
            {
                if (info.Team1Score >= 300 && info.Team2Score >= 300) //Both teams have to have 300+ tickets
                {
                    AppendChangeTeamLog($"Info: WINSWITCH DETECTED ({info.Status}) !!!");
                    await NexWinswitch3KickPlayer(new BreakRuleInfo
                    {
                        Name = info.Name,
                        PersonaId = info.PersonaId,
                        Reason = $"Winswitch {info.Team1Score}-{info.Team2Score}"
                    }, info);
                }
            }                       
        }
        else if (Vari.AutoKickWinSwitching != true)
        {
            await DWebHooks.LogWinSwitchDetected(info);            
        }        
        
    }
    private async Task NexWinswitch3KickPlayer(BreakRuleInfo info, ChangeTeamInfo info2) //tna
    {
        if (!Vari.Server_AdminList_PID.Contains(info.PersonaId))
        {
            if (!Vari.Custom_WhiteList.Contains(info.Name))
            {
                await DWebHooks.LogWinSwitchKick(info, info2);

                var result = await BF1API.AdminKickPlayer(info.PersonaId, info.Reason);

                if (result.IsSuccess)
                {
                    AddKickOKLog(info);
                }
                else
                {
                    AddKickNOLog(info);
                }
            }
        }
    }


    private async Task NexBalancerDetected(ChangeTeamInfo info)
    {
        await DWebHooks.LogBalancer(info);                    
    }
}
