using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Spectre.Console;

namespace AdminToolVG;

public static class Log
{
    public static void C(string s)
    {
        Console.WriteLine(s);
        Debug.WriteLine($"C: {s}");
    }
    public static void CM(string s) // Console Markup
    {
        AnsiConsole.MarkupLine(s);
    }
    public static void ExC(Exception ex)
    {
        AnsiConsole.WriteException(ex);
    }
    public static void I(string s)
    {
        Log.D($"INFO: {s}");
    }
    public static void Ex(Exception ex)
    {
        Log.D($"EX: {ex.Message}");
    }
    public static void Ex(Exception ex, string s)
    {
        Log.D($"EX: {ex.Message}");
    }
    public static void Ex(string s)
    {
        Log.D($"EX: s");
    }
    public static void D(string s)
    {
        Debug.WriteLine(s);
    }
}
