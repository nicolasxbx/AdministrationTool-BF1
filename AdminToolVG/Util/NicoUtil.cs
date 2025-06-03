namespace AdminToolVG;

public static class Util
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

    public static void OpenLink(string url)
    {
        try
        {
            Process.Start(url);
        }
        catch
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                url = url.Replace("&", "^&");
                Process.Start(new ProcessStartInfo("cmd", $"/c start {url}") { CreateNoWindow = true });
            }
        }
    }
}


