using BF1.ServerAdminTools.Common.Utils;
using BF1.ServerAdminTools.Common.Helper;
using BF1.ServerAdminTools.Features.API;
using BF1.ServerAdminTools.Features.API.RespJson;

using Microsoft.Web.WebView2.Core;
using CommunityToolkit.Mvvm.Messaging;

namespace BF1.ServerAdminTools.Windows;

/// <summary>
/// WebView2Window.xaml 的交互逻辑
/// </summary>
public partial class WebView2Window
{
    private const string Uri = "https://accounts.ea.com/connect/auth?response_type=code&locale=en_US&client_id=sparta-backend-as-user-pc";

    public WebView2Window()
    {
        InitializeComponent();
    }

    private async void Window_WebView2_Loaded(object sender, RoutedEventArgs e)
    {
        // 刷新DNS缓存
        CoreUtil.FlushDNSCache();
        LoggerHelper.Info($"Flushed DNS");

        var env = await CoreWebView2Environment.CreateAsync(null, FileUtil.D_Cache_Path, null);
        await WebView2.EnsureCoreWebView2Async(env);

        // 禁止dev菜单
        WebView2.CoreWebView2.Settings.AreDevToolsEnabled = false;
        // 禁止所有菜单
        WebView2.CoreWebView2.Settings.AreDefaultContextMenusEnabled = false;
        // 禁止缩放
        WebView2.CoreWebView2.Settings.IsZoomControlEnabled = false;
        // 禁止显示状态栏，鼠标悬浮在链接上时右下角没有url地址显示
        WebView2.CoreWebView2.Settings.IsStatusBarEnabled = false;

        // 新窗口打开页面的处理
        WebView2.CoreWebView2.NewWindowRequested += CoreWebView2_NewWindowRequested;
        // Url变化的处理
        WebView2.CoreWebView2.SourceChanged += CoreWebView2_SourceChanged;
        // 导航到指定Url
        WebView2.CoreWebView2.Navigate(Uri);
    }

    private void CoreWebView2_NewWindowRequested(object sender, CoreWebView2NewWindowRequestedEventArgs e)
    {
        var deferral = e.GetDeferral();
        e.NewWindow = WebView2.CoreWebView2;
        deferral.Complete();
    }

    private async void CoreWebView2_SourceChanged(object sender, CoreWebView2SourceChangedEventArgs e)
    {
        TextBox_Remid.Clear();
        TextBox_Sid.Clear();
        TextBox_AuthCode.Clear();
        TextBox_SessionId.Clear();

        if (!WebView2.Source.ToString().Contains("http://127.0.0.1/success?code="))
            return;

        var cookies = await WebView2.CoreWebView2.CookieManager.GetCookiesAsync(null);
        if (cookies == null)
        {
            NotifierHelper.Show(NotifierType.Error, $"Login succeeded, but failed to get cookie, please try clearing the cache");
            return;
        }

        foreach (var item in cookies)
        {
            if (item.Name == "remid")
            {
                if (!string.IsNullOrEmpty(item.Value))
                {
                    Vari.Remid = item.Value;
                    TextBox_Remid.Text = Vari.Remid;
                }
                continue;
            }

            if (item.Name == "sid")
            {
                if (!string.IsNullOrEmpty(item.Value))
                {
                    Vari.Sid = item.Value;
                    TextBox_Sid.Text = Vari.Sid;
                }
                continue;
            }
        }

        string code = WebView2.Source.ToString().Replace("http://127.0.0.1/success?code=", "");
        TextBox_AuthCode.Text = code;
        NotifierHelper.Show(NotifierType.Success, $"The login is complete, the SessionId is being obtained, and the Code is { code}");

        var result = await BF1API.GetEnvIdViaAuthCode(code);
        if (result.IsSuccess)
        {
            var envIdViaAuthCode = JsonUtil.JsonDese<EnvIdViaAuthCode>(result.Message);
            Vari.SessionId_Mode2 = envIdViaAuthCode.result.sessionId;
            TextBox_SessionId.Text = Vari.SessionId;
            NotifierHelper.Show(NotifierType.Success, $"Got SessionID successfully:{Vari.SessionId}  |  耗时: {result.ExecTime:0.00} 秒");

            WeakReferenceMessenger.Default.Send("", "SendRemidSid");
        }
        else
        {
            NotifierHelper.Show(NotifierType.Error, $"Failed to get SessionID {result.Message}  |  耗时: {result.ExecTime:0.00} 秒");
        }
    }

    private void Window_WebView2_Closing(object sender, CancelEventArgs e)
    {
        WebView2.Dispose();
    }

    private async void Button_ClearCache_Click(object sender, RoutedEventArgs e) //cant task
    {
        if (MessageBox.Show("Are you sure you want to clear the local cache, this is usually used when the player account information is invalid, you may need to log in to the little helper again", "Warn",
            MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
        {
            await WebView2.CoreWebView2.ExecuteScriptAsync("localStorage.clear()");
            WebView2.CoreWebView2.CookieManager.DeleteAllCookies();

            WebView2.Reload();

            LoggerHelper.Info($"Cleared WebView2 cache successfully");
        }
    }

    private void Button_Cancel_Click(object sender, RoutedEventArgs e)
    {
        this.DialogResult = false;
        this.Close();
    }
}
