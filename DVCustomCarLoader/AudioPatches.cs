using System;
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
}
