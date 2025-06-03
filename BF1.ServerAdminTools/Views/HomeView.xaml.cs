namespace BF1.ServerAdminTools.Views;

/// <summary>
/// HomeView.xaml 的交互逻辑
/// </summary>
public partial class HomeView : UserControl
{
    public HomeView()
    {
        InitializeComponent();
        TextBox_Notice.Text = "To Auto-Kick:\n1. Go to the Rule-Tab and set the weapons you want to ban. (Remember to save the ruleset by pressing 'save'\n2. Press the Auto-Kick Slider and tick the Boxes you want\n3. Congratulations, you are now auto-kicking.\n\nSomething not working?\n1. Restart your game and then the tool. Then spectate the Server you are Admin in.\n2. Verify yourself in 'Auth'\n3. Go to 'Server' and press 'Get current Server Details'\n4. Then try again\n\nYour history of all-time kicks and team-switches is saved as a database\n" + @"in C:\ProgramData\BF1 Server\Data\AdminLog.db ." + "\nTo get to the file, press the 'Open Configuration Folder'-Button in Rules.\nTo read/open it, use https://inloop.github.io/sqlite-viewer/ or another SQL-Viewer";
        TextBox_Change.Text = $"VG Admin Tool, Subsequential developing by Nicolas B., Tool's Core from CrazyZhang666\nIf you want to support me: \nhttps://www.buymeacoffee.com/nex1 \n\n\nSome latest change logs (don't ask which versions):\nAuto-Ping Kicking implemented from scratch.\nCustom Ping Limit added\nCustom Winswitching Limit added\nUI-Design Overhaul (overall theme change, Red Buttons now indicate importance)\nImplemented Auto-Checking of BFBANs in Server from Scratch.\nBoth Ping and BFBAN-Checking will check Players every 60 Seconds.\nWinswitching won't trigger on Conquest Assault Maps now. (Because one team starts with 300 Tickets difference)\nSimplified Frontend UI alot, so Authorizing and Environment-Checking is done automatically. Now you can instantly Auto-Kick on startup.\nNew three functions are now only executable on Whitelisted Servers, as I put way too much work into these. 5€ per permanent Server-Whitelist.\nFixed this Window not loading in 6.5\nFixed Discord Not Logging your tool shutdown when closing BF1 first.\nNow Balance Switchers will get logged into SQL and to Discord.\n Added the Auto-Run feature";
    }
}
