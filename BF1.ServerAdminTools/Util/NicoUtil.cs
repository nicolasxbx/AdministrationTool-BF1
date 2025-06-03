using System.Net.Sockets;

namespace BF1.ServerAdminTools;

public static partial class Util
{
    public static long GetUnixFromDate(DateTime dt)
    {
        return ((DateTimeOffset)dt).ToUnixTimeSeconds();
    }
    public static DateTime GetDateFromUnix(long unixseconds)
    {
        DateTimeOffset dateTimeOffset = DateTimeOffset.FromUnixTimeSeconds(unixseconds);
        return dateTimeOffset.UtcDateTime;
    }
    public static string StringFromDateTime(DateTime dt)
    {
        return dt.ToString("dd.MM.yy HH:mm:ss");
    }
    public static string GetCurrentUTCDateTime()
    {
        return DateTime.UtcNow.ToString("dd.MM.yy HH:mm:ss");
    }

    public static string? GetLocalIPAddress()
    {
        var host = Dns.GetHostEntry(Dns.GetHostName());
        foreach (var ip in host.AddressList)
        {
            if (ip.AddressFamily == AddressFamily.InterNetwork)
            {
                return ip.ToString();
            }
        }
        return null;
    }

    #region Json
    public static dynamic? JsonToValue(JsonElement jsonelement)
    {
        JsonValueKind valuekind;
        try
        {
            valuekind = jsonelement.ValueKind;
        }
        catch (Exception ex)
        {
            Log.Ex(ex, $"JsonToValue Valuekind FAIL");
            return null;
        }

        try
        {
            if (valuekind == JsonValueKind.True || valuekind == JsonValueKind.False)
            {
                bool b = jsonelement.GetBoolean();
                return b;
            }
            else if (valuekind == JsonValueKind.String)
            {
                string s = jsonelement.GetString()!;
                return s;
            }
            else if (valuekind == JsonValueKind.Number)
            {
                double number_double;
                try
                {
                    number_double = jsonelement.GetDouble();

                    if (number_double != (int)number_double) // If not Integer
                    {
                        return number_double;
                    }
                }
                catch { }

                long number_long;
                try
                {
                    number_long = jsonelement.GetInt64();

                    if (number_long >= int.MaxValue) //If l is bigger than the in32 range
                    {
                        return number_long;
                    }
                    else //if l can b
                    {
                        return (int)number_long;
                    }
                }
                catch { }

                return null;
            }
            else
            {
                Log.Ex("JsonToValue - Value type not recognized.");
                return null;
            }
        }
        catch (Exception ex)
        {
            Log.Ex(ex, "JsonToValue, Parsing Fail");
            return null;
        }
    }
    #endregion
}

#region Log
internal static class Log
{
    public static void I(string msg) //Log Information
    {
        string s = $"{Util.GetCurrentUTCDateTime()} INFO: {msg}";
        WriteFileInfo(s);        
        Debug.WriteLine(s);
    }

    public static void Ex(Exception ex) //Log Exceptions
    {
        string s = $"{Util.GetCurrentUTCDateTime()} EX: {ex.Message}\n   {ex.TargetSite}\n{ex.StackTrace}";
        WriteFileEx(s);
        Debug.WriteLine(s);        
    }
    public static void Ex(Exception ex, string msg) //Log Exceptions and addition information
    {
        string s = $"{Util.GetCurrentUTCDateTime()} EX: {ex.Message}\n{msg}\n   {ex.TargetSite}\n{ex.StackTrace}";
        WriteFileEx(s);
        Debug.WriteLine(s);
    }
    public static void Ex(string msg) //Only log a string
    {
        string s = $"{Util.GetCurrentUTCDateTime()} EX: {msg}";
        WriteFileEx(s);
        Debug.WriteLine(s);
    }

    public static void C(string msg)
    {
        string s = msg;        
        Console.WriteLine(s);
        Debug.WriteLine(s);
    }
    public static void C(Exception ex)
    {
        string s = $"EX: {ex.Message}\n{ex.TargetSite}\n{ex.StackTrace}";
        Console.WriteLine(s);
        Debug.WriteLine(s);
    }
    public static void C()
    {
        Console.WriteLine();
    }

    public static void Trace()
    {
        StackTrace st = new();
        string caller = st.GetFrame(1).GetMethod().Name;
        Log.I($"Trace: {caller}");
    }

    private static void WriteFileInfo(string s)
    {
        try
        {
            File.AppendAllText(@"C:\ProgramData\BF1 Server\LogI.txt", s + Environment.NewLine);
        }
        catch
        {
            Debug.WriteLine($"{Util.GetCurrentUTCDateTime()} INFO: Couldn't write File INFO!");
        }
    }
    private static void WriteFileEx(string s)
    {
        try
        {
            File.AppendAllText(@"C:\ProgramData\BF1 Server\LogEx.txt", s + Environment.NewLine);
        }
        catch
        {
            Debug.WriteLine($"{Util.GetCurrentUTCDateTime()} EX: Couldn't write File EX!");
        }
    }
}
#endregion
