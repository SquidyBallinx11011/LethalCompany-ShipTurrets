using BepInEx;
using GameNetcodeStuff;
using HarmonyLib;
using UnityEngine;
using Unity.Netcode;

using TerminalApi;
using TerminalApi.Classes;
using static TerminalApi.Events.Events;
using static TerminalApi.TerminalApi;

namespace ShipTurrets.Patches
{
    [HarmonyPatch(typeof(Terminal))]

    internal class TerminalPatch : NetworkBehaviour
    {
        internal static GameObject mapPropsContainer;
        internal static int boughtTurret = 0;
        internal static int tempGroupCredits = 0;
        [ServerRpc(RequireOwnership = false)]
        [HarmonyPatch("Start")]
        [HarmonyPostfix]
        static void addCommands(ref Terminal __instance)
        {
            tempGroupCredits = __instance.groupCredits;
            if(Plugin.isTerminalHostBuy.Value)
            {
                //Add terminal creation for turrets.
                AddCommand("Front Turret", new CommandInfo()
                {
                    DisplayTextSupplier  = () =>
                    {
                        if(tempGroupCredits > 1000)
                        {
                            boughtTurret += 1;
                            Plugin.logger.LogWarning("Front Turret Enabled.");
                            Plugin.isFrontTurretSpawned.Value = true;
                            return "Front Turret Enabled";
                        }
                        else
                        {
                            Plugin.logger.LogWarning("Can't afford Front Turret.");
                            return "You do not have enough credits to purchase the Front Turret.";
                        }
                    },
                    Category = "store",
                    Description = "Buy the front turret for your ship. (Costs 1000)"
                });
                AddCommand("Rear Turret", new CommandInfo()
                {
                    DisplayTextSupplier  = () =>
                    {
                        if(tempGroupCredits > 1000)
                        {
                            boughtTurret += 1;
                            Plugin.logger.LogWarning("Rear Turret Enabled.");
                            Plugin.isRearTurretSpawned.Value = true;
                            return "Rear Turret Enabled";
                        }
                        else
                        {
                            Plugin.logger.LogWarning("Can't afford rear Turret.");
                            return "You do not have enough credits to purchase the rear Turret.";
                        }
                    },
                    Category = "store",
                    Description = "Buy the rear turret for your ship. (Costs 1000"
                });
            }
        }

        [HarmonyPatch("Update")]
        [HarmonyPostfix]
        static void PatchUpdate(ref Terminal __instance)
        {
            tempGroupCredits = __instance.groupCredits;
            if(boughtTurret > 0)
            {
                __instance.groupCredits -= 1000;
                boughtTurret -= 1;
            }
        }
    }

    
}
