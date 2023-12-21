using AutoMapper;
using CCL.Importer.Types;
using CCL.Types;
using CCL.Types.Effects;
using DV.ThingTypes;
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

        private static Dictionary<MonoBehaviour, MonoBehaviour> s_componentMapCache = new();

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
                .ForMember(s => s.scriptsToDisable, o => o.MapFrom(s => s.scriptsToDisable.Select(x => GetMappedComponent(x))));
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
                    M.Map(source, destination);
                    Object.Destroy(source);
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
            M.Map(source, destination);
            Object.Destroy(source);
            return destination;
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
                Object.Destroy(item.Source);
            }

            // Return only the new components.
            destination = destinationList.Select(x => x.Destination);
        }

        private static MonoBehaviour GetMappedComponent<TSource>(TSource source)
            where TSource : MonoBehaviour
        {
            //// https://www.youtube.com/watch?v=rmQFcVR6vEs
            //return source.gameObject.GetComponent(M.ConfigurationProvider.GetAllTypeMaps()
            //        .First(map => map.SourceType == source.GetType()).DestinationType);

            if (s_componentMapCache.TryGetValue(source, out MonoBehaviour mapped))
            {
                return mapped;
            }

            return source;
        }

        public static void ClearCache()
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
