using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using CCL.Types;
using DV.JObjectExtstensions;
using DV.ThingTypes;
using HarmonyLib;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace CCL.Importer.Patches
{
    static class SaveConstants
    {
        public const string CUSTOM_CAR_KEY = "customcar";
        public const string CUSTOM_CARGO_KEY = "customcargo";
    }

    [HarmonyPatch(typeof(CarsSaveManager))]
    public static class CarsSaveManagerPatch
    {
        [HarmonyPostfix]
        [HarmonyPatch("GetCarSaveData")]
        public static void AugmentCarSaveData(TrainCar car, ref JObject __result)
        {
            if (car.carLivery is CustomLivery customCar)
            {
                __result.SetString(SaveConstants.CUSTOM_CAR_KEY, customCar.id);
            }

            //if (CustomCargoInjector.TryGetCustomCargoByType(car.LoadedCargo, out var customCargo))
            //{
            //    __result.SetString(SaveConstants.CUSTOM_CARGO_KEY, customCargo.Identifier);
            //    __result.SetInt("loadedCargo", (int)CargoType.None);
            //}
        }

        private static readonly MethodInfo trainCarGetPrefab = AccessTools.Method(typeof(TrainCar), nameof(TrainCar.GetCarPrefab));

        [HarmonyTranspiler]
        [HarmonyPatch("InstantiateCar")]
        public static IEnumerable<CodeInstruction> TranspileInstantiateCar(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            // arg 0 - JObject carData

            Label? branchLabel = null;
            foreach (var instruction in instructions)
            {
                if (instruction.Calls(trainCarGetPrefab))
                {
                    // prefab = CarsSaveManagerPatch.GetCarPrefab(carType, carData);
                    yield return new CodeInstruction(OpCodes.Ldarg_0);
                    yield return CodeInstruction.Call(typeof(CarsSaveManagerPatch), nameof(GetCarPrefab));
                    yield return new CodeInstruction(OpCodes.Dup);

                    // if (prefab == null) return null;
                    branchLabel = generator.DefineLabel();
                    yield return new CodeInstruction(OpCodes.Brtrue_S, branchLabel);
                    yield return new CodeInstruction(OpCodes.Ret);
                }
                else
                {
                    if (branchLabel.HasValue)
                    {
                        instruction.labels.Add(branchLabel.Value);
                        branchLabel = null;
                    }

                    yield return instruction;
                }
            }
        }

        private static GameObject? GetCarPrefab(TrainCarType carType, JObject carData)
        {
            if (carType != TrainCarType.NotSet)
            {
                return TrainCar.GetCarPrefab(carType);
            }

            string carId = carData.GetString(SaveConstants.CUSTOM_CAR_KEY);
            if (carId != null && CarTypeInjector.IdToLiveryMap.TryGetValue(carId, out CustomLivery livery))
            {
                return livery.prefab;
            }
            else
            {
                CCLPlugin.Warning($"Encountered car that is neither base game or custom... somehow");
                return null;
            }
        }
    }

    /*
    [HarmonyPatch]
    public static class JobDataCargoPatches
    {
        private const string OBJECT_TYPE_KEY = "$type";
        private const string ARRAY_VALUES_KEY = "$values";
        private const string CUSTOM_CARGO_KEY = "customCargoPerCar";

        private const string JOB_CHAINS_KEY = "jobChains";
        private const string JOB_DEFINITIONS_KEY = "jobChainData";

        private const string TRANSPORT_CARGO_PER_CAR_KEY = "transportedCargoPerCar";

        private static JObject[] GetTypedObjectArray(this JObject parent, string key)
        {
            return parent?.GetJObject(key)?.GetJObjectArray(ARRAY_VALUES_KEY);
        }

        private static int[] GetTypedIntArray(this JObject parent, string key)
        {
            return parent?.GetJObject(key)?.GetIntArray(ARRAY_VALUES_KEY);
        }

        #region Save Data

        [HarmonyPatch(typeof(SaveGameManager), "DoSaveIO")]
        [HarmonyPrefix]
        public static void AdjustSavedJobCargo(SaveGameData data)
        {
            string serializedJobs = data.GetString(SaveGameKeys.Jobs);
            if (string.IsNullOrEmpty(serializedJobs)) return;

            //Main.LogVerbose(serializedJobs);

            var jobData = JObject.Parse(serializedJobs);
            var jobChains = jobData.GetTypedObjectArray(JOB_CHAINS_KEY);
            if (jobChains == null || jobChains.Length == 0) return;

            foreach (var chainJson in jobChains)
            {
                var jobDefinitions = chainJson.GetTypedObjectArray(JOB_DEFINITIONS_KEY);

                foreach (var job in jobDefinitions)
                {
                    string jobType = job.GetString(OBJECT_TYPE_KEY);

                    // transport job
                    if (jobType.Contains(nameof(TransportJobDefinitionData)))
                    {
                        SaveTransportJobCustomCargo(job);
                    }
                }
            }

            data.SetString(SaveGameKeys.Jobs, jobData.ToString());
        }

        private static void SaveTransportJobCustomCargo(JObject job)
        {
            var transportedCargoPerCar = job.GetTypedIntArray(TRANSPORT_CARGO_PER_CAR_KEY);
            if (transportedCargoPerCar != null)
            {
                var customCargos = transportedCargoPerCar.Select(
                    cid =>
                    {
                        if (CustomCargoInjector.TryGetCustomCargoByType((CargoType)cid, out CustomCargo cargo))
                        {
                            return cargo.Identifier;
                        }
                        else
                        {
                            return "";
                        }
                    }).ToArray();

                job.SetStringArray("customCargoPerCar", customCargos);
            }
        }

        #endregion

        #region Load Data

        private static WrappedEnumerator loadRoutineWrapper = null;

        [HarmonyPatch(typeof(SaveLoadController), "Start")]
        [HarmonyPostfix]
        public static void PatchLoadControllerStartMethod(ref System.Collections.IEnumerator __result)
        {
            loadRoutineWrapper = new WrappedEnumerator(__result);
            loadRoutineWrapper.OnMoveNext += AdjustLoadedJobCargo;
            __result = loadRoutineWrapper;
        }

        private static void AdjustLoadedJobCargo()
        {
            if (!LogicController.Instance.initialized)
            {
                return;
            }

            loadRoutineWrapper.OnMoveNext -= AdjustLoadedJobCargo;

            string serializedJobs = SaveGameManager.data?.GetString(SaveGameKeys.Jobs);
            if (!string.IsNullOrEmpty(serializedJobs))
            {
                //Main.LogVerbose(serializedJobs);
                var jobData = JObject.Parse(serializedJobs);
                var jobChains = jobData.GetTypedObjectArray(JOB_CHAINS_KEY);
                if (jobChains != null && jobChains.Length > 0)
                {
                    foreach (var jobChain in jobChains)
                    {
                        var jobDefinitions = jobChain.GetTypedObjectArray(JOB_DEFINITIONS_KEY);
                        if (jobDefinitions != null && jobDefinitions.Length > 0)
                        {
                            foreach (var job in jobDefinitions)
                            {
                                var customCargos = job.GetStringArray(CUSTOM_CARGO_KEY);
                                if (customCargos == null) continue;

                                string jobType = job.GetString(OBJECT_TYPE_KEY);

                                // transport job
                                if (jobType.Contains(nameof(TransportJobDefinitionData)))
                                {
                                    LoadTransportJobCustomCargo(job, customCargos);
                                }
                            }
                        }
                    }

                    SaveGameManager.data.SetString(SaveGameKeys.Jobs, jobData.ToString());
                }
            }
        }

        private static void LoadTransportJobCustomCargo(JObject job, string[] customCargos)
        {
            var cargoPerCarObject = job.GetJObject(TRANSPORT_CARGO_PER_CAR_KEY);
            var transportedCargoPerCar = cargoPerCarObject?.GetIntArray(ARRAY_VALUES_KEY);
            if (transportedCargoPerCar != null)
            {
                for (int i = 0; i < transportedCargoPerCar.Length; i++)
                {
                    if (!string.IsNullOrEmpty(customCargos[i]))
                    {
                        if (CustomCargoInjector.TryGetCargoTypeById(customCargos[i], out CargoType cargo))
                        {
                            transportedCargoPerCar[i] = (int)cargo;
                        }
                        else
                        {
                            transportedCargoPerCar[i] = (int)CargoType.None;
                            Main.Warning($"Unknown custom cargo \"{customCargos[i]}\" when loading job");
                        }
                    }
                }

                cargoPerCarObject.SetIntArray(ARRAY_VALUES_KEY, transportedCargoPerCar);
            }
        }

        #endregion
    }
    */
}
