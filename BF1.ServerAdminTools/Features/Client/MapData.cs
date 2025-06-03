namespace BF1.ServerAdminTools.Features.Client;

public static class MapData
{
    private const string AHU = "奥匈帝国";
    private const string GER = "德意志帝国";
    private const string OTM = "奥斯曼帝国";
    private const string ITA = "意大利王国";
    private const string UK = "大英帝国";
    private const string RM = "皇家海军陆战队";
    private const string USA = "美国";
    private const string FRA = "法国";
    private const string RUS = "俄罗斯帝国";
    private const string BOL = "苏俄红军";

    public struct MapName
    {
        public string English;
        public string Chinese;
        public string Team1;
        public string Team2;
        public string Image;
    }

    /// <summary>
    /// 地图数据
    /// </summary>
    public readonly static List<MapName> AllMapInfo = new()
    {
        new() { English="ID_M_LEVEL_MENU", Chinese="Level Menu", Team1="_DEF", Team2="_DEF" },
        new() { English="ID_M_MP_LEVEL_MOUNTAIN_FORT", Chinese="Monte Grappa", Team1="ITA", Team2="AHU", Image=@"\Assets\Images\Client\Maps\MP_MountainFort_LandscapeLarge-8a517533.jpg" },
        new() { English="ID_M_MP_LEVEL_FOREST", Chinese="Argonne Forest", Team1="USA", Team2="GER", Image=@"\Assets\Images\Client\Maps\MP_Forest_LandscapeLarge-dfbbe910.jpg" },
        new() { English="ID_M_MP_LEVEL_ITALIAN_COAST", Chinese="Italian Coast", Team1="ITA", Team2="AHU", Image=@"\Assets\Images\Client\Maps\MP_ItalianCoast_LandscapeLarge-1503eec7.jpg" },
        new() { English="ID_M_MP_LEVEL_CHATEAU", Chinese="Ballroom Blitz", Team1="USA", Team2="GER", Image=@"\Assets\Images\Client\Maps\MP_Chateau_LandscapeLarge-244d5987.jpg" },
        new() { English="ID_M_MP_LEVEL_SCAR", Chinese="Scar", Team1="GER", Team2="UK", Image=@"\Assets\Images\Client\Maps\MP_Scar_LandscapeLarge-ee25fbd6.jpg" },
        new() { English="ID_M_MP_LEVEL_DESERT", Chinese="Sinai Desert", Team1="UK", Team2="OTM", Image=@"\Assets\Images\Client\Maps\MP_Desert_LandscapeLarge-d8f749da.jpg" },
        new() { English="ID_M_MP_LEVEL_AMIENS", Chinese="Amiens", Team1="GER", Team2="UK", Image=@"\Assets\Images\Client\Maps\MP_Amiens_LandscapeLarge-e195589d.jpg" },
        new() { English="ID_M_MP_LEVEL_SUEZ", Chinese="Suez", Team1="UK", Team2="OTM", Image=@"\Assets\Images\Client\Maps\MP_Suez_LandscapeLarge-f630fc76.jpg" },
        new() { English="ID_M_MP_LEVEL_FAO_FORTRESS", Chinese="Fao Fortress", Team1="UK", Team2="OTM", Image=@"\Assets\Images\Client\Maps\MP_FaoFortress_LandscapeLarge-cad1748e.jpg" },
        new() { English="ID_M_MP_LEVEL_GIANT", Chinese="Giants Shadow", Team1="UK", Team2="GER", Image=@"\Assets\Images\Client\Maps\MP_Giant_LandscapeLarge-dd0b93ef.jpg" },
        new() { English="ID_M_MP_LEVEL_FIELDS", Chinese="Soisson", Team1="FRA", Team2="GER", Image=@"\Assets\Images\Client\Maps\MP_Fields_LandscapeLarge-5f53ddc4.jpg" },
        new() { English="ID_M_MP_LEVEL_GRAVEYARD", Chinese="Rupture", Team1="FRA", Team2="GER", Image=@"\Assets\Images\Client\Maps\MP_Graveyard_LandscapeLarge-bd1012e6.jpg" },
        new() { English="ID_M_MP_LEVEL_UNDERWORLD", Chinese="Fort De Vaux", Team1="GER", Team2="FRA", Image=@"\Assets\Images\Client\Maps\MP_Underworld_LandscapeLarge-b6c5c7e7.jpg" },
        new() { English="ID_M_MP_LEVEL_VERDUN", Chinese="Verdun", Team1="GER", Team2="FRA", Image=@"\Assets\Images\Client\Maps\MP_Verdun_LandscapeLarge-1a364063.jpg" },
        new() { English="ID_M_MP_LEVEL_TRENCH", Chinese="Nivelle Nights", Team1="GER", Team2="FRA", Image=@"\Assets\Images\Client\Maps\MP_Trench_LandscapeLarge-dbd1248f.jpg" },
        new() { English="ID_M_MP_LEVEL_SHOVELTOWN", Chinese="Prise de Tahure", Team1="GER", Team2="FRA", Image=@"\Assets\Images\Client\Maps\MP_Shoveltown_LandscapeLarge-d0aa5920.jpg" },
        new() { English="ID_M_MP_LEVEL_BRIDGE", Chinese="Brusilov Keep", Team1="RUS", Team2="AHU", Image=@"\Assets\Images\Client\Maps\MP_Bridge_LandscapeLarge-5b7f1b62.jpg" },
        new() { English="ID_M_MP_LEVEL_ISLANDS", Chinese="Albions", Team1="GER", Team2="RUS", Image=@"\Assets\Images\Client\Maps\MP_Islands_LandscapeLarge-c9d8272b.jpg" },
        new() { English="ID_M_MP_LEVEL_RAVINES", Chinese="Lupkow Pass", Team1="AHU", Team2="RUS", Image=@"\Assets\Images\Client\Maps\MP_Ravines_LandscapeLarge-1fe0d3f6.jpg" },
        new() { English="ID_M_MP_LEVEL_VALLEY", Chinese="Galicia", Team1="RUS", Team2="AHU", Image=@"\Assets\Images\Client\Maps\MP_Valley_LandscapeLarge-8dc1c7ca.jpg" },
        new() { English="ID_M_MP_LEVEL_TSARITSYN", Chinese="TSARITSYN", Team1="BOL", Team2="RUS", Image=@"\Assets\Images\Client\Maps\MP_Tsaritsyn_LandscapeLarge-2dbd3bf5.jpg" },
        new() { English="ID_M_MP_LEVEL_VOLGA", Chinese="Volga River", Team1="BOL", Team2="RUS", Image=@"\Assets\Images\Client\Maps\MP_Volga_LandscapeLarge-6ac49c25.jpg" },
        new() { English="ID_M_MP_LEVEL_BEACHHEAD", Chinese="Cape Helles", Team1="UK", Team2="OTM", Image=@"\Assets\Images\Client\Maps\MP_Beachhead_LandscapeLarge-5a13c655.jpg" },
        new() { English="ID_M_MP_LEVEL_HARBOR", Chinese="Zeebrugge", Team1="RM", Team2="GER", Image=@"\Assets\Images\Client\Maps\MP_Harbor_LandscapeLarge-d382c7ea.jpg" },
        new() { English="ID_M_MP_LEVEL_NAVAL", Chinese="Heligoland", Team1="RM", Team2="GER", Image=@"\Assets\Images\Client\Maps\MP_Naval_LandscapeLarge-dc2e8daf.jpg" },
        new() { English="ID_M_MP_LEVEL_RIDGE", Chinese="Achi Baba", Team1="UK", Team2="OTM", Image=@"\Assets\Images\Client\Maps\MP_Ridge_LandscapeLarge-8c057a19.jpg" },
        new() { English="ID_M_MP_LEVEL_OFFENSIVE", Chinese="River Somme", Team1="UK", Team2="GER", Image=@"\Assets\Images\Client\Maps\MP_Offensive_LandscapeLarge-6dabdea3.jpg" },
        new() { English="ID_M_MP_LEVEL_HELL", Chinese="Passchendaele", Team1="UK", Team2="GER", Image=@"\Assets\Images\Client\Maps\MP_Hell_LandscapeLarge-7176911c.jpg" },
        new() { English="ID_M_MP_LEVEL_RIVER", Chinese="Caporetto", Team1="AHU", Team2="ITA", Image=@"\Assets\Images\Client\Maps\MP_River_LandscapeLarge-21443ae9.jpg" },
        new() { English="ID_M_MP_LEVEL_ALPS", Chinese="ALPS", Team1="GER", Team2="UK", Image=@"\Assets\Images\Client\Maps\MP_Alps_LandscapeLarge-7ab30e3e.jpg" },
        new() { English="ID_M_MP_LEVEL_BLITZ", Chinese="BLITZ", Team1="GER", Team2="UK", Image=@"\Assets\Images\Client\Maps\MP_Blitz_LandscapeLarge-5e26212f.jpg" },
        new() { English="ID_M_MP_LEVEL_LONDON", Chinese="LONDON", Team1="GER", Team2="UK", Image=@"\Assets\Images\Client\Maps\MP_London_LandscapeLarge-0b51fe46.jpg" }
    };
}
