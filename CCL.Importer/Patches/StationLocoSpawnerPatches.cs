using CCL.Types;
using DV.ThingTypes;
using HarmonyLib;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;

namespace CCL.Importer.Patches
{
    [HarmonyPatch(typeof(StationLocoSpawner))]
    internal class StationLocoSpawnerPatches
    {
        private static Regex s_regex = new("\\[Y\\]_\\[([a-z0-9]*)\\]_\\[(.*)]", RegexOptions.IgnoreCase);

        [HarmonyPostfix, HarmonyPatch(nameof(StationLocoSpawner.Start))]
        private static void StartPostfix(StationLocoSpawner __instance)
        {
            StationSpawnChanceData.ClearDataIfNeeded();

            CCLPlugin.Log($"Finding loco spawn groups to inject into '{__instance.name}'");
            PrintSpawnerInfo(__instance);

            // Get the groups from the liveries.
            foreach (var car in CarManager.CustomCarTypes)
            {
                if (!CarManager.IsCarTypeEnabled(car)) continue;

                foreach (var variant in car.Variants)
                {
                    foreach (var group in variant.LocoSpawnGroups)
                    {
                        // Safety check.
                        if (group.IsDisallowedSpawn())
                        {
                            CCLPlugin.Error($"Attempted to add a custom loco ('{variant.id}') to a disallowed spawn track");
                            continue;
                        }

                        // If the group is supposed to use this spawner...
                        if (group.Track.ToName() == __instance.name)
                        {
                            var wrapper = FromGroup(variant, group);

                            CCLPlugin.LogVerbose($"Injecting loco spawn group [{string.Join(", ", wrapper.liveries.Select(x => x.id))}]...");

                            __instance.locoTypeGroupsToSpawn.Add(wrapper);
                        }
                    }
                }
            }

            // Randomise spawn index again so it doesn't always spawn vanilla the first time.
            __instance.nextLocoGroupSpawnIndex = Random.Range(0, __instance.locoTypeGroupsToSpawn.Count);

            // Calculate spawn chances for this spawner to use in the catalog.
            // If the ID is not from a vanilla station, skip.
            //if (!TryToVanillaStationId(__instance.name, out string id)) return;
            //var id = RailTrackRegistry.RailTrackToLogicTrack[__instance.locoSpawnTrack].ID.yardId;

            // Try to match the track name to a station ID.
            // If successful, add it to the spawn chance data calculation.
            if (!TryMatchName(__instance.locoSpawnTrackName, out string id)) return;
            StationSpawnChanceData.AddData(id, __instance);
        }

        private static bool TryToVanillaStationId(string name, out string id)
        {
            return LocoSpawnGroup.SpawnerNameToId.TryGetValue(name, out id);
        }

        private static bool TryMatchName(string name, out string id)
        {
            var match = s_regex.Match(name);

            if (match.Success)
            {
                id = match.Groups[1].Value;
                return true;
            }

            id = string.Empty;
            return false;
        }

        private static ListTrainCarTypeWrapper FromGroup(TrainCarLivery variant, LocoSpawnGroup group)
        {
            // Add livery at the start if it is missing from the group.
            List<TrainCarLivery> variants = group.AdditionalLiveries.Contains(variant.id) ? new() : new() { variant };

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
            sb.Append($"Length (usable/total): {Mathf.FloorToInt(length) - 3}m/{length:F4}m\n");

            var groups = spawner.locoTypeGroupsToSpawn.Select(x => string.Join(", ", x.liveries.Select(l => l.id)));
            sb.Append($"Default spawns:\n    [{string.Join("],\n    [", groups)}]");

            CCLPlugin.LogVerbose(sb.ToString());
        }
    }
}
