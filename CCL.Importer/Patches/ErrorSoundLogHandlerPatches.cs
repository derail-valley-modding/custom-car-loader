using DV;
using HarmonyLib;
using UnityEngine;

namespace CCL.Importer.Patches
{
    [HarmonyPatch(typeof(ErrorSoundLogHandler))]
    internal class ErrorSoundLogHandlerPatches
    {
        private const int ErrorThreshold = 25;

        private static int s_errorCount = 0;
        private static bool s_reached = false;

        [HarmonyPostfix, HarmonyPatch(nameof(ErrorSoundLogHandler.Awake))]
        private static void AwakePostfix(ErrorSoundLogHandler __instance)
        {
            CCLPlugin.ErrorHandler = __instance;
        }

        [HarmonyPrefix, HarmonyPatch(nameof(ErrorSoundLogHandler.HandleLog))]
        private static void HandleLogPrefix(LogType type)
        {
            if (s_reached || ErrorSoundLogHandler.SoundEnabled) return;

            if (type == LogType.Error || type == LogType.Exception)
            {
                s_errorCount++;

                if (s_errorCount > ErrorThreshold)
                {
                    CCLPlugin.Warning("Reached error threshold, enabling sound pings again");
                    ErrorSoundLogHandler.SoundEnabled = true;
                    s_reached = true;
                }
            }
        }
    }
}
