using HarmonyLib;
using LocoSim.Implementations;
using UnityEngine;

namespace CCL.Importer.Patches
{
    [HarmonyPatch(typeof(Boiler))]
    internal class BoilerPatches
    {
        [HarmonyPrefix, HarmonyPatch("OnBoilerWaterChangeRequested")]
        private static bool OnBoilerWaterChangeRequestedPrefix(ref Boiler __instance, float instantBoilerWaterChangeRequest,
            ref WaterPressureVessel ___vessel, float ___totalVolume, float ___spawnWaterMass, float ___safetyValveOpeningPressure)
        {
            if (instantBoilerWaterChangeRequest != 3f) return true;

            ___vessel = new WaterPressureVessel(___totalVolume, CalculatePressure(___safetyValveOpeningPressure), ___spawnWaterMass);
            __instance.waterMassReadOut.Value = ___vessel.mass;
            __instance.waterChangeRequestedExtIn.Value = 0f;

            return false;
        }

        private static float CalculatePressure(float maxPressure)
        {
            return Mathf.Max(0.1f, Mathf.Floor(maxPressure - 1.999f));
        }
    }
}
