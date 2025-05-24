using DV.ThingTypes;
using HarmonyLib;
using System.Collections.Generic;

namespace CCL.Importer.Patches
{
    [HarmonyPatch(typeof(CarTypes))]
    internal class CarTypesPatches
    {
        public static HashSet<string> SteamLocomotiveIds = new HashSet<string>();

        [HarmonyPostfix, HarmonyPatch(nameof(CarTypes.IsSteamLocomotive))]
        private static void IsSteamLocomotivePostfix(TrainCarLivery carLivery, ref bool __result)
        {
            if (__result) return;

            __result = SteamLocomotiveIds.Contains(carLivery.id);
        }
    }
}
