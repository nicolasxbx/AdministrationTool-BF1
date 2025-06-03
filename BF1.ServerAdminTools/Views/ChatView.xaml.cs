using BF1.ServerAdminTools.Common.Utils;
using BF1.ServerAdminTools.Common.Helper;
using BF1.ServerAdminTools.Features.Chat;
using BF1.ServerAdminTools.Features.Core;
using BF1.ServerAdminTools.Features.Utils;
using BF1.ServerAdminTools.Features.Config;

using CommunityToolkit.Mvvm.Input;

namespace BF1.ServerAdminTools.Views;

/// <summary>
/// ChatView.xaml 的交互逻辑
/// </summary>
public partial class ChatView : UserControl
{
    private ChatConfig ChatConfig { get; set; } = new();

    /// <summary>
    /// 自动发送文本定时器
    /// </summary>
    private Timer timerAutoSendMsg;
    private List<string> queueMsg = new();

    private int queueMsgSleep = 1;

    /// <summary>
    /// 挂机防踢定时器
    /// </summary>
    private Timer timerNoAFK = null;

    public RelayCommand SendChsMessageCommand { get; set; }

    public ChatView()
    {
        InitializeComponent();
        this.DataContext = this;
        MainWindow.ClosingDisposeEvent += MainWindow_ClosingDisposeEvent;

        if (!File.Exists(FileUtil.F_Chat_Path))
        {
            ChatConfig.ChatContents = new();

            for (int i = 0; i < 10; i++)
            {
                ChatConfig.ChatContents.Add(new ChatConfig.ChatContent()
                {
                    Name = $"Custom Chat {i}",
                    Content = "[VG] Vanguard are recruiting - join us at discord.gg/VGBF1 - RULES: NO SMG08, ARTYTRUCK AND HEAVY BOMBER, MAX 200 PING."
                });
            }

            File.WriteAllText(FileUtil.F_Chat_Path, JsonUtil.JsonSeri(ChatConfig));
        }

        if (File.Exists(FileUtil.F_Chat_Path))
        {
            using (var streamReader = new StreamReader(FileUtil.F_Chat_Path))
            {
                ChatConfig = JsonUtil.JsonDese<ChatConfig>(streamReader.ReadToEnd());
            }
        }

        timerAutoSendMsg = new()
        {
            AutoReset = true
        };
        timerAutoSendMsg.Elapsed += TimerAutoSendMsg_Elapsed;

        timerNoAFK = new()
        {
            AutoReset = true,
            Interval = TimeSpan.FromSeconds(30).TotalMilliseconds
        };
        timerNoAFK.Elapsed += TimerNoAFK_Elapsed;

        SendChsMessageCommand = new(SendChsMessage);
    }

    private void MainWindow_ClosingDisposeEvent()
    {
        ChatConfig.ChatContents[RadioButtonWhoIsChecked()].Content = TextBox_InputMsg.Text;

        File.WriteAllText(FileUtil.F_Chat_Path, JsonUtil.JsonSeri(ChatConfig));
    }

    private void TimerAutoSendMsg_Elapsed(object sender, ElapsedEventArgs e)
    {
        ChatHelper.SetIMEStateToEN();

        for (int i = 0; i < queueMsg.Count; i++)
        {
            ChatHelper.SendTextToBf1Game(queueMsg[i]);
            Thread.Sleep(queueMsgSleep * 1000);
        }
    }

    private void TimerNoAFK_Elapsed(object sender, ElapsedEventArgs e)
    {
        ChatHelper.SetIMEStateToEN();

        Memory.SetForegroundWindow();
        Thread.Sleep(50);

        WinAPI.Keybd_Event(WinVK.TAB, WinAPI.MapVirtualKey(WinVK.TAB, 0), 0, 0);
        Thread.Sleep(3000);
        WinAPI.Keybd_Event(WinVK.TAB, WinAPI.MapVirtualKey(WinVK.TAB, 0), 2, 0);
        Thread.Sleep(50);
    }

    private void SendChsMessage()
    {
        AudioUtil.ClickSound();

        ChatHelper.SetIMEStateToEN();

        ChatHelper.KeyPressDelay = (int)Slider_KeyPressDelay.Value;

        if (string.IsNullOrEmpty(TextBox_InputMsg.Text.Trim()))
        {
            NotifierHelper.Show(NotifierType.Warning, "The content of the chat box is empty, the operation is canceled");
            return;
        }

        if (ChatMsg.GetAllocateMemoryAddress() != 0)
        {
            // 将窗口置顶
            Memory.SetForegroundWindow();
            Thread.Sleep(50);

            // 如果聊天框开启，让他关闭
            if (ChatMsg.GetChatIsOpen())
                ChatHelper.KeyPress(WinVK.RETURN, ChatHelper.KeyPressDelay);

            // 模拟按键，开启聊天框
            ChatHelper.KeyPress(WinVK.J, ChatHelper.KeyPressDelay);

            if (ChatMsg.GetChatIsOpen())
            {
                if (ChatMsg.ChatMessagePointer() != 0)
                {
                    // 挂起战地1进程
                    NtProc.SuspendProcess(Memory.GetProcessId());

                    string msg = TextBox_InputMsg.Text.Trim();
                    msg = ChsUtil.ToTraditionalChinese(ChatHelper.ToDBC(msg));
                    var length = PlayerUtil.GetStrLength(msg);
                    Memory.WriteStringUTF8(ChatMsg.GetAllocateMemoryAddress(), null, msg);

                    var startPtr = ChatMsg.ChatMessagePointer() + ChatMsg.OFFSET_CHAT_MESSAGE_START;
                    var endPtr = ChatMsg.ChatMessagePointer() + ChatMsg.OFFSET_CHAT_MESSAGE_END;

                    var oldStartPtr = Memory.Read<long>(startPtr);
                    var oldEndPtr = Memory.Read<long>(endPtr);

                    Memory.Write<long>(startPtr, ChatMsg.GetAllocateMemoryAddress());
                    Memory.Write<long>(endPtr, ChatMsg.GetAllocateMemoryAddress() + length);

                    // 恢复战地1进程
                    NtProc.ResumeProcess(Memory.GetProcessId());
                    ChatHelper.KeyPress(WinVK.RETURN, ChatHelper.KeyPressDelay);

                    // 挂起战地1进程
                    NtProc.SuspendProcess(Memory.GetProcessId());
                    Memory.Write<long>(startPtr, oldStartPtr);
                    Memory.Write<long>(endPtr, oldEndPtr);
                    // 恢复战地1进程
                    NtProc.ResumeProcess(Memory.GetProcessId());

                    NotifierHelper.Show(NotifierType.Success, "Sending text to Battlefield 1 chat box succeeded");
                }
                else
                {
                    NotifierHelper.Show(NotifierType.Warning, "Chat box message pointer not found");
                }
            }
            else
            {
                NotifierHelper.Show(NotifierType.Warning, "Chat box is not open");
            }
        }
        else
        {
            NotifierHelper.Show(NotifierType.Error, "The chat function failed to initialize, please restart the program");
        }
    }

    private void TextBox_InputMsg_TextChanged(object sender, TextChangedEventArgs e)
    {
        TextBlock_TxtLength.Text = $"current text length : {PlayerUtil.GetStrLength(TextBox_InputMsg.Text)} characters";

        if (ChatConfig.ChatContents != null)
            ChatConfig.ChatContents[RadioButtonWhoIsChecked()].Content = TextBox_InputMsg.Text;
    }

    private void RadioButton_DefaultText0_Click(object sender, RoutedEventArgs e)
    {
        TextBox_InputMsg.Text = ChatConfig.ChatContents[RadioButtonWhoIsChecked()].Content;
    }

    private int RadioButtonWhoIsChecked()
    {
        if (RadioButton_DefaultText0 != null && RadioButton_DefaultText0.IsChecked == true)
            return 0;

        if (RadioButton_DefaultText1 != null && RadioButton_DefaultText1.IsChecked == true)
            return 1;

        if (RadioButton_DefaultText2 != null && RadioButton_DefaultText2.IsChecked == true)
            return 2;

        if (RadioButton_DefaultText3 != null && RadioButton_DefaultText3.IsChecked == true)
            return 3;

        if (RadioButton_DefaultText4 != null && RadioButton_DefaultText4.IsChecked == true)
            return 4;

        if (RadioButton_DefaultText5 != null && RadioButton_DefaultText5.IsChecked == true)
            return 5;

        if (RadioButton_DefaultText6 != null && RadioButton_DefaultText6.IsChecked == true)
            return 6;

        if (RadioButton_DefaultText7 != null && RadioButton_DefaultText7.IsChecked == true)
            return 7;

        if (RadioButton_DefaultText8 != null && RadioButton_DefaultText8.IsChecked == true)
            return 8;

        if (RadioButton_DefaultText9 != null && RadioButton_DefaultText9.IsChecked == true)
            return 9;

        return 0;
    }

    private void CheckBox_ActiveAutoSendMsg_Click(object sender, RoutedEventArgs e)
    {
        AudioUtil.ClickSound();//tna
        if (CheckBox_ActiveAutoSendMsg.IsChecked == true)
        {
            queueMsg.Clear();

            if (CheckBox_DefaultText0 != null && CheckBox_DefaultText0.IsChecked == true)
                queueMsg.Add(ChatConfig.ChatContents[0].Content);

            if (CheckBox_DefaultText1 != null && CheckBox_DefaultText1.IsChecked == true)
                queueMsg.Add(ChatConfig.ChatContents[1].Content);

            if (CheckBox_DefaultText2 != null && CheckBox_DefaultText2.IsChecked == true)
                queueMsg.Add(ChatConfig.ChatContents[2].Content);

            if (CheckBox_DefaultText3 != null && CheckBox_DefaultText3.IsChecked == true)
                queueMsg.Add(ChatConfig.ChatContents[3].Content);

            if (CheckBox_DefaultText4 != null && CheckBox_DefaultText4.IsChecked == true)
                queueMsg.Add(ChatConfig.ChatContents[4].Content);

            if (CheckBox_DefaultText5 != null && CheckBox_DefaultText5.IsChecked == true)
                queueMsg.Add(ChatConfig.ChatContents[5].Content);

            if (CheckBox_DefaultText6 != null && CheckBox_DefaultText6.IsChecked == true)
                queueMsg.Add(ChatConfig.ChatContents[6].Content);

            if (CheckBox_DefaultText7 != null && CheckBox_DefaultText7.IsChecked == true)
                queueMsg.Add(ChatConfig.ChatContents[7].Content);

            if (CheckBox_DefaultText8 != null && CheckBox_DefaultText8.IsChecked == true)
                queueMsg.Add(ChatConfig.ChatContents[8].Content);

            if (CheckBox_DefaultText9 != null && CheckBox_DefaultText9.IsChecked == true)
                queueMsg.Add(ChatConfig.ChatContents[9].Content);

            ChatHelper.KeyPressDelay = (int)Slider_KeyPressDelay.Value;

            queueMsgSleep = (int)Slider_AutoSendMsgSleep.Value;

            timerAutoSendMsg.Interval = TimeSpan.FromMinutes(Slider_AutoSendMsg.Value).TotalMilliseconds;
            timerAutoSendMsg.Start();

            NotifierHelper.Show(NotifierType.Notification, "Scheduled sending of specified text is enabled");
        }
        else
        {
            timerAutoSendMsg.Stop();
            NotifierHelper.Show(NotifierType.Notification, "Scheduled sending of specified text is enabled");
        }
    }

    private void CheckBox_ActiveNoAFK_Click(object sender, RoutedEventArgs e)
    {
        AudioUtil.ClickSound();//tna
        if (CheckBox_ActiveNoAFK.IsChecked == true)
        {
            timerNoAFK.Start();
            NotifierHelper.Show(NotifierType.Notification, "Scheduled sending of specified text is enabled");
        }
        else
        {
            timerNoAFK.Stop();
            NotifierHelper.Show(NotifierType.Notification, "Scheduled sending of specified text is enabled");
        }
    }
}
