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

        // ======
        // Removing cargo groups for disabled car types.
        // ======

        [HarmonyPrefix, HarmonyPatch(nameof(StationProceduralJobGenerator.GenerateEmptyHaulBaseData))]
        private static void GenerateEmptyHaulBaseDataPrefix(ref List<CargoGroup> availableCargoTypeGroups)
        {
            availableCargoTypeGroups = CreateNewList(availableCargoTypeGroups);
        }

        [HarmonyPrefix, HarmonyPatch(nameof(StationProceduralJobGenerator.GenerateBaseCargoTrainData))]
        private static void GenerateBaseCargoTrainDataPrefix(ref List<CargoGroup> availableCargoGroups)
        {
            availableCargoGroups = CreateNewList(availableCargoGroups);
        }

        private static List<CargoGroup> CreateNewList(List<CargoGroup> original)
        {
            var list = original.ToList();

            // For each cargo group...
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i] == null || list[i].cargoTypes == null) continue;

                // For each cargo type in the group...
                foreach (var cargoType in list[i].cargoTypes)
                {
                    var cargo = cargoType.ToV2();

                    if (cargo == null) continue;

                    // If all wagons that load the cargo are disabled...
                    if (DV.Globals.G.Types.CargoToLoadableCarTypes.TryGetValue(cargo, out var loadables) &&
                        loadables.Count > 0 &&
                        loadables.All(x => CCLPlugin.Settings.DisabledIds.Contains(x.id)))
                    {

                        CCLPlugin.LogVerbose($"Removed group [{string.Join(", ", list[i].cargoTypes)}] due to '{cargo.id}' only being loaded on disabled car types");
                        list.RemoveAt(i);
                        i--;
                    }
                }
            }

            return list;
        }
    }

    [HarmonyPatch(typeof(StationProceduralJobGenerator))]
    internal class StationProceduralJobGeneratorRandomFromListPatches
    {
        private static IEnumerable<MethodBase> TargetMethods()
        {
            yield return typeof(StationProceduralJobGenerator).GetMethod(nameof(StationProceduralJobGenerator.GetRandomFromList), BindingFlags.Instance | BindingFlags.NonPublic)
                .MakeGenericMethod(typeof(TrainCarType_v2));
            yield return typeof(StationProceduralJobGenerator).GetMethod(nameof(StationProceduralJobGenerator.GetRandomFromList), BindingFlags.Instance | BindingFlags.NonPublic)
                .MakeGenericMethod(typeof(TrainCarLivery));
        }

        [HarmonyPrefix, HarmonyPatch]
        private static void OverrideInput(ref object list)
        {
            switch (list)
            {
                case List<TrainCarType_v2> types:
                    types.RemoveAll(carType => CCLPlugin.Settings.DisabledIds.Contains(carType.id));

                    if (!CCLPlugin.Settings.PreferCCLInJobs) return;

                    if (GetCCLTypesOnlyIfAny(types, out var modified))
                    {
                        list = modified;
                    }
                    break;
                case List<TrainCarLivery> liveries:
                    liveries.RemoveAll(carLivery => CCLPlugin.Settings.DisabledLiveryIds.Contains(carLivery.id));
                    break;
                default:
                    break;
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
