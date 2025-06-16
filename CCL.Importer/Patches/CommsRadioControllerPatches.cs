using CCL.Importer.WorkTrains;
using DV;
using HarmonyLib;
using UnityEngine;

namespace CCL.Importer.Patches
{
    [HarmonyPatch(typeof(CommsRadioController))]
    internal class CommsRadioControllerPatches
    {
        [HarmonyPatch(nameof(CommsRadioController.Awake)), HarmonyPostfix]
        private static void AwakePostfix(CommsRadioController __instance)
        {
            // Create the object as inactive to prevent Awake() from running too early.
            var go = new GameObject(nameof(CommsRadioCCLWorkTrain));
            go.transform.parent = __instance.transform;
            go.SetActive(false);
            var mode = go.AddComponent<CommsRadioCCLWorkTrain>();
            mode.Controller = __instance;
            __instance.allModes.Add(mode);

            // Reactivate the GO with the new mode and refresh the controller.
            go.SetActive(true);
            __instance.ReactivateModes();
        }
    }
}
