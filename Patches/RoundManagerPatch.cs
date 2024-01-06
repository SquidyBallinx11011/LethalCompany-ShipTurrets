using BepInEx;
using GameNetcodeStuff;
using HarmonyLib;
using UnityEngine;
using Unity.Netcode;

namespace ShipTurrets.Patches
{
    [HarmonyPatch(typeof(RoundManager))]
    internal class RoundManagerPatch
    {
        internal static GameObject mapPropsContainer;
        [HarmonyPatch("SpawnMapObjects")]
        [HarmonyPostfix]
        static void spawnTurret(ref RoundManager __instance)
        {
            mapPropsContainer = GameObject.FindGameObjectWithTag("MapPropsContainer");
            Vector3 LocationInfo = ((Component)__instance.playersManager.localPlayerController).transform.position;
            Vector3 turretLocFront = new Vector3(8.894f, 7.2597f, -14.0808f);
            Vector3 turretLocRear = new Vector3(-5.0311f, 5.4649f, -14.1322f);
            foreach (SpawnableMapObject obj in __instance.currentLevel.spawnableMapObjects)
            {
                if (obj.prefabToSpawn.GetComponentInChildren<Turret>() == null) continue;

                //Spawn ship's front turret.
                var shipTurretFront = UnityEngine.Object.Instantiate<GameObject>(obj.prefabToSpawn, turretLocFront, Quaternion.identity, mapPropsContainer.transform);
                shipTurretFront.transform.position = turretLocFront;
                shipTurretFront.transform.forward = new Vector3(1, 0, 0);
                shipTurretFront.GetComponent<NetworkObject>().Spawn(true);
                
                //Spawn ship's rear turret.
                var shipTurretRear = UnityEngine.Object.Instantiate<GameObject>(obj.prefabToSpawn, turretLocRear, Quaternion.identity, mapPropsContainer.transform);
                shipTurretRear.transform.position = turretLocRear;
                shipTurretRear.transform.forward = new Vector3(-1, 0, 0);
                shipTurretRear.GetComponent<NetworkObject>().Spawn(true);
            }


        }
    }
}
