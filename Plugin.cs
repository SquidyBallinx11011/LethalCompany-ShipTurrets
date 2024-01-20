using BepInEx;
using HarmonyLib;
using BepInEx.Logging;
using BepInEx.Configuration;

using ShipTurrets.Patches;

namespace ShipTurrets
{
    [BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    public class Plugin : BaseUnityPlugin
    {
        public static ManualLogSource logger;

        public static ConfigEntry<bool> isFrontTurretSpawned;
        public static ConfigEntry<bool> isRearTurretSpawned;
        public static ConfigEntry<bool> isTurretFF;
        private readonly Harmony harmony = new Harmony(PluginInfo.PLUGIN_GUID);
        private void Awake()
        {
            isFrontTurretSpawned = ((BaseUnityPlugin)this).Config.Bind<bool>("General", "Ship front turret spawn" , true, "Is the front turret spawned onto the player ship");
            isRearTurretSpawned = ((BaseUnityPlugin)this).Config.Bind<bool>("General", "Ship rear turret spawn", true, "Is the rear turret spawned onto the player ship");
            isTurretFF = ((BaseUnityPlugin)this).Config.Bind<bool>("General", "Ship turret friendly fire", false, "Is the turret friendly fire on (it shoots players & enemies)");
            // Plugin startup logic
            Logger.LogInfo($"Plugin {PluginInfo.PLUGIN_GUID} is loaded successfully");
            logger = base.Logger;

            harmony.PatchAll(typeof(Plugin));
            harmony.PatchAll(typeof(RoundManagerPatch));
            harmony.PatchAll(typeof(TurretPatch));
        }
    }
}
