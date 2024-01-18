using AutoMapper;
using CCL.Types.Proxies.Controls;
using DV.CabControls.Spec;
using DV.Interaction;
using DV.Simulation.Ports;
using LocoSim.Definitions;

namespace CCL.Importer.Proxies.Controls
{
    public class ControlSpecReplacer : Profile
    {
        public ControlSpecReplacer()
        {
            CreateMap<LeverProxy, Lever>().AutoCacheAndMap()
                .ForMember(d => d.nonVrStaticInteractionArea, o => o.MapFrom(s => Mapper.GetFromCache(s.nonVrStaticInteractionArea)));
            CreateMap<PullerProxy, Puller>().AutoCacheAndMap()
                .ForMember(d => d.nonVrStaticInteractionArea, o => o.MapFrom(s => Mapper.GetFromCache(s.nonVrStaticInteractionArea)));
            CreateMap<RotaryProxy, Rotary>().AutoCacheAndMap()
                .ForMember(d => d.nonVrStaticInteractionArea, o => o.MapFrom(s => Mapper.GetFromCache(s.nonVrStaticInteractionArea)));
            CreateMap<ToggleSwitchProxy, ToggleSwitch>().AutoCacheAndMap()
                .ForMember(d => d.nonVrStaticInteractionArea, o => o.MapFrom(s => Mapper.GetFromCache(s.nonVrStaticInteractionArea)));
            CreateMap<WheelProxy, Wheel>().AutoCacheAndMap()
                .ForMember(d => d.nonVrStaticInteractionArea, o => o.MapFrom(s => Mapper.GetFromCache(s.nonVrStaticInteractionArea)));
            CreateMap<ButtonProxy, Button>().AutoCacheAndMap()
                .ForMember(d => d.nonVrStaticInteractionArea, o => o.MapFrom(s => Mapper.GetFromCache(s.nonVrStaticInteractionArea)));
            CreateMap<ExternalControlDefinitionProxy, ExternalControlDefinition>().AutoCacheAndMap();
            CreateMap<StaticInteractionAreaProxy, StaticInteractionArea>().AutoCacheAndMap();
            CreateMap<InteractablePortFeederProxy, InteractablePortFeeder>().AutoCacheAndMap();
        }
    }
}