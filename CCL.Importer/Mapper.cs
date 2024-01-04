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
                .ForMember(s => s.scriptsToDisable, o => o.MapFrom(s => s.scriptsToDisable.Select(x => GetFromCache(x) ?? x)));
        }

        public static bool ReplaceAll<T>(T _) => true;
       
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

        /// <summary>
        /// Replaces all instances of sourceType with instances of destType in prefab, after checking each one using canRplace to determine if replacement is allowed
        /// </summary>
        /// <param name="prefab">The Unity GameObject to update</param>
        /// <param name="sourceType">The type to replace</param>
        /// <param name="destType">The type to replace with</param>
        /// <param name="canReplace"></param>
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
                if (canReplace(source))
                {
                    MonoBehaviour destination = source.gameObject.AddComponent(destType) as MonoBehaviour;
                    s_componentMapCache.Add(source, destination);
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
