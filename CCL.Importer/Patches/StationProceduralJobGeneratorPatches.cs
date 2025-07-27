using CCL.Importer.Types;
using DV.ThingTypes;
using DV.ThingTypes.TransitionHelpers;
using HarmonyLib;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

using static StationProceduralJobGenerator;

namespace CCL.Importer.Patches
{
    [HarmonyPatch(typeof(StationProceduralJobGenerator))]
    internal class StationProceduralJobGeneratorPatches
    {
        //[HarmonyPostfix, HarmonyPatch(nameof(StationProceduralJobGenerator.GenerateBaseCargoTrainData))]
        public static void GenerateBaseCargoTrainDataPostfix(List<CarTypesPerCargoTypeData> __result)
        {
            for (int i = 0; i < __result.Count; i++)
            {
                float totalAmount = 0;

                foreach (var car in __result[i].carTypes)
                {
                    if (car is CCL_CarVariant variant)
                    {
                        totalAmount += GetCargoAmount((CCL_CarType)variant.parentType, __result[i].cargoType);
                    }
                }

                if (!Mathf.Approximately(totalAmount, __result[i].totalCargoAmount))
                {
                    __result[i] = new CarTypesPerCargoTypeData(__result[i].carTypes, __result[i].cargoType, totalAmount);
                }
            }
        }

        private static float GetCargoAmount(CCL_CarType carType, CargoType cargo)
        {
            if (carType.CargoAmounts.TryGetValue(cargo.ToV2().id, out float amount))
            {
                return amount;
            }

            return 1.0f;
        }
    }

    [HarmonyPatch(typeof(StationProceduralJobGenerator))]
    internal class StationProceduralJobGeneratorRandomFromListPatches
    {
        private static MethodBase TargetMethod()
        {
            return typeof(StationProceduralJobGenerator).GetMethod(nameof(StationProceduralJobGenerator.GetRandomFromList), BindingFlags.Instance | BindingFlags.NonPublic)
                .MakeGenericMethod(typeof(TrainCarType_v2));
        }

        [HarmonyPrefix, HarmonyPatch]
        private static void OverrideInput(ref List<TrainCarType_v2> list)
        {
            if (!CCLPlugin.Settings.PreferCCLInJobs) return;

            if (GetCCLTypesOnlyIfAny(list, out var modified))
            {
                list = modified;
            }
        }

        public static bool GetCCLTypesOnlyIfAny(List<TrainCarType_v2> original, out List<TrainCarType_v2> result)
        {
            if (original.Any(x => x is CCL_CarType))
            {
                result = new List<TrainCarType_v2>();

                foreach (var item in original)
                {
                    if (item is CCL_CarType ccl)
                    {
                        result.Add(ccl);
                    }
                }

                return true;
            }

            result = original;
            return false;
        }
    }
}
