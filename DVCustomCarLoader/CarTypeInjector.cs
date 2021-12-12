using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CCL_GameScripts;
using CCL_GameScripts.Effects;
using DV.Logic.Job;
using DVCustomCarLoader.LocoComponents;
using DVCustomCarLoader.LocoComponents.DieselElectric;
using HarmonyLib;
using UnityEngine;

namespace DVCustomCarLoader
{
    public static class CarTypeInjector
    {
        public const int CUSTOM_TYPE_OFFSET = 0x4000_0000;
        public const int CUSTOM_TYPE_MASK = CUSTOM_TYPE_OFFSET - 1;
        public const int CUSTOM_TYPE_MAX_VALUE = CUSTOM_TYPE_OFFSET + CUSTOM_TYPE_MASK;

        public const TrainCarType MissingCustomType = (TrainCarType)(-1);

        private static readonly Dictionary<string, TrainCarType> idToCarType = new Dictionary<string, TrainCarType>(StringComparer.CurrentCultureIgnoreCase);
        private static readonly Dictionary<string, CustomCar> idToCustomCar = new Dictionary<string, CustomCar>(StringComparer.CurrentCultureIgnoreCase);
        private static readonly Dictionary<TrainCarType, CustomCar> carTypeToCustomCar = new Dictionary<TrainCarType, CustomCar>();

        public static bool TryGetCarTypeById( string id, out TrainCarType type ) => idToCarType.TryGetValue(id.ToLower(), out type);
        public static bool TryGetCustomCarByType( TrainCarType carType, out CustomCar car ) => carTypeToCustomCar.TryGetValue(carType, out car);
        public static bool TryGetCustomCarById( string id, out CustomCar car ) => idToCustomCar.TryGetValue(id.ToLower(), out car);

        public static bool IsCustomTypeRegistered( TrainCarType carType ) => carTypeToCustomCar.ContainsKey(carType);
        public static bool IsCustomTypeRegistered( string identifier ) => idToCarType.ContainsKey(identifier);

        public static bool IsInCustomRange( TrainCarType carType ) => (int)carType >= CUSTOM_TYPE_OFFSET;

        // Reflected fields
        private static readonly HashSet<TrainCarType> locomotivesMap;
        private static readonly HashSet<TrainCarType> multipleUnitLocos;
        private static readonly Dictionary<TrainCarType, CargoContainerType> CarTypeToContainerType;

        static CarTypeInjector()
        {
            locomotivesMap = AccessTools.Field(typeof(CarTypes), "locomotivesMap")?.GetValue(null) as HashSet<TrainCarType>;
            if( locomotivesMap == null )
            {
                Main.Error("Failed to get CarTypes.locomotivesMap");
            }

            multipleUnitLocos = AccessTools.Field(typeof(CarTypes), "multipleUnitLocos")?.GetValue(null) as HashSet<TrainCarType>;
            if( multipleUnitLocos == null )
            {
                Main.Error("Failed to get CarTypes.multipleUnitLocos");
            }

            CarTypeToContainerType = AccessTools.Field(typeof(CargoTypes), nameof(CargoTypes.CarTypeToContainerType))?.GetValue(null)
                as Dictionary<TrainCarType, CargoContainerType>;
            if( CarTypeToContainerType == null )
            {
                Main.Error("Failed to get CargoTypes.CarTypeToContainerType");
            }
        }

        private static TrainCarType GenerateUniqueCarType( string identifier )
        {
            int hash = identifier.GetHashCode();
            hash = (hash & CUSTOM_TYPE_MASK) + CUSTOM_TYPE_OFFSET;

            // find an untaken TrainCarType
            for( int searchWatchdog = CUSTOM_TYPE_MASK; searchWatchdog >= 0; searchWatchdog-- )
            {
                TrainCarType candidateType = (TrainCarType)hash;
                if( !IsCustomTypeRegistered(candidateType) )
                {
                    return candidateType;
                }

                // move up by 1 and try again
                hash += 1;
                if( hash > CUSTOM_TYPE_MAX_VALUE ) hash = CUSTOM_TYPE_OFFSET;
            }

            Main.Error("No available custom car types, something is VERY wrong");
            return TrainCarType.NotSet;
        }

        public static TrainCarType RegisterCustomCarType( CustomCar car )
        {
            string identifier = car.identifier.ToLower();

            TrainCarType carType = GenerateUniqueCarType(identifier);
            idToCarType.Add(identifier, carType);
            carTypeToCustomCar.Add(carType, car);
            idToCustomCar.Add(identifier, car);

            var trainCar = car.CarPrefab.GetComponent<TrainCar>();
            trainCar.carType = carType;

            car.CarType = carType;

            InjectCarTypesData(car);

            return carType;
        }

        private static void InjectCarTypesData( CustomCar car )
        {
            if( car.LocoType != LocoParamsType.None )
            {
                locomotivesMap?.Add(car.CarType);

                if( car.LocoType == LocoParamsType.DieselElectric )
                {
                    multipleUnitLocos?.Add(car.CarType);
                }
            }

            CarTypeToContainerType.Add(car.CarType, car.CargoClass);
        }

        #region Audio Pooling

        private static GameObject GetPooledAudioPrefab( TrainComponentPool componentPool, TrainCarType carType )
        {
            foreach( var poolData in componentPool.audioPoolReferences.poolData )
            {
                if( poolData.trainCarType == carType )
                {
                    return poolData.audioPrefab;
                }
            }

            return null;
        }

        private static GameObject CopyAudioPrefab<TAudio>(GameObject sourcePrefab, AudioConfig audioConfig)
            where TAudio : CustomLocoAudio
        {
            GameObject newFab = UnityEngine.Object.Instantiate(sourcePrefab, null);
            newFab.SetActive(false);
            UnityEngine.Object.DontDestroyOnLoad(newFab);

            var origAudio = newFab.GetComponentInChildren<LocoTrainAudio>();
            if( origAudio )
            {
                Main.Log($"Adding audio {typeof(TAudio).Name}");
                TAudio newAudio = origAudio.gameObject.AddComponent<TAudio>();
                newAudio.PullSettingsFromOtherAudio(origAudio);
                UnityEngine.Object.DestroyImmediate(origAudio);

                // grab extra components
                newAudio.carFrictionSound = newFab.GetComponentInChildren<CarFrictionSound>(true);
                newAudio.carCollisionSounds = newFab.GetComponentInChildren<CarCollisionSounds>(true);
                newAudio.trainDerailAudio = newFab.GetComponentInChildren<TrainDerailAudio>(true);
                
                if (audioConfig)
                {
                    InitSpecManager.ApplyProxyFields(audioConfig, newAudio);
                }
            }
            else
            {
                Main.Warning($"Couldn't find LocoTrainAudio on prefab {sourcePrefab.name}");
            }

            return newFab;
        }

        public static void InjectLocoAudioToPool( CustomCar car, TrainComponentPool componentPool )
        {
            const int LOCO_POOL_SIZE = 10;
            
            GameObject sourcePrefab;
            GameObject newPrefab;

            var audioConfig = car.CarPrefab.GetComponentInChildren<AudioConfig>(true);

            switch (car.LocoAudioType)
            {
                case LocoAudioBasis.DE2:
                    sourcePrefab = GetPooledAudioPrefab(componentPool, TrainCarType.LocoShunter);
                    if( sourcePrefab )
                    {
                        newPrefab = CopyAudioPrefab<CustomLocoAudioDiesel>(sourcePrefab, audioConfig);

                        var newPoolData = new AudioPoolReferences.AudioPoolData()
                        {
                            trainCarType = car.CarType,
                            audioPrefab = newPrefab,
                            poolSize = LOCO_POOL_SIZE
                        };

                        componentPool.audioPoolReferences.poolData.Add(newPoolData);
                    }
                    else Main.Warning("Couldn't find shunter pooled audio");
                    break;

                case LocoAudioBasis.DE6:
                    sourcePrefab = GetPooledAudioPrefab(componentPool, TrainCarType.LocoDiesel);
                    if( sourcePrefab )
                    {
                        newPrefab = CopyAudioPrefab<CustomLocoAudioDiesel>(sourcePrefab, audioConfig);

                        var newPoolData = new AudioPoolReferences.AudioPoolData()
                        {
                            trainCarType = car.CarType,
                            audioPrefab = newPrefab,
                            poolSize = LOCO_POOL_SIZE
                        };

                        componentPool.audioPoolReferences.poolData.Add(newPoolData);
                    }
                    else Main.Warning("Couldn't find DE6 pooled audio");
                    break;

                case LocoAudioBasis.Steam:
                default:
                    break;
            }
        }

        #endregion
    }

    [HarmonyPatch(typeof(CarTypes), nameof(CarTypes.GetCarPrefab))]
    public static class CarTypes_GetCarPrefab_Patch
    {
        public static bool Prefix( TrainCarType carType, ref GameObject __result )
        {
            if( CarTypeInjector.IsInCustomRange(carType) )
            {
                if( CarTypeInjector.TryGetCustomCarByType(carType, out CustomCar car) )
                {
                    __result = car.CarPrefab;
                }
                else
                {
                    __result = null;
                }
                return false;
            }

            return true;
        }
    }

    [HarmonyPatch(typeof(CarTypes), nameof(CarTypes.DisplayName))]
    public static class CarTypes_DisplayName_Patch
    {
        public static bool Prefix( TrainCarType carType, ref string __result )
        {
            if( CarTypeInjector.IsInCustomRange(carType) )
            {
                if( CarTypeInjector.TryGetCustomCarByType(carType, out CustomCar car) )
                {
                    __result = car.identifier;
                }
                else
                {
                    __result = null;
                }
                return false;
            }

            return true;
        }
    }

    [HarmonyPatch(typeof(CarTypes))]
    public static class CarTypes_LicenseCheck_Patches
    {
        private static bool DoesCarNeedLicense( TrainCarType carType, LocoRequiredLicense license )
        {
            if( CarTypeInjector.TryGetCustomCarByType(carType, out CustomCar car) )
            {
                return (car.RequiredLicense == license);
            }
            else return false;
        }

        [HarmonyPrefix]
        [HarmonyPatch(nameof(CarTypes.IsSteamLocomotive))]
        public static bool IsSteamLocomotive( TrainCarType carType, ref bool __result )
        {
            if( CarTypeInjector.IsInCustomRange(carType) )
            {
                __result = DoesCarNeedLicense(carType, LocoRequiredLicense.Steam);
                return false;
            }
            return true;
        }

        [HarmonyPrefix]
        [HarmonyPatch(nameof(CarTypes.IsShunterLocomotive))]
        public static bool IsShunterLocomotive( TrainCarType carType, ref bool __result )
        {
            if( CarTypeInjector.IsInCustomRange(carType) )
            {
                __result = DoesCarNeedLicense(carType, LocoRequiredLicense.DE2);
                return false;
            }
            return true;
        }

        [HarmonyPrefix]
        [HarmonyPatch(nameof(CarTypes.IsDieselLocomotive))]
        public static bool IsDieselLocomotive( TrainCarType carType, ref bool __result )
        {
            if( CarTypeInjector.IsInCustomRange(carType) )
            {
                __result = DoesCarNeedLicense(carType, LocoRequiredLicense.DE6);
                return false;
            }
            return true;
        }
    }

    [HarmonyPatch(typeof(TrainComponentPool), "Awake")]
    public static class TrainComponentPool_Awake_Patch
    {
        public static void Prefix( TrainComponentPool __instance )
        {
            Main.Log("Injecting custom cars into component pool");

            foreach( CustomCar car in CustomCarManager.CustomCarTypes )
            {
                CarTypeInjector.InjectLocoAudioToPool(car, __instance);
            }
        }
    }
}
