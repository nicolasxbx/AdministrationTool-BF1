﻿using BF1.ServerAdminTools.Features.Client;

namespace BF1.ServerAdminTools.Features.Utils;

public static class PlayerUtil
{
    /// <summary>
    /// 小数类型的时间秒，转为mm:ss格式
    /// </summary>
    /// <param name="time"></param>
    /// <returns></returns>
    public static string SecondsToMMSS(float time)
    {
        if (time >= 0 && time <= 36000)
        {
            var timeSpan = TimeSpan.FromSeconds(time);

            var dateTime = DateTime.Parse(timeSpan.ToString());

            return $"{dateTime:mm:ss}";
        }
        else
        {
            return $"00:00";
        }
    }

    /// <summary>
    /// 小数类型的时间秒，转为分钟
    /// </summary>
    /// <param name="second"></param>
    /// <returns></returns>
    public static int SecondsToMinute(float second)
    {
        if (second >= 0 && second <= 36000)
        {
            return (int)(second / 60);
        }
        else
        {
            return 0;
        }
    }

    /// <summary>
    /// 计算玩家KD比
    /// </summary>
    /// <param name="kill">玩家击杀数</param>
    /// <param name="dead">玩家死亡数</param>
    /// <returns>返回玩家KD比（小数float）<returns>
    public static float GetPlayerKD(int kill, int dead)
    {
        if (kill == 0 && dead >= 0)
        {
            return 0.0f;
        }
        else if (kill > 0 && dead == 0)
        {
            return kill;
        }
        else if (kill > 0 && dead > 0)
        {
            return (float)kill / dead;
        }
        else
        {
            return (float)kill / dead;
        }
    }

    /// <summary>
    /// 计算玩家KPM比
    /// </summary>
    /// <param name="kill"></param>
    /// <param name="minute"></param>
    /// <returns></returns>
    public static float GetPlayerKPM(int kill, float minute)
    {
        if (minute != 0.0f)
        {
            return kill / minute;
        }
        else
        {
            return 0.0f;
        }
    }

    /// <summary>
    /// 获取玩家KPM
    /// </summary>
    /// <param name="kill"></param>
    /// <param name="time"></param>
    /// <returns></returns>
    public static string GetPlayerKPM(float kill, float time)
    {
        if (time < 60)
        {
            return "0.00";
        }
        else
        {
            var minute = (int)(time / 60);
            return $"{kill / minute:0.00}";
        }
    }

    /// <summary>
    /// 计算百分比
    /// </summary>
    /// <param name="num1"></param>
    /// <param name="num2"></param>
    /// <returns></returns>
    public static string GetPlayerPercentage(float num1, float num2)
    {
        if (num2 != 0)
        {
            return $"{num1 / num2 * 100:0.00}%";
        }
        else
        {
            return "0%";
        }
    }

    /// <summary>
    /// 判断战地1输入框字符串长度，中文3，英文1
    /// </summary>
    /// <param name="str">需要判断的字符串</param>
    /// <returns></returns>
    public static int GetStrLength(string str)
    {
        if (string.IsNullOrEmpty(str))
            return 0;

        var ascii = new ASCIIEncoding();
        int tempLen = 0;
        byte[] s = ascii.GetBytes(str);
        for (int i = 0; i < s.Length; i++)
        {
            if (s[i] == 63)
            {
                tempLen += 3;
            }
            else
            {
                tempLen += 1;
            }
        }

        return tempLen;
    }

    /// <summary>
    /// 获取击杀星数
    /// </summary>
    /// <param name="kills"></param>
    /// <returns></returns>
    public static string GetKillStar(int kills)
    {
        if (kills < 100)
        {
            return "";
        }
        else
        {
            int count = kills / 100;

            return $"{count}";
        }
    }

    /// <summary>
    /// 获取玩家ID或队标
    /// </summary>
    /// <param name="originalName"></param>
    /// <param name="isClan"></param>
    /// <returns></returns>
    public static string GetPlayerTargetName(string originalName, bool isClan)
    {
        if (string.IsNullOrEmpty(originalName))
            return "";

        int index = originalName.IndexOf("]");
        string clan;
        string name;
        if (index != -1)
        {
            clan = originalName.Substring(1, index - 1);
            name = originalName.Substring(index + 1);
        }
        else
        {
            clan = "";
            name = originalName;
        }

        if (isClan)
            return clan;
        else
            return name;
    }

    /// <summary>
    /// 获取地图对应中文名称
    /// </summary>
    /// <param name="originMapName"></param>
    /// <returns></returns>
    public static string GetMapChsName(string originMapName)
    {
        int index = MapData.AllMapInfo.FindIndex(var => var.English == originMapName);
        if (index != -1)
            return MapData.AllMapInfo[index].Chinese;
        else
            return originMapName;
    }

    /// <summary>
    /// 获取地图对应预览图
    /// </summary>
    /// <param name="originMapName"></param>
    /// <returns></returns>
    public static string GetMapPrevImage(string originMapName)
    {
        int index = MapData.AllMapInfo.FindIndex(var => var.English == originMapName);
        if (index != -1)
            return MapData.AllMapInfo[index].Image;
        else
            return "";
    }

    /// <summary>
    /// 获取武器对应中文名称
    /// </summary>
    /// <param name="originWeaponName"></param>
    /// <returns></returns>
    public static string GetWeaponChsName(string originWeaponName)
    {
        if (string.IsNullOrEmpty(originWeaponName))
            return "";

        if (originWeaponName.Contains("_KBullet"))
            return "K Bullets";

        if (originWeaponName.Contains("_RGL_Frag"))
            return "RGL Frag";

        if (originWeaponName.Contains("_RGL_Smoke"))
            return "RGL Smoke";

        if (originWeaponName.Contains("_RGL_HE"))
            return "RGL HE";

        int index = WeaponData.AllWeaponInfo.FindIndex(var => var.English == originWeaponName);
        if (index != -1)
            return WeaponData.AllWeaponInfo[index].Chinese;
        else
            return originWeaponName;
    }

    /// <summary>
    /// 获取本地图片路径，如果未找到会返回空字符串
    /// </summary>
    /// <param name="url"></param>
    /// <param name="type"></param>
    /// <returns></returns>
    public static string GetTempImagePath(string url, string type)
    {
        string extension = Path.GetFileName(url);
        switch (type)
        {
            case "maps":
                return ImageData.MapsDict.ContainsKey(extension) ? ImageData.MapsDict[extension] : string.Empty;
            case "weapons":
                return ImageData.WeaponsDict.ContainsKey(extension) ? ImageData.WeaponsDict[extension] : string.Empty;
            case "weapons2":
                return ImageData.Weapons2Dict.ContainsKey(extension) ? ImageData.Weapons2Dict[extension] : string.Empty;
            case "vehicles":
                return ImageData.VehiclesDict.ContainsKey(extension) ? ImageData.VehiclesDict[extension] : string.Empty;
            case "vehicles2":
                return ImageData.Vehicles2Dict.ContainsKey(extension) ? ImageData.Vehicles2Dict[extension] : string.Empty;
            case "classes":
                return ImageData.ClassesDict.ContainsKey(extension) ? ImageData.ClassesDict[extension] : string.Empty;
            case "classes2":
                return ImageData.Classes2Dict.ContainsKey(extension) ? ImageData.Classes2Dict[extension] : string.Empty;
            default:
                return string.Empty;
        }
    }

    /// <summary>
    /// 检查玩是否是管理员或者VIP
    /// </summary>
    /// <param name="personaId"></param>
    /// <param name="list"></param>
    /// <returns></returns>
    public static string CheckAdminVIP(long personaId, List<long> list)
    {
        return list.IndexOf(personaId) != -1 ? "✔" : "";
    }

    /// <summary>
    /// 获取玩家游玩时间，返回分钟数或小时数
    /// </summary>
    /// <param name="second"></param>
    /// <returns></returns>
    public static string GetPlayTime(double second)
    {
        var ts = TimeSpan.FromSeconds(second);

        if (ts.TotalHours < 1)
        {
            return ts.TotalMinutes.ToString("0") + " minutes";
        }

        return ts.TotalHours.ToString("0") + " hours";
    }

    /// <summary>
    /// 获取武器简短名称，用于踢人理由
    /// </summary>
    /// <param name="weaponName"></param>
    /// <returns></returns>
    public static string GetWeaponShortTxt(string weaponName)
    {
        int index = WeaponData.AllWeaponInfo.FindIndex(var => var.English.Equals(weaponName));
        if (index != -1)
        {
            return WeaponData.AllWeaponInfo[index].ShortTxt;
        }

        return weaponName;
    }

    /// <summary>
    /// 获取小队的中文名称
    /// </summary>
    /// <param name="squadID"></param>
    /// <returns></returns>
    public static string GetSquadChsName(int squadID)
    {
        switch (squadID)
        {
            case 0:
                return "-";
            case 1:
                return "A";
            case 2:
                return "B";
            case 3:
                return "C";
            case 4:
                return "D";
            case 5:
                return "E";
            case 6:
                return "F";
            case 7:
                return "G";
            case 8:
                return "H";
            case 9:
                return "I";
            case 10:
                return "J";
            case 11:
                return "K";
            case 12:
                return "L";
            case 13:
                return "M";
            case 14:
                return "N";
            case 15:
                return "O";
            default:
                return squadID.ToString();
        }
    }

    /// <summary>
    /// 获取队伍阵营图片路径
    /// </summary>
    /// <param name="mapName"></param>
    /// <param name="team1Path"></param>
    /// <param name="team2Path"></param>
    public static void GetTeamImage(string mapName, out string team1Path, out string team2Path)
    {
        team1Path = team2Path = string.Empty;

        int index = MapData.AllMapInfo.FindIndex(var => var.English.Equals(mapName));
        if (index != -1 && mapName != "ID_M_LEVEL_MENU")
        {
            team1Path = $"\\Assets\\Images\\Client\\Teams\\{MapData.AllMapInfo[index].Team1}.png";
            team2Path = $"\\Assets\\Images\\Client\\Teams\\{MapData.AllMapInfo[index].Team2}.png";
        }
        else
        {
            team1Path = team2Path = "\\Assets\\Images\\Client\\Teams\\_DEF.png";
        }
    }

    /// <summary>
    /// 获取当前地图游戏模式
    /// </summary>
    /// <param name="modeName"></param>
    /// <returns></returns>
    public static string GetGameMode(string modeName)
    {
        int index = ModeData.AllModeInfo.FindIndex(var => var.Mark.Equals(modeName));
        if (index != -1)
        {
            return ModeData.AllModeInfo[index].Chinese;
        }
        else
        {
            return "";
        }
    }

    /// <summary>
    /// 修正服务器得分数据
    /// </summary>
    /// <param name="score"></param>
    /// <returns></returns>
    public static int FixedServerScore(int score)
    {
        return score < 0 || score > 2000 ? 0 : score;
    }

    /// <summary>
    /// 修正服务器得分数据
    /// </summary>
    /// <param name="score"></param>
    /// <returns></returns>
    public static double FixedServerScore(double score)
    {
        return score < 0 || score > 125 ? 0 : score;
    }
}
