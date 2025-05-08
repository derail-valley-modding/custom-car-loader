using AutoMapper;
using CCL.Types.Proxies.Controls;
using DV.HUD;
using DV.Simulation.Cars;
using DV.Simulation.Controllers;

namespace CCL.Importer.Proxies.Controls
{
    public class OverridableControlReplacer : Profile
    {
        public OverridableControlReplacer()
        {
            CreateMap<ControlBlockerProxy, ControlBlocker>().AutoCacheAndMap();
            CreateMap<ControlBlockerProxy.BlockerDefinition, ControlBlocker.BlockerDefinition>();

            CachedBlocker(CreateMap<OverridableControlProxy, ThrottleControl>().AutoCacheAndMap(s => s.ControlType == OverridableControlType.Throttle));
            CachedBlocker(CreateMap<OverridableControlProxy, BrakeControl>().AutoCacheAndMap(s => s.ControlType == OverridableControlType.TrainBrake));
            CachedBlocker(CreateMap<OverridableControlProxy, ReverserControl>().AutoCacheAndMap(s => s.ControlType == OverridableControlType.Reverser));
            CachedBlocker(CreateMap<OverridableControlProxy, IndependentBrakeControl>().AutoCacheAndMap(s => s.ControlType == OverridableControlType.IndBrake));
            //CreateMap<OverridableControlProxy, HandbrakeControl>().AutoCacheAndMap(s => s.ControlType == OverridableControlType.Handbrake);
            CachedBlocker(CreateMap<OverridableControlProxy, SanderControl>().AutoCacheAndMap(s => s.ControlType == OverridableControlType.Sander));
            CreateMap<HornControlProxy, HornControl>().AutoCacheAndMap().ForMember(d => d.controlBlocker, o => o.MapFrom(s => s.controlBlocker));
            CachedBlocker(CreateMap<OverridableControlProxy, HeadlightsControlFront>().AutoCacheAndMap(s => s.ControlType == OverridableControlType.HeadlightsFront));
            CachedBlocker(CreateMap<OverridableControlProxy, HeadlightsControlRear>().AutoCacheAndMap(s => s.ControlType == OverridableControlType.HeadlightsRear));
            CachedBlocker(CreateMap<OverridableControlProxy, StarterControl>().AutoCacheAndMap(s => s.ControlType == OverridableControlType.StarterControl));
            CachedBlocker(CreateMap<OverridableControlProxy, DynamicBrakeControl>().AutoCacheAndMap(s => s.ControlType == OverridableControlType.DynamicBrake));
            CachedBlocker(CreateMap<OverridableControlProxy, CabLightControl>().AutoCacheAndMap(s => s.ControlType == OverridableControlType.CabLight));
            CachedBlocker(CreateMap<OverridableControlProxy, WipersControl>().AutoCacheAndMap(s => s.ControlType == OverridableControlType.Wipers));
            CreateMap<PowerOffControlProxy, PowerOffControl>().AutoCacheAndMap().ForMember(d => d.controlBlocker, o => o.MapFrom(s => s.controlBlocker));
            CachedBlocker(CreateMap<OverridableControlProxy, IndCabLightControl>().AutoCacheAndMap(s => s.ControlType == OverridableControlType.IndCabLight));
            CachedBlocker(CreateMap<OverridableControlProxy, DynamoControl>().AutoCacheAndMap(s => s.ControlType == OverridableControlType.Dynamo));
            CachedBlocker(CreateMap<OverridableControlProxy, AirPumpControl>().AutoCacheAndMap(s => s.ControlType == OverridableControlType.AirPump));
            CachedBlocker(CreateMap<OverridableControlProxy, BrakeCutoutControl>().AutoCacheAndMap(s => s.ControlType == OverridableControlType.TrainBrakeCutout));

            CreateMap<InteriorControlsManagerProxy, InteriorControlsManager>().AutoCacheAndMap();
            CreateMap<BaseControlsOverriderProxy, BaseControlsOverrider>().AutoCacheAndMap();
            CreateMap<BaseControlsOverriderProxy.PortSetter, BaseControlsOverrider.PortSetter>();
        }

        private static IMappingExpression<TSource, TDestination> CachedBlocker<TSource, TDestination>(IMappingExpression<TSource, TDestination> cfg)
            where TSource : OverridableControlProxy
            where TDestination : OverridableBaseControl
        {
            return cfg.ForMember(d => d.controlBlocker, o => o.MapFrom(s => Mapper.GetFromCache(s.controlBlocker)));
        }
    }
}
