using DV.ThingTypes;
using HarmonyLib;
using System;
using System.Linq;

namespace CCL.Importer.Patches
{
    [HarmonyPatch(typeof(Enum))]
    internal static class EnumPatches
    {
        // Thanks Passenger Jobs mod!

        // Extend the array of actual values with the ones added by the mod.
        [HarmonyPostfix, HarmonyPatch(nameof(Enum.GetValues))]
        private static void GetValuesPostfix(Type enumType, ref Array __result)
        {
            if (enumType == typeof(CargoType))
            {
                __result = ExtendArray(__result, CarManager.AddedValues.ToArray());
            }
        }

        private static Array ExtendArray<T>(Array source, params T[] newValues)
        {
            var result = Array.CreateInstance(typeof(T), source.Length + newValues.Length);
            Array.Copy(source, result, source.Length);
            Array.Copy(newValues, 0, result, source.Length, newValues.Length);
            return result;
        }

        // Consider values defined by the mod as valid enum values.
        [HarmonyPrefix, HarmonyPatch(nameof(Enum.IsDefined))]
        private static bool IsDefinedPrefix(Type enumType, object value, ref bool __result)
        {
            if (enumType == typeof(TrainCarType))
            {
                if (value is int iVal && CarManager.AddedValues.Contains((TrainCarType)iVal) ||
                    value is TrainCarType cVal && CarManager.AddedValues.Contains(cVal))
                {
                    __result = true;
                    return false;
                }
            }

            return true;
        }
    }
}
