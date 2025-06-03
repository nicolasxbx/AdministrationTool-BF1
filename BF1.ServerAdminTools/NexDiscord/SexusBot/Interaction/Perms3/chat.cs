using BF1.ServerAdminTools.Common.Utils;
using BF1.ServerAdminTools.Features.Chat;
using BF1.ServerAdminTools.Features.Core;
using BF1.ServerAdminTools.Features.Utils;
using NexDiscord;

namespace BF1.ServerAdminTools.NexDiscord;

public static partial class SexusBot
{
    public static partial class Interaction
    {
        public static async Task chat()
        {
            string[] words = VariS.Current.words;
            string msg0 = VariS.Current.msg;
            if (words.Length >= 2)
            {
                string chatmsg = "";
                if (words[1] == "rules")
                {
                    chatmsg = "[VG] Vanguard are recruiting - join us at discord.gg/VGBF1 - RULES: NO SMG08, ARTYTRUCK AND HEAVY BOMBER, MAX 200 PING.";
                }
                else
                {
                    chatmsg = msg0.Substring(6);
                }

                if (VariS.chat_function_last_msg == chatmsg) //SPAM
                {
                    VariS.chat_function_last_msg = chatmsg;
                    return;
                }
                else //NOT SPAM
                {
                    VariS.chat_function_last_msg = chatmsg;
                }

                bool b = ChatFunction(chatmsg);
                if (b == true)
                {
                    await OutAnsi($"{Ansi.Cyan}✅ Message successfully sent to the ingame chat!");
                }
                else
                {
                    await OutAnsi($"{Ansi.Red}❌ An error occured. Check if the host's game is in borderless mode.");
                }
            }
        }
        
        public static bool ChatFunction(string chatmsg)
        {
            AudioUtil.ClickSound();

            ChatHelper.SetIMEStateToEN();

            ChatHelper.KeyPressDelay = (int)50;

            if (string.IsNullOrEmpty(chatmsg.Trim()))
            {
                //NotifierHelper.Show(NotifierType.Warning, "The content of the chat box is empty, the operation is canceled");
                return false;
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

                        string msg = chatmsg.Trim();
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

                        //NotifierHelper.Show(NotifierType.Success, "Sending text to Battlefield 1 chat box succeeded");
                        return true;
                    }
                    else
                    {
                        //NotifierHelper.Show(NotifierType.Warning, "Chat box message pointer not found");
                        return false;
                    }
                }
                else
                {
                    //NotifierHelper.Show(NotifierType.Warning, "Chat box is not open");
                    return false;
                }
            }
            else
            {
                //NotifierHelper.Show(NotifierType.Error, "The chat function failed to initialize, please restart the program");
                return false;
            }
        }
    }
}

