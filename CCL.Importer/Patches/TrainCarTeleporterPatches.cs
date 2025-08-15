using HarmonyLib;
using System.Collections.Generic;
using System.Linq;

namespace CCL.Importer.Patches
{
    [HarmonyPatch(typeof(TrainCarTeleporter))]
    internal class TrainCarTeleporterPatches
    {
        [HarmonyPostfix, HarmonyPatch(nameof(TrainCarTeleporter.GetConnectedLocoMultipleUnitCars))]
        private static void GetConnectedLocoMultipleUnitCarsPostfix(TrainCar loco, ref List<TrainCar> __result)
        {
            if (__result != null) return;

            var set = CarManager.GetInstancedTrainset(loco);

            if (set.Length < 2) return;

            __result = set.ToList();
        }
    }
}
