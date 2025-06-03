using BF1.ServerAdminTools.Common.Utils;
using BF1.ServerAdminTools.Common.Helper;
using BF1.ServerAdminTools.Features.API;

namespace BF1.ServerAdminTools.Windows;

/// <summary>
/// CustomKickWindow.xaml 的交互逻辑
/// </summary>
public partial class CustomKickWindow
{
    public string PlayerName { get; set; }
    public long PersonaId { get; set; }

    public CustomKickWindow(string playerName, long personaId)
    {
        InitializeComponent();
        this.DataContext = this;

        PlayerName = playerName;
        PersonaId = personaId;
    }

    private async void Button_KickPlayer_Click(object sender, RoutedEventArgs e)
    {
        AudioUtil.ClickSound();
        this.Hide();

        var reason = TextBox_CustomReason.Text.Trim();
        if (string.IsNullOrEmpty(reason))
            reason = "-1";
        else
            reason = ChsUtil.ToTraditionalChinese(reason);

        NotifierHelper.Show(NotifierType.Information, $"kicking player { PlayerName} ...");

        var result = await BF1API.AdminKickPlayer(PersonaId, reason);
        if (result.IsSuccess)
        {
            NotifierHelper.Show(NotifierType.Success, $"Kicked Player {PlayerName} Success | Time: {result.ExecTime:0.00} seconds");
        }
        else
        {
            NotifierHelper.Show(NotifierType.Error, $"Kicked Player {PlayerName} Fail {result.Message} | Time: {result.ExecTime:0.00} seconds");
        }

        this.Close();
    }
}
