using BF1.ServerAdminTools.Models;
using BF1.ServerAdminTools.Windows;
using BF1.ServerAdminTools.Common.Utils;
using BF1.ServerAdminTools.Common.Helper;
using BF1.ServerAdminTools.Features.Data;
using BF1.ServerAdminTools.Features.Utils;
using BF1.ServerAdminTools.Features.API;
using BF1.ServerAdminTools.Features.API.RespJson;

namespace BF1.ServerAdminTools.Views;

/// <summary>
/// DetailView.xaml 的交互逻辑
/// </summary>
public partial class DetailView : UserControl
{
    public DetailModel DetailModel { get; set; } = new();

    public class Map
    {
        public string mapPrettyName { get; set; }
        public string mapImage { get; set; }
        public string modePrettyName { get; set; }
    }

    public class RSPInfo
    {
        public int Index { get; set; }
        public string platform { get; set; }
        public string nucleusId { get; set; }
        public string personaId { get; set; }
        public string platformId { get; set; }
        public string displayName { get; set; }
        public string avatar { get; set; }
        public string accountId { get; set; }
    }

    private ServerDetails serverDetails = null;
    private bool isGetServerDetailsOK = false;

    public DetailView()
    {
        InitializeComponent();
        this.DataContext = this;
        MainWindow.ClosingDisposeEvent += MainWindow_ClosingDisposeEvent;

        Task.Run(() =>
        {
            Task.Delay(5000).Wait();
        }).ContinueWith((t) =>
        {
            var thread0 = new Thread(UpdateServerDetils)
            {
                IsBackground = true
            };
            thread0.Start();
        });
    }

    private void MainWindow_ClosingDisposeEvent()
    {

    }

    private async void UpdateServerDetils() //void
    {
        bool isClear = true;

        while (true)
        {
            if (Vari.GameId == string.Empty)
            {
                if (!isClear)
                {
                    isClear = true;

                    this.Dispatcher.Invoke(() =>
                    {
                        DetailModel.ServerName = string.Empty;
                        DetailModel.ServerDescription = string.Empty;
                        DetailModel.ServerOwnerName = string.Empty;
                        DetailModel.ServerOwnerPersonaId = string.Empty;
                        DetailModel.ServerID = string.Empty;
                        DetailModel.ServerGameID = string.Empty;

                        DetailModel.ServerOwnerImage = string.Empty;

                        ListBox_Map.Items.Clear();
                        ListBox_Admin.Items.Clear();
                        ListBox_VIP.Items.Clear();
                        ListBox_BAN.Items.Clear();

                        Vari.Server_AdminList_PID.Clear();
                        Vari.Server_AdminList_Name.Clear();
                        Vari.Server_VIPList.Clear();
                    });
                }
            }
            else
            {
                if (isClear)
                {
                    isClear = false;

                    await this.Dispatcher.Invoke(async () =>
                    {
                        await GetFullServerDetails(true);
                    });
                }
            }
            Thread.Sleep(1000);
        }
    }

    public async Task GetFullServerDetails(bool isInit = false)
    {
        if (!string.IsNullOrEmpty(Vari.SessionId))
        {
            if (!string.IsNullOrEmpty(Vari.GameId))
            {
                DetailModel.ServerName = "Getting...";
                DetailModel.ServerDescription = "Getting...";
                DetailModel.ServerOwnerName = "Getting...";
                DetailModel.ServerOwnerPersonaId = "Getting...";
                DetailModel.ServerID = "Getting...";
                DetailModel.ServerGameID = "Getting...";

                DetailModel.ServerOwnerImage = string.Empty;

                ListBox_Map.Items.Clear();
                ListBox_Admin.Items.Clear();
                ListBox_VIP.Items.Clear();
                ListBox_BAN.Items.Clear();

                Vari.Server_AdminList_PID.Clear();
                Vari.Server_AdminList_Name.Clear();
                Vari.Server_VIPList.Clear();

                /////////////////////////////////////////////////////////////////////////////////

                if (!isInit)
                    NotifierHelper.Show(NotifierType.Information, $"getting server {Vari.GameId} in detailed data...");

                await BF1API.SetAPILocale();
                var result = await BF1API.GetFullServerDetails();
                                
                if (result.IsSuccess)
                {
                    var fullServerDetails = JsonUtil.JsonDese<FullServerDetails>(result.Message);
                                        
                    Vari.ServerId = fullServerDetails.result.rspInfo.server.serverId;
                    Vari.PersistedGameId = fullServerDetails.result.rspInfo.server.persistedGameId;

                    DetailModel.ServerName = fullServerDetails.result.serverInfo.name;
                    Vari.CurrentServerName = DetailModel.ServerName;
                    DetailModel.ServerDescription = fullServerDetails.result.serverInfo.description;

                    DetailModel.ServerID = Vari.ServerId;
                    DetailModel.ServerGameID = Vari.GameId;

                    try //tna
                    {
                        DetailModel.ServerOwnerName = fullServerDetails.result.rspInfo.owner.displayName;
                        DetailModel.ServerOwnerPersonaId = fullServerDetails.result.rspInfo.owner.personaId;
                        DetailModel.ServerOwnerImage = fullServerDetails.result.rspInfo.owner.avatar;
                    }
                    catch (Exception ex)
                    {
                        Log.Ex(ex);
                    }

                    try //tna
                    {
                        foreach (var item in fullServerDetails.result.serverInfo.rotation)
                        {
                            ListBox_Map.Items.Add(new Map()
                            {
                                mapImage = PlayerUtil.GetTempImagePath(item.mapImage, "maps"),
                                mapPrettyName = ChsUtil.ToSimplifiedChinese(item.mapPrettyName),
                                modePrettyName = ChsUtil.ToSimplifiedChinese(item.modePrettyName)
                            });
                        }
                    }
                    catch (Exception ex)
                    {
                        Log.Ex(ex);
                    }
                    // 地图列表
                    


                    // 服主
                    int index = 0;
                    try
                    {
                        ListBox_Admin.Items.Add(new RSPInfo()
                        {
                            Index = index++,
                            avatar = fullServerDetails.result.rspInfo.owner.avatar,
                            displayName = fullServerDetails.result.rspInfo.owner.displayName,
                            personaId = fullServerDetails.result.rspInfo.owner.personaId
                        });
                        Vari.Server_AdminList_PID.Add(long.Parse(fullServerDetails.result.rspInfo.owner.personaId));
                        Vari.Server_AdminList_Name.Add(fullServerDetails.result.rspInfo.owner.displayName);
                    }
                    catch (Exception ex)
                    {
                        Log.Ex(ex);
                    }


                    // 管理员列表
                    try //tna
                    {
                        foreach (var item in fullServerDetails.result.rspInfo.adminList)
                        {
                            ListBox_Admin.Items.Add(new RSPInfo()
                            {
                                Index = index++,
                                avatar = item.avatar,
                                displayName = item.displayName,
                                personaId = item.personaId
                            });

                            Vari.Server_AdminList_PID.Add(long.Parse(item.personaId));
                            Vari.Server_AdminList_Name.Add(item.displayName);
                        }

                        // VIP列表
                        index = 1;
                        foreach (var item in fullServerDetails.result.rspInfo.vipList)
                        {
                            ListBox_VIP.Items.Add(new RSPInfo()
                            {
                                Index = index++,
                                avatar = item.avatar,
                                displayName = item.displayName,
                                personaId = item.personaId
                            });

                            Vari.Server_VIPList.Add(long.Parse(item.personaId));
                        }

                        // BAN列表
                        index = 1;
                        foreach (var item in fullServerDetails.result.rspInfo.bannedList)
                        {
                            ListBox_BAN.Items.Add(new RSPInfo()
                            {
                                Index = index++,
                                avatar = item.avatar,
                                displayName = item.displayName,
                                personaId = item.personaId
                            });
                        }
                    }
                    catch (Exception ex)
                    {
                        Log.Ex(ex);
                    }





                    if (!isInit)
                        NotifierHelper.Show(NotifierType.Success, $"Got Server {Vari.GameId} Detailed data success |  Time: {result.ExecTime:0.00} Seconds");
                }
                else
                {
                    if (!isInit)
                        NotifierHelper.Show(NotifierType.Error, $"get server {Vari.GameId} 详细数据msg: {result.Message}  |  Time: {result.ExecTime:0.00} seconds");
                }
            }
            else
            {
                if (!isInit)
                    NotifierHelper.Show(NotifierType.Warning, "Please enter the server first to get the GameID");
            }
        }
        else
        {
            if (!isInit)
                NotifierHelper.Show(NotifierType.Warning, "Please get the player SessionID first");
        }
    }

    private async void Button_GetFullServerDetails_Click(object sender, RoutedEventArgs e)
    {
        AudioUtil.ClickSound();

        await GetFullServerDetails();
    }

    private async void Button_LeaveCurrentGame_Click(object sender, RoutedEventArgs e)
    {
        AudioUtil.ClickSound();

        if (!string.IsNullOrEmpty(Vari.SessionId))
        {
            if (!string.IsNullOrEmpty(Vari.GameId))
            {
                NotifierHelper.Show(NotifierType.Information, $"leaving server { Vari.GameId} ...");

                var result = await BF1API.LeaveGame();
                if (result.IsSuccess)
                {
                    NotifierHelper.Show(NotifierType.Success, $"leave the server { Vari.GameId} success  |  Time: {result.ExecTime:0.00} seconds");
                }
                else
                {
                    NotifierHelper.Show(NotifierType.Error, $"leave the server {Vari.GameId} msg: {result.Message}  |  Time: {result.ExecTime:0.00} seconds");
                }
            }
            else
            {
                NotifierHelper.Show(NotifierType.Warning, "Please enter the server first to get the GameID");
            }
        }
        else
        {
            NotifierHelper.Show(NotifierType.Warning, "Please get the player SessionID first");
        }
    }

    private async void ListBox_Map_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        int index = ListBox_Map.SelectedIndex;
        if (index != -1)
        {
            Map currMap = ListBox_Map.SelectedItem as Map;

            if (!string.IsNullOrEmpty(Vari.PersistedGameId))
            {
                string mapInfo = currMap.modePrettyName + " - " + currMap.mapPrettyName;

                var changeMapWindow = new ChangeMapWindow(mapInfo, currMap.mapImage);
                changeMapWindow.Owner = MainWindow.ThisMainWindow;
                if (changeMapWindow.ShowDialog() == true)
                {
                    NotifierHelper.Show(NotifierType.Information, $"server is being replaced { Vari.GameId} The map is {currMap.mapPrettyName} ...");

                    var result = await BF1API.ChangeServerMap(Vari.PersistedGameId, index);
                    if (result.IsSuccess)
                    {
                        NotifierHelper.Show(NotifierType.Success, $"Replace the server {Vari.GameId} The map is {currMap.mapPrettyName} success  |  Time: {result.ExecTime:0.00} seconds");
                    }
                    else
                    {
                        NotifierHelper.Show(NotifierType.Error, $"Replace the server {Vari.GameId} The map is {currMap.mapPrettyName} fail {result.Message}  |  Time: {result.ExecTime:0.00} seconds");
                    }
                }
            }
            else
            {
                NotifierHelper.Show(NotifierType.Warning, "PersistedGameId is abnormal, please get the server details again");
            }
        }

        // 使ListBox能够响应重复点击
        ListBox_Map.SelectedIndex = -1;
    }

    private async void Button_RemoveSelectedAdmin_Click(object sender, RoutedEventArgs e)
    {
        AudioUtil.ClickSound();

        RSPInfo rSPInfo = ListBox_Admin.SelectedItem as RSPInfo;

        NotifierHelper.Show(NotifierType.Information, $"Removing server administrator { rSPInfo.displayName} ...");

        var result = await BF1API.RemoveServerAdmin(rSPInfo.personaId);
        if (result.IsSuccess)
        {
            NotifierHelper.Show(NotifierType.Success, $"remove server administrator { rSPInfo.displayName} success  |  Time: {result.ExecTime:0.00} seconds");
        }
        else
        {
            NotifierHelper.Show(NotifierType.Error, $"remove server administrator {rSPInfo.displayName} Message: {result.Message}  |  Time: {result.ExecTime:0.00} secondss");
        }
    }

    private async void Button_AddNewAdmin_Click(object sender, RoutedEventArgs e)
    {
        AudioUtil.ClickSound();

        NotifierHelper.Show(NotifierType.Information, $"Adding Admin {TextBox_NewAdminName.Text} ...");

        var result = await BF1API.AddServerAdmin(TextBox_NewAdminName.Text);
        if (result.IsSuccess)
        {
            NotifierHelper.Show(NotifierType.Success, $"Added server administrator {TextBox_NewAdminName.Text} success  |  Time: {result.ExecTime:0.00} seconds");
        }
        else
        {
            NotifierHelper.Show(NotifierType.Error, $"Failed to remove {TextBox_NewAdminName.Text} {result.Message}  |  Time: {result.ExecTime:0.00} seconds");
        }
    }

    private async void Button_RemoveSelectedVIP_Click(object sender, RoutedEventArgs e)
    {
        AudioUtil.ClickSound();

        RSPInfo rSPInfo = ListBox_VIP.SelectedItem as RSPInfo;

        NotifierHelper.Show(NotifierType.Information, $"Removing VIP {rSPInfo.displayName} ...");

        var result = await BF1API.RemoveServerVip(rSPInfo.personaId);
        if (result.IsSuccess)
        {
            NotifierHelper.Show(NotifierType.Success, $"Removed VIP {rSPInfo.displayName} success  |  Time: {result.ExecTime:0.00} seconds");
        }
        else
        {
            NotifierHelper.Show(NotifierType.Error, $"Failed to remove VIP {rSPInfo.displayName} msg: {result.Message}  |  Time: {result.ExecTime:0.00} seconds");
        }
    }

    private async void Button_AddNewVIP_Click(object sender, RoutedEventArgs e)
    {
        AudioUtil.ClickSound();

        NotifierHelper.Show(NotifierType.Information, $"Adding VIP {TextBox_NewVIPName.Text} ...");

        var result = await BF1API.AddServerVip(TextBox_NewVIPName.Text);
        if (result.IsSuccess)
        {
            NotifierHelper.Show(NotifierType.Success, $"Add VIP {TextBox_NewVIPName.Text} success  |  Time: {result.ExecTime:0.00} second");
        }
        else
        {
            NotifierHelper.Show(NotifierType.Error, $"Failed to add VIP {TextBox_NewVIPName.Text} msg: {result.Message}  |  Time: {result.ExecTime:0.00} second");
        }
    }

    private async void Button_RemoveSelectedBAN_Click(object sender, RoutedEventArgs e)
    {
        AudioUtil.ClickSound();

        RSPInfo rSPInfo = ListBox_BAN.SelectedItem as RSPInfo;

        NotifierHelper.Show(NotifierType.Information, $"Adding BAN {rSPInfo.displayName} ...");

        var result = await BF1API.RemoveServerBan(rSPInfo.personaId);
        if (result.IsSuccess)
        {
            NotifierHelper.Show(NotifierType.Success, $"Removed BAN {rSPInfo.displayName} success  |  Time: {result.ExecTime:0.00} second");
        }
        else
        {
            NotifierHelper.Show(NotifierType.Error, $"Failed to remove BAN {rSPInfo.displayName} msg: {result.Message}  |  Time: {result.ExecTime:0.00} second");
        }
    }

    private async void Button_AddNewBAN_Click(object sender, RoutedEventArgs e)
    {
        AudioUtil.ClickSound();

        NotifierHelper.Show(NotifierType.Information, $"Adding BAN {TextBox_NewBANName.Text} 中...");

        var result = await BF1API.AddServerBan(TextBox_NewBANName.Text);
        if (result.IsSuccess)
        {
            NotifierHelper.Show(NotifierType.Success, $"Added BAN {TextBox_NewBANName.Text} success  |  Time: {result.ExecTime:0.00} second");
        }
        else
        {
            NotifierHelper.Show(NotifierType.Error, $"Failed to add BAN {TextBox_NewBANName.Text} msg: {result.Message}  |  Time: {result.ExecTime:0.00} second");
        }
    }

    private async void Button_KickSelectedSpectator_Click(object sender, RoutedEventArgs e)
    {
        AudioUtil.ClickSound();

        if (!string.IsNullOrEmpty(Vari.SessionId))
        {
            SpectatorInfo info = ListBox_Spectator.SelectedItem as SpectatorInfo;

            NotifierHelper.Show(NotifierType.Information, $"kicking player { info.Name} 中...");

            var reason = ChsUtil.ToTraditionalChinese(TextBox_KickSelectedSpectatorReason.Text);

            var result = await BF1API.AdminKickPlayer(info.PersonaId, reason);
            if (result.IsSuccess)
            {
                NotifierHelper.Show(NotifierType.Success, $"kicked the player { info.Name} success  |  Time: {result.ExecTime:0.00} second");
            }
            else
            {
                NotifierHelper.Show(NotifierType.Error, $"Failed to kick {info.Name} msg: {result.Message}  |  Time: {result.ExecTime:0.00} second");
            }
        }
        else
        {
            NotifierHelper.Show(NotifierType.Warning, "Couldn't get SessionID");
        }
    }

    private void Button_RefreshSpectatorList_Click(object sender, RoutedEventArgs e)
    {
        AudioUtil.ClickSound();

        ListBox_Spectator.Items.Clear();

        //string defaultAvatar = "https://secure.download.dm.origin.com/production/avatar/prod/1/599/208x208.JPEG";
        int index = 1;
        foreach (var item in Vari.Server_SpectatorList)
        {
            ListBox_Spectator.Items.Add(new SpectatorInfo()
            {
                Index = index++,
                Avatar = @"\Assets\Images\Other\Avatar.jpg",
                Name = item.Name,
                PersonaId = item.PersonaId
            });
        }
    }

    private async void Button_GetServerDetails_Click(object sender, RoutedEventArgs e)
    {
        AudioUtil.ClickSound();

        if (!string.IsNullOrEmpty(Vari.SessionId))
        {
            if (!string.IsNullOrEmpty(Vari.ServerId))
            {
                NotifierHelper.Show(NotifierType.Information, $"getting serverdetails of {Vari.ServerId}...");

                var result = await BF1API.GetServerDetails();
                if (result.IsSuccess)
                {
                    serverDetails = JsonUtil.JsonDese<ServerDetails>(result.Message);
                    Vari.NexServerDetails = serverDetails;
                    TextBox_ServerName.Text = serverDetails.result.serverSettings.name;
                    TextBox_ServerDescription.Text = serverDetails.result.serverSettings.description;

                    isGetServerDetailsOK = true;

                    NotifierHelper.Show(NotifierType.Success, $"Got Server {Vari.ServerId} success  |  Time: {result.ExecTime:0.00} seconds");
                }
                else
                {
                    NotifierHelper.Show(NotifierType.Error, $"Failed to get ServerId {Vari.ServerId} msg: {result.Message}  |  Time: {result.ExecTime:0.00} seconds");
                }
            }
            else
            {
                NotifierHelper.Show(NotifierType.Warning, "Please enter the server first to obtain ServerID");
            }
        }
        else
        {
            NotifierHelper.Show(NotifierType.Warning, "Please get the player SessionID first");
        }
    }

    private async void Button_UpdateServer_Click(object sender, RoutedEventArgs e)
    {
        AudioUtil.ClickSound();

        if (!isGetServerDetailsOK)
        {
            NotifierHelper.Show(NotifierType.Warning, $"Please obtain server information before performing this operation");
            return;
        }

        var serverName = TextBox_ServerName.Text.Trim();
        var serverDescription = TextBox_ServerDescription.Text.Trim();

        if (string.IsNullOrEmpty(serverName))
        {
            NotifierHelper.Show(NotifierType.Warning, $"Server name cannot be empty");
            return;
        }

        if (!string.IsNullOrEmpty(Vari.SessionId))
        {
            if (!string.IsNullOrEmpty(Vari.ServerId))
            {
                NotifierHelper.Show(NotifierType.Information, $"Updating server { Vari.ServerId} 数据中...");

                UpdateServerReqBody reqBody = new UpdateServerReqBody();
                reqBody.jsonrpc = "2.0";
                reqBody.method = "RSP.updateServer";

                var tempParams = new UpdateServerReqBody.Params
                {
                    deviceIdMap = new UpdateServerReqBody.Params.DeviceIdMap()
                    {
                        machash = Guid.NewGuid().ToString()
                    },
                    game = "tunguska",
                    serverId = Vari.ServerId,
                    bannerSettings = new UpdateServerReqBody.Params.BannerSettings()
                    {
                        bannerUrl = "",
                        clearBanner = true
                    }
                };

                var tempMapRotation = new UpdateServerReqBody.Params.MapRotation();
                var temp = serverDetails.result.mapRotations[0];
                var tempMaps = new List<UpdateServerReqBody.Params.MapRotation.MapsItem>();
                foreach (var item in temp.maps)
                {
                    tempMaps.Add(new UpdateServerReqBody.Params.MapRotation.MapsItem()
                    {
                        gameMode = item.gameMode,
                        mapName = item.mapName
                    });
                }
                tempMapRotation.maps = tempMaps;
                tempMapRotation.rotationType = temp.rotationType;
                tempMapRotation.mod = temp.mod;
                tempMapRotation.name = temp.name;
                tempMapRotation.description = temp.description;
                tempMapRotation.id = "100";

                tempParams.mapRotation = tempMapRotation;

                tempParams.serverSettings = new UpdateServerReqBody.Params.ServerSettings()
                {
                    name = serverName,
                    description = serverDescription,

                    message = serverDetails.result.serverSettings.message,
                    password = serverDetails.result.serverSettings.password,
                    bannerUrl = serverDetails.result.serverSettings.bannerUrl,
                    mapRotationId = serverDetails.result.serverSettings.mapRotationId,
                    customGameSettings = serverDetails.result.serverSettings.customGameSettings
                };

                reqBody.@params = tempParams;
                reqBody.id = Guid.NewGuid().ToString();

                var result = await BF1API.UpdateServer(reqBody);

                if (result.IsSuccess)
                {
                    NotifierHelper.Show(NotifierType.Success, $"Updated Server {Vari.ServerId} success  |  Time: {result.ExecTime:0.00} seconds");
                }
                else
                {
                    NotifierHelper.Show(NotifierType.Error, $"updating server {Vari.ServerId} msg: {result.Message}  |  Time: {result.ExecTime:0.00} seconds");
                }
            }
            else
            {
                NotifierHelper.Show(NotifierType.Warning, "Please enter the server first to obtain the ServerID");
            }
        }
        else
        {
            NotifierHelper.Show(NotifierType.Warning, "Please get the player SessionID first");
        }

        isGetServerDetailsOK = false;
    }

    private void Button_SetServerDetails2Traditional_Click(object sender, RoutedEventArgs e)
    {
        AudioUtil.ClickSound();

        var serverDescription = TextBox_ServerDescription.Text.Trim();

        if (string.IsNullOrEmpty(serverDescription))
        {
            NotifierHelper.Show(NotifierType.Warning, $"Server description cannot be empty");
            return;
        }

        TextBox_ServerDescription.Text = ChsUtil.ToTraditionalChinese(serverDescription);

        NotifierHelper.Show(NotifierType.Success, $"Convert server description text to Traditional Chinese success");
    }    
}
