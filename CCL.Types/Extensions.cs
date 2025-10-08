using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CCL.Types
{
    public static class Extensions
    {
        public static List<T> GetComponentsInChildren<T>(this IEnumerable<GameObject> prefabs, bool includeInactive = false)
        {
            var list = new List<T>();

            foreach (var prefab in prefabs)
            {
                list.AddRange(prefab.gameObject.GetComponentsInChildren<T>(includeInactive));
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

        public static void ResetLocal(this Transform transform)
        {
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity;
            transform.localScale = Vector3.one;
        }

        public static void CopyLocalFrom(this Transform target, Transform source)
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

        // Swaps the values at the 2 indexes of the list.
        public static void Swap<T>(this IList<T> list, int indexA, int indexB)
        {
            (list[indexB], list[indexA]) = (list[indexA], list[indexB]);
        }

        // https://stackoverflow.com/a/23797795
        public static bool ContainsDuplicates<T>(this IEnumerable<T> enumerable)
        {
            var knownKeys = new HashSet<T>();
            return enumerable.Any(item => !knownKeys.Add(item));
        }

        public static bool ContainsDuplicates<T, TIdentifier>(this IEnumerable<T> enumerable, Func<T, TIdentifier> discriminator)
        {
            var knownKeys = new HashSet<TIdentifier>();
            return enumerable.Any(item => !knownKeys.Add(discriminator(item)));
        }

        public static SelfValidationResult Pass(this ISelfValidation component, out string message)
        {
            message = string.Empty;
            return SelfValidationResult.Pass;
        }

        public static SelfValidationResult FailForNull(this ISelfValidation component, string name, out string message)
        {
            message = $"{name} cannot be null";
            return SelfValidationResult.Fail;
        }

        public static Component Self(this ISelfValidation component)
        {
            return (Component)component;
        }
    }
}
