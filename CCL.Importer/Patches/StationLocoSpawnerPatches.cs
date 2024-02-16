using CCL.Importer.Types;
using CCL.Types;
using DV.CabControls.Spec;
using DV.ThingTypes;
using HarmonyLib;
using System.Collections.Generic;
using System.Linq;
using static CCL.Types.CustomCarVariant;
using static UnityEditor.UIElements.ToolbarMenu;

namespace CCL.Importer.Patches
{
    [HarmonyPatch(typeof(StationLocoSpawner))]
    internal class StationLocoSpawnerPatches
    {
        [HarmonyPostfix, HarmonyPatch(nameof(StationLocoSpawner.Start))]
        private static void StartPostfix(StationLocoSpawner __instance)
        {
            CCLPlugin.Log($"Finding loco spawn groups to inject into '{__instance.name}'");

            foreach (var car in CarManager.CustomCarTypes)
            {
                foreach (var variant in car.Variants)
                {
                    foreach (var group in variant.LocoSpawnGroupsIds)
                    {
                        if (group.Track.ToName() == __instance.name)
                        {
                            CCLPlugin.Log($"Injecting loco spawn group [{variant.id}, {string.Join(", ", group.Liveries)}]");
                            __instance.locoTypeGroupsToSpawn.Add(FromGroup(variant, group));
                        }
                    }
                }
            }
        }

        private static ListTrainCarTypeWrapper FromGroup(TrainCarLivery variant, LocoSpawnGroupIds group)
        {
            List<TrainCarLivery> variants = new() { variant };

            foreach (var item in group.Liveries)
            {
                if (DV.Globals.G.Types.TryGetLivery(item, out TrainCarLivery livery))
                {
                    variants.Add(livery);
                }
            }

            return new ListTrainCarTypeWrapper(variants);
        }
    }
}
