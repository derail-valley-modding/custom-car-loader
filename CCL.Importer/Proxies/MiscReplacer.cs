using AutoMapper;
using CCL.Types;
using CCL.Types.Proxies;
using DV;

namespace CCL.Importer.Proxies
{
    internal class MiscReplacer : Profile
    {
        public MiscReplacer()
        {
            CreateMap<TeleportArcPassThroughProxy, TeleportArcPassThrough>();
            CreateMap<InternalExternalSnapshotSwitcherProxy, InternalExternalSnapshotSwitcher>()
                .AutoCacheAndMap();

            CreateMap<ExplosionModelHandlerProxy, ExplosionModelHandler>().AutoCacheAndMap();
            CreateMap<ExplosionModelHandlerProxy.MaterialSwapData, ExplosionModelHandler.MaterialSwapData>();
            CreateMap<ExplosionModelHandlerProxy.GameObjectSwapData, ExplosionModelHandler.GameObjectSwapData>();

            CreateMap<PlayerDistanceGameObjectsDisablerProxy, PlayerDistanceGameObjectsDisabler>()
                .AutoCacheAndMap()
                .ForMember(d => d.disableSqrDistance, o => o.MapFrom(d => d.disableDistance * d.disableDistance));
            CreateMap<PlayerDistanceMultipleGameObjectsOptimizerProxy, PlayerDistanceMultipleGameObjectsOptimizer>()
                .AutoCacheAndMap()
                .ForMember(d => d.disableSqrDistance, o => o.MapFrom(d => d.disableDistance * d.disableDistance))
                .ForMember(s => s.scriptsToDisable, o => o.MapFrom(s => Mapper.GetFromCacheOrSelf(s.scriptsToDisable)));

            CreateMap<InteriorNonStandardLayerProxy, InteriorNonStandardLayer>().AutoCacheAndMap()
                .AfterMap(InteriorNonStandardLayerAfter);
        }

        private void InteriorNonStandardLayerAfter(InteriorNonStandardLayerProxy src, InteriorNonStandardLayer dest)
        {
            if (src.includeChildren)
            {
                dest.gameObject.SetLayersRecursive(src.Layer);
            }
            else
            {
                dest.gameObject.SetLayer(src.Layer);
            }
        }
    }
}
