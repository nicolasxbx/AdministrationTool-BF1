using BF1.ServerAdminTools.Views;
using BF1.ServerAdminTools.Models;
using BF1.ServerAdminTools.Common.Utils;
using BF1.ServerAdminTools.Common.Helper;
using BF1.ServerAdminTools.Features.Core;
using BF1.ServerAdminTools.Features.Chat;

using CommunityToolkit.Mvvm.Input;
using BF1.ServerAdminTools.NexDiscord;

namespace BF1.ServerAdminTools;

/// <summary>
/// MainWindow.xaml 的交互逻辑
/// </summary>
public partial class MainWindow
{
    public delegate void ClosingDispose();
    public static event ClosingDispose ClosingDisposeEvent;
    

    // 向外暴露主窗口实例
    public static MainWindow ThisMainWindow;

    // 声明一个变量，用于存储软件开始运行的时间
    private DateTime Origin_DateTime;

    ///////////////////////////////////////////////////////

    public MainModel MainModel { get; set; } = new();

    public RelayCommand<string> NavigateCommand { get; private set; }

    ///////////////////////////////////////////////////////

    private HomeView HomeView { get; set; } = new();
    private AuthView AuthView { get; set; } = new();
    private ServerView ServerView { get; set; } = new(); 
    private QueryView QueryView { get; set; } = new();
    private ScoreView ScoreView { get; set; } = new();
    private DetailView DetailView { get; set; } = new();
    public RuleView RuleView { get; set; } = new();
    private LogView LogView { get; set; } = new();
    private ChatView ChatView { get; set; } = new();
    private RobotView RobotView { get; set; } = new();
    private OptionView OptionView { get; set; } = new();
    private AboutView AboutView { get; set; } = new();

    ///////////////////////////////////////////////////////

    // 当前View名称
    private string CurrentViewName = string.Empty;

    public MainWindow()
    {
        InitializeComponent();
    }

    private void Window_Main_Loaded(object sender, RoutedEventArgs e)
    {
        Log.I("Main Window Loaded");
        this.DataContext = this;
        ThisMainWindow = this;

        NavigateCommand = new(Navigate);
        // 首页导航
        Navigate("RuleView"); //tna Start View is RuleView
        

        ////////////////////////////////

        MainModel.AppRunTime = "Loading...";

        // 获取当前时间，存储到对于变量中
        Origin_DateTime = DateTime.Now;

        ////////////////////////////////

        var thread0 = new Thread(InitThread)
        {
            IsBackground = true
        };
        thread0.Start();

        var thread1 = new Thread(UpdateState)
        {
            IsBackground = true
        };
        thread1.Start();
    }

    private async void Window_Main_Closing(object sender, CancelEventArgs e) //event
    {
        Log.I("Window Main Closing");
        // 关闭事件
        if(Vari.SexusBot.IsRunning == true)
        {
            await VariS.client.LogoutAsync();
            await VariS.client.StopAsync();
        }        
        await DWebHooks.LogMonitoringOFF(); //tna              
        Log.I("Window_Main_Closing called");
        //Thread.Sleep(2000);

        ClosingDisposeEvent();
        LoggerHelper.Info($"The call to close event succeeded");

        SQLiteHelper.CloseConnection();
        LoggerHelper.Info($"Closed the database link successfully");

        ChatMsg.FreeMemory();
        LoggerHelper.Info($"Freed memory");
        Memory.CloseHandle();
        LoggerHelper.Info($"Closed handle");
                
        Application.Current.Shutdown();
        LoggerHelper.Info($"Program closes\n\n");
    }

    /// <summary>
    /// 初始化线程
    /// </summary>
    private void InitThread() //was async NEX
    {       
        CoreUtil.FlushDNSCache();
        LoggerHelper.Info("Flush DNS cache successfully");

        LoggerHelper.Info($"Detecting version updates...");
        this.Dispatcher.Invoke(() =>
        {
            NotifierHelper.Show(NotifierType.None, $"Currently doing the Sex...");
        });
        
        // 获取版本更新
        var webConfig = HttpHelper.HttpClientGET(CoreUtil.Config_Address).Result;
        if (!string.IsNullOrEmpty(webConfig))
        {
            /*
            var updateInfo = JsonUtil.JsonDese<UpdateInfo>(webConfig);

            CoreUtil.ServerVersionInfo = new Version(updateInfo.Version);
            CoreUtil.Notice_Address = updateInfo.Address.Notice;
            CoreUtil.Change_Address = updateInfo.Address.Change;

            // 获取最新公告
            await HttpHelper.HttpClientGET(CoreUtil.Notice_Address).ContinueWith((t) =>
            {
                if (t != null)
                    WeakReferenceMessenger.Default.Send(t.Result, "Notice");
                else
                    WeakReferenceMessenger.Default.Send("Failed to get the latest announcement content!", "Notice");
            });
            // 获取更新日志
            await HttpHelper.HttpClientGET(CoreUtil.Change_Address).ContinueWith((t) =>
            {
                if (t != null)
                    WeakReferenceMessenger.Default.Send(t.Result, "Change");
                else
                    WeakReferenceMessenger.Default.Send("Failed to get changelog information!", "Change");
            });
            */
            LoggerHelper.Info($"Sex completed. {Vari.Nexversion}");
            //await BF1API.SetAPILocale(); //tna
            this.Dispatcher.Invoke(() =>
            {
                NotifierHelper.Show(NotifierType.None, $"Nortsex completed. Many Men involved.");
            });


            /*
            if (CoreUtil.ServerVersionInfo > CoreUtil.ClientVersionInfo)
            {
                LoggerHelper.Info($"new version discovered {CoreUtil.ServerVersionInfo}");

                this.Dispatcher.Invoke(() =>
                {
                    if (MessageBox.Show($"Detected that a new version has been released, do you want to go to the update now?                   " +
                        $"\n\n{updateInfo.Latest.Date}\n{updateInfo.Latest.Change}\n\nIt is strongly recommended that you use the latest version!",
                        "new version discovered", MessageBoxButton.YesNo, MessageBoxImage.Information) == MessageBoxResult.Yes)
                    {
                        var UpdateWindow = new UpdateWindow(updateInfo)
                        {
                            Owner = this
                        };
                        UpdateWindow.ShowDialog();
                    }
                });
            }
            else
            {
                LoggerHelper.Info($"Sex completed. {CoreUtil.ServerVersionInfo}");
                //await BF1API.SetAPILocale(); //tna
                this.Dispatcher.Invoke(() =>
                {
                    NotifierHelper.Show(NotifierType.None, $"Nortsex completed. Many Men involved.");
                });
            }*/
        }
    }

    /// <summary>
    /// 主窗口UI更新线程
    /// </summary>
    private async void UpdateState() //thread
    {        
        while (true)
        {
            // 获取软件运行时间
            MainModel.AppRunTime = CoreUtil.ExecDateDiff(Origin_DateTime, DateTime.Now);

            if (!ProcessUtil.IsAppRun(CoreUtil.TargetAppName))
            {
                await this.Dispatcher.Invoke(async () =>
                {                    
                    await DWebHooks.LogMonitoringOFF(); //tna                    
                    Log.Ex("Battlefield Closed! Tool closing...");
                    Log.I("Battlefield Closed! Tool closing...");
                    //LoggerHelper.Info($"DISCORD MESSAGE SENT (NEX)1");
                    NotifierHelper.Show(NotifierType.Warning, "Battlefield closed! Tool is closing....");
                    Thread.Sleep(3000);

                    this.Close();
                });
                return;
            }
            Thread.Sleep(1000);
        }
    }
    /*
    private async Task NexClosing()
    {
        NotifierHelper.Show(NotifierType.Warning, "Battlefield closed! Tool is closing....");
        await DiscNex.LogMonitoringOFF(); //tna
        await SexusBot.ScoreboardOffline();
        Log.Ex("Battlefield Closed! Tool closing...");
        Log.I("Battlefield Closed! Tool closing...");       
        
        Thread.Sleep(3000);
        this.Dispatcher.Invoke(() =>
        {
            this.Close();
        });
    }*/

    /// <summary>
    /// View页面导航
    /// </summary>
    /// <param name="viewName"></param>
    private void Navigate(string viewName)
    {        
        if (viewName == null || string.IsNullOrEmpty(viewName))
            return;

        GC.Collect();
        GC.WaitForPendingFinalizers();

        // 防止重复触发
        if (CurrentViewName != viewName)
            CurrentViewName = viewName;
        else
            return;

        switch (viewName)
        {
            case "HomeView":
                ContentControl_Main.Content = HomeView;
                break;
            case "AuthView":
                ContentControl_Main.Content = AuthView;
                break;
            case "ServerView":
                ContentControl_Main.Content = ServerView;
                break;
            case "QueryView":
                ContentControl_Main.Content = QueryView;
                break;
            case "ScoreView":
                ContentControl_Main.Content = ScoreView;
                break;
            case "DetailView":
                ContentControl_Main.Content = DetailView;
                break;
            case "RuleView":
                ContentControl_Main.Content = RuleView;
                break;
            case "LogView":
                ContentControl_Main.Content = LogView;
                break;
            case "ChatView":
                ContentControl_Main.Content = ChatView;
                break;
            case "RobotView":
                ContentControl_Main.Content = RobotView;
                break;
            case "OptionView":
                ContentControl_Main.Content = OptionView;
                break;
            case "AboutView":
                ContentControl_Main.Content = AboutView;
                break;
        }
    }
}
