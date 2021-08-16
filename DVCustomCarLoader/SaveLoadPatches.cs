using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DV.JObjectExtstensions;
using DV.Logic.Job;
using HarmonyLib;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace DVCustomCarLoader
{
    static class SaveConstants
    {
        public const string CUSTOM_CAR_KEY = "customcar";
    }

    [HarmonyPatch(typeof(CarsSaveManager), "GetCarSaveData")]
    public static class CarsSaveManager_GetSaveData_Patch
    {
        public static void Postfix( TrainCar car, ref JObject __result )
        {
            if( CarTypeInjector.TryGetCustomCarByType(car.carType, out CustomCar customCar) )
            {
                // custom car detected, save its type
                __result.SetString(SaveConstants.CUSTOM_CAR_KEY, customCar.identifier);
            }
        }
    }
	
    [HarmonyPatch(typeof(CarsSaveManager), "InstantiateCar")]
    public static class CarsSaveManager_InstantiateCar_Patch
    {
		private static bool IsVectorValid( Vector3? vec )
		{
			return vec.HasValue && !NumberUtil.AnyInfinityMinMaxNaN(vec.Value);
		}

		public static bool Prefix( JObject carData, RailTrack[] tracks, ref TrainCar __result )
        {
			string customType = carData.GetString(SaveConstants.CUSTOM_CAR_KEY);
			if( customType == null )
			{
				// use default instantiate
				return true;
			}

			if( !CarTypeInjector.TryGetCustomCarById(customType, out CustomCar customCarType) )
            {
				Main.ModEntry.Logger.Warning($"Found a saved custom car of unknown type ({customType}), skipping this car");
				__result = null;
				return false;
			}
			
			// proper custom type, proceed with the spawning

			// car info
			string carId = carData.GetString("id");
			string carGuid = carData.GetString("carGuid");
			bool playerSpawnedCar = carData.GetBool("playerSpawn") ?? false;
			Vector3? position = carData.GetVector3("position");
			Vector3? rotation = carData.GetVector3("rotation");

			// bogie1
			bool bogie1Derailed = carData.GetBool("bog1Derailed") ?? false;
			int? bogie1TrackChildIdx = carData.GetInt("bog1TrackChildInd");
			double? bogie1TrackPosition = carData.GetDouble("bog1PosOnTrack");

			// bogie2
			bool bogie2Derailed = carData.GetBool("bog2Derailed") ?? false;
			int? bogie2TrackChildIdx = carData.GetInt("bog2TrackChildInd");
			double? bogie2TrackPosition = carData.GetDouble("bog2PosOnTrack");

			// state
			bool? coupledFront = carData.GetBool("coupledF");
			bool? coupledRear = carData.GetBool("coupledR");
			bool exploded = carData.GetBool("exploded") ?? false;
			int? loadedCargoTypeId = carData.GetInt("loadedCargo");

			JObject carState = carData.GetJObject("carState");
			JObject locoState = carData.GetJObject("locoState");

			if( carId == null || carGuid == null || !IsVectorValid(position) || !IsVectorValid(rotation) ||
				(!bogie1Derailed && (bogie1TrackChildIdx == null || bogie1TrackPosition == null)) || 
				(!bogie2Derailed && (bogie2TrackChildIdx == null || bogie2TrackPosition == null)) || 
				coupledFront == null || coupledRear == null || loadedCargoTypeId == null || !Enum.IsDefined(typeof(CargoType), loadedCargoTypeId) )
			{
				Main.ModEntry.Logger.Error("Error while loading car data (not all data was present), skipping this entry!");
				__result = null;
				return false;
			}

			CargoType loadedCargoType = (CargoType)loadedCargoTypeId.Value;

			RailTrack bogie1Track = (!bogie1Derailed) ? tracks[bogie1TrackChildIdx.Value] : null;
			double bogie1PositionAlongTrack = (!bogie1Derailed) ? bogie1TrackPosition.Value : 0.0;

			RailTrack bogie2Track = (!bogie2Derailed) ? tracks[bogie2TrackChildIdx.Value] : null;
			double bogie2PositionAlongTrack = (!bogie2Derailed) ? bogie2TrackPosition.Value : 0.0;

			TrainCar trainCar = CarSpawner.SpawnLoadedCar(
				customCarType.CarPrefab, carId, carGuid, playerSpawnedCar,
				position.Value + WorldMover.currentMove, Quaternion.Euler(rotation.Value),
				bogie1Derailed, bogie1Track, bogie1PositionAlongTrack,
				bogie2Derailed, bogie2Track, bogie2PositionAlongTrack,
				coupledFront.Value, coupledRear.Value);

			if( loadedCargoType != CargoType.None )
			{
				trainCar.logicCar.LoadCargo(trainCar.cargoCapacity, loadedCargoType, null);
			}
			if( exploded )
			{
				TrainCarExplosion.UpdateTrainCarModelToExploded(trainCar);
			}
			if( carState != null )
			{
				CarStateSave component = trainCar.GetComponent<CarStateSave>();
				if( component != null )
				{
					component.SetCarStateSaveData(carState);
				}
			}
			if( locoState != null )
			{
				LocoStateSave component2 = trainCar.GetComponent<LocoStateSave>();
				if( component2 != null )
				{
					component2.SetLocoStateSaveData(locoState);
				}
			}

			__result = trainCar;
			return false;
		}
    }
}
