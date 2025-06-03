using BF1.ServerAdminTools.Common.Utils;
using BF1.ServerAdminTools.Features.API;
using BF1.ServerAdminTools.Features.API.RespJson;
using BF1.ServerAdminTools.Features.Data;
using BF1.ServerAdminTools.Features.Utils;
using BF1.ServerAdminTools.Models;
using NexDiscord;

namespace BF1.ServerAdminTools.NexDiscord;

public static partial class SexusBot
{
    public static partial class Interaction
    {
        public static async Task sus()
        {
            string[] words = VariS.Current.words;
            if (words.Length >= 2)
            {
                string name = words[1];
                try
                {
                    PlayerData p = FindPlayerInServer(name);
                    if (p == null)
                    {
                        await OutAnsi($"Player was {Ansi.Red}not{Ansi.None} found on the server.");
                    }
                    else
                    {
                        List<WeaponStats> result = await CheckSus(p.PersonaId);

                        if (result == null)
                        {
                            await OutAnsi($"{Ansi.Bold}No{Ansi.None} suspicious weapons found for {Ansi.B.Blue}{name}{Ansi.None}.");
                        }
                        else
                        {
                            try
                            {
                                var header = new StringBuilder();
                                var sb = new StringBuilder();
                                header.Append(String.Format("{0,-26} {1,6} {2,5} {3,6} {4,5} {5,5} {6,10}\n\n", "Weapon", "Kills", "KPM", "Acc", "HS", "H/K", "Time"));
                                foreach (WeaponStats ws in result)
                                {
                                    sb.Append(String.Format("{0,-26} {1,6} {2,5} {3,6} {4,5} {5,5} {6,10}\n", ws.name, ws.kills, ws.killsPerMinute, ws.hitsVShots, ws.headshotsVKills, ws.hitVKills, ws.time));
                                }
                                await OutAnsi($"{Ansi.Bold}Suspicious{Ansi.None} weapons for {Ansi.B.Blue}{name}{Ansi.None}:\n\n{header}{sb}");
                            }
                            catch (Exception ex) { Log.Ex(ex); }
                        }
                    }
                }
                catch (Exception ex) { Log.Ex(ex); }
            }
            else
            {
                await OutAnsi($"Syntax: {Ansi.B.Magenta}.sus{Ansi.None} <name>");
            }
        }

        public static async Task<List<WeaponStats>> CheckSus(long personaId)
        {
            RespContent result = await BF1API.GetWeaponsByPersonaId(personaId);
            if (result.IsSuccess)
            {
                GetWeapons getWeapons = JsonUtil.JsonDese<GetWeapons>(result.Message);

                List<WeaponStats> weapons = new List<WeaponStats>();
                foreach (GetWeapons.ResultItem res in getWeapons.result)
                {
                    foreach (GetWeapons.ResultItem.WeaponsItem wea in res.weapons)
                    {
                        if (wea.stats.values.kills == 0)
                            continue;

                        weapons.Add(new WeaponStats()
                        {
                            name = ChsUtil.ToSimplifiedChinese(wea.name),
                            imageUrl = PlayerUtil.GetTempImagePath(wea.imageUrl, "weapons2"),
                            star = PlayerUtil.GetKillStar((int)wea.stats.values.kills),
                            kills = (int)wea.stats.values.kills,
                            killsPerMinute = PlayerUtil.GetPlayerKPM(wea.stats.values.kills, wea.stats.values.seconds),
                            headshots = (int)wea.stats.values.headshots,
                            headshotsVKills = PlayerUtil.GetPlayerPercentage(wea.stats.values.headshots, wea.stats.values.kills),
                            shots = (int)wea.stats.values.shots,
                            hits = (int)wea.stats.values.hits,
                            hitsVShots = PlayerUtil.GetPlayerPercentage(wea.stats.values.hits, wea.stats.values.shots),
                            hitVKills = $"{wea.stats.values.hits / wea.stats.values.kills:0.00}",
                            time = PlayerUtil.GetPlayTime(wea.stats.values.seconds)
                        });
                    }
                }
                weapons.Sort((a, b) => b.kills.CompareTo(a.kills));

                List<WeaponStats> susweapons = new();
                int minkills = Vari.Sus_minkills;
                int minacc = Vari.Sus_minacc;
                int minhs = Vari.Sus_minhs;
                foreach (WeaponStats w in weapons.Where(w => w.kills >= minkills))
                {
                    double acc;
                    double hs;
                    double kpm;
                    try
                    {
                        acc = double.Parse(w.hitsVShots.Remove(w.hitsVShots.Length - 1));
                        hs = double.Parse(w.headshotsVKills.Remove(w.headshotsVKills.Length - 1));
                        kpm = double.Parse(w.killsPerMinute);
                    }
                    catch
                    {
                        Log.Ex($"Sus-Check, parsing error: {w.hitsVShots}, {w.headshotsVKills}, {w.killsPerMinute}");
                        return null;
                    }

                    try
                    {
                        
                        if (kpm < 1)
                        {
                            continue;
                        }

                        if (acc >= minacc && acc <= 100) //Acc
                        {
                            if (Features.Client.WeaponData.AllWeaponInfo.Exists(
                            x => x.Chinese == w.name 
                            &&
                            (x.Class.Contains("Weapon") || x.Class.Contains("Pistol"))
                            && 
                            w.name != "Obrez Pistol" 
                            && 
                            x.Class.Contains("Assault Weapon") == false      
                            )
                            == true)
                            {
                                susweapons.Add(w);
                                Log.I($"SUS CHECK - Acc DETEC: {acc}");
                            }
                        }
                        else if (hs >= minhs) //HS
                        {
                            susweapons.Add(w);
                            Log.I($"SUS CHECK - HS DETEC: {hs}");
                        }
                    }
                    catch (Exception ex)
                    {
                        Log.Ex(ex);
                        return null;
                    }
                }
                if (susweapons.Count >= 1)
                {
                    return susweapons;                    
                }
                else
                {
                    return null;
                }
                ;
                //Name, Kills, KPM, Acc, HS, H/K, Time
            }
            else
            {
                return null;
            }
        }
    }
}

