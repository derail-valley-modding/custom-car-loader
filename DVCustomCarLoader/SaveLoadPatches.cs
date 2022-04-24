using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CCL_GameScripts;
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
		public const string CUSTOM_CARGO_KEY = "customcargo";
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

			if (CustomCargoInjector.TryGetCustomCargoByType(car.LoadedCargo, out var customCargo))
            {
				__result.SetString(SaveConstants.CUSTOM_CARGO_KEY, customCargo.Identifier);
				__result.SetInt("loadedCargo", (int)CargoType.None);
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
			// check for custom cargo type
			string customCargo = carData.GetString(SaveConstants.CUSTOM_CARGO_KEY);
			if (customCargo != null)
            {
				if (CustomCargoInjector.TryGetCargoTypeById(customCargo, out var cargo))
                {
					carData.SetInt("loadedCargo", (int)cargo);
                }
            }

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
				coupledFront == null || coupledRear == null || loadedCargoTypeId == null )
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
}
