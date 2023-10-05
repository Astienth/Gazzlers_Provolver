using System;
using System.Linq;
using System.Threading;
using Banzai.GAZZLERS;
using BepInEx;
using BepInEx.Logging;
using HarmonyLib;

namespace ForceTube_Gazzlers
{
    [BepInPlugin("org.bepinex.plugins.ForceTube_Gazzlers", "Gazzlers protube integration", "1.0")]
    public class ForceTube_Gazzlers : BaseUnityPlugin
    {
        internal static ManualLogSource Log;

        public void Awake()
        {
            Log = base.Logger;
            InitializeProTube();
            // patch all functions
            var harmony = new Harmony("protube.patch.gazzlers");
            harmony.PatchAll();
        }


        private async void InitializeProTube()
        {
            Log?.LogMessage("Initializing ProTube gear...");
            await ForceTubeVRInterface.InitAsync(true);
            Thread.Sleep(10000);
        }

        [HarmonyPatch(typeof(Reloader), "Reload")]
        public class bhaptics_ReloadWeapon
        {
            [HarmonyPostfix]
            public static void Postfix(Reloader __instance)
            {
                ForceTubeVRInterface.Rumble(126, 20f, ForceTubeVRChannel.pistol1);
            }
        }

        [HarmonyPatch(typeof(PlayerWeapon), "OnShootEvent")]
        public class bhaptics_OnShootEvent
        {
            public static BarrelID[] Shorty = { BarrelID.Shorty_Common, BarrelID.Shorty_Rare, BarrelID.Shorty_Legendary };
            // shotgun equivalent I think
            public static BarrelID[] BoltBlaster = { BarrelID.BoltBlaster_Common, BarrelID.BoltBlaster_Rare, BarrelID.BoltBlaster_Legendary };
            // shoots bombs/mines
            public static BarrelID[] SparkPump = { BarrelID.SparkPump_Common, BarrelID.SparkPump_Rare, BarrelID.SparkPump_Legendary };
            // shoots fireballs
            public static BarrelID[] BlazeBaller = { BarrelID.BlazeBaller_Common, BarrelID.BlazeBaller_Rare, BarrelID.BlazeBaller_Legendary };
            // rockets
            public static BarrelID[] FuseBoxers = { BarrelID.FuseBox_Common, BarrelID.FuseBox_Rare, BarrelID.FuseBox_Legendary };
            //lasers
            public static BarrelID[] FryGuy = { BarrelID.FryGuy_Common, BarrelID.FryGuy_Rare, BarrelID.FryGuy_Legendary };

            [HarmonyPostfix]
            public static void Postfix(PlayerWeapon __instance)
            {
                BarrelID equippedBarrel = (BarrelID) Singleton<Player>.Instance.upgrades.equipedBarrel.item_id;
                
                if (Shorty.Contains(equippedBarrel))
                {
                    ForceTubeVRInterface.Kick(210, ForceTubeVRChannel.pistol1);
                    return;
                }
                if (BlazeBaller.Contains(equippedBarrel))
                {
                    ForceTubeVRInterface.Kick(240, ForceTubeVRChannel.pistol1);
                    return;
                }
                if (FryGuy.Contains(equippedBarrel))
                {
                    ForceTubeVRInterface.Rumble(220, 150f, ForceTubeVRChannel.pistol1);
                    return;
                }
                if (BoltBlaster.Contains(equippedBarrel))
                {
                    ForceTubeVRInterface.Shoot(230, 160, 50f, ForceTubeVRChannel.pistol1);
                    return;
                }
                if (FuseBoxers.Contains(equippedBarrel))
                {
                    ForceTubeVRInterface.Shoot(240, 230, 80f, ForceTubeVRChannel.pistol1);
                    return;
                }
                if (SparkPump.Contains(equippedBarrel))
                {
                    ForceTubeVRInterface.Shoot(210, 120, 40f, ForceTubeVRChannel.pistol1);
                    return;
                }

                //default
                ForceTubeVRInterface.Kick(210, ForceTubeVRChannel.pistol1);
            }
        }
    }
}

