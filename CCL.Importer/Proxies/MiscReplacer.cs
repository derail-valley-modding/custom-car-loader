using AutoMapper;
using CCL.Types;
using CCL.Types.Proxies;
using DV;
using DV.Interaction;
using DV.RemoteControls;
using DV.Simulation.Cars;
using DV.ThingTypes.TransitionHelpers;
using LocoSim.DVExtensions.Test;
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
            CreateMap<ResourceExplosionBaseProxy, ResourceExplosionBase>().AutoCacheAndMap()
                .ForMember(d => d.explosionPrefab, o => o.MapFrom(s => QuickAccess.Explosions.GetExplosionPrefab(s.explosionPrefab)))
                .ForMember(d => d.explosionLiquid, o => o.Ignore())
                .AfterMap(ResourceExplosionBaseAfter);

            CreateMap<PlayerDistanceGameObjectsDisablerProxy, PlayerDistanceGameObjectsDisabler>().AutoCacheAndMap()
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

            CreateMap<LocoZoneBlockerProxy, LocoZoneBlocker>().AutoCacheAndMap()
                .ForMember(d => d.cab, o => o.MapFrom(s => Mapper.GetFromCache(s.cab)));
            CreateMap<InvalidTeleportLocationReactionProxy, InvalidTeleportLocationReaction>().AutoCacheAndMap()
                .ForMember(d => d.blocker, o => o.MapFrom(s => Mapper.GetFromCache(s.blocker)))
                .AfterMap(InvalidTeleportLocationReactionAfter);

            CreateMap<SimDataDisplaySimControllerProxy, SimDataDisplaySimController>().AutoCacheAndMap();
        }

        private void ResourceExplosionBaseAfter(ResourceExplosionBaseProxy src, ResourceExplosionBase dest)
        {
            if (Utilities.IsVanillaCargo(src.explosionLiquid) && Globals.G.Types.TryGetCargo(src.explosionLiquid, out var cargo))
            {
                dest.explosionLiquid = cargo.v1;
            }
            else
            {
                CCLPlugin.Error($"Cargo type '{src.explosionLiquid}' is not a vanilla cargo, this is not supported for ResourceExplosionBase!");
            }
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

        private void InvalidTeleportLocationReactionAfter(InvalidTeleportLocationReactionProxy src, InvalidTeleportLocationReaction dest)
        {
            dest.gameObject.tag = "NO_TELEPORT";
            dest.gameObject.SetLayer(DVLayer.Teleport_Destination);

            var instance = Object.Instantiate(dest, dest.transform.parent);
            instance.gameObject.SetLayer(DVLayer.Train_Walkable);
            instance.name = "WALK_blocker";
            Object.DestroyImmediate(instance);
        }
    }
}
