using CCL_GameScripts;
using DV.Logic.Job;
using HarmonyLib;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace DVCustomCarLoader
{
    public static class Extensions
    {
        public static bool IsEnvironmental( this ResourceType type )
        {
            return
                (type == ResourceType.EnvironmentDamageCargo) ||
                (type == ResourceType.EnvironmentDamageFuel) ||
                (type == ResourceType.EnvironmentDamageCoal);
        }

        public static IEnumerable<T> GetComponentsByInterface<T>( this GameObject gameObject )
            where T : class
        {
            if( !typeof(T).IsInterface )
            {
                throw new ArgumentException($"GetComponentsInChildrenByInterface - Type {typeof(T).Name} is not an interface");
            }
            if( !gameObject )
            {
                throw new ArgumentNullException("gameObject");
            }

            return gameObject.GetComponents<MonoBehaviour>()
                .Where(comp => comp && comp.GetType().GetInterfaces().Contains(typeof(T)))
                .Cast<T>();
        }

        public static T GetComponentByInterface<T>( this GameObject gameObject )
            where T : class
        {
            if( !typeof(T).IsInterface )
            {
                throw new ArgumentException($"GetComponentsInChildrenByInterface - Type {typeof(T).Name} is not an interface");
            }
            if( !gameObject )
            {
                throw new ArgumentNullException("gameObject");
            }

            return gameObject.GetComponents<MonoBehaviour>()
                .FirstOrDefault(comp => comp && comp.GetType().GetInterfaces().Contains(typeof(T)))
                as T;
        }

        public static IEnumerable<T> GetComponentsInChildrenByInterface<T>( this GameObject gameObject )
            where T : class
        {
            if( !typeof(T).IsInterface )
            {
                throw new ArgumentException($"GetComponentsInChildrenByInterface - Type {typeof(T).Name} is not an interface");
            }
            if( !gameObject )
            {
                throw new ArgumentNullException("gameObject");
            }

            return gameObject.GetComponentsInChildren<MonoBehaviour>(true)
                .Where(comp => comp && comp.GetType().GetInterfaces().Contains(typeof(T)))
                .Cast<T>();
        }

        public static bool EqualsOneOf<T>( this T compare, params T[] values )
        {
            foreach( T v in values )
            {
                if( compare.Equals(v) ) return true;
            }
            return false;
        }

        public static bool SafeAny<T>(this IEnumerable<T> array)
        {
            return (array != null) && array.Any();
        }

        public static float Mapf( float fromMin, float fromMax, float toMin, float toMax, float value )
        {
            float fromRange = fromMax - fromMin;
            float toRange = toMax - toMin;
            return (value - fromMin) * (toRange / fromRange) + toMin;
        }
        
        public static bool IsCustomCargoClass(this CargoContainerType containerType)
        {
            return containerType == (CargoContainerType)BaseCargoContainerType.Custom;
        }

        public static bool IsCustomCargoType(this CargoType cargoType)
        {
            return cargoType == (CargoType)BaseCargoType.Custom;
        }
        public static bool TryDequeue<T>(this Queue<T> queue, out T result)
        {
            if (queue.Count == 0)
            {
                result = default(T);
                return false;
            }

            result = queue.First();
            return true;
        }

        public static void Deconstruct<TKey, TValue>(this KeyValuePair<TKey, TValue> kvp, out TKey key, out TValue value)
        {
            key = kvp.Key;
            value = kvp.Value;
        }
    }

    public static class AccessToolsExtensions
    {
        public static void SaveTo<T>(this FieldInfo field, out T dest, object source = null)
        {
            if (field != null)
            {
                dest = (T)field.GetValue(source);
            }
            else
            {
                dest = default;
            }
        }

        public static void SaveTo<T>(this PropertyInfo property, out T dest, object source = null)
        {
            if (property != null)
            {
                dest = (T)property.GetValue(source);
            }
            else
            {
                dest = default;
            }
        }
    }

    public class WrappedEnumerator : IEnumerator
    {
        private readonly IEnumerator enumerator;
        public event Action OnMoveNext;
        public event Action OnComplete;

        public WrappedEnumerator(IEnumerator toWrap)
        {
            enumerator = toWrap;
        }

        public object Current => enumerator.Current;

        public bool MoveNext()
        {
            bool result = enumerator.MoveNext();

            OnMoveNext?.Invoke();
            if (!result)
            {
                OnComplete?.Invoke();
            }

            return result;
        }

        public void Reset()
        {
            enumerator.Reset();
        }
    }

    [HarmonyPatch(typeof(Enum), nameof(Enum.IsDefined))]
    public static class EnumPatch
    {
        public static bool Prefix(Type enumType, object value, ref bool __result)
        {
            if ((enumType == typeof(TrainCarType)) && (value is TrainCarType carType))
            {
                __result = CarTypeInjector.IsCustomTypeRegistered(carType);
                if (__result) return false;
            }
            else if ((enumType == typeof(CargoType)) && (value is CargoType cargoType))
            {
                __result = CustomCargoInjector.IsCustomTypeRegistered(cargoType);
                if (__result) return false;
            }
            return true;
        }
    }
}
