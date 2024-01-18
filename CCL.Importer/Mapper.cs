using AutoMapper;
using CCL.Importer.Types;
using CCL.Types;
using DV.ThingTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace CCL.Importer
{
    internal static class Mapper
    {
        private static readonly List<ICacheConfig> s_configCache = new();
        private static readonly Dictionary<MonoBehaviour, MonoBehaviour> s_componentMapCache = new();
        // Mapper needs to be declared after the caches, or it will cause reflection problems,
        // as the config will try to access the caches.
        private static readonly MapperConfiguration _config = new(Configure);

        private static IMapper? _map;
        public static IMapper M => _map ??= _config.CreateMapper();

        // This interface is only used to be able to store the generic type without the generics.
        private interface ICacheConfig
        {
            public void StoreComponentsInChildrenInCache(GameObject prefab);

            public void ConvertFromCache(GameObject prefab);
        }

        private class CacheConfig<TSource, TDestination> : ICacheConfig
            where TSource : MonoBehaviour
            where TDestination : MonoBehaviour
        {
            private Predicate<TSource>? _shouldMap;

            public CacheConfig()
            {
                _shouldMap = null;
            }

            public CacheConfig(Predicate<TSource> shouldMap)
            {
                _shouldMap = shouldMap;
            }

            public void StoreComponentsInChildrenInCache(GameObject prefab)
            {
                foreach (var source in prefab.GetComponentsInChildren<TSource>())
                {
                    // If there is no condition or the condition is true.
                    if (_shouldMap == null || _shouldMap(source))
                    {
                        var destination = source.gameObject.AddComponent<TDestination>();
                        s_componentMapCache.Add(source, destination);
                    }
                }
            }
            public void ConvertFromCache(GameObject prefab)
            {
                foreach (MonoBehaviour source in prefab.GetComponentsInChildren<TSource>())
                {
                    if (s_componentMapCache.TryGetValue(source, out MonoBehaviour cached))
                    {
                        M.Map(source, cached);
                        UnityEngine.Object.Destroy(source);
                    }
                }
            }
        }

        /// <summary>
        /// Adds a type map config to be automatically processed.
        /// </summary>
        /// <typeparam name="TSource">The proxy component type.</typeparam>
        /// <typeparam name="TDestination">The real component type.</typeparam>
        internal static void AddConfig<TSource, TDestination>()
            where TSource : MonoBehaviour
            where TDestination : MonoBehaviour
        {
            ICacheConfig config = new CacheConfig<TSource, TDestination>();
            s_configCache.Add(config);
        }

        /// <summary>
        /// Adds a type map config to be automatically processed.
        /// </summary>
        /// <typeparam name="TSource">The proxy component type.</typeparam>
        /// <typeparam name="TDestination">The real component type.</typeparam>
        /// <param name="shouldMap">The condition that must be met for the map to be possible.</param>
        internal static void AddConfig<TSource, TDestination>(Predicate<TSource> shouldMap)
            where TSource : MonoBehaviour
            where TDestination : MonoBehaviour
        {
            s_configCache.Add(new CacheConfig<TSource, TDestination>(shouldMap));
        }

        /// <summary>
        /// Process all the proxy maps of a prefab.
        /// </summary>
        internal static void ProcessConfigs(GameObject prefab)
        {
            // First create all the real components from each proxy.
            foreach (var item in s_configCache)
            {
                item.StoreComponentsInChildrenInCache(prefab);
            }

            // Then map all components that were created.
            foreach (var item in s_configCache)
            {
                item.ConvertFromCache(prefab);
            }
        }

        private static void Configure(IMapperConfigurationExpression cfg)
        {
            cfg.CreateMap<CustomCarVariant, CCL_CarVariant>()
                .ForMember(c => c.parentType, o => o.Ignore());

            cfg.CreateMap<CustomCarType, CCL_CarType>()
                .ForMember(c => c.carInstanceIdGenBase, o => o.MapFrom(ccl => ccl.carIdPrefix))
                .ForMember(c => c.liveries, o => o.ConvertUsing(new LiveriesConverter()));

            cfg.CreateMap<CustomCarType.BrakesSetup, TrainCarType_v2.BrakesSetup>();
            cfg.CreateMap<CustomCarType.DamageSetup, TrainCarType_v2.DamageSetup>();
            cfg.AddMaps(Assembly.GetExecutingAssembly());
        }

        /// <summary>
        /// Replaces TSource with TDestination using AutoMapper
        /// 
        /// Unlike MapComponentsInChildren this will only map one and targets a *source* instead of a *prefab*
        /// </summary>
        /// <typeparam name="TSource">Type of the component being replaced</typeparam>
        /// <typeparam name="TDestination">Type of component to replace with</typeparam>
        /// <param name="source">Component being replaced, will be destroyed before this returns</param>
        /// <returns>The replaced component which can be used in other mappers</returns>
        public static TDestination MapComponent<TSource, TDestination>(TSource source)
            where TSource : MonoBehaviour
            where TDestination: MonoBehaviour
        {
            var destination = source.gameObject.AddComponent<TDestination>();
            s_componentMapCache.Add(source, destination);
            M.Map(source, destination);
            UnityEngine.Object.Destroy(source);
            return destination;
        }

        /// <summary>
        /// Replaces TSource with TDestination using AutoMapper
        /// 
        /// Unlike MapComponentsInChildren this will only map one and targets a *source* instead of a *prefab*
        /// </summary>
        /// <typeparam name="TSource">Type of the component being replaced</typeparam>
        /// <typeparam name="TDestination">Type of component to replace with</typeparam>
        /// <param name="source">Component being replaced, will be destroyed before this returns</param>
        /// <param name="destination">The component added to replace the original</param>
        public static void MapComponent<TSource, TDestination>(TSource source, out TDestination destination)
            where TSource : MonoBehaviour
            where TDestination : MonoBehaviour
        {
            destination = MapComponent<TSource, TDestination>(source);
        }

        /// <summary>
        /// Gets the mapped version of a <see cref="MonoBehaviour"/> if one has been cached.
        /// </summary>
        /// <param name="source">The source (usually proxy) component.</param>
        /// <returns>The mapped <see cref="MonoBehaviour"/>. If there is no mapped version, <c>null</c>.</returns>
        internal static MonoBehaviour GetFromCache(MonoBehaviour source)
        {
            s_componentMapCache.TryGetValue(source, out MonoBehaviour output);
            return output;
        }

        /// <summary>
        /// Gets an enumerable in which each <see cref="MonoBehaviour"/> is its mapped version if one has been cached.
        /// </summary>
        /// <param name="source">The enumerable of source (usually proxies) components.</param>
        /// <returns>The enumerable of <see cref="MonoBehaviour"/>s. If there is no mapped version, it may contain <c>null</c> values.</returns>
        internal static IEnumerable<MonoBehaviour> GetFromCache(IEnumerable<MonoBehaviour> source)
        {
            return source.Select(scr => GetFromCache(scr));
        }

        /// <summary>
        /// Gets the mapped version of a <see cref="MonoBehaviour"/> if one has been cached.
        /// </summary>
        /// <param name="source">The source (usually proxy) component.</param>
        /// <returns>The mapped <see cref="MonoBehaviour"/>. If there is no mapped version, it will return instead <paramref name="source"/>.</returns>
        internal static MonoBehaviour GetFromCacheOrSelf(MonoBehaviour source)
        {
            s_componentMapCache.TryGetValue(source, out MonoBehaviour output);
            return output ?? source;
        }

        /// <summary>
        /// Gets an enumerable in which each <see cref="MonoBehaviour"/> is its mapped version if one has been cached.
        /// </summary>
        /// <param name="source">The enumerable of source (usually proxies) components.</param>
        /// <returns>
        /// A collection of <see cref="MonoBehaviour"/>. If there is no mapped version for a given one, it will be the original <see cref="MonoBehaviour"/>
        /// that was in <paramref name="source"/>.
        /// </returns>
        internal static IEnumerable<MonoBehaviour> GetFromCacheOrSelf(IEnumerable<MonoBehaviour> source)
        {
            return source.Select(scr => GetFromCacheOrSelf(scr));
        }

        public static void ClearComponentCache()
        {
            s_componentMapCache.Clear();
        }

        private class LiveriesConverter : IValueConverter<List<CustomCarVariant>, List<TrainCarLivery>>
        {
            public List<TrainCarLivery> Convert(List<CustomCarVariant> sourceMember, ResolutionContext context)
            {
                return sourceMember.Select(v =>
                {
                    var l = ScriptableObject.CreateInstance<CCL_CarVariant>();
                    M.Map(v, l);
                    return l;
                }).Cast<TrainCarLivery>().ToList();
            }
        }
    }
}
