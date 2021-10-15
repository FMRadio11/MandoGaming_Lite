using BepInEx;
using R2API.Utils;
using UnityEngine;
using R2API;
using R2API.Networking;

namespace MandoGamingLite
{
    [BepInDependency("com.bepis.r2api")]
    [BepInPlugin("com.FMRadio11.MGLite", "Mando Gaming (Lite)", "1.0.0")]
    [R2APISubmoduleDependency(nameof(EffectAPI), nameof(LanguageAPI), nameof(LoadoutAPI), nameof(PrefabAPI), nameof(ProjectileAPI), nameof(NetworkingHelpers))]
    [NetworkCompatibility(CompatibilityLevel.EveryoneMustHaveMod, VersionStrictness.EveryoneNeedSameModVersion)]
    public class MandoPlugin : BaseUnityPlugin
    {

        public void Awake()
        {
            MandoGaming.MandoLog = this.Logger;
            MandoGaming.MandoLog.LogInfo("Welcome to Mando Gaming Lite! If you want to customize values further, enable the custom config option then run this mod again to generate the needed file.");
            MandoGaming.MGLite();
        }
    }
}
