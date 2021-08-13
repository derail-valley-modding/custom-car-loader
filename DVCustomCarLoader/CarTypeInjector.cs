using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CCL_GameScripts;
using DV.Logic.Job;
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

        public static bool TryGetCarTypeById( string id, out TrainCarType type ) => idToCarType.TryGetValue(id, out type);
        public static bool TryGetCustomCarByType( TrainCarType carType, out CustomCar car ) => carTypeToCustomCar.TryGetValue(carType, out car);
        public static bool TryGetCustomCarById( string id, out CustomCar car ) => idToCustomCar.TryGetValue(id, out car);

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
}
