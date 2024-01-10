using AutoMapper;
using CCL.Types.Proxies;
using DV;
using DV.Rain;
using System.ComponentModel.Composition;
using UnityEngine;

namespace CCL.Importer.Proxies
{
    [Export(typeof(IProxyReplacer))]
    internal class MiscReplacer : Profile, IProxyReplacer
    {
        public MiscReplacer()
        {
            CreateMap<TeleportArcPassThroughProxy, TeleportArcPassThrough>();
            CreateMap<InternalExternalSnapshotSwitcherProxy, InternalExternalSnapshotSwitcher>();

            CreateMap<WindowProxy, Window>()
                .ForMember(d => d.duplicates, o => o.MapFrom(s => Mapper.GetFromCache(s.duplicates)));

            CreateMap<PlayerDistanceGameObjectsDisablerProxy, PlayerDistanceGameObjectsDisabler>()
                .ForMember(d => d.disableSqrDistance, o => o.MapFrom(d => d.disableDistance * d.disableDistance));
            CreateMap<PlayerDistanceMultipleGameObjectsOptimizerProxy, PlayerDistanceMultipleGameObjectsOptimizer>()
                .ForMember(d => d.disableSqrDistance, o => o.MapFrom(d => d.disableDistance * d.disableDistance))
                .ForMember(s => s.scriptsToDisable, o => o.MapFrom(s => Mapper.GetFromCacheOrSelf(s.scriptsToDisable)));
        }

        public void CacheAndReplaceProxies(GameObject prefab)
        {
            prefab.StoreComponentsInChildrenInCache<WindowProxy, Window>();
            prefab.StoreComponentsInChildrenInCache<PlayerDistanceGameObjectsDisablerProxy, PlayerDistanceGameObjectsDisabler>();
            prefab.StoreComponentsInChildrenInCache<PlayerDistanceMultipleGameObjectsOptimizerProxy, PlayerDistanceMultipleGameObjectsOptimizer>();
        }

        public void MapProxies(GameObject prefab)
        {
            prefab.ConvertFromCache<InternalExternalSnapshotSwitcherProxy, InternalExternalSnapshotSwitcher>();
            prefab.ConvertFromCache<WindowProxy, Window>();
            prefab.ConvertFromCache<PlayerDistanceGameObjectsDisablerProxy, PlayerDistanceGameObjectsDisabler>();
            prefab.ConvertFromCache<PlayerDistanceMultipleGameObjectsOptimizerProxy, PlayerDistanceMultipleGameObjectsOptimizer>();
        }

        public void ReplaceProxiesUncached(GameObject prefab) { }
    }
}
