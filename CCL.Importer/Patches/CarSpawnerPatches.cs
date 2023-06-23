using CCL.Types;
using HarmonyLib;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace CCL.Importer.Patches
{
    [HarmonyPatch(typeof(CarSpawner))]
    public static class CarSpawnerPatches
    {
        private static TrainCar GetDisabledTrainCar(GameObject prefab)
        {
            return prefab.GetComponentsInChildren<TrainCar>(true).FirstOrDefault();
        }

        [HarmonyTranspiler]
        [HarmonyPatch(nameof(CarSpawner.GetFromPool))]
        public static IEnumerable<CodeInstruction> TranspileGetFromPool(IEnumerable<CodeInstruction> instructions)
        {
            MethodInfo getcomponentMethod = AccessTools.Method(typeof(GameObject), nameof(GameObject.GetComponent), generics: new[] { typeof(TrainCar) });

            foreach (var instruction in instructions)
            {
                if (instruction.Calls(getcomponentMethod))
                {
                    yield return CodeInstruction.Call(typeof(CarSpawnerPatches), nameof(GetDisabledTrainCar));
                }
                else
                {
                    yield return instruction;
                }
            }
        }

        private static GameObject EnableCar(GameObject carObj)
        {
            if (!carObj.activeSelf)
            {
                carObj.SetActive(true);
            }
            return carObj;
        }

        [HarmonyTranspiler]
        [HarmonyPatch(nameof(CarSpawner.SpawnCar))]
        [HarmonyPatch(nameof(CarSpawner.SpawnLoadedCar))]
        public static IEnumerable<CodeInstruction> TranspileSpawnCar(IEnumerable<CodeInstruction> instructions)
        {
            MethodInfo getComponentInChildren = AccessTools.Method(typeof(GameObject), nameof(GameObject.GetComponentInChildren), generics: new[] { typeof(TrainCar) });

            foreach (var instruction in instructions)
            {
                if (instruction.Calls(getComponentInChildren))
                {
                    yield return CodeInstruction.Call(typeof(CarSpawnerPatches), nameof(EnableCar));
                }
                yield return instruction;
            }
        }

        //[HarmonyPrefix]
        //[HarmonyPatch(nameof(CarSpawner.SpawnCar))]
        //public static bool SpawnCar(
        //    GameObject carToSpawn, RailTrack track, Vector3 position, Vector3 forward, bool playerSpawnedCar,
        //    ref TrainCar __result, bool ___useCarPooling)
        //{
        //    TrainCar prefabCar = carToSpawn.GetComponentInChildren<TrainCar>(true);
        //    if (!(prefabCar.carLivery is CustomLivery customLivery))
        //    {
        //        return true;
        //    }

        //    GameObject carObj = ___useCarPooling ? 
        //        CarSpawner.Instance.GetFromPool(carToSpawn) :
        //        UnityEngine.Object.Instantiate(carToSpawn);

        //    if (!carObj.activeSelf)
        //    {
        //        carObj.SetActive(true);
        //    }

        //    TrainCar spawnedCar = carObj.GetComponentInChildren<TrainCar>();
        //    //_cargoCapacityField.SetValue(spawnedCar, customCarType.CargoCapacity);
        //    spawnedCar.playerSpawnedCar = playerSpawnedCar;
        //    spawnedCar.InitializeNewLogicCar();
        //    spawnedCar.SetTrack(track, position, forward);

        //    RaiseCarSpawned(spawnedCar);

        //    __result = spawnedCar;
        //    return false;
        //}

        //[HarmonyPrefix]
        //[HarmonyPatch(nameof(CarSpawner.SpawnLoadedCar))]
        //public static bool SpawnLoadedCar(
        //    GameObject carToSpawn,
        //    string carId, string carGuid, bool playerSpawnedCar, Vector3 position, Quaternion rotation,
        //    bool bogie1Derailed, RailTrack bogie1Track, double bogie1PositionAlongTrack,
        //    bool bogie2Derailed, RailTrack bogie2Track, double bogie2PositionAlongTrack,
        //    bool couplerFCoupled, bool couplerRCoupled,
        //    ref TrainCar __result)
        //{
        //    TrainCar prefabCar = carToSpawn.GetComponentInChildren<TrainCar>(true);
        //    if (!(prefabCar.carLivery is CustomLivery customLivery))
        //    {
        //        return true;
        //    }

        //    GameObject carObj = UnityEngine.Object.Instantiate(carToSpawn, position, rotation);
        //    if (!carObj.activeSelf)
        //    {
        //        carObj.SetActive(true);
        //    }

        //    TrainCar spawnedCar = carObj.GetComponentInChildren<TrainCar>();
        //    //_cargoCapacityField.SetValue(spawnedCar, customCarType.CargoCapacity);
        //    spawnedCar.playerSpawnedCar = playerSpawnedCar;
        //    spawnedCar.InitializeExistingLogicCar(carId, carGuid);

        //    if (!bogie1Derailed)
        //    {
        //        spawnedCar.Bogies[0].SetTrack(bogie1Track, bogie1PositionAlongTrack);
        //    }
        //    else
        //    {
        //        spawnedCar.Bogies[0].SetDerailedOnLoadFlag(true);
        //    }

        //    if (!bogie2Derailed)
        //    {
        //        spawnedCar.Bogies[1].SetTrack(bogie2Track, bogie2PositionAlongTrack);
        //    }
        //    else
        //    {
        //        spawnedCar.Bogies[1].SetDerailedOnLoadFlag(true);
        //    }

        //    spawnedCar.frontCoupler.forceCoupleStateOnLoad = true;
        //    spawnedCar.frontCoupler.loadedCoupledState = couplerFCoupled;
        //    spawnedCar.rearCoupler.forceCoupleStateOnLoad = true;
        //    spawnedCar.rearCoupler.loadedCoupledState = couplerRCoupled;

        //    RaiseCarSpawned(spawnedCar);

        //    __result = spawnedCar;
        //    return false;
        //}

        //private static void RaiseCarSpawned(TrainCar car)
        //{
        //    if (!(AccessTools.Field(typeof(CarSpawner), nameof(CarSpawner.CarSpawned)).GetValue(CarSpawner.Instance) is MulticastDelegate mcd))
        //    {
        //        CCLPlugin.Error("Couldn't get CarSpawner.CarSpawned delegate");
        //        return;
        //    }

        //    var carSpawnedDelegates = mcd.GetInvocationList();

        //    var args = new object[] { car };
        //    foreach (Delegate d in carSpawnedDelegates)
        //    {
        //        d.Method.Invoke(d.Target, args);
        //    }
        //}
    }
}
