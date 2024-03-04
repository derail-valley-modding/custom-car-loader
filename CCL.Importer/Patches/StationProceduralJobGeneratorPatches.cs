using CCL.Importer.Types;
using DV.ThingTypes;
using HarmonyLib;
using System.Collections.Generic;
using UnityEngine;

using static StationProceduralJobGenerator;

namespace CCL.Importer.Patches
{
    [HarmonyPatch(typeof(StationProceduralJobGenerator))]
    internal class StationProceduralJobGeneratorPatches
    {
        [HarmonyPostfix, HarmonyPatch(nameof(StationProceduralJobGenerator.GenerateBaseCargoTrainData))]
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
            if (carType.CargoAmounts.TryGetValue(cargo, out float amount))
            {
                return amount;
            }

            return 1.0f;
        }
    }
}
