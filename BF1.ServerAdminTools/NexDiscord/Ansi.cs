using BF1.ServerAdminTools.Converters;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;

namespace NexDiscord;

public static class Ansi
{
    
    public static string None { get; } = "\u001b[0m";
    public static string U { get; } = "\u001b";
    public static string Bold { get; } = "\u001b[0;1m";
    public static string UL { get; } = "\u001b[0;4m";

    public static string Black { get; } = "\u001b[0;30m";
    public static string Red { get; } = "\u001b[0;31m";
    public static string Green { get; } = "\u001b[0;32m";
    public static string Yellow { get; } = "\u001b[0;33m";
    public static string Blue { get; } = "\u001b[0;34m";
    public static string Magenta { get; } = "\u001b[0;35m";
    public static string Cyan { get; } = "\u001b[0;36m";
    public static string White { get; } = "\u001b[0;37m";   

    public static string BG_Black { get; } = "\u001b[0;40m";
    public static string BG_Red { get; } = "\u001b[0;41m";
    public static string BG_Magenta { get; } = "\u001b[0;45m";
    public static string BG_Cyan { get; } = "\u001b[0;46m";
    public static string BG_White { get; } = "\u001b[0;47m";

    public static class B
    {
        public static string None { get; } = "\u001b[0;1m";
        public static string Black { get; } = "\u001b[0;1;30m";
        public static string Red { get; } = "\u001b[0;1;31m";
        public static string Green { get; } = "\u001b[0;1;32m";
        public static string Yellow { get; } = "\u001b[0;1;33m";
        public static string Blue { get; } = "\u001b[0;1;34m";
        public static string Magenta { get; } = "\u001b[0;1;35m";
        public static string Cyan { get; } = "\u001b[0;1;36m";
        public static string White { get; } = "\u001b[0;1;37m";

        public static string Bold { get; } = "\u001b[0;1m";
        public static string UL { get; } = "\u001b[0;1;4m";

        public static string BG_Black { get; } = "\u001b[0;1;40m";
        public static string BG_Red { get; } = "\u001b[0;1;41m";
        public static string BG_Magenta { get; } = "\u001b[0;1;45m";
        public static string BG_Cyan { get; } = "\u001b[0;1;46m";
        public static string BG_White { get; } = "\u001b[0;1;47m";
    }

    public static class Num
    {
        public static string Black { get; } = "30";
        public static string Red { get; } = "31";
        public static string Green { get; } = "32";
        public static string Yellow { get; } = "33";
        public static string Blue { get; } = "34";
        public static string Magenta { get; } = "35";
        public static string Cyan { get; } = "36";
        public static string White { get; } = "37";

        public static string Bold { get; } = "1";
        public static string UL { get; } = "4";

        public static string BG_Black { get; } = "40";
        public static string BG_Red { get; } = "41";
        public static string BG_Magenta { get; } = "45";
        public static string BG_Cyan { get; } = "46";
        public static string BG_White { get; } = "47";
    }

    public static string A(string attribute)
    {
        return $"\u001b[0;{attribute}m";
    }
    public static string A(string attribute, string attribute2) 
    {
        return $"\u001b[0;{attribute};{attribute2}m";
    }
    public static string A(string attribute, string attribute2, string attribute3)
    {
        return $"\u001b[0;{attribute};{attribute2};{attribute3}m";
    }
    public static string AB()//Bold
    {
        return "\u001b[0;1m";
    }
    public static string AB(string attribute) 
    {
        return $"\u001b[0;1;{attribute}m";
    }
    public static string AB(string attribute, string attribute2)
    {
        return $"\u001b[0;1;{attribute};{attribute2}m";
    }
    public static string AB(string attribute, string attribute2, string attribute3)
    {
        return $"\u001b[0;1;{attribute};{attribute2};{attribute3}m";
    }
}
