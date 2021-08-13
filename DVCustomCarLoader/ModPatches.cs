using System;
using System.Collections.Generic;
using System.Reflection;
using CCL_GameScripts;
using DVCustomCarLoader.LocoComponents;
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
                    Main.Log("Loco Lights traincar patch not found, skipping");
                }
            }
            catch( Exception ex )
            {
                Main.Log("Not creating Loco Lights patch");
#if DEBUG
                Main.ModEntry.Logger.LogException(ex);
#endif
            }
        }

        static bool Prefix( TrainCar car )
        {
            var simParams = car.gameObject.GetComponent<SimParamsBase>();
            if( simParams ) return false;
            return true;
        }
    }
}
