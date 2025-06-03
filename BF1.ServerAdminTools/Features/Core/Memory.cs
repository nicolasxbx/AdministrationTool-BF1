﻿using BF1.ServerAdminTools.Common.Helper;

namespace BF1.ServerAdminTools.Features.Core;

public static class Memory
{
    private static Process process;
    private static IntPtr windowHandle;
    private static IntPtr processHandle;
    private static int processId;
    private static long pBaseAddress;

    public static bool Initialize(string ProcessName)
    {
        try
        {
            LoggerHelper.Info($"目标程序名称 {ProcessName}");
            var pArray = Process.GetProcessesByName(ProcessName);
            if (pArray.Length > 0)
            {
                // 默认取第一个
                process = pArray[0];
                // 二次验证
                foreach (var item in pArray)
                {
                    if (item.MainWindowTitle.Equals("Battlefield™ 1"))
                        process = item;
                }

                windowHandle = process.MainWindowHandle;
                Vari.NwindowHandle = windowHandle;
                LoggerHelper.Info($"目标程序窗口句柄 {windowHandle}");
                processId = process.Id;
                Vari.NprocessId = processId;
                LoggerHelper.Info($"目标程序进程ID {processId}");
                processHandle = WinAPI.OpenProcess(ProcessAccessFlags.All, false, processId);
                Vari.NprocessHandle = processHandle;
                LoggerHelper.Info($"目标程序进程句柄 {processHandle}");
                if (process.MainModule != null)
                {
                    pBaseAddress = process.MainModule.BaseAddress.ToInt64();
                    Vari.NpBaseAddress = pBaseAddress;
                    LoggerHelper.Info($"目标程序主模块基址 0x{pBaseAddress:x}");
                    return true;
                }
                else
                {
                    LoggerHelper.Error($"发生错误，目标程序主模块基址为空");
                    return false;
                }
            }
            else
            {
                LoggerHelper.Error($"发生错误，未发现目标进程");
                return false;
            }
        }
        catch (Exception ex)
        {
            LoggerHelper.Error($"战地1内存模块初始化异常", ex);
            Log.Ex(ex);
            return false;
        }
    }

    public static void CloseHandle()
    {
        if (processHandle != IntPtr.Zero)
            WinAPI.CloseHandle(processHandle);
    }

    public static IntPtr GetWindowHandle()
    {
        return windowHandle;
    }

    public static IntPtr GetProcessHandle()
    {
        return processHandle;
    }

    public static long GetBaseAddress()
    {
        return pBaseAddress;
    }

    public static bool IsForegroundWindow()
    {
        return windowHandle == WinAPI.GetForegroundWindow();
    }

    public static void SetForegroundWindow()
    {
        WinAPI.SetForegroundWindow(windowHandle);
    }

    public static int GetProcessId()
    {
        return processId;
    }

    public static WindowData GetGameWindowData()
    {
        // 获取指定窗口句柄的窗口矩形数据和客户区矩形数据
        WinAPI.GetWindowRect(windowHandle, out W32RECT windowRect);
        WinAPI.GetClientRect(windowHandle, out W32RECT clientRect);

        // 计算窗口区的宽和高
        int windowWidth = windowRect.Right - windowRect.Left;
        int windowHeight = windowRect.Bottom - windowRect.Top;

        // 处理窗口最小化
        if (windowRect.Left == -32000)
        {
            return new WindowData()
            {
                Left = 0,
                Top = 0,
                Width = 1,
                Height = 1
            };
        }

        // 计算客户区的宽和高
        int clientWidth = clientRect.Right - clientRect.Left;
        int clientHeight = clientRect.Bottom - clientRect.Top;

        // 计算边框
        int borderWidth = (windowWidth - clientWidth) / 2;
        int borderHeight = windowHeight - clientHeight - borderWidth;

        return new WindowData()
        {
            Left = windowRect.Left += borderWidth,
            Top = windowRect.Top += borderHeight,
            Width = clientWidth,
            Height = clientHeight
        };
    }

    private static long GetPtrAddress(long pointer, int[] offset)
    {
        if (offset != null)
        {
            byte[] buffer = new byte[8];
            WinAPI.ReadProcessMemory(processHandle, pointer, buffer, buffer.Length, out _);

            for (int i = 0; i < (offset.Length - 1); i++)
            {
                pointer = BitConverter.ToInt64(buffer, 0) + offset[i];
                WinAPI.ReadProcessMemory(processHandle, pointer, buffer, buffer.Length, out _);
            }

            pointer = BitConverter.ToInt64(buffer, 0) + offset[offset.Length - 1];
        }
        Vari.NPtrAddress = pointer;
        return pointer;
    }

    public static T Read<T>(long basePtr, int[] offsets) where T : struct
    {
        byte[] buffer = new byte[Marshal.SizeOf(typeof(T))];
        WinAPI.ReadProcessMemory(processHandle, GetPtrAddress(basePtr, offsets), buffer, buffer.Length, out _);
        return ByteArrayToStructure<T>(buffer);
    }

    public static T Read<T>(long address) where T : struct
    {
        byte[] buffer = new byte[Marshal.SizeOf(typeof(T))];
        WinAPI.ReadProcessMemory(processHandle, address, buffer, buffer.Length, out _);
        return ByteArrayToStructure<T>(buffer);
    }

    public static void Write<T>(long basePtr, int[] offsets, T value) where T : struct
    {
        byte[] buffer = StructureToByteArray(value);
        WinAPI.WriteProcessMemory(processHandle, GetPtrAddress(basePtr, offsets), buffer, buffer.Length, out _);
    }

    public static void Write<T>(long address, T value) where T : struct
    {
        byte[] buffer = StructureToByteArray(value);
        WinAPI.WriteProcessMemory(processHandle, address, buffer, buffer.Length, out _);
    }

    public static string ReadString(long address, int size)
    {
        byte[] buffer = new byte[size];
        WinAPI.ReadProcessMemory(processHandle, address, buffer, size, out _);

        for (int i = 0; i < buffer.Length; i++)
        {
            if (buffer[i] == 0)
            {
                byte[] _buffer = new byte[i];
                Buffer.BlockCopy(buffer, 0, _buffer, 0, i);
                return Encoding.ASCII.GetString(_buffer);
            }
        }

        return Encoding.ASCII.GetString(buffer);
    }

    public static string ReadString(long basePtr, int[] offsets, int size)
    {
        byte[] buffer = new byte[size];
        WinAPI.ReadProcessMemory(processHandle, GetPtrAddress(basePtr, offsets), buffer, size, out _);

        for (int i = 0; i < buffer.Length; i++)
        {
            if (buffer[i] == 0)
            {
                byte[] _buffer = new byte[i];
                Buffer.BlockCopy(buffer, 0, _buffer, 0, i);
                return Encoding.ASCII.GetString(_buffer);
            }
        }

        return Encoding.ASCII.GetString(buffer);
    }

    public static void WriteString(long basePtr, int[] offsets, string str)
    {
        byte[] buffer = new ASCIIEncoding().GetBytes(str);
        WinAPI.WriteProcessMemory(processHandle, GetPtrAddress(basePtr, offsets), buffer, buffer.Length, out _);
    }

    public static void WriteStringUTF8(long basePtr, int[] offsets, string str)
    {
        byte[] buffer = new UTF8Encoding().GetBytes(str);
        WinAPI.WriteProcessMemory(processHandle, GetPtrAddress(basePtr, offsets), buffer, buffer.Length, out _);
    }

    //////////////////////////////////////////////////////////////////

    public static bool IsValid(long Address)
    {
        return Address >= 0x10000 && Address < 0x000F000000000000;
    }

    private static T ByteArrayToStructure<T>(byte[] bytes) where T : struct
    {
        var handle = GCHandle.Alloc(bytes, GCHandleType.Pinned);
        try
        {
            var obj = Marshal.PtrToStructure(handle.AddrOfPinnedObject(), typeof(T));
            if (obj != null)
                return (T)obj;
            else
                return default(T);
        }
        finally
        {
            handle.Free();
        }
    }

    private static byte[] StructureToByteArray(object obj)
    {
        int length = Marshal.SizeOf(obj);
        byte[] array = new byte[length];
        IntPtr pointer = Marshal.AllocHGlobal(length);
        Marshal.StructureToPtr(obj, pointer, true);
        Marshal.Copy(pointer, array, 0, length);
        Marshal.FreeHGlobal(pointer);        
        return array;
    }
}

public struct WindowData
{
    public int Left;
    public int Top;
    public int Width;
    public int Height;
}
