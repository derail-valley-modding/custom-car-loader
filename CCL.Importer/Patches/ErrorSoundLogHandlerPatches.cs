using DV;
using HarmonyLib;

namespace CCL.Importer.Patches
{
    [HarmonyPatch(typeof(ErrorSoundLogHandler))]
    internal class ErrorSoundLogHandlerPatches
    {
        [HarmonyPostfix, HarmonyPatch(nameof(ErrorSoundLogHandler.Awake))]
        private static void AwakePostfix(ErrorSoundLogHandler __instance)
        {
            ErrorHandler.SoundHandler = __instance;
        }
    }
}
