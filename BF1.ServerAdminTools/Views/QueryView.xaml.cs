using BF1.ServerAdminTools.Models;
using BF1.ServerAdminTools.Common.Utils;
using BF1.ServerAdminTools.Common.Helper;
using BF1.ServerAdminTools.Features.API;
using BF1.ServerAdminTools.Features.API.RespJson;
using BF1.ServerAdminTools.Features.Utils;

using RestSharp;
using CommunityToolkit.Mvvm.Input;

namespace BF1.ServerAdminTools.Views;

/// <summary>
/// QueryView.xaml 的交互逻辑
/// </summary>
public partial class QueryView : UserControl
{
    public class WeaponStats
    {
        public string name { get; set; }
        public string imageUrl { get; set; }
        public string star { get; set; }

        public int kills { get; set; }
        public string killsPerMinute { get; set; }

        public int headshots { get; set; }
        public string headshotsVKills { get; set; }

        public int shots { get; set; }
        public int hits { get; set; }
        public string hitsVShots { get; set; }

        public string hitVKills { get; set; }
        public string time { get; set; }
    }

    public class VehicleStats
    {
        public string name { get; set; }
        public string imageUrl { get; set; }
        public string star { get; set; }

        public int kills { get; set; }
        public string killsPerMinute { get; set; }

        public int destroyed { get; set; }
        public string time { get; set; }
    }

    public QueryModel QueryModel { get; set; } = new();
    public ObservableCollection<string> PlayerDataOC { get; set; } = new();
    public ObservableCollection<WeaponStats> WeaponStatsOC { get; set; } = new();
    public ObservableCollection<VehicleStats> VehicleStatsOC { get; set; } = new();

    private int count = 0;

    public RelayCommand QueryPlayerCommand { get; set; }

    public QueryView()
    {
        InitializeComponent();
        this.DataContext = this;

        QueryPlayerCommand = new(QueryPlayer);

        QueryModel.LoadingVisibility = Visibility.Collapsed;

        QueryModel.PlayerName = "TheNexusAce";
    }

    private async void QueryPlayer() //has to be void
    {
        AudioUtil.ClickSound();

        if (string.IsNullOrEmpty(QueryModel.PlayerName))
        {
            NotifierHelper.Show(NotifierType.Warning, "The target player name is empty, the operation is canceled");
            return;
        }

        if (!string.IsNullOrEmpty(Vari.Remid) && !string.IsNullOrEmpty(Vari.Sid))
        {
            QueryModel.LoadingVisibility = Visibility.Visible;
            ClearData();
            NotifierHelper.Show(NotifierType.Information, "Inquiring, please wait...");

            var str = "https://accounts.ea.com/connect/auth?response_type=token&locale=zh_CN&client_id=ORIGIN_JS_SDK&redirect_uri=nucleus%3Arest";
            //var str = "https://accounts.ea.com/connect/auth?response_type=token&locale=en_US&client_id=ORIGIN_JS_SDK&redirect_uri=nucleus%3Arest";
            var options = new RestClientOptions(str)
            {
                MaxTimeout = 5000,
                FollowRedirects = false
            };

            var client = new RestClient(options);
            var request = new RestRequest()
                .AddHeader("Cookie", $"remid={Vari.Remid};sid={Vari.Sid};");

            var response = await client.ExecuteGetAsync(request);
            if (response.StatusCode == HttpStatusCode.OK)
            {
                JsonNode jNode = JsonNode.Parse(response.Content);
                var access_token = jNode["access_token"].GetValue<string>();

                str = $"https://gateway.ea.com/proxy/identity/personas?namespaceName=cem_ea_id&displayName={QueryModel.PlayerName}";
                options = new RestClientOptions(str)
                {
                    MaxTimeout = 5000,
                    FollowRedirects = false
                };

                client = new RestClient(options);
                request = new RestRequest()
                   .AddHeader("X-Expand-Results", true)
                   .AddHeader("Authorization", $"Bearer {access_token}");

                response = await client.ExecuteGetAsync(request);
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    jNode = JsonNode.Parse(response.Content);
                    if (jNode["personas"]!["persona"] != null)
                    {
                        var personaId = jNode["personas"]!["persona"][0]["personaId"].GetValue<long>();
                        await QueryRecord(personaId);
                    }
                    else
                    {
                        QueryModel.LoadingVisibility = Visibility.Collapsed;
                        NotifierHelper.Show(NotifierType.Warning, $"玩家 {QueryModel.PlayerName} 不存在");
                    }
                }
                else
                {
                    QueryModel.LoadingVisibility = Visibility.Collapsed;
                    NotifierHelper.Show(NotifierType.Error, "网络请求错误");
                }
            }
            else
            {
                QueryModel.LoadingVisibility = Visibility.Collapsed;
                NotifierHelper.Show(NotifierType.Error, "EA Server unresponsive.");
            }
        }
        else
        {
            NotifierHelper.Show(NotifierType.Error, "操作失败，玩家Remid或Sid为空");
        }
    }

    private void ClearData()
    {
        count = 0;

        QueryModel.Avatar = string.Empty;
        QueryModel.Emblem = string.Empty;

        QueryModel.PersonaId = string.Empty;
        QueryModel.Rank = string.Empty;
        QueryModel.PlayTime = string.Empty;
        QueryModel.PlayingServer = string.Empty;

        PlayerDataOC.Clear();
        WeaponStatsOC.Clear();
        VehicleStatsOC.Clear();
    }

    private async Task QueryRecord(long personaId)
    {
        await DetailedStats(personaId);

        await GetWeapons(personaId);
        await GetVehicles(personaId);

        await GetPersonas(personaId);
    }

    private void IsAllFinish()
    {
        count++;

        if (count == 4)
        {
            QueryModel.LoadingVisibility = Visibility.Collapsed;
        }
    }

    private async Task GetPersonas(long personaId)
    {
        var result = await BF1API.GetPersonasByIds(personaId);
        if (result.IsSuccess)
        {
            try //tna
            { 
            JsonNode jNode = JsonNode.Parse(result.Message);
            QueryModel.Avatar = jNode["result"]![$"{personaId}"]!["avatar"].GetValue<string>();
            QueryModel.PersonaId = jNode["result"]![$"{personaId}"]!["personaId"].GetValue<string>();

            QueryModel.Rank = $"Rank : 0";
            }
            catch(Exception ex) //tna
            {
                Log.Ex(ex);
                NotifierHelper.Show(NotifierType.Information, ex.Message);
            }
            //tna
            IsAllFinish();
        }

        result = await BF1API.GetEquippedEmblem(personaId);
        if (result.IsSuccess)
        {
            JsonNode jNode = JsonNode.Parse(result.Message);
            if (jNode["result"] != null)
            {
                var img = jNode["result"].GetValue<string>();
                QueryModel.Emblem = img.Replace("[SIZE]", "256").Replace("[FORMAT]", "png");
            }
        }

        result = await BF1API.GetServersByPersonaIds(personaId);
        if (result.IsSuccess)
        {
            JsonNode jNode = JsonNode.Parse(result.Message);

            var obj = jNode["result"]![$"{personaId}"];
            if (obj != null)
            {
                var name = obj["name"].GetValue<string>();
                QueryModel.PlayingServer= $"Playing : {name}";
            }
            else
            {
                QueryModel.PlayingServer = $"Playing : none";
            }
        }
    }

    private async Task DetailedStats(long personaId)
    {
        var result = await BF1API.DetailedStatsByPersonaId(personaId);
        if (result.IsSuccess)
        {
            var detailed = JsonUtil.JsonDese<DetailedStats>(result.Message);

            var basic = detailed.result.basicStats;
            QueryModel.PlayTime = $"Playtime : {PlayerUtil.GetPlayTime(basic.timePlayed)}";

            IsAllFinish();

            await Task.Run(() =>
            {
                AddPlayerInfo($"KD : {PlayerUtil.GetPlayerKD(basic.kills, basic.deaths):0.00}");
                AddPlayerInfo($"KPM : {basic.kpm}");
                AddPlayerInfo($"SPM : {basic.spm}");

                AddPlayerInfo($"Accuracy : {detailed.result.accuracyRatio * 100:0.00}%");
                AddPlayerInfo($"HS% : {PlayerUtil.GetPlayerPercentage(detailed.result.headShots, basic.kills)}");
                AddPlayerInfo($"Headshots : {detailed.result.headShots}");

                AddPlayerInfo($"Highest Kill Streak : {detailed.result.highestKillStreak}");
                AddPlayerInfo($"Furthest Headshot Distance : {detailed.result.longestHeadShot}");
                AddPlayerInfo($"Best Class : {detailed.result.favoriteClass}");

                AddPlayerInfo("");

                AddPlayerInfo($"Kills : {basic.kills}");
                AddPlayerInfo($"Deaths : {basic.deaths}");
                AddPlayerInfo($"KillAssists : {detailed.result.killAssists}");

                AddPlayerInfo($"avengerKills : {detailed.result.avengerKills}");
                AddPlayerInfo($"saviorKills : {detailed.result.saviorKills}");
                AddPlayerInfo($"Revives : {detailed.result.revives}");
                AddPlayerInfo($"Heals : {detailed.result.heals}");
                AddPlayerInfo($"Repairs : {detailed.result.repairs}");

                AddPlayerInfo("");

                AddPlayerInfo($"Wins : {basic.wins}");
                AddPlayerInfo($"Losses : {basic.losses}");
                AddPlayerInfo($"Win Rate : {PlayerUtil.GetPlayerPercentage(basic.wins, detailed.result.roundsPlayed)}");
                AddPlayerInfo($"Skill : {basic.skill}");
                AddPlayerInfo($"Rounds Played : {detailed.result.roundsPlayed}");
                AddPlayerInfo($"Dogtags taken : {detailed.result.dogtagsTaken}");

                AddPlayerInfo($"Squad Score : {detailed.result.squadScore}");
                AddPlayerInfo($"Award Score : {detailed.result.awardScore}");
                AddPlayerInfo($"Bonus Score : {detailed.result.bonusScore}");
            });
        }
    }

    private async Task GetWeapons(long personaId)
    {
        var result = await BF1API.GetWeaponsByPersonaId(personaId);
        if (result.IsSuccess)
        {
            var getWeapons = JsonUtil.JsonDese<GetWeapons>(result.Message);

            var weapons = new List<WeaponStats>();
            foreach (var res in getWeapons.result)
            {
                foreach (var wea in res.weapons)
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

            IsAllFinish();

            await Task.Run(() =>
            {
                foreach (var item in weapons)
                {
                    this.Dispatcher.Invoke(DispatcherPriority.Background, new Action(() =>
                    {
                        WeaponStatsOC.Add(item);
                    }));
                }
            });
        }
    }

    private async Task GetVehicles(long personaId)
    {
        var result = await BF1API.GetVehiclesByPersonaId(personaId);
        if (result.IsSuccess)
        {
            var getVehicles = JsonUtil.JsonDese<GetVehicles>(result.Message);

            var vehicles = new List<VehicleStats>();
            foreach (var res in getVehicles.result)
            {
                foreach (var veh in res.vehicles)
                {
                    if (veh.stats.values.kills == 0)
                        continue;

                    vehicles.Add(new VehicleStats()
                    {
                        name = ChsUtil.ToSimplifiedChinese(veh.name),
                        imageUrl = PlayerUtil.GetTempImagePath(veh.imageUrl, "vehicles2"),
                        star = PlayerUtil.GetKillStar((int)veh.stats.values.kills),
                        kills = (int)veh.stats.values.kills,
                        killsPerMinute = PlayerUtil.GetPlayerKPM(veh.stats.values.kills, veh.stats.values.seconds),
                        destroyed = (int)veh.stats.values.destroyed,
                        time = PlayerUtil.GetPlayTime(veh.stats.values.seconds)
                    });
                }
            }

            vehicles.Sort((a, b) => b.kills.CompareTo(a.kills));

            IsAllFinish();

            await Task.Run(() =>
            {
                foreach (var item in vehicles)
                {
                    this.Dispatcher.Invoke(DispatcherPriority.Background, new Action(() =>
                    {
                        VehicleStatsOC.Add(item);
                    }));
                }
            });
        }
    }

    private void AddPlayerInfo(string str)
    {
        this.Dispatcher.Invoke(DispatcherPriority.Background, new Action(() =>
        {
            PlayerDataOC.Add(str);
        }));
    }
}
