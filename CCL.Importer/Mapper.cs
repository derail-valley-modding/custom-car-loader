using AutoMapper;
using AutoMapper.Internal;
using CCL.Importer.Types;
using CCL.Types;
using CCL.Types.Proxies;
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
        private static readonly MapperConfiguration _config = new(Configure);

        private static IMapper? _map;
        public static IMapper M => _map ??= _config.CreateMapper();

        private static readonly Dictionary<MonoBehaviour, MonoBehaviour> s_componentMapCache = new();

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

            cfg.CreateMap<TeleportArcPassThroughProxy, TeleportArcPassThrough>();
            cfg.CreateMap<WindowProxy, DV.Rain.Window>()
                .ForMember(x => x.duplicates, options => options.Ignore());
            cfg.CreateMap<InternalExternalSnapshotSwitcherProxy, DV.InternalExternalSnapshotSwitcher>();

            cfg.CreateMap<PlayerDistanceGameObjectsDisablerProxy, PlayerDistanceGameObjectsDisabler>()
                .ForMember(d => d.disableSqrDistance, o => o.MapFrom(d => d.disableDistance * d.disableDistance));
            // Make sure these maps are run LAST.
            cfg.CreateMap<PlayerDistanceMultipleGameObjectsOptimizerProxy, PlayerDistanceMultipleGameObjectsOptimizer>()
                .ForMember(d => d.disableSqrDistance, o => o.MapFrom(d => d.disableDistance * d.disableDistance))
                .ForMember(s => s.scriptsToDisable, o => o.MapFrom(s => s.scriptsToDisable.Select(x => GetFromCache(x))));
        }

        /// <summary>
        /// Replaces TSource with TDestination using AutoMapper
        /// </summary>
        /// <typeparam name="TSource">The MonoBehaviour script to replace</typeparam>
        /// <typeparam name="TDestination">The MonoBehaviour script that will replace it</typeparam>
        /// <param name="prefab">The game object on which to conduct replacements - operates recursively</param>
        public static void MapComponentsInChildren<TSource, TDestination>(this GameObject prefab)
            where TSource : MonoBehaviour
            where TDestination : MonoBehaviour
        {
            prefab.MapComponentsInChildren<TSource, TDestination>((source) => true);
        }

        public static void MapComponentsInChildren<TSource, TDestination>(this GameObject prefab, System.Func<TSource, bool> canReplace)
            where TSource : MonoBehaviour
            where TDestination : MonoBehaviour
        {
            foreach (var source in prefab.GetComponentsInChildren<TSource>())
            {
                if (canReplace(source))
                {
                    var destination = source.gameObject.AddComponent<TDestination>();
                    s_componentMapCache.Add(source, destination);
                    M.Map(source, destination);
                    UnityEngine.Object.Destroy(source);
                }
            }
        }

        public static void MapComponentsInChildren(this GameObject prefab, Type sourceType, Type destinationType)
        {
            prefab.MapComponentsInChildren(sourceType, destinationType, _=>true);
        }

        /// <summary>
        /// Replaces sourceType with DestinationType using AutoMapper <see cref="MapComponentsInChildren{TSource, TDestination}(GameObject, Func{TSource, bool})"/>
        /// </summary>
        /// <param name="prefab"></param>
        /// <param name="sourceType"></param>
        /// <param name="destinationType"></param>
        /// <param name="canReplace"></param>
        public static void MapComponentsInChildren(this GameObject prefab, Type sourceType, Type destinationType, Predicate<MonoBehaviour> canReplace)
        {
            if (!sourceType.GetTypeInheritance().Contains(typeof(MonoBehaviour)) || 
                !destinationType.GetTypeInheritance().Contains(typeof(MonoBehaviour)))
            {
                CCLPlugin.Warning("Attempted to map an unsupported type - only monobheaviours are supported");
                return;
            }
            foreach (MonoBehaviour source in prefab.GetComponentsInChildren(sourceType))
            {
                if (canReplace(source))
                {
                    MonoBehaviour destination = (MonoBehaviour)source.gameObject.AddComponent(destinationType);
                    s_componentMapCache.Add(source, destination);
                    M.Map(source, destination, sourceType, destinationType);
                    UnityEngine.Object.Destroy(source);
                }
            }
        }

        public static void StoreComponentsInChildrenInCache<TSource, TDestination>(this GameObject prefab, System.Func<TSource, bool> canReplace)
            where TSource : MonoBehaviour
            where TDestination : MonoBehaviour
        {
            foreach (var source in prefab.GetComponentsInChildren<TSource>())
            {
                if (canReplace(source))
                {
                    var destination = source.gameObject.AddComponent<TDestination>();
                    s_componentMapCache.Add(source, destination);
                }
            }
        }

        public static void StoreComponentsInChildrenInCache(this GameObject prefab, Type sourceType, Type destType, Predicate<MonoBehaviour> canReplace)
        {
            if (!sourceType.GetTypeInheritance().Contains(typeof(MonoBehaviour)) ||
                !destType.GetTypeInheritance().Contains(typeof(MonoBehaviour)))
            {
                CCLPlugin.Warning("Attempted to map an unsupported type - only monobheaviours are supported");
                return;
            }
            foreach(MonoBehaviour source in prefab.GetComponentsInChildren(sourceType))
            {
                MonoBehaviour destination = source.gameObject.AddComponent(destType) as MonoBehaviour;
                s_componentMapCache.Add(source, destination);
            }
        }

        public static void ConvertFromCache<TSource, TDestination>(this GameObject prefab, System.Func<TSource, bool> canReplace)
            where TSource : MonoBehaviour
            where TDestination : MonoBehaviour
        {
            foreach (var source in prefab.GetComponentsInChildren<TSource>())
            {
                if (canReplace(source))
                {
                    MonoBehaviour storedValue;
                    s_componentMapCache.TryGetValue(source, out storedValue);
                    if (null != storedValue)
                    {
                        Mapper.M.Map(source, (TDestination)storedValue);
                        GameObject.Destroy(source);
                    }
                }
            }
        }

        public static void ConvertFromCache(this GameObject prefab, Type sourceType, Type destType, Predicate<MonoBehaviour> canReplace)
        {
            if (!sourceType.GetTypeInheritance().Contains(typeof(MonoBehaviour)) ||
                !destType.GetTypeInheritance().Contains(typeof(MonoBehaviour)))
            {
                CCLPlugin.Warning("Attempted to map an unsupported type - only monobheaviours are supported");
                return;
            }
            foreach (MonoBehaviour source in prefab.GetComponentsInChildren(sourceType))
            {
                if (canReplace(source))
                {
                   s_componentMapCache.TryGetValue(source, out MonoBehaviour storedValue);
                    if (null != storedValue && destType.IsInstanceOfType(storedValue))
                    {
                        Mapper.M.Map(source, storedValue, sourceType, destType);
                        GameObject.Destroy(source);
                    }
                }
            }
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
        public static TDestination MapComponent<TSource, TDestination>(this TSource source)
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

        public static void MapMultipleComponents<TSource, TDestination>(IEnumerable<TSource> source, out IEnumerable<TDestination> destination)
            where TSource : MonoBehaviour
            where TDestination : MonoBehaviour
        {
            List<(TSource Source, TDestination Destination)> destinationList = new();

            // Start by adding all components.
            foreach (var item in source)
            {
                destinationList.Add((item, item.gameObject.AddComponent<TDestination>()));
            }

            // Once all components are added, it's safe to map them.
            foreach (var item in destinationList)
            {
                M.Map(item.Source, item.Destination);
                s_componentMapCache.Add(item.Source, item.Destination);
                UnityEngine.Object.Destroy(item.Source);
            }

            // Return only the new components.
            destination = destinationList.Select(x => x.Destination);
        }

        public static void ClearCache()
        {
            s_componentMapCache.Clear();
        }

        internal static MonoBehaviour GetFromCache(MonoBehaviour source)
        {
            MonoBehaviour output = null;
            s_componentMapCache.TryGetValue(source, out output);
            return output;
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
