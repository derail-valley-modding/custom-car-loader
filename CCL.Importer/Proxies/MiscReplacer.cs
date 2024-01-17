using AutoMapper;
using CCL.Types.Proxies;
using CCL.Types.Proxies.Wheels;
using DV;
using DV.Rain;
using DV.Wheels;

namespace CCL.Importer.Proxies
{
    internal class MiscReplacer : Profile
    {
        public MiscReplacer()
        {
            CreateMap<TeleportArcPassThroughProxy, TeleportArcPassThrough>();
            CreateMap<WindowProxy, Window>()
                .CacheAndProcessProxyAutomatically()
                .ForMember(d => d.duplicates, o => o.MapFrom(s => Mapper.GetFromCache(s.duplicates)));
            CreateMap<InternalExternalSnapshotSwitcherProxy, InternalExternalSnapshotSwitcher>()
                .CacheAndProcessProxyAutomatically();

            CreateMap<PlayerDistanceGameObjectsDisablerProxy, PlayerDistanceGameObjectsDisabler>()
                .CacheAndProcessProxyAutomatically()
                .ForMember(d => d.disableSqrDistance, o => o.MapFrom(d => d.disableDistance * d.disableDistance));
            CreateMap<PlayerDistanceMultipleGameObjectsOptimizerProxy, PlayerDistanceMultipleGameObjectsOptimizer>()
                .CacheAndProcessProxyAutomatically()
                .ForMember(d => d.disableSqrDistance, o => o.MapFrom(d => d.disableDistance * d.disableDistance))
                .ForMember(s => s.scriptsToDisable, o => o.MapFrom(s => Mapper.GetFromCacheOrSelf(s.scriptsToDisable)));

            // Types that aren't MonoBehaviours but are used in proxies.
            CreateMap<PoweredWheelRotationViaAnimationProxy.AnimatorStartTimeOffsetPair, PoweredWheelRotationViaAnimation.AnimatorStartTimeOffsetPair>();
            CreateMap<PoweredWheelRotationViaCodeProxy.TransformRotationConfig, TransformRotationConfig>();
            CreateMap<ExplosionModelHandlerProxy.MaterialSwapData, ExplosionModelHandler.MaterialSwapData>();
            CreateMap<ExplosionModelHandlerProxy.GameObjectSwapData, ExplosionModelHandler.GameObjectSwapData>();
        }
    }
}
