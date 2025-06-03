namespace BF1.ServerAdminTools;

public static class Util_BF1
{
    #region Calculations
    public static bool UnbalanceChecker(int t1, int t2)
    {
        int scorediff = t1 - t2;
        //int minscore;
        int maxscore = Math.Max(t1, t2);        

        if (maxscore <= 150 || maxscore >= 650)
        {
            return false;
        }

        if (maxscore <= 300)
        {
            if (scorediff >= 100 || scorediff <= -100)
            {
                return true;
            }
        }

        if (maxscore <= 400)
        {
            if (scorediff >= 150 || scorediff <= -150)
            {
                return true;
            }
        }

        if (maxscore <= 500)
        {
            if (scorediff >= 200 || scorediff <= -200)
            {
                return true;
            }
        }

        if (maxscore <= 650)
        {
            if (scorediff >= 250 || scorediff <= -250)
            {
                return true;
            }
        }

        return false;
    }

    public static (int, int) StrengthCalculation()
    {
        if (Vari.ServerLiveInfo == null || Vari.NexStatisticData_Team1 == null)
        {
            return (-1, -1);
        }

        int strength_t1 = 0;
        int strength_t2 = 0;

        strength_t1 = (int)(Vari.NexStatisticData_Team1.Rank150PlayerCount * 60 + Vari.NexTeamKD1 * 150);
        strength_t2 = (int)(Vari.NexStatisticData_Team2.Rank150PlayerCount * 60 + Vari.NexTeamKD2 * 150);

        if (strength_t1 >= 1000)
        {
            strength_t1 = 999;
        }
        if (strength_t2 >= 1000)
        {
            strength_t2 = 999;
        }

        //Trying to stop weird overflow where it shows the max int decimal
        if (strength_t1 > 1000 || strength_t2 > 1000 || strength_t1 < -1000 || strength_t2 < -1000)
        {
            strength_t1 = 0;
            strength_t2 = 0;
        }

        return (strength_t1, strength_t2);
    }

    public static int WinPercentageCalculation(int s1, int s2)
    {
        if (Vari.ServerLiveInfo == null)
        {
            return -1;
        }

        int scorediff = (Vari.ServerLiveInfo.Team1Score - Vari.ServerLiveInfo.Team2Score);

        int winprediction_t1 = (int)(scorediff * 0.08) + 50; //625 Diff = 99% Win Chance

        //Factor in 5% of Strength-Difference.
        winprediction_t1 += Convert.ToInt32((s1 - s2) * 0.05);

        if (winprediction_t1 >= 100)
        {
            winprediction_t1 = 99;
        }
        if (winprediction_t1 <= 0)
        {
            winprediction_t1 = 1;
        }

        return winprediction_t1;
    }
    #endregion

    #region Misc
    public static PlayerData? FindPlayerInServer(string name)
    {
        try
        {
            foreach (PlayerData p in Vari.Playerlist_All)
            {
                if (p.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase) == true)
                {
                    return p;                    
                }
            }
            return null;
        }
        catch (Exception ex)
        {
            Log.Ex(ex);
            return null;
        }
    }

    public static bool IsNotAdminOrWhitelisted(string name, long pid)
    {
        try
        {
            if (Vari.ServerDetails_AdminList_PID.Contains(pid) || Vari.Custom_Player_WhiteList.Contains(name))
            {
                return false; //Is Admin or Whitelisted
            }
            else
            {
                return true; //Is Not.
            }
        }
        catch
        {
            return false; //Error occured.
        }
    }
    public static bool GetSessionID()
    {
        var result = Search.SearchMemory(Offsets.SessionIDMask);
        if (result != string.Empty)
        {
            Vari.SessionID = result;
            return true;
        }
        else
        {
            return false;
        }
    }
    #endregion

    #region AdminActions
    public static class AdminActions
    {
        #region Chat
        public static bool ChatFunction(string chatmsg)
        {
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
        #endregion

        #region Switch Players
        public static async Task<(bool, string)> TryMovePlayer(string name)
        {
            PlayerData? playerdata = FindPlayerInServer(name);

            if (playerdata is null)
            {
                return (false, "Player was not found playing on the Server.");
            }

            RespContent result;
            int endteam = 0;

            if (playerdata.TeamId == 1)
            {
                result = await BF1API.AdminMovePlayer(playerdata.PersonaId, 1);
                endteam = 2;
            }
            else
            {
                result = await BF1API.AdminMovePlayer(playerdata.PersonaId, 2);
                endteam = 1;
            }

            if (result.IsSuccess)
            {
                return (true, endteam.ToString());
            }
            else
            {
                return (false, $"Error: {result.Message}");
            }
        }
        static bool currently_in_loop = false;
        public static async Task SwitchLocalPlayer_AwaitFreeSlot(bool through_hotkey = false)
        {            
            if (currently_in_loop)
            {
                return;
            }

            //Console.Beep(300, 50);
            var localplayer = Vari.Playerlist_All.Find(x => x.Name == Vari.CurrentUsername);
            if (localplayer == null)
            {
                Log.C("Local Player could not be fetched. Return and try again.");
                Console.ReadLine();
                return;
            }
            long personaid = localplayer.PersonaId;
            int teamid = localplayer.TeamId;

            if (teamid == 0) { return; }

            if (!through_hotkey)
            {
                Log.C("To abort, press escape. Waiting for a free slot ...");
            }
            int enemy_count = 100;                                      
            
            while (true)
            {
                currently_in_loop = true;

                //Let User escape with Escape
                if (Console.KeyAvailable && Console.ReadKey(true).Key == ConsoleKey.Escape)
                {
                    Console.Beep(300, 50);
                    Log.D("Aborted");
                    currently_in_loop = false;
                    break;
                }

                if (teamid == 1)
                {
                    enemy_count = Vari.Playerlist_Team2.Count;
                }
                else
                {
                    enemy_count = Vari.Playerlist_Team1.Count;
                }

                if (enemy_count > 0 && enemy_count < 32)
                {
                    Log.D(enemy_count.ToString());
                    //Console.Beep(400, 50);
                    if (!through_hotkey)
                    {
                        Log.C("Free Slot detected. Trying to move player...");
                    }
                    var result = await BF1API.AdminMovePlayer(personaid, teamid);

                    if (result.IsSuccess == true)
                    {
                        if (!through_hotkey)
                        {
                            Log.C("Success!");
                            Console.ReadLine();
                        }
                    }
                    else
                    {
                        if (!through_hotkey)
                        {
                            Log.C("Failed to switch player.");
                            Console.ReadLine();
                        }
                    }

                    currently_in_loop = false;
                    break;
                }

                Thread.Sleep(2000);
            }
        }
        #endregion

        #region Kick
        public static class Kick
        {            
            public static async Task<(bool, string)> AutoKick(BreakRuleInfo info, bool ForPing)
            {
                if (Vari.OnlyKickWhenPlayerCountAbove > 0 && Vari.Playerlist_All.Count <= Vari.OnlyKickWhenPlayerCountAbove) //0 means off
                {
                    return (false, $"Player count is below {Vari.OnlyKickWhenPlayerCountAbove}");
                }

                if (!IsNotAdminOrWhitelisted(info.Name, info.PersonaId))
                {
                    return (false, $"Player is Admin or Whitelisted");
                }
                
                return await PureKick(info, ForPing);
            }            
            public static async Task<(bool, string)> ManualKick(BreakRuleInfo info)
            {
                if (!IsNotAdminOrWhitelisted(info.Name, info.PersonaId))
                {
                    return (false, $"Player is Admin or Whitelisted");
                }

                return await PureKick(info, false);
            }
            public static async Task<(bool, string)> FindAndManualKick(string name, string reason)
            {
                PlayerData? playerdata = FindPlayerInServer(name);
                if (playerdata is null)
                {
                    return (false, "Player was not found playing on the Server.");
                }

                BreakRuleInfo info = new BreakRuleInfo
                {
                    Name = playerdata.Name,
                    PersonaId = playerdata.PersonaId,
                    Reason = reason,
                    Status = ""
                };

                return await ManualKick(info);
            }

            public static async Task<(bool, string)> PureKick(BreakRuleInfo info, bool ForPing)
            {
                //Log.D($"wouldve kicked {info.Name}");
                //return (true, "");
                try
                {
                    RespContent result = await BF1API.AdminKickPlayer(info.PersonaId, info.Reason);

                    if (result.IsSuccess)
                    {
                        info.Flag = 1;
                        info.Status = "kicked out";
                        info.Time = DateTime.UtcNow;

                        if(ForPing)
                        {
                            LogKick(info);
                            await DWebHooks.LogPingKick(info);
                        }
                        else
                        {
                            LogKick(info);
                            await DWebHooks.LogOK(info);                            
                        }                        

                        return (true, "");
                    }
                    else
                    {
                        info.Flag = 2;
                        info.Status = "Kick failed, " + result.Message;
                        info.Time = DateTime.UtcNow;

                        LogKick(info, true);

                        await DWebHooks.LogNO(info);

                        return (false, $"Error: {result.Message}");
                    }
                }
                catch (Exception ex)
                {
                    Log.Ex(ex);
                    return (false, $"Error: {ex.Message}");
                }
            }
        }
        #endregion

        #region Sus
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
        #endregion

        #region Ban
        public static async Task<bool> BanPlayer(string name)
        {
            var result = await BF1API.AddServerBan(name);

            if (result.IsSuccess)
            {
                return true;
            }
            return false;
        }
        public static async Task<bool> UnbanPlayer(long pid)
        {
            var result = await BF1API.RemoveServerBan(pid.ToString());

            if (result.IsSuccess)
            {
                return true;
            }
            return false;
        }
        #endregion

        #region Get ID from PID
        public static async Task<string> GetIDFromPID(long personaId)
        {
            var result = await BF1API.GetPersonasByIds(personaId);
            string id;
            if (result.IsSuccess)
            {
                try //tna
                {
                    Debug.WriteLine(result.Message);
                    JsonNode jNode = JsonNode.Parse(result.Message);
                    id = jNode["result"]![$"{personaId}"]!["nucleusId"].GetValue<string>();

                    return id;
                }
                catch (Exception ex) //tna
                {
                    Log.Ex(ex);
                    return "";
                }
            }
            else
            {
                return "";
            }
        }
        #endregion

        #region Server Presets
        public static async Task<RespContent> Preset_UpdateMapRotation(GetServerDetailsBody detailsbody, List<MapsItem> maps)
        {
            UpdateServerReqBody request_body = new()
            {
                jsonrpc = "2.0",
                method = "RSP.updateServer"
            };

            var request_params = new UpdateServerReqBody.Params
            {
                deviceIdMap = new UpdateServerReqBody.Params.DeviceIdMap()
                {
                    machash = Guid.NewGuid().ToString()
                },
                game = "tunguska",
                serverId = Vari.ServerDetails.ServerID, //Vari.ServerDetails.ServerID,
                bannerSettings = new UpdateServerReqBody.Params.BannerSettings()
                {
                    bannerUrl = "",
                    clearBanner = true
                }
            };

            var request_maprotation = detailsbody.result.mapRotations[0];
            request_maprotation.maps = maps;

            request_params.mapRotation = new()
            {
                id = "100",
                description = request_maprotation.description,
                name = request_maprotation.name,
                mod = request_maprotation.mod,
                maps = request_maprotation.maps,
                rotationType = request_maprotation.rotationType
            };

            request_params.serverSettings = detailsbody.result.serverSettings;
            request_body.@params = request_params;
            request_body.id = Guid.NewGuid().ToString();                        
           
            Log.D($"request_body: {JsonSerializer.Serialize(request_body)}");            
            return await BF1API.UpdateServer(request_body);
        }

        public static async Task<GetServerDetailsBody?> Preset_GetSettings() //TODO ADD MAPS
        {
            var serverdetails = await BF1API.GetServerDetails();
            string details_json = serverdetails.Message;
            JsonNode node_full;
            GetServerDetailsBody? body = new();
            try
            {
                node_full = JsonNode.Parse(details_json)!;
                body = node_full.Deserialize<GetServerDetailsBody>();
            }
            catch(Exception ex)
            {
                Log.Ex(ex, "AAAA");
                return null;
            }

            if (body.result == null)
            {
                Log.D("RES NULLL");
                return null;
            }

            if (body.jsonrpc == null)
            {
                Log.D("JS NULLL");
                return null;
            }

            return body;
        }
        
        #endregion
    }
    #endregion

    #region Logging Kicks/Switches
    #region LogKick
    public static int iteraton_kicks = 1;
    public static string _lastkickname = "";  
    public static void LogKick(BreakRuleInfo info, bool failed = false)
    {
        //Stop Repetition
        if (info.Name == _lastkickname)
        {
            return;
        }
        _lastkickname = info.Name;

        //If Fail, check if Admin
        if (failed == true && !Vari.ServerDetails_AdminList_Name.Contains(Vari.CurrentUsername))
        {
            info.Reason = info.Reason + " +[User not Admin on Server]";
        }

        //Add 
        Vari.Logs_Kicks.Add(new()
        {
            DateTime = DateTime.UtcNow,
            Iteration = iteraton_kicks,
            Fail = failed,
            Name = info.Name,
            PID = info.PersonaId,
            Reason = info.Reason,
        });
        iteraton_kicks++;

        if (failed)
        {
            Log.D("FAIL: " + info.Name);
        }
        else
        {
            Log.D($"Kicked: {info.Name}, status: {info.Reason}");
        }        
    }
    #endregion

    #region TeamSwitch
    public static int iteraton_switches = 1;
    public static string _lastteamchangename = "";
    public static async void HandleTeamChange(ChangeTeamInfo info)
    {
        if (info.Name == _lastteamchangename)
        {
            return;
        }
        _lastteamchangename = info.Name;

        var result = TeamSwitchReview(info); //Adds to the status if it was a win or balance switch
        info.Status = result.Item2;

        Vari.Logs_Switches.Add(new()
        {
            DateTime = DateTime.UtcNow,
            Iteration = iteraton_switches,
            Name = info.Name,
            PID = info.PersonaId,
            Status = info.Status,
            Map = Vari.CurrentMapName,
        });
        iteraton_switches++;

        Log.D("SWITCH: " + info.Name);

        if (result.Item1 == 1) //Winswitcher
        {
            if (Vari.AutoKickWinSwitching)
            {
                var result2 = await AdminActions.Kick.FindAndManualKick(info.Name, $"Winswitch {info.Team1Score}-{info.Team2Score}");
                if (result2.Item1)
                {
                    await DWebHooks.LogWinSwitchKick(info);
                }
                else
                {
                    await DWebHooks.LogNO(new BreakRuleInfo
                    {
                        Name = info.Name,
                        PersonaId = info.PersonaId,
                        Reason = $"Winswitch {info.Team1Score}-{info.Team2Score}",
                    });
                }
            }
            else
            {
                await DWebHooks.LogWinSwitchDetected(info);
            }            
        }
        else if (result.Item1 == 2) //Balancer
        {
            await DWebHooks.LogBalancer(info);
        }
    }
    public static (int, string) TeamSwitchReview(ChangeTeamInfo info) // 0 = nothing, 1 = Winswitch, 2 = Balancer
    {
        int t1 = info.Team1Score;
        int t2 = info.Team2Score;

        string scoreinfo = $"{t1} <- {t2}";

        if (info.Status == "T1 -> T2")
        {
            scoreinfo = $"{t1} -> {t2}";
        }

        bool unbalanced = UnbalanceChecker(t1, t2);

        if (!unbalanced)
        {
            return (0, scoreinfo);
        }

        if (Vari.NexConquestAssaultMapNames.Contains(Vari.CurrentMapName) == true && (t1 < 300 || t2 < 300)) //No Winswitch if its Conquest assault
        {
            return (0, scoreinfo);
        }

        int whostomps;
        if (t1 > t2)
        {
            whostomps = 1; //team 1 is stomping
        }
        else
        {
            whostomps = 2; //team 2 is stomping  
        }

        bool switchedtoteam2;
        if (info.Status == "T1 -> T2") //To Team 2
        {
            switchedtoteam2 = true;
        }
        else
        {
            switchedtoteam2 = false;
        }

        if (whostomps == 1)
        {
            if (switchedtoteam2)
            {
                return (2, $"{scoreinfo}  (Balance to T2)"); //Balanced to T2
            }
            else //To Team 1
            {
                return (1, $"{scoreinfo}  (Winswitch to T1!)"); //Winswitch to T1
            }
        }
        else if (whostomps == 2)
        {
            if (switchedtoteam2)
            {
                return (1, $"{scoreinfo}  (Winswitch to T2!)"); //Winswitch to T2
            }
            else //To Team 1
            {
                return (2, $"{scoreinfo}  (Balance to T1)"); //Balanced to T1
            }
        }

        return (0, scoreinfo);
    }
    #endregion
    #endregion
}
