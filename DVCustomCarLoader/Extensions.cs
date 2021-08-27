using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
                .Where(comp => comp.GetType().GetInterfaces().Contains(typeof(T)))
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
                .FirstOrDefault(comp => comp.GetType().GetInterfaces().Contains(typeof(T)))
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
                .Where(comp => comp.GetType().GetInterfaces().Contains(typeof(T)))
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
    }
}
