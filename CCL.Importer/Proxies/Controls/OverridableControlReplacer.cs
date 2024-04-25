using AutoMapper;
using CCL.Types.Proxies.Controls;
using DV.HUD;
using DV.Simulation.Cars;
using DV.Simulation.Controllers;
using System;
using UnityEngine;

namespace CCL.Importer.Proxies.Controls
{
    public class OverridableControlReplacer : Profile
    {
        public OverridableControlReplacer()
        {
            CreateMap<ControlBlockerProxy, ControlBlocker>().AutoCacheAndMap();

            CachedBlocker(CreateMap<OverridableControlProxy, ThrottleControl>().AutoCacheAndMap(s => s.ControlType == OverridableControlType.Throttle));
            CachedBlocker(CreateMap<OverridableControlProxy, BrakeControl>().AutoCacheAndMap(s => s.ControlType == OverridableControlType.TrainBrake));
            CachedBlocker(CreateMap<OverridableControlProxy, ReverserControl>().AutoCacheAndMap(s => s.ControlType == OverridableControlType.Reverser));
            CachedBlocker(CreateMap<OverridableControlProxy, IndependentBrakeControl>().AutoCacheAndMap(s => s.ControlType == OverridableControlType.IndBrake));
            //CreateMap<OverridableControlProxy, HandbrakeControl>().AutoCacheAndMap(s => s.ControlType == OverridableControlType.Handbrake);
            CachedBlocker(CreateMap<OverridableControlProxy, SanderControl>().AutoCacheAndMap(s => s.ControlType == OverridableControlType.Sander));
            //CreateMap<OverridableControlProxy, HornControl>().AutoCacheAndMap(s => s.ControlType == OverridableControlType.Horn);
            CreateMap<HornControlProxy, HornControl>().AutoCacheAndMap().ForMember(d => d.controlBlocker, o => o.MapFrom(s => s.controlBlocker));
            CachedBlocker(CreateMap<OverridableControlProxy, HeadlightsControlFront>().AutoCacheAndMap(s => s.ControlType == OverridableControlType.HeadlightsFront));
            CachedBlocker(CreateMap<OverridableControlProxy, HeadlightsControlRear>().AutoCacheAndMap(s => s.ControlType == OverridableControlType.HeadlightsRear));
            CachedBlocker(CreateMap<OverridableControlProxy, DynamicBrakeControl>().AutoCacheAndMap(s => s.ControlType == OverridableControlType.DynamicBrake));
            //CreateMap<OverridableControlProxy, PowerOffControl>().AutoCacheAndMap(s => s.ControlType == OverridableControlType.FuelCutoff);
            CreateMap<PowerOffControlProxy, PowerOffControl>().AutoCacheAndMap().ForMember(d => d.controlBlocker, o => o.MapFrom(s => s.controlBlocker));

            CreateMap<InteriorControlsManagerProxy, InteriorControlsManager>().AutoCacheAndMap();
            CreateMap<BaseControlsOverriderProxy, BaseControlsOverrider>().AutoCacheAndMap();
            CreateMap<BaseControlsOverriderProxy.PortSetter, BaseControlsOverrider.PortSetter>();
        }

        private static IMappingExpression<TSource, TDestination> CachedBlocker<TSource, TDestination>(IMappingExpression<TSource, TDestination> cfg)
            where TSource : OverridableControlProxy
            where TDestination : OverridableBaseControl
        {
            return cfg.ForMember(d => d.controlBlocker, o => o.MapFrom(s => s.controlBlocker));
        }
    }
}
