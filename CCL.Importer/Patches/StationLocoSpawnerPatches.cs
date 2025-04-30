using CCL.Types;
using DV.ThingTypes;
using HarmonyLib;
using System.Collections.Generic;
using UnityEngine;

namespace CCL.Importer.Patches
{
    [HarmonyPatch(typeof(StationLocoSpawner))]
    internal class StationLocoSpawnerPatches
    {
        [HarmonyPostfix, HarmonyPatch(nameof(StationLocoSpawner.Start))]
        private static void StartPostfix(StationLocoSpawner __instance)
        {
            CCLPlugin.Log($"Finding loco spawn groups to inject into '{__instance.name}'");
            CCLPlugin.LogVerbose($"Track length: {RailTrackRegistry.RailTrackToLogicTrack[__instance.locoSpawnTrack].length}m");

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
                            CCLPlugin.LogVerbose($"Injecting loco spawn group [{variant.id}, {string.Join(", ", group.AdditionalLiveries)}]");
                            __instance.locoTypeGroupsToSpawn.Add(FromGroup(variant, group));
                        }
                    }
                }
            }

            // Randomise spawn index again so it doesn't always spawn vanilla the first time.
            __instance.nextLocoGroupSpawnIndex = Random.Range(0, __instance.locoTypeGroupsToSpawn.Count);

            // Calculate spawn chances for this spawner to use in the catalog.
            // If the ID is not from a vanilla station, skip.
            if (!TryToVanillaStationId(__instance.name, out string id)) return;

            // Get the station chances instead of spawner chances.
            if (!CatalogGenerator.SpawnChances.TryGetValue(id, out var chances))
            {
                chances = new();
                CatalogGenerator.SpawnChances.Add(id, chances);
            }

            // Get how many spawn groups each car type is in.
            Dictionary<string, int> groupCounts = new();

            foreach (var group in __instance.locoTypeGroupsToSpawn)
            {
                foreach (var livery in group.liveries)
                {
                    string parentId = livery.parentType.id;

                    // Add a count if it doesn't exist.
                    if (!groupCounts.ContainsKey(parentId))
                    {
                        groupCounts.Add(parentId, 1);
                    }
                    else
                    {
                        groupCounts[parentId]++;
                    }
                }
            }

            foreach (var count in groupCounts)
            {
                // Add the chance for this type if it doesn't yet exist.
                if (!chances.ContainsKey(count.Key))
                {
                    chances.Add(count.Key, 0);
                }

                // Actual chance is the maximum of all chances at each spawner of the station.
                chances[count.Key] = Mathf.Max(chances[count.Key], (float)count.Value / __instance.locoTypeGroupsToSpawn.Count);
            }
        }

        private static bool TryToVanillaStationId(string name, out string id)
        {
            return LocoSpawnGroup.SpawnerNameToId.TryGetValue(name, out id);
        }

        private static ListTrainCarTypeWrapper FromGroup(TrainCarLivery variant, LocoSpawnGroup group)
        {
            List<TrainCarLivery> variants = new() { variant };

            foreach (var item in group.AdditionalLiveries)
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
