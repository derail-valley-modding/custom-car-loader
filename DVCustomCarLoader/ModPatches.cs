using System;
using System.Collections.Generic;
using System.Reflection;
using CCL_GameScripts;
using DVCustomCarLoader.LocoComponents;
using HarmonyLib;

namespace DVCustomCarLoader
{
    [HarmonyPatch(typeof(TrainCar), "InitAudio")]
    public static class TrainCar_InitAudioPatch
    {
        [HarmonyPriority(Priority.First)]
        static bool Prefix( TrainCar __instance )
        {
            var simParams = __instance.gameObject.GetComponent<SimParamsBase>();
            if( simParams ) return false;
            return true;
        }
    }

    [HarmonyPatch]
    public static class LocoLights_Patch
    {
        static MethodBase TargetMethod()
        {
            Type trainCarPatch = AccessTools.TypeByName("TrainCar_Start_Patch");
            if( trainCarPatch != null )
            {
                return AccessTools.Method(trainCarPatch, "DoCreate", new[] { typeof(TrainCar) });
            }
            return null;
        }

        static bool Prefix( TrainCar car )
        {
            var simParams = car.gameObject.GetComponent<SimParamsBase>();
            if( simParams ) return false;
            return true;
        }
    }
}
