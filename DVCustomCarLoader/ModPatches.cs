using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using CCL_GameScripts;
using DV.Simulation.Brake;
using HarmonyLib;
using UnityEngine;
using UnityModManagerNet;

namespace DVCustomCarLoader
{
    public class CoroutineRunner : MonoBehaviour { }

    internal static class ModLoadWaiter
    {
        private static CoroutineRunner _runner;

        public static void Initialize()
        {
            Main.LogVerbose("Init CoroRunner");
            var runObj = new GameObject("CoroRunner");
            GameObject.DontDestroyOnLoad(runObj);
            _runner = runObj.AddComponent<CoroutineRunner>();
        }

        public static void AddWaiter(string modId, Action onLoad, float maxWait = 5)
        {
            if (!_runner) Initialize();

            _runner.StartCoroutine(WaitForModLoaded(modId, onLoad, maxWait));
        }

        private static IEnumerator WaitForModLoaded(string modId, Action onLoad, float maxWait)
        {
            float startTime = Time.realtimeSinceStartup;
            var modEntry = UnityModManager.FindMod(modId);

            if (modEntry == null)
            {
                Main.LogAlways($"Didn't find mod {modId} to wait for, ignoring");
                yield break;
            }

            Main.LogVerbose("Awaiting " + modId);

            do
            {
                if (modEntry.Active)
                {
                    onLoad?.Invoke();
                    yield break;
                }

                yield return null;
            }
            while (true && ((Time.realtimeSinceStartup - startTime) <= maxWait));
        }
    }

    public static class LocoLights_Patch
    {
        public static void TryCreatePatch( Harmony harmony )
        {
            try
            {
                Type trainCarPatch = AccessTools.TypeByName("LocoLightsMod.TrainCar_Start_Patch");
                if( trainCarPatch != null )
                {
                    var target = AccessTools.Method(trainCarPatch, "DoCreate", new[] { typeof(TrainCar) });
                    var prefix = AccessTools.Method(typeof(LocoLights_Patch), "Prefix");

                    harmony.Patch(target, new HarmonyMethod(prefix));
                }
                else
                {
                    Main.LogAlways("Loco Lights traincar patch not found, skipping");
                }
            }
            catch( Exception ex )
            {
                Main.LogAlways("Not creating Loco Lights patch");
                Main.LogVerbose(ex.ToString());
            }
        }

        static bool Prefix( TrainCar car )
        {
            var simParams = car.gameObject.GetComponent<SimParamsBase>();
            if( simParams ) return false;
            return true;
        }
    }

    // Zeibach's Air Brake mod
    public static class AirBrake_Patch
    {
        public static float MaxMainReservoirPressure = BrakeSystemConsts.MAX_MAIN_RES_PRESSURE;

        public static void TryCreatePatch()
        {
            try
            {
                Type constants = AccessTools.TypeByName("DvMod.AirBrake.Constants");
                if (constants != null)
                {
                    var maxPressureField = AccessTools.Field(constants, "MaxMainReservoirPressure");
                    MaxMainReservoirPressure = (float)maxPressureField.GetValue(null);
                }
                else
                {
                    Main.LogAlways("Air Brake Mod not found, skipping");
                }
            }
            catch (Exception ex)
            {
                Main.LogAlways("Not creating Air Brake Mod patch");
                Main.LogVerbose(ex.ToString());
            }
        }
    }
}
