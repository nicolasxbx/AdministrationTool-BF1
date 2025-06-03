using BF1.ServerAdminTools.Common.Helper;
using BF1.ServerAdminTools.Common.Utils;
using BF1.ServerAdminTools.Features.API;
using BF1.ServerAdminTools.Features.Chat;
using BF1.ServerAdminTools.Features.Client;
using BF1.ServerAdminTools.Features.Core;
using BF1.ServerAdminTools.Models;
using Chinese;


namespace BF1.ServerAdminTools;

/// <summary>
/// LoadWindow.xaml 的交互逻辑
/// </summary>
public partial class LoadWindow
{
    public LoadModel LoadModel { get; set; } = new();

    public LoadWindow()
    {
        InitializeComponent();
        Log.I("Tool started.");
    }
    
    private void Window_Auth_Loaded(object sender, RoutedEventArgs e)
    {
        this.DataContext = this;

        //AudioUtil.SP_winxpstartup2.Play();        

        Task.Run(() =>
        {
            try
            {
                LoadModel.LoadState = "Loading VG Admin Tool.....";

                // 客户端程序版本号
                LoadModel.VersionInfo = $"Version: {Vari.Nexversion}";
                // 最后编译时间
                LoadModel.BuildDate = $"Obrez > Revolver Edition";

                LoggerHelper.Info("start initialization procedure...");
                LoggerHelper.Info($"Current program version number { Vari.Nexversion}");
                LoggerHelper.Info($"The last compilation time of the current program {CoreUtil.ClientBuildTime}");

                ProcessUtil.CloseThirdProcess();

                // 检测目标程序有没有启动
                if (!ProcessUtil.IsBf1Run())
                {
                    LoadModel.LoadState = $"Battlefield 1 not found! Are yer stupid?";
                    LoggerHelper.Error("Battlefield 1 process not found");
                    Log.Ex("Bf1 Process not found.");
                    Task.Delay(2000).Wait();

                    this.Dispatcher.Invoke(() =>
                    {
                        Application.Current.Shutdown();
                        return;
                    });
                }

                // 初始化
                if (Memory.Initialize(CoreUtil.TargetAppName))
                {
                    LoggerHelper.Info("Battlefield 1 memory module initialized successfully");
                    Log.I("Battlefield 1 memory module initialized successfully");
                }
                else
                {
                    LoadModel.LoadState = $"Battlefield 1 memory module initialization failed!Program is about to close";
                    LoggerHelper.Error("Battlefield 1 memory module initialization failed");
                    Log.Ex("Battlefield 1 memory module initialization failed");
                    Task.Delay(2000).Wait();

                    this.Dispatcher.Invoke(() =>
                    {
                        Application.Current.Shutdown();
                        return;
                    });
                }

                BF1API.Init();
                LoggerHelper.Info("Battlefield 1 API module initialized successfully");
                Log.I("Battlefield 1 API module initialized successfully");

                ImageData.InitDict();
                LoggerHelper.Info("The local image cache library was initialized successfully");

                ChineseConverter.ToTraditional("Free, cross - platform, open source!");
                LoggerHelper.Info("The simplified and traditional translation library was initialized successfully");

                // 创建文件夹
                Directory.CreateDirectory(FileUtil.D_Cache_Path);
                Directory.CreateDirectory(FileUtil.D_Config_Path);
                Directory.CreateDirectory(FileUtil.D_Data_Path);
                Directory.CreateDirectory(FileUtil.D_Log_Path);
                Directory.CreateDirectory(FileUtil.D_Robot_Path);

                // 释放必要文件
                FileUtil.ExtractResFile(FileUtil.Resource_Path + "config.yml", FileUtil.D_Robot_Path + "\\config.yml");
                FileUtil.ExtractResFile(FileUtil.Resource_Path + "go-cqhttp.exe", FileUtil.D_Robot_Path + "\\go-cqhttp.exe");

                SQLiteHelper.Initialize();
                LoggerHelper.Info($"SQLite database initialized successfully");
                Log.I($"SQLite database initialized successfully");

                ChatMsg.AllocateMemory();
                LoggerHelper.Info($"Chinese chat pointers assigned successfully 0x{ChatMsg.GetAllocateMemoryAddress():x}");
                Log.I($"Chinese chat pointers assigned successfully 0x{ChatMsg.GetAllocateMemoryAddress():x}");

                this.Dispatcher.Invoke(() =>
                {
                    var mainWindow = new MainWindow();
                    mainWindow.Show();
                    // 转移主程序控制权
                    Application.Current.MainWindow = mainWindow;
                    // 关闭初始化窗口
                    this.Close();
                });
            }
            catch (Exception ex)
            {
                LoadModel.LoadState = "An unknown exception has occurred! Program is about to close";
                Log.Ex(ex, $"An unknown exception has occurred! Program is about to close ");
                Task.Delay(2000).Wait();

                this.Dispatcher.Invoke(() =>
                {
                    Application.Current.Shutdown();
                });
            }
        });
    }
}
