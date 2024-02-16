using CCL.Types;
using DV.ThingTypes;
using HarmonyLib;
using System.Collections.Generic;

namespace CCL.Importer.Patches
{
    [HarmonyPatch(typeof(StationLocoSpawner))]
    internal class StationLocoSpawnerPatches
    {
        [HarmonyPostfix, HarmonyPatch(nameof(StationLocoSpawner.Start))]
        private static void StartPostfix(StationLocoSpawner __instance)
        {
            CCLPlugin.Log($"Finding loco spawn groups to inject into '{__instance.name}'");

            // Get the groups from the liveries.
            foreach (var car in CarManager.CustomCarTypes)
            {
                foreach (var variant in car.Variants)
                {
                    foreach (var group in variant.LocoSpawnGroups)
                    {
                        // If the group is supposed to use this spawner...
                        if (group.Track.ToName() == __instance.name)
                        {
                            CCLPlugin.Log($"Injecting loco spawn group [{variant.id}, {string.Join(", ", group.Liveries)}]");
                            __instance.locoTypeGroupsToSpawn.Add(FromGroup(variant, group));
                        }
                    }
                }
            }

            // Randomise spawn index again so it doesn't always spawn vanilla the first time.
            __instance.nextLocoGroupSpawnIndex = UnityEngine.Random.Range(0, __instance.locoTypeGroupsToSpawn.Count);
        }

        private static ListTrainCarTypeWrapper FromGroup(TrainCarLivery variant, LocoSpawnGroup group)
        {
            List<TrainCarLivery> variants = new() { variant };

            foreach (var item in group.Liveries)
            {
                if (DV.Globals.G.Types.TryGetLivery(item, out TrainCarLivery livery))
                {
                    variants.Add(livery);
                }
                else
                {
                    CCLPlugin.Error($"Could not find livery '{item}'");
                }
            }

            return new ListTrainCarTypeWrapper(variants);
        }
    }
}
