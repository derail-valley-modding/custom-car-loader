using CCL.Types;
using DV.Simulation.Controllers;
using DV.ThingTypes;
using DV.ThingTypes.TransitionHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace CCL.Importer
{
    public static class Extensions
    {
        public static bool IsEnvironmental(this ResourceType_v2 type)
        {
            return type.canDamageEnvironment;
        }

        public static IEnumerable<T> GetComponentsByInterface<T>(this GameObject gameObject)
            where T : class
        {
            if (!typeof(T).IsInterface)
            {
                throw new ArgumentException($"GetComponentsInChildrenByInterface - Type {typeof(T).Name} is not an interface");
            }
            if (!gameObject)
            {
                throw new ArgumentNullException("gameObject");
            }

            return gameObject.GetComponents<MonoBehaviour>()
                .Where(comp => comp && comp.GetType().GetInterfaces().Contains(typeof(T)))
                .Cast<T>();
        }

        public static T? GetComponentByInterface<T>(this GameObject gameObject)
            where T : class
        {
            if (!typeof(T).IsInterface)
            {
                throw new ArgumentException($"GetComponentsInChildrenByInterface - Type {typeof(T).Name} is not an interface");
            }
            if (!gameObject)
            {
                throw new ArgumentNullException("gameObject");
            }

            return gameObject.GetComponents<MonoBehaviour>()
                .FirstOrDefault(comp => comp && comp.GetType().GetInterfaces().Contains(typeof(T)))
                as T;
        }

        public static IEnumerable<T> GetComponentsInChildrenByInterface<T>(this GameObject gameObject)
            where T : class
        {
            if (!typeof(T).IsInterface)
            {
                throw new ArgumentException($"GetComponentsInChildrenByInterface - Type {typeof(T).Name} is not an interface");
            }
            if (!gameObject)
            {
                throw new ArgumentNullException("gameObject");
            }

            return gameObject.GetComponentsInChildren<MonoBehaviour>(true)
                .Where(comp => comp && comp.GetType().GetInterfaces().Contains(typeof(T)))
                .Cast<T>();
        }

        public static void RefreshChildren<T>(this ARefreshableChildrenController<T> controller)
            where T : MonoBehaviour
        {
            controller.entries = controller.gameObject.GetComponentsInChildren<T>(true);
        }

        public static bool EqualsOneOf<T>(this T compare, params T[] values)
        {
            foreach (T v in values)
            {
                if (compare!.Equals(v)) return true;
            }
            return false;
        }

        public static bool SafeAny<T>(this IEnumerable<T> array)
        {
            return (array != null) && array.Any();
        }

        public static bool TryFind<T>(this List<T> list, Predicate<T> match, out T value)
        {
            value = list.Find(match);

            if (value == null)
            {
                return false;
            }

            return true;
        }

        public static float Mapf(float fromMin, float fromMax, float toMin, float toMax, float value)
        {
            float fromRange = fromMax - fromMin;
            float toRange = toMax - toMin;
            return (value - fromMin) * (toRange / fromRange) + toMin;
        }

        public static T GetCached<T>(ref T? cacheValue, Func<T> getter) where T : class
        {
            cacheValue ??= getter();
            return cacheValue;
        }

        public static T GetCached<T>(ref T? cacheValue, Func<T> getter) where T : struct
        {
            if (!cacheValue.HasValue)
            {
                cacheValue = getter();
            }
            return cacheValue.Value;
        }

        //public static bool IsCustomCargoClass(this CargoContainerType containerType)
        //{
        //    return containerType == (CargoContainerType)BaseCargoContainerType.Custom;
        //}

        //public static bool IsCustomCargoType(this CargoType cargoType)
        //{
        //    return cargoType == (CargoType)BaseCargoType.Custom;
        //}
    }

    public static class AccessToolsExtensions
    {
        public static void SaveTo<T>(this FieldInfo field, out T dest, object? source = null)
        {
            if (field != null)
            {
                dest = (T)field.GetValue(source);
            }
            else
            {
                dest = default!;
            }
        }

        public static void SaveTo<T>(this PropertyInfo property, out T dest, object? source = null)
        {
            if (property != null)
            {
                dest = (T)property.GetValue(source);
            }
            else
            {
                dest = default!;
            }
        }
    }

    //[HarmonyPatch(typeof(Enum), nameof(Enum.IsDefined))]
    //public static class EnumPatch
    //{
    //    public static bool Prefix(Type enumType, object value, ref bool __result)
    //    {
    //        if ((enumType == typeof(TrainCarType)) && (value is TrainCarType carType))
    //        {
    //            __result = CarTypeInjector.IsCustomTypeRegistered(carType);
    //            if (__result) return false;
    //        }
    //        else if ((enumType == typeof(CargoType)) && (value is CargoType cargoType))
    //        {
    //            __result = CustomCargoInjector.IsCustomTypeRegistered(cargoType);
    //            if (__result) return false;
    //        }
    //        return true;
    //    }
    //}

    public static class EnumExtensions
    {
        public static GameObject ToTypePrefab(this BogieType bogie)
        {
            return ((TrainCarType)bogie).ToV2().prefab;
        }

        public static GameObject ToTypePrefab(this BufferType buffer)
        {
            return ((TrainCarType)buffer).ToV2().prefab;
        }
    }
}
