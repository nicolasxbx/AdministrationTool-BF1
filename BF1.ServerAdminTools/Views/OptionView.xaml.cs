using BF1.ServerAdminTools.Common.Utils;

namespace BF1.ServerAdminTools.Views;

/// <summary>
/// OptionView.xaml 的交互逻辑
/// </summary>
public partial class OptionView : UserControl
{
    public OptionView()
    {
        InitializeComponent();
        this.DataContext = this;
        MainWindow.ClosingDisposeEvent += MainWindow_ClosingDisposeEvent;

        //TextBlock_ClientVersionInfo.Text = $"ClientVersion：{CoreUtil.ClientVersionInfo} i heckin love nexus";
        TextBlock_ClientVersionInfo.Text = $"Version: {Vari.Nexversion}";
        TextBlock_LastWriteTime.Text = $"Build Time: {File.GetLastWriteTime(Process.GetCurrentProcess().MainModule.FileName)}";

        switch (AudioUtil.ClickSoundIndex)
        {
            case 0:
                RadioButton_ClickAudioSelect0.IsChecked = true;
                break;
            case 1:
                RadioButton_ClickAudioSelect1.IsChecked = true;
                break;
            case 2:
                RadioButton_ClickAudioSelect2.IsChecked = true;
                break;
            case 3:
                RadioButton_ClickAudioSelect3.IsChecked = true;
                break;
            case 4:
                RadioButton_ClickAudioSelect4.IsChecked = true;
                break;
            case 5:
                RadioButton_ClickAudioSelect5.IsChecked = true;
                break;
        }       
    }

    private void MainWindow_ClosingDisposeEvent()
    {

    }

    private void RadioButton_ClickAudioSelect_Click(object sender, RoutedEventArgs e)
    {
        string str = (sender as RadioButton).Content.ToString();

        switch (str)
        {
            case "0":
                AudioUtil.ClickSoundIndex = 0;                
                break;
            case "1":
                AudioUtil.ClickSoundIndex = 1;
                AudioUtil.ClickSound();
                break;
            case "2":
                AudioUtil.ClickSoundIndex = 2;
                AudioUtil.ClickSound();
                break;
            case "3":
                AudioUtil.ClickSoundIndex = 3;
                AudioUtil.ClickSound();
                break;
            case "4":
                AudioUtil.ClickSoundIndex = 4;
                AudioUtil.ClickSound();
                break;
            case "5":
                AudioUtil.ClickSoundIndex = 5;
                AudioUtil.ClickSound();
                break;
        }
    }

    private void TextBox_PingLimit_TextChanged(object sender, TextChangedEventArgs e) //tna
    {
        string s = TextBox_PingLimit.Text;
        int i = 200;

        if (int.TryParse(s, out i))
        {
            if (i >= 100 && i <= 500)
            {
                Vari.PingLimit = i;
                TextBox_PingLimitText.Text = "\n✔️Ping Limit(100-500):";
            }
            else
            {
                TextBox_PingLimitText.Text = "\n❌Ping Limit(100-500):";
            }
        }
        else
        {
            TextBox_PingLimitText.Text = "\n❌Ping Limit(100-500):";
        }
    }

    private void TextBox_TicketLimit_TextChanged(object sender, TextChangedEventArgs e) //tna
    {
        string s = TextBox_TicketLimit.Text;
        int i = 200;

        if (int.TryParse(s, out i))
        {
            if (i >= 100 && i <= 500)
            {
                Vari.TicketLimit = i;
                TextBox_TicketLimitText.Text = "\n✔️Minimum Ticket Difference for Winswitching(100-500):";
            }
            else
            {
                TextBox_TicketLimitText.Text = "\n❌Minimum Ticket Difference for Winswitching(100-500):";
            }
        }
        else
        {
            TextBox_TicketLimitText.Text = "\n❌Minimum Ticket Difference for Winswitching(100-500):";
        }
    }

    private void CheckBox_Webhook_Click(object sender, RoutedEventArgs e)
    {
        if (CheckBox_Webhook.IsChecked == true)
        {
            /*if (Vari.NexGlobalWhiteListServers.Contains(Vari.GameId) == true)
            {
                Vari.NexCustomWebhook = true;
            }*/
            Vari.Webhooks.UseCustomWebhooks = true;
            /*
            else
            {
                Vari.NexCustomWebhook = false;
                CheckBox_Webhook.IsChecked = false;
                NotifierHelper.Show(NotifierType.Error, $"Server is not Whitelisted.");
            }*/
        }
        else
        {
            Vari.Webhooks.UseCustomWebhooks = false;
            CheckBox_Webhook.IsChecked = false;
        }
    }    
    
}
