using CCL.Importer.Types;
using CCL.Importer.WorkTrains;
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

            // First summon is free since CCL does not spawn the work train on purchase.
            if (!WorkTrainPurchaseHandler.HasBeenSummonedBefore(livery))
            {
                __result = 0;
                return false;
            }

            __result = Mathf.Min(livery.SummonPrice, Globals.G.GameParams.WorkTrainSummonMaxPrice);
            return false;
        }
    }
}
