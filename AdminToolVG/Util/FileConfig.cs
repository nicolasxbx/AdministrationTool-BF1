using Nucs.JsonSettings;

namespace AdminToolVG;

public static class FileConfig
{
    public static BannedWeaponConfig CurrentConfig { get; set; } = new();
    public static void Load()
    {
        CurrentConfig = JsonSettings.Load<BannedWeaponConfig>(Vari.RuleSetConfigFilePath);
        Log.D($"RuleSets Count: {CurrentConfig.WeaponConfigs.Count}");
    }
}

public class BannedWeaponConfig : JsonSettings
{
    public override string FileName { get; set; } = Vari.RuleSetConfigFilePath;
    public List<Ruleset> WeaponConfigs { get; set; } = new();
    public List<MapPreset> MapPresets { get; set; } = new();
    public WebhookConfig WebhookConfig { get; set; } = new();
    public string Auth_ID { get; set; } = string.Empty;
    public bool AudioDisabled { get; set; } = false;
    public BannedWeaponConfig() { }
    public BannedWeaponConfig(string fileName) : base(fileName) { }
}
public class Ruleset
{
    public string ConfigName { get; set; } = "";
    public List<string> Weapons { get; set; } = new();    
    public int Pinglimit { get; set; } = 0;
    public int KickAbovePlayerCount { get; set; } = 0;
    public string RulesChat { get; set; } = "";
}
public class MapPreset
{
    public string name { get; set; }
    public List<MapsItem> maps { get; set; }
}
public class WebhookConfig
{
    public string monitoring { get; set; } = string.Empty;
    public string itemkicks { get; set; } = string.Empty;
    public string pingkicks { get; set; } = string.Empty;
    public string winswitch_bfban { get; set; } = string.Empty;
    public string balancers { get; set; } = string.Empty;
}
