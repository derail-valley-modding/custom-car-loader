using CCL.Importer.Types;
using DV.Booklets;
using DV.ThingTypes;
using DV.ThingTypes.TransitionHelpers;
using HarmonyLib;
using System.Collections.Generic;

namespace CCL.Importer.Patches
{
    [HarmonyPatch(typeof(C))]
    internal class CPatches
    {
        [HarmonyPrefix, HarmonyPatch(nameof(C.GetCarsTotalMass))]
        private static bool GetCarsTotalMassPrefix(List<Car_data> cars, List<CargoType> cargoPerCar, ref float __result)
        {
            // Won't run the patch code so just execute the base method.
            if (cars == null || cargoPerCar == null || cars.Count != cargoPerCar.Count) return true;

            float total = 0f;
            for (int i = 0; i < cars.Count; i++)
            {
                total += cars[i].carOnlyMass;
            }

            for (int j = 0; j < cargoPerCar.Count; j++)
            {
                var v2 = cargoPerCar[j].ToV2();
                var data = cars[j];

                if (v2 != null)
                {
                    if (data.type.parentType is CCL_CarType car)
                    {
                        if (car.CargoAmounts.TryGetValue(v2.id, out var mult))
                        {
                            if (mult != 1)
                            {
                                total += data.capacity * v2.massPerUnit * mult;
                                continue;
                            }
                        }
                        else
                        {
                            CCLPlugin.Error($"Could not get cargo '{v2.id}' amount for car type {car.id}! Using default calculation for mass");
                        }
                    }

                    total += data.capacity * v2.massPerUnit;
                }
            }

            __result = total;
            return false;
        }
    }
}
