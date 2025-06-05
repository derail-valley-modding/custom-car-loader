using DV.ThingTypes;
using HarmonyLib;
using System.Collections.Generic;
using UnityEngine;

namespace CCL.Importer.Patches
{
    [HarmonyPatch(typeof(CargoType_v2))]
    internal class CargoType_v2Patches
    {
        public static readonly Dictionary<CargoType_v2, Sprite> OriginalSprites = new();

        [HarmonyPostfix, HarmonyPatch(nameof(CargoType_v2.HasVisibleModelForCarType))]
        private static void HasVisibleModelForCarTypePostfix(CargoType_v2 __instance, TrainCarType_v2 carType)
        {
            // Store the original sprite for reset purposes.
            if (!OriginalSprites.ContainsKey(__instance))
            {
                OriginalSprites.Add(__instance, __instance.icon);
            }

            __instance.icon = CargoInjector.GetCargoIcon(__instance, carType);
        }

        public static void ResetAll()
        {
            foreach (var item in OriginalSprites)
            {
                item.Key.icon = item.Value;
            }
        }
    }
}
