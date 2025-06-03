using BF1.ServerAdminTools;
using Discord;
using Discord.WebSocket;
using LinkDotNet.NUniqueHardwareID;
using System.Reactive;
using System.Security.Cryptography;
using System.Security.Policy;
using System.Threading.Channels;
using System.Windows.Media.TextFormatting;

namespace AdminToolVG;

internal class Program
{
    public static bool initialized = false;
    static async Task Main(string[] args)
    {
        Console.Title = $"BF1 Admin Tool CLI {Vari.Nexversion}";

        AppDomain.CurrentDomain.ProcessExit += new EventHandler(CurrentDomain_ProcessExit!);

        SetHotKeys();        

        await ShowInitScreen();
        
        while (true)
        {
            Console.Clear();
            initialized = true;
            await Navigation.SelectionMenu();
        }
    }    

    #region Closing Handler
    static async void CurrentDomain_ProcessExit(object sender, EventArgs e)
    {
        Log.C("\nClosing...");
        if (Vari.AutoKickBreakPlayer)
        {
            await DWebHooks.LogMonitoringOFF();
        }        
    }
    #endregion

    #region Init / Check if BF1 is Run
    static async Task ShowInitScreen()
    {
        await AnsiConsole.Status()
            .Spinner(Spinner.Known.Clock)
            .StartAsync("[deeppink1_1]Initializing...[/]", async ctx =>
            {
                if (await Init() is false)
                {
                    Console.ReadLine();
                    return;
                }
            });
    }
    static async Task<bool> Init()
    {        
        ProcessUtil.CloseThirdProcess();

		// EA Anti Cheat now makes this tool useless
		Log.CM($"The new 2025 EA Anti Cheat does not allow this tool anymore. :(");
        return false;

		if (!ProcessUtil.IsBf1Run())
        {
            Log.CM($"Battlefield 1 process not found");
            return false;
        }

        if (!Memory.Initialize(CoreUtil.TargetAppName))
        {
            Log.CM("Battlefield 1 memory module initialization failed");
            return false;
        }

        BF1API.Init();
        Log.D("BF1 API Initialized.");

        Directory.CreateDirectory(FileUtil.D_Cache_Path);
        Directory.CreateDirectory(FileUtil.D_Config_Path);
        Directory.CreateDirectory(FileUtil.D_Data_Path);
        Directory.CreateDirectory(FileUtil.D_Log_Path);
        Directory.CreateDirectory(FileUtil.D_Robot_Path);

        //SQLiteHelper.Initialize();
        //Log.D("SQLiteHelper Initialized.");

        ChatMsg.AllocateMemory();
        Log.D("Chat Memory Allocated.");

        CoreUtil.FlushDNSCache();
        Log.D("DNS Cache Flushed.");

        Services.StartServices();
        Log.D("Services started.");

        await Task.Delay(500);
        if (!Util_BF1.GetSessionID())
        {
            Log.CM("Failed to get SessionID. Restarting your game should fix this.");

            return false;
        }
        Log.D($"SessionID initialized. {Vari.SessionID}");

        if (!await GetUserName())
        {
            Log.CM("Failed to get Username. Restarting your game should fix this.");
            return false;
        }
        Log.D("Username initialized.");

        InitWeaponItems();
        Navigation.RulePrompts.SetInitialRules();

        FileConfig.Load();

        GetHWID();
        //await DebugTest();

        return true;
    }

    internal static async Task<bool> GetUserName()
    {
        await BF1API.SetAPILocale();
        var result = await BF1API.GetWelcomeMessage();
        if (result.IsSuccess)
        {
            var welcomeMsg = JsonUtil.JsonDese<WelcomeMsg>(result.Message);
            Vari.WelcomeMessage = welcomeMsg.result.firstMessage;           

            string[] splitwords = Vari.WelcomeMessage.Split(' ');
            string lastword = splitwords[splitwords.Length - 1];
            string username = lastword.Remove(lastword.Length - 1);
            Vari.CurrentUsername = username;
            return true;
        }
        return false;
    }

    internal static void InitWeaponItems()
    {        
        foreach (var item in WeaponData.AllWeaponInfo)
        {
            Vari.WeaponItemsAndBanFlags.Add(new RuleWeaponModel()
            {
                Class = item.Class,
                Name = item.Chinese,
                English = item.English,
                Team1 = false,
                Team2 = false
            });
        }
    }

    internal static void GetHWID()
    {
        try
        {
            var hardwareIdGenerator = new UniqueHardwareId
            {
                UseMACAddress = true,
                UseCPUInformation = true,
            };
            Vari.HWID = hardwareIdGenerator.CalculateHardwareId();
            Log.D($"HWID: {Vari.HWID}");
        }
        catch (Exception ex)
        {
            Log.Ex(ex);
        }
    }
    #endregion

    #region Hotkeys
    static void SetHotKeys()
    {
        HotKeyManager.RegisterHotKey(Keys.P, HotKeyManager.KeyModifiers.Alt);
        HotKeyManager.RegisterHotKey(Keys.O, HotKeyManager.KeyModifiers.Alt);
        HotKeyManager.HotKeyPressed += new EventHandler<HotKeyManager.HotKeyEventArgs>(HotKeyManager_HotKeyPressed); 
    }
    static async void HotKeyManager_HotKeyPressed(object? sender, HotKeyManager.HotKeyEventArgs e)
    {        
        if (e.Key.ToString() == "P") //P
        {
            await Util_BF1.AdminActions.SwitchLocalPlayer_AwaitFreeSlot(true);
        }
        else if (e.Key.ToString() == "O") //O
        {            
            if (Util_BF1.AdminActions.ChatFunction(Vari.CurrentRuleString) == true)
            {
                //Console.Beep(200, 50);
                //Console.Beep(400, 50);
            }
            else
            {
                //Console.Beep(400, 50);
                //Console.Beep(200, 50);
            }
        }
        else
        {
            Log.D(e.Key.ToString());
        }
        return;        
    }
    #endregion
        
    static async Task test3()
    {
        Thread t_bot = new Thread(Navigation.StartBot)
        {
            IsBackground = true,
        };
        t_bot.Start();

        Thread.Sleep(3500);

        var guild = VariS.client.GetGuild(947870312572268584);
        //channel = guild.GetChannel(1118286041191817359) as IMessageChannel;

        //await GTBan("test", "test2");

        await Task.Delay(-1);

        await SexusBot.Stop();
    }
}