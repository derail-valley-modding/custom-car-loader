using CCL.Importer.Types;
using DV.ThingTypes;
using DV.ThingTypes.TransitionHelpers;
using HarmonyLib;

namespace CCL.Importer.Patches
{
    [HarmonyPatch(typeof(CargoContent))]
    internal class CargoContentPatches
    {
        [HarmonyPostfix, HarmonyPatch(nameof(CargoContent.OnCargoLoaded))]
        private static void CargoLoadedPostfix(CargoContent __instance, CargoType cargoType)
        {
            // Don't do anything for non-CCL car types.
            if (__instance.trainCar.carLivery.parentType is not CCL_CarType car) return;

            var id = cargoType.ToV2().id;

            if (!car.CargoAmounts.TryGetValue(id, out var mult))
            {
                CCLPlugin.Error($"Could not get cargo '{id}' amount for car type {car.id}! Defaulting to 1.");
                return;
            }

            __instance.cargoMassFull = __instance.cargoMassCurrent *= mult;
        }
    }
}
