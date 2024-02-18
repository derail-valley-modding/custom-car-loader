using System;
using System.Collections.Generic;
using UnityEngine;

namespace CCL.Types
{
    public static class Extensions
    {
        public static List<T> GetComponentsInChildren<T>(this IEnumerable<GameObject> prefabs)
        {
            var list = new List<T>();

            foreach (var prefab in prefabs)
            {
                list.AddRange(prefab.gameObject.GetComponentsInChildren<T>());
            }

            return list;
        }

        public static string ToName(this SpawnTrack track)
        {
            return LocoSpawnGroup.TrackToSpawnerName[track];
        }

        // https://stackoverflow.com/a/444818
        public static bool Contains(this string source, string toCheck, StringComparison comp)
        {
            return source?.IndexOf(toCheck, comp) >= 0;
        }
    }
}
