using System;
using BepInEx;
using GameNetcodeStuff;
using HarmonyLib;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace ShipTurrets.Patches
{

    [HarmonyPatch(typeof(Turret))]
    internal class TurretPatch
    {
        public static float viewRadius = 20f;
        public static float viewAngle = 180f;

        public static float missChance = 0.97f;

        private static Vector3 turretLocFront = new Vector3(8.90f, 8.19f, -14.09f);
        private static Vector3 turretLocRear = new Vector3(-5.04f, 6.40f, -14.12f);

        [HarmonyPatch("Update")]
        [HarmonyPrefix]
        public static void PatchUpdateBefore(ref Turret __instance)
        {
            //Check if turret is on ship for ship turret behaviour, otherwise skip.
            if(isShipTurret(__instance))
            {
                //Get private class variables.
                TurretMode turretModeLastFrame = (TurretMode)AccessTools.Field(typeof(Turret), "turretModeLastFrame").GetValue(__instance);

                //Check state and get enemies if detect or charging.
                List<EnemyAICollisionDetect> enemies;
                switch (turretModeLastFrame)
                {
                    case TurretMode.Detection:
                        enemies = GetEnemies(GetTargets(__instance));
                        if (enemies.Any())
                        {
                            __instance.turretMode = TurretMode.Charging;
                        }
                        break;
                    case TurretMode.Charging:
                        enemies = GetEnemies(GetTargets(__instance));
                        if (enemies.Any())
                        {
                            __instance.turretMode = TurretMode.Firing;
                        }
                        break;
                }
                //Return if turret not firing and beserk.
                if (__instance.turretMode != TurretMode.Firing && __instance.turretMode != TurretMode.Berserk)
                    return;
                //If still detecting enemies, turret bullets target enemies.
                enemies = GetEnemies(GetTargets(__instance));
                if (!TargetEnemies(__instance, enemies) && (turretModeLastFrame == TurretMode.Firing || turretModeLastFrame == TurretMode.Berserk))
                    __instance.turretMode = TurretMode.Detection;

                if(Plugin.isTurretFF.Value)
                {
                    if (__instance.CheckForPlayersInLineOfSight(3f) == GameNetworkManager.Instance.localPlayerController)
                    {
                        if (GameNetworkManager.Instance.localPlayerController.health - 34 > 0)
                        {
                            GameNetworkManager.Instance.localPlayerController.DamagePlayer(34, hasDamageSFX: true, callRPC: true, CauseOfDeath.Gunshots);
                        }
                        else
                        {
                            GameNetworkManager.Instance.localPlayerController.KillPlayer(__instance.aimPoint.forward * 40f, spawnBody: true, CauseOfDeath.Gunshots);
                        }
                    }
                }
                
                return;
            }
        }

        [HarmonyPatch("TurnTowardsTargetIfHasLOS")]
        [HarmonyPrefix]
        public static void PatchTurnTowardsTargetIfHasLOS(ref Turret __instance)
        {
            bool hasLineOfSight = (bool)AccessTools.Field(typeof(Turret), "hasLineOfSight").GetValue(__instance);
            float lostLOSTimer = (float)AccessTools.Field(typeof(Turret), "lostLOSTimer").GetValue(__instance);
            List<EnemyAICollisionDetect> enemies = GetEnemies(GetTargets(__instance));
            if (enemies.Any())
            {
                using List<EnemyAICollisionDetect>.Enumerator enumerator = enemies.GetEnumerator();
                if (enumerator.MoveNext())
                {
                    EnemyAICollisionDetect current = enumerator.Current;
                    hasLineOfSight = true;
                    lostLOSTimer = 0f;
                    AccessTools.Field(typeof(Turret), "hasLineOfSight").SetValue(__instance, true);
                    AccessTools.Field(typeof(Turret), "lostLOSTimer").SetValue(__instance, 0f);
                    __instance.tempTransform.position = ((Component)current.mainScript).transform.position;
                    Transform tempTransform = __instance.tempTransform;
                    tempTransform.position -= Vector3.up * 0.15f;
                    __instance.turnTowardsObjectCompass.LookAt(__instance.tempTransform);
                }
            }
            if (hasLineOfSight)
            {
                hasLineOfSight = false;
                lostLOSTimer = 0f;
                AccessTools.Field(typeof(Turret), "hasLineOfSight").SetValue(__instance, false);
                AccessTools.Field(typeof(Turret), "lostLOSTimer").SetValue(__instance, 0f);
            }
        }

        public static List<EnemyAICollisionDetect> GetTargets(Turret turret)
        {
            List<EnemyAICollisionDetect> retList = new List<EnemyAICollisionDetect>();

            //Collect list of colliders in Sphere and iterate over them.
            Collider[] array = Physics.OverlapSphere(turret.aimPoint.position, viewRadius);
            for (int i = 0; i < array.Length; i++)
            {
                Transform transform = ((Component)array[i]).transform;
                EnemyAICollisionDetect component = ((Component)transform).GetComponent<EnemyAICollisionDetect>();

                //If object is not null, add to list as visible target for further checks.
                if ((UnityEngine.Object)(object)component != (UnityEngine.Object)null)
                {
                    EnemyAICollisionDetect enemyComponent = ((Component)transform).GetComponent<EnemyAICollisionDetect>();
                    retList.Add(enemyComponent);
                }
            }
            return retList;
        }

        public static List<EnemyAICollisionDetect> GetEnemies(List<EnemyAICollisionDetect> targets)
        {
            //Get enemies and make checks they are valid for targeting.
            List<EnemyAICollisionDetect> retList = new List<EnemyAICollisionDetect>();
            targets.ForEach(delegate(EnemyAICollisionDetect target)
            {
                //Check if enemy is live.
                if (!target.mainScript.isEnemyDead)
                {
                    retList.Add(target);
                }
            });
            return retList;
        }

        public static bool TargetEnemies(Turret turret, List<EnemyAICollisionDetect> visibleEnemies)
        {
            //Temporary cheat to get turret to shoot enemies dead.
            bool ret = false;
            if(visibleEnemies.Count > 0)
            {
                //Random chance to kill creature to mimic multiple-shots/health.
                if(UnityEngine.Random.Range(0f, 1f) > missChance)
                {
                    //Shot hit the the enemy. Probably killed them.
                    visibleEnemies[0].mainScript.HitEnemyOnLocalClient(3);
                    Plugin.logger.LogInfo((object)(visibleEnemies[0].mainScript.enemyType.enemyName + " was hit."));
                    ret = true;
                }
                else
                {
                    //Shot missed the enemy.
                    ret = false;
                }
            }
            return ret;
        }

        private static bool isShipTurret(Turret turret)
        {
            //Check if location of turret is in expect ship turret locations.
            if(Vector3.Distance(turret.transform.position, turretLocFront) < 1.0 || Vector3.Distance(turret.transform.position, turretLocRear) < 1.0)
                return true;
            else
                return false;
        }
    }
}