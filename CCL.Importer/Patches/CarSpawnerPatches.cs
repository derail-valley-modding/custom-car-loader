using HarmonyLib;
using UnityEngine;

namespace CCL.Importer.Patches
{
    [HarmonyPatch(typeof(CarSpawner))]
    internal class CarSpawnerPatches
    {
        [HarmonyPrefix, HarmonyPatch(nameof(CarSpawner.BaseSpawn))]
        private static void BaseSpawnPrefix(GameObject carToSpawn)
        {
            var car = carToSpawn.GetComponentInChildren<TrainCar>();

            if (car != null)
            {
                ErrorHandler.OverrideErrorContext($"{car.carLivery.id} spawn");
            }
            else
            {
                ErrorHandler.OverrideErrorContext($"UNKNOWN spawn");
            }
        }

        [HarmonyPostfix, HarmonyPatch(nameof(CarSpawner.BaseSpawn))]
        private static void BaseSpawnPostfix()
        {
            ErrorHandler.ClearOverride();
        }
    }
}
