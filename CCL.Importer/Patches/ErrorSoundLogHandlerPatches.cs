using DV;
using HarmonyLib;
using UnityEngine;

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

        [HarmonyPrefix, HarmonyPatch(nameof(ErrorSoundLogHandler.HandleLog))]
        private static void HandleLogPrefix(LogType type)
        {
            ErrorHandler.HandleUnityError(type);
        }
    }
}
