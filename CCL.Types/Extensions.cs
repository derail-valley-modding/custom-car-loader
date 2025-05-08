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

        public static bool IsDefined<T>(this T enumValue)
            where T : Enum
        {
            return Enum.IsDefined(typeof(T), enumValue);
        }
        public static string ToCamelCase(this string str) =>
            !string.IsNullOrEmpty(str) && str.Length > 1 ? char.ToLowerInvariant(str[0]) + str.Substring(1) : str.ToLowerInvariant();

        public static T AddComponentCopy<T>(this GameObject go, T component) where T : Component
        {
            return ComponentUtil.CopyComponent(component, go.AddComponent<T>());
        }

        // https://stackoverflow.com/a/10120982
        public static float ClosestTo(this IEnumerable<float> collection, float target)
        {
            var closest = float.PositiveInfinity;
            var minDifference = float.PositiveInfinity;

            foreach (var element in collection)
            {
                var difference = Math.Abs(element - target);
                if (minDifference > difference)
                {
                    minDifference = difference;
                    closest = element;
                }
            }

            return closest;
        }

        public static Vector3 Flattened(this Vector3 vector)
        {
            return new Vector3(vector.x, 0, vector.z);
        }

        public static Vector2 FlattenedVector2(this Vector3 vector)
        {
            return new Vector2(vector.x, vector.z);
        }

        public static void Reset(this Transform transform)
        {
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity;
            transform.localScale = Vector3.one;
        }

        public static void CopyLocal(this Transform target, Transform source)
        {
            target.localPosition = source.localPosition;
            target.localRotation = source.localRotation;
            target.localScale = source.localScale;
        }

        public static bool TryFind(this Transform transform, string n, out Transform result)
        {
            result = transform.Find(n);

            return result != null;
        }

        public static int FirstIndexMatch<TItem>(this IEnumerable<TItem> items, Predicate<TItem> condition)
        {
            int index = 0;

            foreach (var item in items)
            {
                if (condition(item)) return index;

                index++;
            }

            return -1;
        }
    }
}
