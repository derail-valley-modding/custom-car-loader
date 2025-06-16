using CCL.Importer.Types;
using DV;
using HarmonyLib;
using UnityEngine;

namespace CCL.Importer.Patches
{
    [HarmonyPatch(typeof(CommsRadioCrewVehicle))]
    internal class CommsRadioCrewVehiclePatches
    {
        [HarmonyPrefix, HarmonyPatch(nameof(CommsRadioCrewVehicle.SummonPrice), MethodType.Getter)]
        private static bool SummonPricePrefix(CommsRadioCrewVehicle __instance, ref float __result)
        {
            if (__instance.selectedCar.livery is not CCL_CarVariant livery) return true;

            __result = Mathf.Min(livery.SummonPrice, Globals.G.GameParams.WorkTrainSummonMaxPrice);
            return false;
        }
    }
}
