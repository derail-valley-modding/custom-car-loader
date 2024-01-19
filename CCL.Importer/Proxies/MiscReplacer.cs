using AutoMapper;
using CCL.Types.Proxies;
using DV;
using DV.Rain;

namespace CCL.Importer.Proxies
{
    internal class MiscReplacer : Profile
    {
        public MiscReplacer()
        {
            CreateMap<TeleportArcPassThroughProxy, TeleportArcPassThrough>();
            CreateMap<WindowProxy, Window>()
                .AutoCacheAndMap()
                .ForMember(d => d.duplicates, o => o.MapFrom(s => Mapper.GetFromCache(s.duplicates)));
            CreateMap<InternalExternalSnapshotSwitcherProxy, InternalExternalSnapshotSwitcher>()
                .AutoCacheAndMap();

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
