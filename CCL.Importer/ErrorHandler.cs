using DV;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

namespace CCL.Importer
{
    internal static class ErrorHandler
    {
        private const int ErrorCount = 10;
        private const float ErrorThreshold = 1.0f;

        private static bool s_reached = false;
        private static bool s_overrideErrorDetected = false;
        private static Queue<float> s_timeStamps = new();

        public static ErrorSoundLogHandler? SoundHandler { get; set; }
        public static string? Override { get; private set; }

        public static void HandleUnityError(LogType type)
        {
            if (s_reached || CCLPlugin.Settings.SpamWarningDisabled) return;

            if (type == LogType.Error || type == LogType.Exception)
            {
                if (CCLPlugin.Settings.UseDeepLogging)
                {
                    CCLPlugin.Log(new StackTrace(2, true).ToString());
                }

                if (string.IsNullOrEmpty(Override))
                {
                    RegularCheck();
                }
                else
                {
                    TriggerOverride();
                }
            }
        }

        private static void RegularCheck()
        {
            var current = Time.realtimeSinceStartup;

            if (s_timeStamps.Count > ErrorCount)
            {
                var first = s_timeStamps.Dequeue();

                if (current - first < ErrorThreshold)
                {
                    s_reached = true;
                    AddStaticFailure("[Game] Error spam detected, one of your mods is probably broken\n" +
                        "If it's not a mod you're developing, you should upload the game's log to Altfuture's discord server");
                    ScriptForFailures.SetSpamFlag();
                }
            }

            s_timeStamps.Enqueue(current);
        }

        public static void OverrideErrorContext(string name)
        {
            ClearOverride();
            Override = name;
        }

        public static void ClearOverride()
        {
            if (!string.IsNullOrEmpty(Override) && s_overrideErrorDetected)
            {
                AddContextualFailure($"{Override} failure(s) detected");
                CCLPlugin.Warning($"Error Override: {Override}");
            }

            Override = null;
        }

        public static void TriggerOverride()
        {
            s_overrideErrorDetected = true;
        }

        public static void AddLoadFailure(string message)
        {
            ScriptForFailures.LoadFailures.Add(message);
            ObjectHelper.CreateFailuresHolder();
        }

        public static void AddStaticFailure(string message)
        {
            ScriptForFailures.StaticFailures.Add(message);
            ObjectHelper.CreateFailuresHolder();
        }

        public static void AddContextualFailure(string message)
        {
            ScriptForFailures.AddRepeatableFailure(message);

            if (CCLPlugin.Settings.AlwaysShowPopups)
            {
                ObjectHelper.CreateFailuresHolder();
            }
        }
    }
}
