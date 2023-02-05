using HarmonyLib;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using UnityEngine;

namespace DVCustomCarLoader.Patches
{
    [HarmonyPatch]
    public static class IndicatorPatches
    {
        private static readonly MethodInfo _SetLight = AccessTools.Method(typeof(IndicatorEmission), "SetLight");

        private static void SafeSetRenderEmission(Renderer renderer, Color baseColor, float min, float max, float level)
        {
            if (renderer)
            {
                renderer.material.SetColor("_EmissionColor", baseColor * Mathf.Lerp(min, max, level));
            }
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(IndicatorEmission), "OnValueSet")]
        public static bool IndicatorEmission_OnValueSet(IndicatorEmission __instance, ref Renderer ___renderer,
            ref float ___smoothValue, ref float ___smoothVelo)
        {
            if (!Application.isPlaying)
            {
                return false;
            }

            float normalized = __instance.GetNormalizedValue(true);

            if (__instance.binary)
            {
                normalized = (__instance.value > __instance.valueThreshold) ? 1 : 0;
            }

            if (!___renderer)
            {
                ___renderer = __instance.GetComponentInChildren<Renderer>();
            }

            if (__instance.lag == 0f)
            {
                SafeSetRenderEmission(___renderer, __instance.emissionColor, __instance.minEmission, __instance.maxEmission, normalized);
                _SetLight.Invoke(__instance, new object[] { normalized });
            }
            else
            {
                ___smoothValue = Mathf.SmoothDamp(___smoothValue, normalized, ref ___smoothVelo, __instance.lag);
                SafeSetRenderEmission(___renderer, __instance.emissionColor, __instance.minEmission, __instance.maxEmission, ___smoothValue);
                _SetLight.Invoke(__instance, new object[] { ___smoothValue });
            }
            return false;
        }
    }
}
