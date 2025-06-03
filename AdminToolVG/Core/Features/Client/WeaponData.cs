namespace BF1.ServerAdminTools.Features.Client;

public static class WeaponData
{
    public struct WeaponName
    {
        public string Class;
        public string English;
        public string Chinese;
        public string ShortTxt;
    }

    /// <summary>
    /// 全部武器信息，ShortTxt不超过16个字符
    /// </summary>
    public readonly static List<WeaponName> AllWeaponInfo = new()
    {
        // 配枪
        new WeaponName(){ Class="Pistol", English="U_M1911", Chinese="M1911", ShortTxt="M1911" },
        new WeaponName(){ Class="Pistol", English="U_LugerP08", Chinese="P08 Pistol", ShortTxt="P08" },
        new WeaponName(){ Class="Pistol", English="U_FN1903", Chinese="Mle 1903", ShortTxt="M1903" },
        new WeaponName(){ Class="Pistol", English="U_BorchardtC93", Chinese="C93", ShortTxt="C93" },
        new WeaponName(){ Class="Pistol", English="U_SmithWesson", Chinese="No. 3 Revolver", ShortTxt="No3 Rv" },
        new WeaponName(){ Class="Pistol", English="U_Kolibri", Chinese="Kolibri", ShortTxt="Kolibr" },
        new WeaponName(){ Class="Pistol", English="U_NagantM1895", Chinese="Nagant Revolver", ShortTxt="Nag Rv" },
        new WeaponName(){ Class="Pistol", English="U_Obrez", Chinese="Obrez Pistol", ShortTxt="Obrez" },
        new WeaponName(){ Class="Pistol", English="U_Webley_Mk6", Chinese="Revolver Mk VI", ShortTxt="Mk VI" },
        new WeaponName(){ Class="Pistol", English="U_M1911_Preorder_Hellfighter", Chinese="Hellfighter M1911", ShortTxt="M1911" },
        new WeaponName(){ Class="Pistol", English="U_LugerP08_Wep_Preorder", Chinese="Red Baron's P08", ShortTxt="P08 RB" },
        new WeaponName(){ Class="Pistol", English="U_M1911_Suppressed", Chinese="M1911 Silencer", ShortTxt="M1911S" },
        new WeaponName(){ Class="Pistol", English="U_SingleActionArmy", Chinese="Peacekeeper", ShortTxt="Peacek" },

        // 手榴弹
        new WeaponName(){ Class="Grenade", English="U_FragGrenade", Chinese="Frag Grenade", ShortTxt="FragG" },
        new WeaponName(){ Class="Grenade", English="U_GermanStick", Chinese="German Stick", ShortTxt="GermG" },
        new WeaponName(){ Class="Grenade", English="U_GasGrenade", Chinese="Gas Grenade", ShortTxt="GasG" },
        new WeaponName(){ Class="Grenade", English="U_ImpactGrenade", Chinese="Impact Grenade", ShortTxt="ImpG" },
        new WeaponName(){ Class="Grenade", English="U_Incendiary", Chinese="Incendiary Grenade", ShortTxt="IncG" },
        new WeaponName(){ Class="Grenade", English="U_MiniGrenade", Chinese="Mini Grenade", ShortTxt="MiniG" },
        new WeaponName(){ Class="Grenade", English="U_SmokeGrenade", Chinese="Smoke Grenade", ShortTxt="SmokeG" },
        new WeaponName(){ Class="Grenade", English="U_Grenade_AT", Chinese="Light Anti-Tank Grenade", ShortTxt="LATG" },
        new WeaponName(){ Class="Grenade", English="U_ImprovisedGrenade", Chinese="Improvised Grenade", ShortTxt="ImprvG" },
        new WeaponName(){ Class="Grenade", English="U_RussianBox", Chinese="Russian Grenade", ShortTxt="RussG" },

        ////////////////////////////////// 突击兵 Assault //////////////////////////////////

        // 突击兵 主要武器
        new WeaponName(){ Class="Assault Weapon", English="U_RemingtonM10_Wep_Slug", Chinese="Model 10-A Slug", ShortTxt="M10 Sl" },
        new WeaponName(){ Class="Assault Weapon", English="U_RemingtonM10_Wep_Choke", Chinese="Model 10-A Hunter", ShortTxt="M10 Hu" },
        new WeaponName(){ Class="Assault Weapon", English="U_RemingtonM10", Chinese="Model 10-A Factory", ShortTxt="M10 Fa" },
        new WeaponName(){ Class="Assault Weapon", English="U_Winchester1897_Wep_Sweeper", Chinese="M97 Trench-Gun Sweeper", ShortTxt="M97 Sw" },
        new WeaponName(){ Class="Assault Weapon", English="U_Winchester1897_Wep_LowRecoil", Chinese="M97 Trench-Gun Back-Bored", ShortTxt="M97 BB" },
        new WeaponName(){ Class="Assault Weapon", English="U_Winchester1897_Wep_Choke", Chinese="M97 Hunter", ShortTxt="M97 Hu" },
        new WeaponName(){ Class="Assault Weapon", English="U_MP18_Wep_Trench", Chinese="MP 18 Trench", ShortTxt="MP18 T" },
        new WeaponName(){ Class="Assault Weapon", English="U_MP18_Wep_Burst", Chinese="MP 18 Burst", ShortTxt="MP18 Bu" },
        new WeaponName(){ Class="Assault Weapon", English="U_MP18_Wep_Accuracy", Chinese="MP 18 Optical", ShortTxt="MP18 Op" },
        new WeaponName(){ Class="Assault Weapon", English="U_BerettaM1918_Wep_Trench", Chinese="Automatico M1918 Trench", ShortTxt="AutomT" },
        new WeaponName(){ Class="Assault Weapon", English="U_BerettaM1918_Wep_Stability", Chinese="Automatico M1918 Storm", ShortTxt="AutomS" },
        new WeaponName(){ Class="Assault Weapon", English="U_BerettaM1918", Chinese="Automatico M1918 Factory", ShortTxt="AutomF" },
        new WeaponName(){ Class="Assault Weapon", English="U_BrowningA5_Wep_LowRecoil", Chinese="12g Automatic Back-Bored", ShortTxt="12G Ba" },
        new WeaponName(){ Class="Assault Weapon", English="U_BrowningA5_Wep_Choke", Chinese="12g Automatic Hunter", ShortTxt="12g LR" },
        new WeaponName(){ Class="Assault Weapon", English="U_BrowningA5_Wep_ExtensionTube", Chinese="12g Automatic Extended", ShortTxt="12g Ex" },
        new WeaponName(){ Class="Assault Weapon", English="U_Hellriegel1915", Chinese="Hellriegel 1915 Factory", ShortTxt="Hell F" },
        new WeaponName(){ Class="Assault Weapon", English="U_Hellriegel1915_Wep_Accuracy", Chinese="Hellriegel 1915 Defensive", ShortTxt="Hell D" },
        new WeaponName(){ Class="Assault Weapon", English="U_Winchester1897_Wep_Preorder", Chinese="Hellfighter Trench Shotgun", ShortTxt="Hell T" },
        new WeaponName(){ Class="Assault Weapon", English="U_SjogrenShotgun", Chinese="Sjörgen Inertial Factory", ShortTxt="Sjrg F" },
        new WeaponName(){ Class="Assault Weapon", English="U_SjogrenShotgun_Wep_Slug", Chinese="Sjörgen Inertial Slug", ShortTxt="Sjrg S" },
        new WeaponName(){ Class="Assault Weapon", English="U_Ribeyrolles", Chinese="Ribeyrolles 1917 Factory", ShortTxt="Riby F" },
        new WeaponName(){ Class="Assault Weapon", English="U_Ribeyrolles_Wep_Optical", Chinese="Ribeyrolles 1918 Optical", ShortTxt="Riby O" },
        new WeaponName(){ Class="Assault Weapon", English="U_RemingtonModel1900", Chinese="Model 1900", ShortTxt="Mdl 19" },
        new WeaponName(){ Class="Assault Weapon", English="U_RemingtonModel1900_Wep_Slug", Chinese="Model 1900 Slug", ShortTxt="Mdl19S" },
        new WeaponName(){ Class="Assault Weapon", English="U_MaximSMG", Chinese="SMG 08/18 Factory (VG BAN)", ShortTxt="Smg08" },
        new WeaponName(){ Class="Assault Weapon", English="U_MaximSMG_Wep_Accuracy", Chinese="SMG 08/18 Optical (VG BAN)", ShortTxt="Smg08O" },
        new WeaponName(){ Class="Assault Weapon", English="U_SteyrM1912_P16", Chinese="Maschinenpistole M1912/P.16", ShortTxt="Mschi" },
        new WeaponName(){ Class="Assault Weapon", English="U_SteyrM1912_P16_Wep_Burst", Chinese="Maschinenpistole M1912/P.16 Burst", ShortTxt="Msch B" },
        new WeaponName(){ Class="Assault Weapon", English="U_Mauser1917Trench", Chinese="M1917 Trench Carbine", ShortTxt="M19TC" },
        new WeaponName(){ Class="Assault Weapon", English="U_Mauser1917Trench_Wep_Scope", Chinese="M1917 Patrol Carbine", ShortTxt="M19PC" },
        new WeaponName(){ Class="Assault Weapon", English="U_ChauchatSMG", Chinese="RSC SMG", ShortTxt="RSCSMG" },
        new WeaponName(){ Class="Assault Weapon", English="U_ChauchatSMG_Wep_Optical", Chinese="RSC SMG Optical", ShortTxt="RSCSMO" },
        new WeaponName(){ Class="Assault Weapon", English="U_M1919Thompson_Wep_Trench", Chinese="Annihilator Trench", ShortTxt="Anni T" },
        new WeaponName(){ Class="Assault Weapon", English="U_M1919Thompson_Wep_Stability", Chinese="Annihilator Storm", ShortTxt="Anni S" },
        new WeaponName(){ Class="Assault Weapon", English="U_FrommerStopAuto", Chinese="Frommer Stop Auto", ShortTxt="From A" },
        new WeaponName(){ Class="Assault Weapon", English="U_SawnOffShotgun", Chinese="Sawed Off Shotgun", ShortTxt="SawOff" },

        // 突击兵 配枪
        new WeaponName(){ Class="Assault Pistol", English="U_GasserM1870", Chinese="Gasser M1870", ShortTxt="Gasser" },
        new WeaponName(){ Class="Assault Pistol", English="U_LancasterHowdah", Chinese="Howdah Pistol", ShortTxt="Howdah" },
        new WeaponName(){ Class="Assault Pistol", English="U_Hammerless", Chinese="1903 Hammerless", ShortTxt="Hmmrls" },

        // 突击兵 配备一二
        new WeaponName(){ Class="Assault Gadget", English="U_Dynamite", Chinese="Dynashite (Nexus disapproves)", ShortTxt="Dynmit" },
        new WeaponName(){ Class="Assault Gadget", English="U_ATGrenade", Chinese="Anti-Tank Grenade (Gadget)", ShortTxt="AT Gad" },
        new WeaponName(){ Class="Assault Gadget", English="U_ATMine", Chinese="Anti-Tank Mine", ShortTxt="ATMine" },
        new WeaponName(){ Class="Assault Gadget", English="U_BreechGun", Chinese="AT Rocket Gun", ShortTxt="AT Rck" },
        new WeaponName(){ Class="Assault Gadget", English="U_BreechGun_Flak", Chinese="AA Rocket Guun", ShortTxt="AA Rck" },

        ////////////////////////////////// 医疗兵 Medic   //////////////////////////////////

        // Medic Weapon
        new WeaponName(){ Class="Medic Weapon", English="U_CeiRigottiM1895_Wep_Trench", Chinese="Cei-Rigotti Trench", ShortTxt="CeiRiT" },
        new WeaponName(){ Class="Medic Weapon", English="U_CeiRigottiM1895_Wep_Range", Chinese="Cei-Rigotti Optical", ShortTxt="CeiRi" },
        new WeaponName(){ Class="Medic Weapon", English="U_CeiRigottiM1895", Chinese="Cei-Rigotti Factory", ShortTxt="M1895 YC" },
        new WeaponName(){ Class="Medic Weapon", English="U_MauserSL1916_Wep_Scope", Chinese="Selbstlader M1916 Optical", ShortTxt="M1916 SSS" },
        new WeaponName(){ Class="Medic Weapon", English="U_MauserSL1916_Wep_Range", Chinese="Selbstlader M1916 Marksman", ShortTxt="M1916 MZJ" },
        new WeaponName(){ Class="Medic Weapon", English="U_MauserSL1916", Chinese="Selbstlader M1916 Factory", ShortTxt="M1916 YC" },
        new WeaponName(){ Class="Medic Weapon", English="U_WinchesterM1907_Wep_Trench", Chinese="M1907 SL Trench", ShortTxt="M1907 JGZ" },
        new WeaponName(){ Class="Medic Weapon", English="U_WinchesterM1907_Wep_Auto", Chinese="M1907 SL Sweeper", ShortTxt="M1907 SD" },
        new WeaponName(){ Class="Medic Weapon", English="U_WinchesterM1907", Chinese="M1907 SL Factory", ShortTxt="M1907 YC" },
        new WeaponName(){ Class="Medic Weapon", English="U_Mondragon_Wep_Range", Chinese="Mondragon Optical", ShortTxt="Mondragon MZJ" },
        new WeaponName(){ Class="Medic Weapon", English="U_Mondragon_Wep_Stability", Chinese="Mondragon Storm", ShortTxt="Mondragon CF" },
        new WeaponName(){ Class="Medic Weapon", English="U_Mondragon_Wep_Bipod", Chinese="Mondragon Sniper", ShortTxt="Mondragon JJS" },
        new WeaponName(){ Class="Medic Weapon", English="U_RemingtonModel8", Chinese="Autoloading 8.35 Factory", ShortTxt="8.35 YC" },
        new WeaponName(){ Class="Medic Weapon", English="U_RemingtonModel8_Wep_Scope", Chinese="Autoloading 8.35 Scope", ShortTxt="8.35 SSS" },
        new WeaponName(){ Class="Medic Weapon", English="U_RemingtonModel8_Wep_ExtendedMag", Chinese="Autoloading 8.25 Extended", ShortTxt="8.25 JC" },
        new WeaponName(){ Class="Medic Weapon", English="U_Luger1906", Chinese="Selbstlader 1906 Factory", ShortTxt="1906 YC" },
        new WeaponName(){ Class="Medic Weapon", English="U_Luger1906_Wep_Scope", Chinese="Selbstlader 1906 Sniper", ShortTxt="1906 JJS" },
        new WeaponName(){ Class="Medic Weapon", English="U_RSC1917_Wep_Range", Chinese="RSC 1917 Optical", ShortTxt="RSC 1917 MZJ" },
        new WeaponName(){ Class="Medic Weapon", English="U_RSC1917", Chinese="RSC 1917 Factory", ShortTxt="RSC 1917 YC" },
        new WeaponName(){ Class="Medic Weapon", English="U_FedorovAvtomat_Wep_Trench", Chinese="Federov Avtomat Trench", ShortTxt="Fedorov HGZ" },
        new WeaponName(){ Class="Medic Weapon", English="U_FedorovAvtomat_Wep_Range", Chinese="Federov Avtomat Optical", ShortTxt="Fedorov MZJ" },
        new WeaponName(){ Class="Medic Weapon", English="U_GeneralLiuRifle", Chinese="General Liu Rifle Factory", ShortTxt="GeneralLiu YC" },
        new WeaponName(){ Class="Medic Weapon", English="U_GeneralLiuRifle_Wep_Stability", Chinese="General Liu Rifle Storm", ShortTxt="GeneralLiu CF" },
        new WeaponName(){ Class="Medic Weapon", English="U_FarquharHill_Wep_Range", Chinese="Farquhar-Hill Optical", ShortTxt="Farquhar MZJ" },
        new WeaponName(){ Class="Medic Weapon", English="U_FarquharHill_Wep_Stability", Chinese="Farquhar-Hill Storm", ShortTxt="Farquhar CF" },
        new WeaponName(){ Class="Medic Weapon", English="U_BSAHowellM1916", Chinese="Howell Automatic Factory", ShortTxt="Howell YC" },
        new WeaponName(){ Class="Medic Weapon", English="U_BSAHowellM1916_Wep_Scope", Chinese="Howell Automatic Sniper", ShortTxt="Howell JJS" },
        new WeaponName(){ Class="Medic Weapon", English="U_FedorovDegtyarev", Chinese="Federov Degtyarev", ShortTxt="Fedorov SL" },

        // 医疗兵 配枪
        new WeaponName(){ Class="Medic Pistol", English="U_WebFosAutoRev_455Webley", Chinese="Auto Revolver", ShortTxt="Auto Rev" },
        new WeaponName(){ Class="Medic Pistol", English="U_MauserC96", Chinese="C96", ShortTxt="C96" },
        new WeaponName(){ Class="Medic Pistol", English="U_Mauser1914", Chinese="Taschenpistole M1914", ShortTxt="M1914" },

        // 医疗兵 配备一二
        new WeaponName(){ Class="Medic Gadget", English="U_Syringe", Chinese="Syringe", ShortTxt="Syringe" },
        new WeaponName(){ Class="Medic Gadget", English="U_MedicBag", Chinese="Medic Crate", ShortTxt="MedicBag" },
        new WeaponName(){ Class="Medic Gadget", English="U_Bandages", Chinese="Bandage Pouch", ShortTxt="Bandages" },
        new WeaponName(){ Class="Medic Gadget", English="_RGL_Frag", Chinese="Rifle Grenade - FRG", ShortTxt="RGL Frag" },
        new WeaponName(){ Class="Medic Gadget", English="_RGL_Smoke", Chinese="Rifle Grenade - SMK", ShortTxt="RGL Smoke" },
        new WeaponName(){ Class="Medic Gadget", English="_RGL_HE", Chinese="Rifle Grenade - HE", ShortTxt="RGL HE" },

        ////////////////////////////////// 支援兵 Support //////////////////////////////////

        // Support Weapon
        new WeaponName(){ Class="Support Weapon", English="U_LewisMG_Wep_Suppression", Chinese="Lewis Gun Suppressive", ShortTxt="LewisMG YZ" },
        new WeaponName(){ Class="Support Weapon", English="U_LewisMG_Wep_Range", Chinese="Lewis Gun Optical", ShortTxt="LewisMG MZJ" },
        new WeaponName(){ Class="Support Weapon", English="U_LewisMG", Chinese="Lewis Gun low Weight", ShortTxt="LewisMG QLH" },
        new WeaponName(){ Class="Support Weapon", English="U_HotchkissM1909_Wep_Stability", Chinese="M1909 Benet-Mercie Storm", ShortTxt="M1909 CF" },
        new WeaponName(){ Class="Support Weapon", English="U_HotchkissM1909_Wep_Range", Chinese="M1909 Benet-Mercie Optical", ShortTxt="M1909 MZJ" },
        new WeaponName(){ Class="Support Weapon", English="U_HotchkissM1909_Wep_Bipod", Chinese="M1909 Benet-Mercie Telescopic", ShortTxt="M1909 WYMJ" },
        new WeaponName(){ Class="Support Weapon", English="U_MadsenMG_Wep_Trench", Chinese="Madsen MG Trench", ShortTxt="MadsenMG HGZ" },
        new WeaponName(){ Class="Support Weapon", English="U_MadsenMG_Wep_Stability", Chinese="Madsen MS Storm", ShortTxt="MadsenMG CF" },
        new WeaponName(){ Class="Support Weapon", English="U_MadsenMG", Chinese="Madsen MG Low Weight", ShortTxt="MadsenMG QLH" },
        new WeaponName(){ Class="Support Weapon", English="U_Bergmann1915MG_Wep_Suppression", Chinese="MG15 n.A. Suppressive", ShortTxt="MG15 YZ" },
        new WeaponName(){ Class="Support Weapon", English="U_Bergmann1915MG_Wep_Stability", Chinese="MG15 n.A. Storm", ShortTxt="MG15 CF" },
        new WeaponName(){ Class="Support Weapon", English="U_Bergmann1915MG", Chinese="MG15 n.A. Low Weight", ShortTxt="MG15 QLH" },
        new WeaponName(){ Class="Support Weapon", English="U_BARM1918_Wep_Trench", Chinese="Bar M1918 Trench", ShortTxt="M1918 HGZ" },
        new WeaponName(){ Class="Support Weapon", English="U_BARM1918_Wep_Stability", Chinese="Bar M1918 Storm", ShortTxt="M1918 CF" },
        new WeaponName(){ Class="Support Weapon", English="U_BARM1918_Wep_Bipod", Chinese="Bar M1918 Telescopic", ShortTxt="M1918 WYMJ" },
        new WeaponName(){ Class="Support Weapon", English="U_HuotAutoRifle", Chinese="Huto Automatic Low Weight", ShortTxt="Huot QLH" },
        new WeaponName(){ Class="Support Weapon", English="U_HuotAutoRifle_Wep_Range", Chinese="Huot Automatic Optical", ShortTxt="Huot HGZ" },
        new WeaponName(){ Class="Support Weapon", English="U_Chauchat", Chinese="Chauchat Low Weight", ShortTxt="Chauchat QLH" },
        new WeaponName(){ Class="Support Weapon", English="U_Chauchat_Wep_Bipod", Chinese="Chauchat Telescopic", ShortTxt="Chauchat WYMJ" },
        new WeaponName(){ Class="Support Weapon", English="U_ParabellumLMG", Chinese="Parabellum MG14/17 Low Weight", ShortTxt="MG1417 QLH" },
        new WeaponName(){ Class="Support Weapon", English="U_ParabellumLMG_Wep_Suppression", Chinese="Parabellum MG14/17 Suppressive", ShortTxt="MG1417 YZ" },
        new WeaponName(){ Class="Support Weapon", English="U_PerinoM1908", Chinese="Perino Model 1908 Low Weight", ShortTxt="M1908 QLH" },
        new WeaponName(){ Class="Support Weapon", English="U_PerinoM1908_Wep_Defensive", Chinese="Perino Model 1908 Defensive", ShortTxt="M1908 FY" },
        new WeaponName(){ Class="Support Weapon", English="U_BrowningM1917", Chinese="M1917 MG Low Weight", ShortTxt="M1917 QLH" },
        new WeaponName(){ Class="Support Weapon", English="U_BrowningM1917_Wep_Suppression", Chinese="M1917 MG Suppressive", ShortTxt="M1917 WYMJ" },
        new WeaponName(){ Class="Support Weapon", English="U_MG0818", Chinese="IMG 08/18", ShortTxt="MG0818 QLH" },
        new WeaponName(){ Class="Support Weapon", English="U_MG0818_Wep_Defensive", Chinese="IMG 08/18 Suppressive", ShortTxt="MG0818 YZ" },
        new WeaponName(){ Class="Support Weapon", English="U_WinchesterBurton_Wep_Trench", Chinese="Burton LMR Trench", ShortTxt="Burton LMR ZH" },
        new WeaponName(){ Class="Support Weapon", English="U_WinchesterBurton_Wep_Optical", Chinese="Burton LMR Optical", ShortTxt="Burton LMR HZJ" },
        new WeaponName(){ Class="Support Weapon", English="U_MauserC96AutoPistol", Chinese="C96 Auto Pistol", ShortTxt="C96 KBQ" },
        new WeaponName(){ Class="Support Weapon", English="U_LugerArtillery", Chinese="P08 Artillerie", ShortTxt="P08 Artillerie" },
        new WeaponName(){ Class="Support Weapon", English="U_PieperCarbine", Chinese="Pieper M1893", ShortTxt="M1893" },
        new WeaponName(){ Class="Support Weapon", English="U_M1911_Stock", Chinese="M1911 Extended", ShortTxt="M1911 JC" },
        new WeaponName(){ Class="Support Weapon", English="U_FN1903stock", Chinese="Mle 1903 Extended", ShortTxt="Mle 1903 JC" },
        new WeaponName(){ Class="Support Weapon", English="U_C93Carbine", Chinese="C93 Carbine", ShortTxt="C93 KBQ" },

        // 支援兵 配枪
        new WeaponName(){ Class="Support Pistol", English="U_SteyrM1912", Chinese="Repetierpistole M1912", ShortTxt="M1912" },
        new WeaponName(){ Class="Support Pistol", English="U_Bulldog", Chinese="Bull Dog Revolver", ShortTxt="Bulldog" },
        new WeaponName(){ Class="Support Pistol", English="U_BerettaM1915", Chinese="Modello 1915", ShortTxt="Modello 1915" },

        // Support Gadget
        new WeaponName(){ Class="Support Gadget", English="U_AmmoCrate", Chinese="Ammo Crate", ShortTxt="Ammo Crate" },
        new WeaponName(){ Class="Support Gadget", English="U_AmmoPouch", Chinese="Ammo Pouch", ShortTxt="Ammo Pouch" },
        new WeaponName(){ Class="Support Gadget", English="U_Mortar", Chinese="Mortar - AIR", ShortTxt="Mortar KB" },
        new WeaponName(){ Class="Support Gadget", English="U_Mortar_HE", Chinese="Mortar -  HE", ShortTxt="Mortar GB" },
        new WeaponName(){ Class="Support Gadget", English="U_Wrench", Chinese="Repair Tool", ShortTxt="Wrench" },
        new WeaponName(){ Class="Support Gadget", English="U_LimpetMine", Chinese="Limpet Charge", ShortTxt="Limpet Mine" },
        new WeaponName(){ Class="Support Gadget", English="U_Crossbow", Chinese="Crossbow Launcher - FRG", ShortTxt="Crossbow PP" },
        new WeaponName(){ Class="Support Gadget", English="U_Crossbow_HE", Chinese="Crossbow Launcher - HE", ShortTxt="Crossbow GB" },

        ////////////////////////////////// 侦察兵 Scout   //////////////////////////////////

        // Scout Weapon
        new WeaponName(){ Class="Scout Weapon", English="U_WinchesterM1895_Wep_Trench", Chinese="Russian 1895 Trench", ShortTxt="1895 HGZ" },
        new WeaponName(){ Class="Scout Weapon", English="U_WinchesterM1895_Wep_Long", Chinese="Russian 1895 Sniper", ShortTxt="1895 JJS" },
        new WeaponName(){ Class="Scout Weapon", English="U_WinchesterM1895", Chinese="Russian 1895 Infantry", ShortTxt="1895 BB" },
        new WeaponName(){ Class="Scout Weapon", English="U_Gewehr98_Wep_Scope", Chinese="Gewehr 98 Marksman", ShortTxt="G98 SSS" },
        new WeaponName(){ Class="Scout Weapon", English="U_Gewehr98_Wep_LongRange", Chinese="Gewehr 98 Sniper", ShortTxt="G98 JJS" },
        new WeaponName(){ Class="Scout Weapon", English="U_Gewehr98", Chinese="Gewehr 98 Infantry", ShortTxt="G98 BB" },
        new WeaponName(){ Class="Scout Weapon", English="U_LeeEnfieldSMLE_Wep_Scope", Chinese="SMLE MKIII Marksman", ShortTxt="MKIII SSS" },
        new WeaponName(){ Class="Scout Weapon", English="U_LeeEnfieldSMLE_Wep_Med", Chinese="SMLE MKIII Carbine", ShortTxt="MKIII KBQ" },
        new WeaponName(){ Class="Scout Weapon", English="U_LeeEnfieldSMLE", Chinese="SMLE MKIII Infantry", ShortTxt="MKIII BB" },
        new WeaponName(){ Class="Scout Weapon", English="U_SteyrManM1895_Wep_Scope", Chinese="Gewehr M.95 Marksman", ShortTxt="G95 SSS" },
        new WeaponName(){ Class="Scout Weapon", English="U_SteyrManM1895_Wep_Med", Chinese="Gewehr M.95 Carbine", ShortTxt="G95 KBQ" },
        new WeaponName(){ Class="Scout Weapon", English="U_SteyrManM1895", Chinese="Gewehr M.95 Infantry", ShortTxt="G95 BB" },
        new WeaponName(){ Class="Scout Weapon", English="U_SpringfieldM1903_Wep_Scope", Chinese="M1903 Marksman", ShortTxt="M1903 SSS" },
        new WeaponName(){ Class="Scout Weapon", English="U_SpringfieldM1903_Wep_LongRange", Chinese="M1903 Carbine", ShortTxt="M1903 JJS" },
        new WeaponName(){ Class="Scout Weapon", English="U_SpringfieldM1903_Wep_Pedersen", Chinese="M1903 Infantry", ShortTxt="M1903 SY" },
        new WeaponName(){ Class="Scout Weapon", English="U_MartiniHenry", Chinese="Martini-Henry Infantry", ShortTxt="MartiniHenry BB" },
        new WeaponName(){ Class="Scout Weapon", English="U_MartiniHenry_Wep_LongRange", Chinese="Martini-Henry Sniper", ShortTxt="MartiniHenry JJS" },
        new WeaponName(){ Class="Scout Weapon", English="U_LeeEnfieldSMLE_Wep_Preorder", Chinese="Lawrance of Arabia's SMLE", ShortTxt="SMLE LLS" },
        new WeaponName(){ Class="Scout Weapon", English="U_Lebel1886_Wep_LongRange", Chinese="Lebel Model 1886 Sniper", ShortTxt="M1886 JJS" },
        new WeaponName(){ Class="Scout Weapon", English="U_Lebel1886", Chinese="Lebel Model 1886 Infantry", ShortTxt="M1886 BB" },
        new WeaponName(){ Class="Scout Weapon", English="U_MosinNagant1891", Chinese="Mosin-Nagant Infantry", ShortTxt="M91 BB" },
        new WeaponName(){ Class="Scout Weapon", English="U_MosinNagant1891_Wep_Scope", Chinese="Mosin-Nagant Marksman", ShortTxt="M91 SSS" },
        new WeaponName(){ Class="Scout Weapon", English="U_VetterliVitaliM1870", Chinese="Vetterli-Vitali M1870/87 Infantry", ShortTxt="M1870 BB" },
        new WeaponName(){ Class="Scout Weapon", English="U_VetterliVitaliM1870_Wep_Med", Chinese="Vetterli-Vitali M1870/87 Carbine", ShortTxt="M1870 KBQ" },
        new WeaponName(){ Class="Scout Weapon", English="U_Type38Arisaka", Chinese="Type 38 Arisaka Infantry", ShortTxt="Type38 BB" },
        new WeaponName(){ Class="Scout Weapon", English="U_Type38Arisaka_Wep_Scope", Chinese="Type 38 Arisaka Patrol", ShortTxt="Type38 XL" },
        new WeaponName(){ Class="Scout Weapon", English="U_CarcanoCarbine", Chinese="Carcano M91 Carbine", ShortTxt="M91 KBQ" },
        new WeaponName(){ Class="Scout Weapon", English="U_CarcanoCarbine_Wep_Scope", Chinese="Carcano M91 Patrol Carbine", ShortTxt="M91 KBQ XL" },
        new WeaponName(){ Class="Scout Weapon", English="U_RossMkIII", Chinese="Ross MKIII Infantry", ShortTxt="RossMkIII BB" },
        new WeaponName(){ Class="Scout Weapon", English="U_RossMkIII_Wep_Scope", Chinese="Ross MKIII Marksman", ShortTxt="RossMkIII SSS" },
        new WeaponName(){ Class="Scout Weapon", English="U_Enfield1917", Chinese="M1917 Enfield Infantry", ShortTxt="M1917 BB" },
        new WeaponName(){ Class="Scout Weapon", English="U_Enfield1917_Wep_LongRange", Chinese="M1917 Enfield Silenced", ShortTxt="M1917 XYQ" },

        // 侦察兵 配枪
        new WeaponName(){ Class="Scout Pistol", English="U_MarsAutoPistol", Chinese="Mars Automatic", ShortTxt="MarsAutoPistol" },
        new WeaponName(){ Class="Scout Pistol", English="U_Bodeo1889", Chinese="Bodeo 1889", ShortTxt="Bodeo 1889" },
        new WeaponName(){ Class="Scout Pistol", English="U_FrommerStop", Chinese="Frommer Stop", ShortTxt="Frommer Stop" },

        // Scout Gadget
        new WeaponName(){ Class="Scout Gadget", English="U_FlareGun", Chinese="Flare Gun - Spot", ShortTxt="Flare Gun ZC" },
        new WeaponName(){ Class="Scout Gadget", English="U_FlareGun_Flash", Chinese="Flare Gun - Flash", ShortTxt="Flare Gun SG" },
        new WeaponName(){ Class="Scout Gadget", English="U_TrPeriscope", Chinese="Trench Periscope", ShortTxt="Tr Periscope" },
        new WeaponName(){ Class="Scout Gadget", English="U_Shield", Chinese="Sniper Shield", ShortTxt="Shield" },
        new WeaponName(){ Class="Scout Gadget", English="U_HelmetDecoy", Chinese="Sniper Decoy", ShortTxt="Helmet Decoy" },
        new WeaponName(){ Class="Scout Gadget", English="U_TripWireBomb", Chinese="Tripwire Bomb - HE", ShortTxt="Trip Wire Bomb" },
        new WeaponName(){ Class="Scout Gadget", English="U_TripWireGas", Chinese="Tripwire Bomb - GAS", ShortTxt="Trip Wire Gas" },
        new WeaponName(){ Class="Scout Gadget", English="U_TripWireBurn", Chinese="Tripwire Bomb - INC", ShortTxt="Trip Wire Burn" },
        new WeaponName(){ Class="Scout Gadget", English="_KBullet", Chinese="K Bullets", ShortTxt="K Bullet" },

        /////////////////////////////////////////////////////////////////////////////

        // Elite Class
        new WeaponName(){ Class="Elite Class", English="U_MaximMG0815", Chinese="MG081 ELITE KIT", ShortTxt="Maxim MG0815" },
        new WeaponName(){ Class="Elite Class", English="U_VillarPerosa", Chinese="Villar Perosa (aka op shit gun)", ShortTxt="Villar Perosa" },
        new WeaponName(){ Class="Elite Class", English="U_FlameThrower", Chinese="Wex Flame Thrower", ShortTxt="Wex" },
        new WeaponName(){ Class="Elite Class", English="U_Incendiary_Hero", Chinese="Incendiary Hero", ShortTxt="Incendiary Hero" },
        new WeaponName(){ Class="Elite Class", English="U_RoyalClub", Chinese="Royal Club", ShortTxt="Royal Club" },
        new WeaponName(){ Class="Elite Class", English="U_MartiniGrenadeLauncher", Chinese="Martini Grenade Launcher", ShortTxt="Martini GL" },
        new WeaponName(){ Class="Elite Class", English="U_SawnOffShotgun_FK", Chinese="SawOff", ShortTxt="SawnOffShotgun" },
        new WeaponName(){ Class="Elite Class", English="U_FlareGun_Elite", Chinese="FlareGun Elite", ShortTxt="FlareGun Elite" },
        new WeaponName(){ Class="Elite Class", English="U_SpawnBeacon", Chinese="Spawn Beacon", ShortTxt="Spawn Beacon" },
        new WeaponName(){ Class="Elite Class", English="U_TankGewehr", Chinese="Tankgewehr M1918", ShortTxt="Tank Gewehr" },
        new WeaponName(){ Class="Elite Class", English="U_TrPeriscope_Elite", Chinese="Tr Periscope", ShortTxt="Tr Periscope" },
        new WeaponName(){ Class="Elite Class", English="U_ATGrenade_VhKit", Chinese="U ATGrenade VhKit", ShortTxt="AT Grenade" },

        ///////////////////////////////////////////////////////////////////////////////////

        
        new WeaponName(){ Class="Tank", English="ID_P_VNAME_MARKV", Chinese="Mark V Landship", ShortTxt="Mark V" },
        new WeaponName(){ Class="Tank", English="ID_P_VNAME_A7V", Chinese="AV7 Heavy Tank", ShortTxt="AV7" },
        new WeaponName(){ Class="Tank", English="ID_P_VNAME_FT17", Chinese="FT-17 Light-Tank", ShortTxt="FT-17" },
        new WeaponName(){ Class="Tank", English="ID_P_VNAME_ARTILLERYTRUCK", Chinese="Artillery Truck (VG BAN)", ShortTxt="ArtyTruck" },
        new WeaponName(){ Class="Tank", English="ID_P_VNAME_STCHAMOND", Chinese="STCHAMOND", ShortTxt="STCHAMOND" },
        new WeaponName(){ Class="Tank", English="ID_P_VNAME_ASSAULTTRUCK", Chinese="ASSAULTTRUCK putilov??", ShortTxt="ASSAULTTRUCK" },

        ////////////////
        
        new WeaponName(){ Class="Plane", English="ID_P_VNAME_HALBERSTADT", Chinese="HALBERSTADT", ShortTxt="HALBERSTADT" },
        new WeaponName(){ Class="Plane", English="ID_P_VNAME_RUMPLER", Chinese="RUMPLER", ShortTxt="RUMPLER" },
        new WeaponName(){ Class="Plane", English="ID_P_VNAME_BRISTOL", Chinese="BRISTOL", ShortTxt="BRISTOL" },
        new WeaponName(){ Class="Plane", English="ID_P_VNAME_SALMSON", Chinese="SALMSON", ShortTxt="SALMSON" },
        new WeaponName(){ Class="Plane", English="ID_P_VNAME_DH10", Chinese="Airco DH.10", ShortTxt="DH10" },
        new WeaponName(){ Class="Plane", English="ID_P_VNAME_HBG1", Chinese="HBG1", ShortTxt="HBG1" },
        new WeaponName(){ Class="Plane", English="ID_P_VNAME_CAPRONI", Chinese="CAPRONI CA.5", ShortTxt="CAPRONI" },
        new WeaponName(){ Class="Plane", English="ID_P_VNAME_GOTHA", Chinese="GOTHA", ShortTxt="GOTHA" },
        new WeaponName(){ Class="Plane", English="ID_P_VNAME_SOPWITH", Chinese="SOPWITH", ShortTxt="SOPWITH" },
        new WeaponName(){ Class="Plane", English="ID_P_VNAME_ALBATROS", Chinese="ALBATROS", ShortTxt="ALBATROS" },
        new WeaponName(){ Class="Plane", English="ID_P_VNAME_DR1", Chinese="DR.1", ShortTxt="DR1" },
        new WeaponName(){ Class="Plane", English="ID_P_VNAME_SPAD", Chinese="SPAD S XIII", ShortTxt="SPAD S XIII" },
        new WeaponName(){ Class="Plane", English="ID_P_VNAME_ILYAMUROMETS", Chinese="HEAVY BOMBER ILYA MUROMETS (VG BAN)", ShortTxt="Heavy Bomber" },
        new WeaponName(){ Class="Plane", English="ID_P_VNAME_ASTRATORRES", Chinese="ASTRATORRES", ShortTxt="ASTRATORRES" },

        ////////////////

        new WeaponName(){ Class="Ferry", English="ID_P_VNAME_HMS_LANCE", Chinese="HMS LANCE MARK V", ShortTxt="Mark V" },

        ////////////////
        
        new WeaponName(){ Class="Horse Abuser", English="ID_P_VNAME_HORSE", Chinese="HORSE", ShortTxt="HORSE" },

        ////////////////
        
        new WeaponName(){ Class="Vehicle-Infantry Weapon", English="U_WinchesterM1895_Horse", Chinese="Russian 1895 HORSE", ShortTxt="M1895 Horse" },
        new WeaponName(){ Class="Vehicle-Infantry Weapon", English="U_AmmoPouch_Cav", Chinese="Ammo Pouch", ShortTxt="Ammo Pouch" },
        new WeaponName(){ Class="Vehicle-Infantry Weapon", English="U_Bandages_Cav", Chinese="Bandages", ShortTxt="Bandages" },
        new WeaponName(){ Class="Vehicle-Infantry Weapon", English="U_Grenade_AT_Cavalry", Chinese="GRENADES AT CAVALRY", ShortTxt="Grenade AT" },
        new WeaponName(){ Class="Vehicle-Infantry Weapon", English="U_LugerP08_VhKit", Chinese="P08", ShortTxt="LugerP08 VhKit" },

        ////////////////
        
        new WeaponName(){ Class="Special", English="ID_P_INAME_U_MORTAR", Chinese="MORTAR", ShortTxt="MORTAR" },
        new WeaponName(){ Class="Special", English="ID_P_INAME_MORTAR_HE", Chinese="MORTAR HE", ShortTxt="MORTAR HE" },

        /////////////////////////////////////////////////////////////////////////////

        // transport vehicle
        new WeaponName(){ Class="transport vehicle", English="ID_P_VNAME_NSU", Chinese="transport vehicle MC 3.5HP ", ShortTxt="NSU" },
        new WeaponName(){ Class="transport vehicle", English="ID_P_VNAME_MOTORCYCLE", Chinese="transport vehicle MC 18J ", ShortTxt="MOTORCYCLE" },
        new WeaponName(){ Class="transport vehicle", English="ID_P_VNAME_EHRHARDT", Chinese="transport vehicle EV4 ", ShortTxt="EHRHARDT" },
        new WeaponName(){ Class="transport vehicle", English="ID_P_VNAME_ROMFELL", Chinese="transport vehicle Romfell ", ShortTxt="ROMFELL" },
        new WeaponName(){ Class="transport vehicle", English="ID_P_VNAME_ROLLS", Chinese="transport vehicle RNAS ", ShortTxt="ROLLS" },
        new WeaponName(){ Class="transport vehicle", English="ID_P_VNAME_MODEL30", Chinese="transport vehicle M30 ", ShortTxt="MODEL30" },
        new WeaponName(){ Class="transport vehicle", English="ID_P_VNAME_TERNI", Chinese="transport vehicle F.T. ", ShortTxt="TERNI" },
        new WeaponName(){ Class="transport vehicle", English="ID_P_VNAME_MERCEDES_37", Chinese="transport vehicle 37/95 ", ShortTxt="MERCEDES 37" },
        new WeaponName(){ Class="transport vehicle", English="ID_P_VNAME_BENZ_MG", Chinese="transport vehicle KFT ", ShortTxt="BENZ MG" },
        new WeaponName(){ Class="transport vehicle", English="ID_P_VNAME_MAS15", Chinese="transport vehicle M.A.S ", ShortTxt="MAS15" },
        new WeaponName(){ Class="transport vehicle", English="ID_P_VNAME_YLIGHTER", Chinese="transport vehicle Y-Lighter ", ShortTxt="MAS15" },

        /////////////////////////////////////////////////////////////////////////////

        // Static
        new WeaponName(){ Class="Static", English="ID_P_VNAME_BL9", Chinese="BL 9.2 ", ShortTxt="BL9" },
        new WeaponName(){ Class="Static", English="ID_P_VNAME_TURRET", Chinese="TURRET", ShortTxt="TURRET" },
        new WeaponName(){ Class="Static", English="ID_P_VNAME_AASTATION", Chinese="AAStation", ShortTxt="AASTATION" },
        new WeaponName(){ Class="Static", English="ID_P_VNAME_FIELDGUN", Chinese="Fieldgun FK 96", ShortTxt="FIELDGUN" },
        new WeaponName(){ Class="Static", English="ID_P_INAME_MAXIM", Chinese="MAXIM", ShortTxt="MAXIM" },
        new WeaponName(){ Class="Static", English="ID_P_VNAME_COASTALBATTERY", Chinese="COASTALBATTERY 350/52", ShortTxt="COASTALBATTERY" },
        new WeaponName(){ Class="Static", English="ID_P_VNAME_SK45GUN", Chinese="SK45GUN", ShortTxt="SK45GUN" },

        /////////////////////////////////////////////////////////////////////////////

        // Behemoth
        new WeaponName(){ Class="Behemoth", English="ID_P_VNAME_ARMOREDTRAIN", Chinese="ARMOREDTRAIN", ShortTxt="ARMOREDTRAIN" },
        new WeaponName(){ Class="Behemoth", English="ID_P_VNAME_ZEPPELIN", Chinese="ZEPPELIN", ShortTxt="ZEPPELIN" },
        new WeaponName(){ Class="Behemoth", English="ID_P_VNAME_IRONDUKE", Chinese="IRONDUKE", ShortTxt="IRONDUKE" },
        new WeaponName(){ Class="Behemoth", English="ID_P_VNAME_CHAR", Chinese="Char 2C", ShortTxt="CHAR" },

        /////////////////////////////////////////////////////////////////////////////
        
        // 近战
        new WeaponName(){ Class="Melee Weapon", English="U_GrenadeClub", Chinese="Grenade Club", ShortTxt="Grenade Club" },
        new WeaponName(){ Class="Melee Weapon", English="U_Club", Chinese="Club", ShortTxt="Club" },

        // 其他
        new WeaponName(){ Class="Other", English="U_GasMask", Chinese="Gas Mask", ShortTxt="Gas Mask" },
    };
}
