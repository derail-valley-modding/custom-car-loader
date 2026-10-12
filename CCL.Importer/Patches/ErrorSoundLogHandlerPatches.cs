using DV;
using HarmonyLib;
using System.Collections.Generic;
using UnityEngine;

namespace CCL.Importer.Patches
{
    [HarmonyPatch(typeof(ErrorSoundLogHandler))]
    internal class ErrorSoundLogHandlerPatches
    {
        private const int ErrorCount = 10;
        private const float ErrorThreshold = 1.0f;

        private static bool s_reached = false;
        private static Queue<float> s_timeStamps = new();

        [HarmonyPostfix, HarmonyPatch(nameof(ErrorSoundLogHandler.Awake))]
        private static void AwakePostfix(ErrorSoundLogHandler __instance)
        {
            CCLPlugin.ErrorHandler = __instance;
        }

        [HarmonyPrefix, HarmonyPatch(nameof(ErrorSoundLogHandler.HandleLog))]
        private static void HandleLogPrefix(LogType type)
        {
            if (s_reached || CCLPlugin.Settings.SpamWarningDisabled) return;

            if (type == LogType.Error || type == LogType.Exception)
            {
                var current = Time.realtimeSinceStartup;

                if (s_timeStamps.Count > ErrorCount)
                {
                    var first = s_timeStamps.Dequeue();

                    if (current - first < ErrorThreshold)
                    {
                        s_reached = true;
                        CCLPlugin.AddStaticFailure("[Game] Error spam detected, one of your mods is probably broken\n" +
                            "If it's not a mod you're developing, you should upload the game's log to Altfuture's discord server");
                        ScriptForFailures.SetSpamFlag();
                    }
                }

                s_timeStamps.Enqueue(current);
            }
        }
    }
}
