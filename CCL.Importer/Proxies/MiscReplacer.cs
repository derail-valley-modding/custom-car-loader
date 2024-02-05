using AutoMapper;
using CCL.Types.Proxies;
using DV;

namespace CCL.Importer.Proxies
{
    internal class MiscReplacer : Profile
    {
        public MiscReplacer()
        {
            ShouldMapField = f => AutoMapperHelper.IsPublicOrSerialized(f);

            CreateMap<TeleportArcPassThroughProxy, TeleportArcPassThrough>();
            CreateMap<InternalExternalSnapshotSwitcherProxy, InternalExternalSnapshotSwitcher>()
                .AutoCacheAndMap();

            CreateMap<ExplosionModelHandlerProxy, ExplosionModelHandler>().AutoCacheAndMap().AfterMap((x, y) =>
                CCLPlugin.Log($"{x.gameObjectSwaps.Length} -> {y.gameObjectSwaps.Length}"));
            CreateMap<ExplosionModelHandlerProxy.MaterialSwapData, ExplosionModelHandler.MaterialSwapData>();
            CreateMap<ExplosionModelHandlerProxy.GameObjectSwapData, ExplosionModelHandler.GameObjectSwapData>();

            CreateMap<PlayerDistanceGameObjectsDisablerProxy, PlayerDistanceGameObjectsDisabler>()
                .AutoCacheAndMap()
                .ForMember(d => d.disableSqrDistance, o => o.MapFrom(d => d.disableDistance * d.disableDistance));
            CreateMap<PlayerDistanceMultipleGameObjectsOptimizerProxy, PlayerDistanceMultipleGameObjectsOptimizer>()
                .AutoCacheAndMap()
                .ForMember(d => d.disableSqrDistance, o => o.MapFrom(d => d.disableDistance * d.disableDistance))
                .ForMember(s => s.scriptsToDisable, o => o.MapFrom(s => Mapper.GetFromCacheOrSelf(s.scriptsToDisable)));
        }
    }
}
