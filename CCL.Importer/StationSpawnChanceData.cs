using DV.ThingTypes;
using System.Collections.Generic;
using System.Linq;

namespace CCL.Importer
{
    public class StationSpawnChanceData
    {
        private static bool s_flagForClear = false;

        public static readonly Dictionary<string, StationSpawnChanceData> Data = new();

        private class StationSpawnTrack
        {
            public string Name;
            public List<ListTrainCarTypeWrapper> SpawnGroups;

            public StationSpawnTrack(StationLocoSpawner spawner)
            {
                Name = spawner.name;
                SpawnGroups = spawner.locoTypeGroupsToSpawn;
            }

            public float GetChance(TrainCarLivery livery)
            {
                // If there are no groups, chance is 0. Failsafe to prevent division by 0.
                if (SpawnGroups.Count == 0) return 0.0f;
                // Chance is # of groups with the livery divided by total groups.
                return SpawnGroups.Count(x => x.liveries.Any(l => l.parentType == livery.parentType)) / (float)SpawnGroups.Count;
            }

            // For debugging, displays the group in RUE without needing to check every livery individually.
            private string QuickString => $"[{string.Join("], [", SpawnGroups.Select(x => string.Join(", ", x.liveries.Select(l => l.id))))}]";
        }

        private Dictionary<TrainCarLivery, float> _chances = new();
        private List<StationSpawnTrack> _spawnTracks = new();

        public float GetChance(TrainCarLivery livery)
        {
            if (_chances.TryGetValue(livery, out var chance)) return chance;

            // Start inverted.
            chance = 1.0f;

            foreach (var track in _spawnTracks)
            {
                // Calculate chance of not spawning.
                chance *= 1.0f - track.GetChance(livery);
            }

            // Complement of chance of not spawning is the chance of spawning at least 1.
            _chances[livery] = chance = 1 - chance;
            return chance;
        }

        public static void AddData(string id, StationLocoSpawner spawner)
        {
            if (!Data.TryGetValue(id, out var data))
            {
                data = new();
                Data.Add(id, data);
            }
            
            data._spawnTracks.Add(new StationSpawnTrack(spawner));
        }

        public static void ClearDataIfNeeded()
        {
            if (s_flagForClear)
            {
                CCLPlugin.Log("Clearing station spawn data...");
                Data.Clear();
                s_flagForClear = false;
            }
        }

        internal static void FlagForClearing()
        {
            s_flagForClear = true;
        }
    }
}
