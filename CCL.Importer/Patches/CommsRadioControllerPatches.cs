using CCL.Importer.Types;
using CCL.Importer.WorkTrains;
using DV;
using HarmonyLib;
using System.Linq;
using UnityEngine;

namespace CCL.Importer.Patches
{
    [HarmonyPatch(typeof(CommsRadioController))]
    internal class CommsRadioControllerPatches
    {
        [HarmonyPostfix, HarmonyPatch(nameof(CommsRadioController.Awake))]
        private static void AwakePostfix(CommsRadioController __instance)
        {
            // Create the object as inactive to prevent Awake() from running too early.
            var go = new GameObject(nameof(CommsRadioCCLWorkTrain));
            go.transform.parent = __instance.transform;
            go.SetActive(false);
            var mode = go.AddComponent<CommsRadioCCLWorkTrain>();
            mode.Controller = __instance;
            __instance.allModes.Add(mode);

            // Reactivate the GO.
            go.SetActive(true);

            __instance.crewVehicleControl.CarSummoned += OnCarSummoned;
        }

        private static void OnCarSummoned(TrainCar car)
        {
            if (car == null || car.carLivery is not CCL_CarVariant livery) return;

            WorkTrainPurchaseHandler.SetAsSummoned(livery);
        }

        [HarmonyPostfix, HarmonyPatch(nameof(CommsRadioController.UpdateModesAvailability))]
        private static void UpdateModesAvailabilityPostfix(CommsRadioController __instance)
        {
            var mode = __instance.allModes.OfType<CommsRadioCCLWorkTrain>().FirstOrDefault();

            if (mode == null) return;

            var index = __instance.allModes.IndexOf(mode);
            __instance.disabledModeIndices.Remove(index);

            // Match mode availability with the default work train mode.
            if (__instance.disabledModeIndices.Contains(__instance.allModes.IndexOf(__instance.crewVehicleControl)))
            {
                __instance.disabledModeIndices.Add(index);

                if (__instance.activeModeIndex == index)
                {
                    __instance.SetNextMode();
                }
            }
        }
    }
}
