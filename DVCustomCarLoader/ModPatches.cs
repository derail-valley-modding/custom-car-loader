using System;
using System.Collections.Generic;
using System.Reflection;
using CCL_GameScripts;
using DVCustomCarLoader.LocoComponents;
using DVOwnership;
using HarmonyLib;

namespace DVCustomCarLoader
{
    //[HarmonyPatch(typeof(TrainCar), "InitAudio")]
    //public static class TrainCar_InitAudioPatch
    //{
    //    [HarmonyPriority(Priority.First)]
    //    static bool Prefix( TrainCar __instance )
    //    {
    //        var simParams = __instance.gameObject.GetComponent<SimParamsBase>();
    //        if( simParams ) return false;
    //        return true;
    //    }
    //}

    public static class LocoLights_Patch
    {
        public static void TryCreatePatch( Harmony harmony )
        {
            try
            {
                Type trainCarPatch = AccessTools.TypeByName("LocoLightsMod.TrainCar_Start_Patch");
                if( trainCarPatch != null )
                {
                    var target = AccessTools.Method(trainCarPatch, "DoCreate", new[] { typeof(TrainCar) });
                    var prefix = AccessTools.Method(typeof(LocoLights_Patch), "Prefix");

                    harmony.Patch(target, new HarmonyMethod(prefix));
                }
                else
                {
                    Main.LogAlways("Loco Lights traincar patch not found, skipping");
                }
            }
            catch( Exception ex )
            {
                Main.LogAlways("Not creating Loco Lights patch");
                Main.LogVerbose(ex.ToString());
            }
        }

        static bool Prefix( TrainCar car )
        {
            var simParams = car.gameObject.GetComponent<SimParamsBase>();
            if( simParams ) return false;
            return true;
        }
    }

    public static class Ownership_Patch
    {
        public static void TryCreatePatch( Harmony harmony )
        {
            try
            {
                Type EquipmentPurchaser = AccessTools.TypeByName("DVOwnership.CommsRadioEquipmentPurchaser");
                if (EquipmentPurchaser != null)
                {
                    var target = AccessTools.Method(EquipmentPurchaser, "GetAllCarTypes");
                    var postfix = AccessTools.Method(typeof(Ownership_Patch), "Postfix");

                    harmony.Patch(target, postfix: new HarmonyMethod(postfix));
                }
                else
                {
                    Main.LogAlways("Ownership comms radio equipment purchaser not found, skipping");
                }
            }
            catch (Exception ex)
            {
                Main.LogAlways("Not creating Ownership patch");
                Main.LogVerbose(ex.ToString());
            }
        }

        static void PostFix( ref IEnumerable<TrainCarType> __result )
        {
            foreach (var car in CustomCarManager.CustomCarTypes)
            {
                __result.AddItem(car.CarType);
            }
        }
    }
}
