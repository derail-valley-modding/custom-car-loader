using HarmonyLib;
using UnityEngine;

namespace CCL.Importer.Patches
{
    [HarmonyPatch(typeof(IndicatorGaugeLagging))]
    internal class IndicatorGaugeLaggingPatches
    {
        [HarmonyPrefix, HarmonyPatch(nameof(IndicatorGaugeLagging.Update))]
        private static bool UpdatePrefix(IndicatorGaugeLagging __instance)
        {
            var dif = __instance.targetValue - __instance.value;

            if (!__instance.assumeIsPaused && Mathf.Abs(dif) > __instance.updateThreshold)
            {
                var smoothDif = Mathf.Clamp(dif / __instance.smoothTime * Time.deltaTime, -dif, dif);
                __instance.previousValue = __instance.value += smoothDif;
                __instance.SetNeedleRotation(__instance.value);
                __instance.FireValueChanged();
            }

            return false;
        }
    }
}
