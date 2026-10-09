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
        private const float ErrorThreshold = 0.75f;

        private static bool s_reached = false;
        private static Queue<float> s_timeStamps = new();

        [HarmonyPostfix, HarmonyPatch(nameof(ErrorSoundLogHandler.Awake))]
        private static void AwakePostfix(ErrorSoundLogHandler __instance)
        {
            CCLPlugin.ErrorHandler = __instance;
        }

        [HarmonyPrefix, HarmonyPatch(nameof(ErrorSoundLogHandler.HandleLog))]
        private static void HandleLogPrefix(ErrorSoundLogHandler __instance, LogType type)
        {
            if (s_reached) return;

            if (type == LogType.Error || type == LogType.Exception)
            {
                var current = Time.realtimeSinceStartup;

                if (s_timeStamps.Count > ErrorCount)
                {
                    var first = s_timeStamps.Dequeue();

                    if (current - first < ErrorThreshold)
                    {
                        s_reached = true;
                        CCLPlugin.AddStaticFailure("[Game] Error spam detected, one of your mods is probably very broken");

                        if (!ErrorSoundLogHandler.SoundEnabled)
                        {
                            PlaySound(__instance);
                        }
                    }
                }

                s_timeStamps.Enqueue(current);
            }
        }

        private static void PlaySound(ErrorSoundLogHandler handler)
        {
            float num = handler.pitchRandomizeRange * 0.5f;
            handler.sound.pitch = Random.Range(1f - num, 1f + num);
            handler.sound.Play();
        }
    }
}
