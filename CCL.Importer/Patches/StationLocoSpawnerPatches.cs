using CCL.Types;
using DV.ThingTypes;
using HarmonyLib;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace CCL.Importer.Patches
{
    [HarmonyPatch(typeof(StationLocoSpawner))]
    internal class StationLocoSpawnerPatches
    {
        [HarmonyPostfix, HarmonyPatch(nameof(StationLocoSpawner.Start))]
        private static void StartPostfix(StationLocoSpawner __instance)
        {
            StationSpawnChanceData.ClearDataIfNeeded();

            CCLPlugin.Log($"Finding loco spawn groups to inject into '{__instance.name}'");
            PrintSpawnerInfo(__instance);

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
                            if (group.AdditionalLiveries.Length > 0)
                            {
                                CCLPlugin.LogVerbose($"Injecting loco spawn group [{variant.id}, {string.Join(", ", group.AdditionalLiveries)}]");
                            }
                            else
                            {
                                CCLPlugin.LogVerbose($"Injecting loco spawn group [{variant.id}]");
                            }
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

            //var id = RailTrackRegistry.RailTrackToLogicTrack[__instance.locoSpawnTrack].ID.yardId;
            StationSpawnChanceData.AddData(id, __instance);
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

        private static void PrintSpawnerInfo(StationLocoSpawner spawner)
        {
            var length = (float)RailTrackRegistry.RailTrackToLogicTrack[spawner.locoSpawnTrack].length;
            var sb = new StringBuilder("Additional info:\n");
            sb.AppendLine($"Length (usable/total): {Mathf.FloorToInt(length) - 3}m/{length:F4}m");

            var groups = spawner.locoTypeGroupsToSpawn.Select(x => string.Join(", ", x.liveries.Select(l => l.id)));
            sb.AppendLine($"Default spawns:\n    [{string.Join("],\n    [", groups)}]");

            CCLPlugin.LogVerbose(sb.ToString());
        }
    }
}
