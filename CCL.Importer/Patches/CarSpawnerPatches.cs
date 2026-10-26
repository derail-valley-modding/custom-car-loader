using HarmonyLib;
using System;
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

        [HarmonyFinalizer, HarmonyPatch(nameof(CarSpawner.BaseSpawn))]
        private static void BaseSpawnFinalizer(Exception __exception)
        {
            if (__exception != null)
            {
                ErrorHandler.TriggerOverride();
            }

            ErrorHandler.ClearOverride();
        }
    }
}
