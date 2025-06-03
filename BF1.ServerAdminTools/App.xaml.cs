using BF1.ServerAdminTools.Common.Helper;
using BF1.ServerAdminTools.Common.Utils;
using BF1.ServerAdminTools.Features.Chat;
using BF1.ServerAdminTools.NexDiscord;

namespace BF1.ServerAdminTools
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : Application
    {
        public static Mutex AppMainMutex;

        protected override void OnStartup(StartupEventArgs e)
        {            
            AppMainMutex = new Mutex(true, ResourceAssembly.GetName().Name, out var createdNew);

            if (createdNew)
            {
                RegisterEvents();
                base.OnStartup(e);
            }
            else
            {
                Log.Ex("Program already running, closing...");
                MessageBox.Show("Please don't open it again, the program is already running\nIf it keeps prompting, please go to \"Task Manager - Details (win7 is a process)\" to end the program",
                    "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                Current.Shutdown();
            }

            //nex            
            bool bot = false;
            bool live = false;
            List<string> start_arguments = e.Args.ToList();
            
            if(Vari.Debug_Start_DiscordBot)
            {
                start_arguments.Clear();
                start_arguments.Add("-bot");
                start_arguments.Add("-live");
                start_arguments.Add("-ar");
            }

            foreach (string s in start_arguments) //Startup
            {
                if (s == "-bot" && Vari.DiscordMode == true)
                {
                    bot = true;
                    Vari.SexusBot.BotInStartupParams = true;
                    StartBot();
                }
                if (s == "-live" && Vari.DiscordMode == true)
                {
                    live = true;
                }
                if (s == "-ar")
                {
                    Vari.AutoRun = true;
                }
            }

            if (bot == true && live == true) //Scoreboard
            {
                Vari.SexusBot.LiveFunctionsEnabled = true;
                Vari.SexusBot.LiveSusEnabled = true;
            }            
        }

        private async void StartBot() //has to be void
        {
            await SexusBot.Start();
        }

        private void RegisterEvents()
        {
            // UI线程未捕获异常处理事件（UI主线程）
            this.DispatcherUnhandledException += App_DispatcherUnhandledException;

            // 非UI线程未捕获异常处理事件（例如自己创建的一个子线程）
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

            // Task线程内未捕获异常处理事件
            TaskScheduler.UnobservedTaskException += TaskScheduler_UnobservedTaskException;
        }

        private void App_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            string str = GetExceptionMsg(e.Exception, e.ToString());
            FileUtil.SaveErrorLog(str);
            Log.Ex(str);
        }

        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            string str = GetExceptionMsg(e.ExceptionObject as Exception, e.ToString());
            FileUtil.SaveErrorLog(str);
            Log.Ex(str);
        }

        private void TaskScheduler_UnobservedTaskException(object sender, UnobservedTaskExceptionEventArgs e)
        {
            string str = GetExceptionMsg(e.Exception, e.ToString());
            FileUtil.SaveErrorLog(str);
            Log.Ex(str);
        }

        /// <summary>
        /// 生成自定义异常消息
        /// </summary>
        /// <param name="ex">异常对象</param>
        /// <param name="backStr">备用异常消息：当ex为null时有效</param>
        /// <returns>异常字符串文本</returns>
        private static string GetExceptionMsg(Exception ex, string backStr)
        {
            var sb = new StringBuilder();
            sb.AppendLine("【Time】：" + DateTime.Now.ToString());
            if (ex != null)
            {
                sb.AppendLine("【Exception】：" + ex.GetType().Name);
                sb.AppendLine("【Exception Info】：" + ex.Message);
                sb.AppendLine("【Stack Call】：\n" + ex.StackTrace);
            }
            else
            {
                sb.AppendLine("【Unhandled exception】：" + backStr);
            }
            return sb.ToString();
        }


        public static async Task RestartNex()
        {
            Log.I("Restarting .......");            
            if (Vari.SexusBot.IsRunning == true)
            {
                await VariS.client.LogoutAsync();
                await VariS.client.StopAsync();
            }
            await DWebHooks.LogMonitoringOFF();
            SQLiteHelper.CloseConnection();
           
            ChatMsg.FreeMemory();            

            AppMainMutex.Dispose();
            AppMainMutex = null;
            System.Windows.Forms.Application.Restart();            
            Process.GetCurrentProcess().Kill();            
        }
    }
}
