using AutoMapper;
using CCL.Types;
using CCL.Types.Proxies;
using DV;
using DV.Interaction;
using DV.RemoteControls;
using DV.Simulation.Cars;
using DV.ThingTypes.TransitionHelpers;
using UnityEngine;

namespace CCL.Importer.Proxies
{
    internal class MiscReplacer : Profile
    {
        private static GameObject? s_cabHightlightGlow = null;

        private static GameObject CabHighlightGlow =>
            Extensions.GetCached(ref s_cabHightlightGlow,
                () => DV.ThingTypes.TrainCarType.LocoShunter.ToV2().prefab.transform.Find(CarPartNames.Cab.HIGHLIGHT_GLOW).gameObject);

        public MiscReplacer()
        {
            CreateMap<TeleportArcPassThroughProxy, TeleportArcPassThrough>();
            CreateMap<InternalExternalSnapshotSwitcherProxy, InternalExternalSnapshotSwitcher>()
                .AutoCacheAndMap();
            CreateMap<InternalExternalSnapshotSwitcherDoorsAndWindowsProxy, InternalExternalSnapshotSwitcherDoorsAndWindows>()
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
            CreateMap<CabTeleportDestinationProxy, CabTeleportDestination>().AutoCacheAndMap()
                .ForMember(m => m.hoverGlow, o => o.MapFrom(m => Mapper.GetFromCache(m.hoverGlow)));
            CreateMap<TeleportHoverGlowProxy, TeleportHoverGlow>().AutoCacheAndMap()
                .AfterMap(TeleportHoverGlowAfter);
            CreateMap<GrabberRaycastPassThroughProxy, GrabberRaycastPassThrough>().AutoCacheAndMap();
            CreateMap<HighlightTagProxy, HighlightTagProxy>().AutoCacheAndMap();

            CreateMap<MultipleUnitStateObserverProxy, MultipleUnitStateObserver>().AutoCacheAndMap();
            CreateMap<RemoteControllerModuleProxy, RemoteControllerModule>().AutoCacheAndMap();

            CreateMap<FireProxy, Fire>().AutoCacheAndMap()
                .ReplaceInstancedObjects();
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

        private void TeleportHoverGlowAfter(TeleportHoverGlowProxy src, TeleportHoverGlow dest)
        {
            dest.highlight = Object.Instantiate(CabHighlightGlow, dest.transform);
        }
    }
}
