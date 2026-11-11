using DV;
using HarmonyLib;
using UnityEngine;

namespace CCL.Importer.Patches
{
    [HarmonyPatch(typeof(ErrorSoundLogHandler))]
    internal class ErrorSoundLogHandlerPatches
    {
        private static int s_last = -1;
        private static float s_total = -1;

        [HarmonyPostfix, HarmonyPatch(nameof(ErrorSoundLogHandler.Awake))]
        private static void AwakePostfix(ErrorSoundLogHandler __instance)
        {
            ErrorHandler.SoundHandler = __instance;
        }

        [HarmonyPrefix, HarmonyPatch(nameof(ErrorSoundLogHandler.HandleLog))]
        private static bool HandleLogPrefix(ErrorSoundLogHandler __instance, LogType type)
        {
            if (!CCLPlugin.Settings.UseMusicalPings) return true;

            if (ErrorSoundLogHandler.SoundEnabled && (type == LogType.Error || type == LogType.Exception))
            {
                var times = CCLPlugin.Settings.Timing;
                var notes = CCLPlugin.Settings.Notes;
                var speed = CCLPlugin.Settings.Tempo / 60.0f;

                if (s_total < 0)
                {
                    s_total = 0;

                    foreach (var item in times)
                    {
                        s_total += item;
                    }
                }

                var time = (Time.realtimeSinceStartup * speed) % s_total;
                int index = 0;

                while (time > times[index])
                {
                    time -= times[index];
                    index++;
                }

                if (s_last != index)
                {
                    __instance.sound.pitch = Mathf.Pow(2, notes[index] / 12.0f);
                    __instance.sound.Play();
                }

                s_last = index;
            }

            return false;
        }
    }
}
