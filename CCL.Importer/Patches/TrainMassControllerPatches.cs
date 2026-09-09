using CCL.Importer.Types;
using DV.ThingTypes;
using DV.ThingTypes.TransitionHelpers;
using HarmonyLib;
using System.Reflection;

namespace CCL.Importer.Patches
{
    [HarmonyPatch(typeof(TrainMassController))]
    internal class TrainMassControllerPatches
    {
        private class CargoState
        {
            public CargoType_v2? CargoType;
            public float OriginalMass;

            public void SetStuff(CargoType_v2 cargo)
            {
                CargoType = cargo;
                OriginalMass = CargoType.massPerUnit;
            }

            public void Reset()
            {
                if (CargoType == null) return;

                CargoType.massPerUnit = OriginalMass;
            }
        }

        // Pain of the overload wrapper...
        [HarmonyTargetMethod]
        private static MethodBase GetMethod() => typeof(TrainMassController).GetMethod(nameof(TrainMassController.UpdateTrainCarMass));

        [HarmonyPrefix, HarmonyPatch]
        private static void UpdateTrainCarMassPrefix(TrainMassController __instance, out CargoState __state)
        {
            __state = new CargoState();

            // Don't do anything for non-CCL car types.
            if (__instance.car.carLivery.parentType is not CCL_CarType car) return;

            var cargo = __instance.car.LoadedCargo;
            if (cargo == CargoType.None) return;

            var v2 = cargo.ToV2();
            if (!car.CargoAmounts.TryGetValue(v2.id, out var mult))
            {
                CCLPlugin.Error($"Could not get cargo '{v2.id}' amount for car type {car.id}! Defaulting to 1.");
                return;
            }

            // Don't do anything and avoid the trouble.
            if (mult == 1) return;

            __state.SetStuff(v2);
            v2.massPerUnit *= mult;
        }

        [HarmonyPostfix, HarmonyPatch]
        private static void UpdateTrainCarMassPostfix(CargoState __state)
        {
            __state.Reset();
        }
    }
}
