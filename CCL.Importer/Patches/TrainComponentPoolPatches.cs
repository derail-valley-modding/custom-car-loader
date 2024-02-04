using HarmonyLib;

namespace CCL.Importer.Patches
{
    [HarmonyPatch(typeof(TrainComponentPool))]
    internal static class TrainComponentPoolPatches
    {
        [HarmonyPrefix]
        [HarmonyPatch(nameof(TrainComponentPool.GeneratePools))]
        public static void GeneratePools(TrainComponentPool __instance)
        {
            CarAudioProcessor.InjectAudioPrefabs(__instance);
        }
    }
}
