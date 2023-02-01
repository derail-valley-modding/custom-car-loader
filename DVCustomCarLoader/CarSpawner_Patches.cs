using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using DV;
using HarmonyLib;
using UnityEngine;

namespace DVCustomCarLoader
{
    [HarmonyPatch(typeof(CommsRadioCarSpawner), nameof(CommsRadioCarSpawner.UpdateCarTypesToSpawn))]
    public static class CommsRadioCarSpawner_UpdateCarTypesToSpawn_Patch
    {
        public static void Prefix( ref bool allowLocoSpawning )
        {
            allowLocoSpawning = true;
        }

        public static void Postfix( List<TrainCarType> ___carTypesToSpawn )
        {
            var customCarTypes = CustomCarManager.CustomCarTypes.Select(car => car.CarType);
            ___carTypesToSpawn.AddRange(customCarTypes);
        }
    }

    [HarmonyPatch(typeof(CarSpawner))]
    public static class CarSpawner_Patches
    {
        private static readonly FieldInfo _cargoCapacityField = AccessTools.Field(typeof(TrainCar), nameof(TrainCar.cargoCapacity));
        private static Delegate[] carSpawnedDelegates = null;

        private static void RaiseCarSpawned( TrainCar car )
        {
            if( carSpawnedDelegates == null )
            {
                if( !(AccessTools.Field(typeof(CarSpawner), nameof(CarSpawner.CarSpawned)).GetValue(null) is MulticastDelegate mcd) )
                {
                    Main.Error("Couldn't get CarSpawner.CarSpawned delegate");
                    return;
                }

                carSpawnedDelegates = mcd.GetInvocationList();
            }

            var args = new object[] { car };
            foreach( Delegate d in carSpawnedDelegates )
            {
                d.Method.Invoke(d.Target, args);
            }
        }

        [HarmonyPrefix]
        [HarmonyPatch(nameof(CarSpawner.SpawnCar))]
        public static bool SpawnCar(
            GameObject carToSpawn, RailTrack track, Vector3 position, Vector3 forward, bool playerSpawnedCar,
            ref TrainCar __result)
        {
            TrainCar prefabCar = carToSpawn.GetComponentInChildren<TrainCar>(true);
            if(!CarTypeInjector.TryGetCustomCarByType(prefabCar.carType, out CustomCar customCarType))
            {
                return true;
            }

            GameObject carObj = UnityEngine.Object.Instantiate(carToSpawn);
            if( !carObj.activeSelf )
            {
                carObj.SetActive(true);
            }
            TrainCar spawnedCar = carObj.GetComponentInChildren<TrainCar>();

            _cargoCapacityField.SetValue(spawnedCar, customCarType.CargoCapacity);
            spawnedCar.playerSpawnedCar = playerSpawnedCar;
            spawnedCar.InitializeNewLogicCar();
            spawnedCar.SetTrack(track, position, forward);

            RaiseCarSpawned(spawnedCar);

            __result = spawnedCar;
            return false;
        }

        [HarmonyPrefix]
        [HarmonyPatch(nameof(CarSpawner.SpawnLoadedCar))]
        public static bool SpawnLoadedCar(
            GameObject carToSpawn,
            string carId, string carGuid, bool playerSpawnedCar, Vector3 position, Quaternion rotation,
            bool bogie1Derailed, RailTrack bogie1Track, double bogie1PositionAlongTrack,
            bool bogie2Derailed, RailTrack bogie2Track, double bogie2PositionAlongTrack,
            bool couplerFCoupled, bool couplerRCoupled,
            ref TrainCar __result)
        {
            TrainCar prefabCar = carToSpawn.GetComponentInChildren<TrainCar>(true);
            if(!CarTypeInjector.TryGetCustomCarByType(prefabCar.carType, out CustomCar customCarType))
            {
                return true;
            }

            GameObject carObj = UnityEngine.Object.Instantiate(carToSpawn, position, rotation);
            if( !carObj.activeSelf )
            {
                carObj.SetActive(true);
            }

            TrainCar spawnedCar = carObj.GetComponentInChildren<TrainCar>();

            _cargoCapacityField.SetValue(spawnedCar, customCarType.CargoCapacity);
            spawnedCar.playerSpawnedCar = playerSpawnedCar;
            spawnedCar.InitializeExistingLogicCar(carId, carGuid);

            if( !bogie1Derailed )
            {
                spawnedCar.Bogies[0].SetTrack(bogie1Track, bogie1PositionAlongTrack);
            }
            else
            {
                spawnedCar.Bogies[0].SetDerailedOnLoadFlag(true);
            }

            if( !bogie2Derailed )
            {
                spawnedCar.Bogies[1].SetTrack(bogie2Track, bogie2PositionAlongTrack);
            }
            else
            {
                spawnedCar.Bogies[1].SetDerailedOnLoadFlag(true);
            }

            spawnedCar.frontCoupler.forceCoupleStateOnLoad = true;
            spawnedCar.frontCoupler.loadedCoupledState = couplerFCoupled;
            spawnedCar.rearCoupler.forceCoupleStateOnLoad = true;
            spawnedCar.rearCoupler.loadedCoupledState = couplerRCoupled;

            //spawnedCar.OnDestroyCar += CustomCarManager.DeregisterCar;
            //CustomCarManager.RegisterSpawnedCar(spawnedCar, identifier);

            RaiseCarSpawned(spawnedCar);

            __result = spawnedCar;
            return false;
        }
    }
}
