using BF1.ServerAdminTools.Windows;
using BF1.ServerAdminTools.Common.Utils;
using BF1.ServerAdminTools.Common.Helper;
using BF1.ServerAdminTools.Features.Core;
using BF1.ServerAdminTools.Features.API;
using BF1.ServerAdminTools.Features.API.RespJson;
using BF1.ServerAdminTools.Features.Config;

using RestSharp;
using CommunityToolkit.Mvvm.Messaging;

namespace BF1.ServerAdminTools.Views;

/// <summary>
/// AuthView.xaml 的交互逻辑
/// </summary>
public partial class AuthView : UserControl
{
    private AuthConfig AuthConfig { get; set; } = new();

    public ObservableCollection<string> ComboBox_ConfigNames { get; set; } = new();

    public AuthView()
    {
        InitializeComponent();        
        this.DataContext = this;
        MainWindow.ClosingDisposeEvent += MainWindow_ClosingDisposeEvent;

        var timerAutoRefreshMode1 = new Timer
        {
            AutoReset = true,
            Interval = TimeSpan.FromMinutes(5).TotalMilliseconds
        };
        timerAutoRefreshMode1.Elapsed += TimerAutoRefreshMode1_Elapsed;
        timerAutoRefreshMode1.Start();

        var timerAutoRefreshMode2 = new Timer
        {
            AutoReset = true,
            Interval = TimeSpan.FromMinutes(30).TotalMilliseconds
        };
        timerAutoRefreshMode2.Elapsed += TimerAutoRefreshMode2_Elapsed;
        timerAutoRefreshMode2.Start();

        Task.Run(() =>
        {
            TimerAutoRefreshMode1_Elapsed(null, null);
            TimerAutoRefreshMode2_Elapsed(null, null);            
        });

        WeakReferenceMessenger.Default.Register<string, string>(this, "SendRemidSid", (s, e) =>
        {
            this.Dispatcher.Invoke(() =>
            {
                TextBox_InfoSessionID.Text = Vari.SessionId; //tna
                //TextBox_InfoRemidSid.Text = $"Sid:\n{Globals.Sid}\nRemid:\n{Globals.Remid}"; //tna
                TextBox_InfoSessionID2.Text = Vari.SessionId_Mode2; //tna

                SaveAuthConfig();
            });
        });

        if (!File.Exists(FileUtil.F_Auth_Path))
        {
            AuthConfig.IsUseMode1 = true;
            AuthConfig.AuthInfos = new();

            for (int i = 0; i < 10; i++)
            {
                AuthConfig.AuthInfos.Add(new AuthConfig.AuthInfo()
                {
                    AuthName = $"Custom Authorization {i}",
                    Sid = "",
                    Remid = "",
                    SessionId = ""
                });
            }

            File.WriteAllText(FileUtil.F_Auth_Path, JsonUtil.JsonSeri(AuthConfig));
        }

        if (File.Exists(FileUtil.F_Auth_Path))
        {
            using (var streamReader = new StreamReader(FileUtil.F_Auth_Path))
            {
                AuthConfig = JsonUtil.JsonDese<AuthConfig>(streamReader.ReadToEnd());

                Vari.IsUseMode1 = AuthConfig.IsUseMode1;
                if (AuthConfig.IsUseMode1)
                {
                    RadioButton_Mode1.IsChecked = true;
                    RadioButton_Mode2.IsChecked = false;
                    TextBox_InfoRemidSid.Clear(); //tna
                }
                else
                {
                    RadioButton_Mode1.IsChecked = false;
                    RadioButton_Mode2.IsChecked = true;
                    TextBox_InfoRemidSid.Text = $"Sid:\n{Vari.Sid}\nRemid:\n{Vari.Remid}"; //tna
                }

                foreach (var item in AuthConfig.AuthInfos)
                {
                    ComboBox_ConfigNames.Add(item.AuthName);
                }

                ApplyAuthByIndex(0);
            }
        }
    }

    private void MainWindow_ClosingDisposeEvent()
    {
        AuthConfig.IsUseMode1 = Vari.IsUseMode1;
        File.WriteAllText(FileUtil.F_Auth_Path, JsonUtil.JsonSeri(AuthConfig));
    }

    private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
    {
        ProcessUtil.OpenLink(e.Uri.OriginalString);
        e.Handled = true;
    }

    private void TimerAutoRefreshMode1_Elapsed(object sender, ElapsedEventArgs e)
    {
        if (Vari.IsUseMode1)
        {
            var str = Search.SearchMemory(Offsets.SessionIDMask);
            if (str != string.Empty)
            {
                Vari.SessionId_Mode1 = str;
                LoggerHelper.Info($"Got SessionID successfully {Vari.SessionId}");
            }
            else
            {
                LoggerHelper.Error($"Failed to Get SessionID");
            }
        }
    }

    private async void TimerAutoRefreshMode2_Elapsed(object sender, ElapsedEventArgs e) //event
    {
        if (!Vari.IsUseMode1)
        {
            try
            {
                if (!string.IsNullOrEmpty(Vari.Remid) && !string.IsNullOrEmpty(Vari.Sid))
                {
                    var str = "https://accounts.ea.com/connect/auth?response_type=code&locale=en-US&client_id=sparta-backend-as-user-pc"; //tna changed locale
                    var options = new RestClientOptions(str)
                    {
                        MaxTimeout = 5000,
                        FollowRedirects = false
                    };

                    var client = new RestClient(options);
                    var request = new RestRequest()
                        .AddHeader("Cookie", $"remid={Vari.Remid};sid={Vari.Sid};");

                    LoggerHelper.Info($"Current Remid is {Vari.Remid}");
                    LoggerHelper.Info($"Currend Sid is {Vari.Sid}");

                    var response = await client.ExecuteGetAsync(request);
                    if (response.StatusCode == HttpStatusCode.Redirect)
                    {
                        string code = response.Headers.ToList()
                            .Find(x => x.Name == "Location")
                            .Value.ToString();

                        LoggerHelper.Info($"The current Location is {code}");

                        if (code.Contains("http://127.0.0.1/success?code="))
                        {
                            Vari.Remid = response.Cookies[0].Value;
                            Vari.Sid = response.Cookies[1].Value;

                            LoggerHelper.Info($"Current Remid is {Vari.Remid}");
                            LoggerHelper.Info($"Current Sid is {Vari.Sid}");

                            code = code.Replace("http://127.0.0.1/success?code=", "");
                            var result = await BF1API.GetEnvIdViaAuthCode(code);
                            if (result.IsSuccess)
                            {
                                var envIdViaAuthCode = JsonUtil.JsonDese<EnvIdViaAuthCode>(result.Message);
                                Vari.SessionId_Mode2 = envIdViaAuthCode.result.sessionId;
                                LoggerHelper.Info($"Refresh SessionID successfully {Vari.SessionId}");
                            }
                            else
                            {
                                LoggerHelper.Error($"code {code}");
                            }
                        }
                        else
                        {
                            LoggerHelper.Error($"code {code}");
                        }
                    }
                    else
                    {
                        LoggerHelper.Error($"Error {Vari.Remid}");
                    }
                }
                else
                {
                    LoggerHelper.Error($"Error ");
                }
            }
            catch (Exception ex)
            {
                Log.Ex(ex);
                LoggerHelper.Error($"Error ", ex);
            }
        }
    }

    /// <summary>
    /// 应用授权
    /// </summary>
    /// <param name="index"></param>
    private void ApplyAuthByIndex(int index)
    {
        var auth = AuthConfig.AuthInfos[index];

        Vari.Sid = auth.Sid;
        Vari.Remid = auth.Remid;
        Vari.SessionId_Mode2 = auth.SessionId;

        TextBox_InfoSessionID.Text = Vari.SessionId; //tna
        //TextBox_InfoRemidSid.Text = $"Sid:\n{Globals.Sid}\nRemid:\n{Globals.Remid}"; //tna
        TextBox_InfoSessionID2.Text = Vari.SessionId_Mode2; //tna
    }

    /// <summary>
    /// 保存授权
    /// </summary>
    private void SaveAuthConfig()
    {
        var index = ComboBox_CustomConfigName.SelectedIndex;
        if (index == -1)
            return;

        var auth = AuthConfig.AuthInfos[index];

        auth.Sid = Vari.Sid;
        auth.Remid = Vari.Remid;
        auth.SessionId = Vari.SessionId;
    }

    ///////////////////////////////////////////////////////////////////////////////////////////

    public async void Button_VerifyPlayerSessionId_Click(object sender, RoutedEventArgs e)
    {
        AudioUtil.ClickSound();
        //TextBox_InfoNex.Text = $"SessionId: {Globals.SessionId}\nSid: {Globals.Sid}\nRemid: {Globals.Remid}\n Gameid: {Globals.GameId}\nPersistedGameID: {Globals.PersistedGameId}";
        //TextBlock_CheckSessionIdStatus.FontWeight = FontWeights.Normal;//tna
        //TextBox_InfoNex.AppendText($"SessionId: {Globals.SessionId}\n"); //tna
        TextBox_InfoSessionID.Text = Vari.SessionId; //tna
        TextBlock_CheckSessionIdStatus.FontSize = 16; //tna

        await NexVerifySessionID(); //tna
    }

    private async Task NexVerifySessionID() //tna
    {
        if (!string.IsNullOrEmpty(Vari.SessionId))
        {
            TextBlock_CheckSessionIdStatus.Text = "Veryfing, please wait...";
            TextBlock_CheckSessionIdStatus.Background = Brushes.Gray;
            NotifierHelper.Show(NotifierType.Information, "Veryfing, please wait...");

            await BF1API.SetAPILocale();
            var result = await BF1API.GetWelcomeMessage();
            if (result.IsSuccess)
            {
                var welcomeMsg = JsonUtil.JsonDese<WelcomeMsg>(result.Message);
                //TextBlock_CheckSessionIdStatus.FontWeight = FontWeights.Bold;//tna
                TextBlock_CheckSessionIdStatus.FontSize = 20; //tna

                var msg = welcomeMsg.result.firstMessage;
                Vari.WelcomeMessageNex = msg; //tna

                TextBlock_CheckSessionIdStatus.Text = msg;
                TextBlock_CheckSessionIdStatus.Background = Brushes.Green;

                NotifierHelper.Show(NotifierType.Success, $"Verification Succeeded  |  Time: {result.ExecTime:0.00} seconds");
                //NotifierHelper.Show(NotifierType.Success, $"Verification Succeeded {msg}  |  Time: {result.ExecTime:0.00} seconds"); TNA                
            }
            else
            {
                TextBlock_CheckSessionIdStatus.Text = "Verification Failed";
                TextBlock_CheckSessionIdStatus.Background = Brushes.Red;

                NotifierHelper.Show(NotifierType.Error, $"Verification Failed {result.Message}  |  Time: {result.ExecTime:0.00} seconds");
            }
        }
        else
        {
            NotifierHelper.Show(NotifierType.Warning, "Please obtain the player's SessionID before performing this operation");
        }
    }   

    private void Button_ReadPlayerSessionId_Click(object sender, RoutedEventArgs e)
    {
        AudioUtil.ClickSound();

        NotifierHelper.Show(NotifierType.Information, "Fetching, please wait...");

        Task.Run(() =>
        {
            return Search.SearchMemory(Offsets.SessionIDMask);
        }).ContinueWith((str) =>
        {
            if (str.Result != string.Empty)
            {
                Vari.SessionId_Mode1 = str.Result;
                NotifierHelper.Show(NotifierType.Success, $"Got player SessionID successfully {Vari.SessionId}");                
            }
            else
            {
                LoggerHelper.Error($"Failed to get player SessionID");
                NotifierHelper.Show(NotifierType.Error, $"Failed to get player SessionID, please try to refresh server list");
            }
        });
    }

    private void Button_ReNameAuth_Click(object sender, RoutedEventArgs e)
    {
        AudioUtil.ClickSound();

        var name = TextBox_ReNameAuth.Text.Trim();
        if (string.IsNullOrEmpty(name))
            return;

        var index = ComboBox_CustomConfigName.SelectedIndex;
        if (index == -1)
            return;

        ComboBox_ConfigNames[index] = name;
        AuthConfig.AuthInfos[index].AuthName = name;

        ComboBox_CustomConfigName.SelectedIndex = index;
    }

    private void Button_SaveCurrentAuth_Click(object sender, RoutedEventArgs e)
    {
        AudioUtil.ClickSound();

        SaveAuthConfig();
    }

    private void ComboBox_CustomConfigName_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        var index = ComboBox_CustomConfigName.SelectedIndex;
        if (index == -1)
            return;

        ApplyAuthByIndex(index);
    }

    private void GetPlayerRemidSid()
    {
        if (CoreUtil.IsWebView2DependencyInstalled())
        {
            var webView2Window = new WebView2Window()
            {
                Owner = MainWindow.ThisMainWindow
            };
            webView2Window.ShowDialog();
        }
        else
        {
            NotifierHelper.Show(NotifierType.Warning, "The corresponding dependencies of WebView2 are not detected, please install the dependencies or manually obtain cookies");
        }
    }

    private void Button_GetPlayerRemidSid_Click(object sender, RoutedEventArgs e)
    {
        AudioUtil.ClickSound();

        GetPlayerRemidSid();
    }

    private void RadioButton_Mode12_Click(object sender, RoutedEventArgs e)
    {
        Vari.IsUseMode1 = RadioButton_Mode1.IsChecked == true;
        if (RadioButton_Mode1.IsChecked == true)
        {
            TextBox_InfoRemidSid.Clear(); //tna
            TextBox_InfoSessionID2.Clear(); //tna
        }
        else
        {
            TextBox_InfoRemidSid.Text = $"Sid:\n{Vari.Sid}\nRemid:\n{Vari.Remid}"; //tna
            TextBox_InfoSessionID2.Text = Vari.SessionId_Mode2; //tna
        }
    }

    private void RadioButton_Mode2_Checked(object sender, RoutedEventArgs e)
    {

    }

    private void RadioButton_Mode2_Checked_1(object sender, RoutedEventArgs e)
    {

    }
}
