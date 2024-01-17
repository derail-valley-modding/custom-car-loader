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
            CreateMap<LeverProxy, Lever>().CacheAndProcessProxyAutomatically()
                .ForMember(d => d.nonVrStaticInteractionArea, o => o.MapFrom(s => Mapper.GetFromCache(s.nonVrStaticInteractionArea)));
            CreateMap<PullerProxy, Puller>().CacheAndProcessProxyAutomatically()
                .ForMember(d => d.nonVrStaticInteractionArea, o => o.MapFrom(s => Mapper.GetFromCache(s.nonVrStaticInteractionArea)));
            CreateMap<RotaryProxy, Rotary>().CacheAndProcessProxyAutomatically()
                .ForMember(d => d.nonVrStaticInteractionArea, o => o.MapFrom(s => Mapper.GetFromCache(s.nonVrStaticInteractionArea)));
            CreateMap<ToggleSwitchProxy, ToggleSwitch>().CacheAndProcessProxyAutomatically()
                .ForMember(d => d.nonVrStaticInteractionArea, o => o.MapFrom(s => Mapper.GetFromCache(s.nonVrStaticInteractionArea)));
            CreateMap<WheelProxy, Wheel>().CacheAndProcessProxyAutomatically()
                .ForMember(d => d.nonVrStaticInteractionArea, o => o.MapFrom(s => Mapper.GetFromCache(s.nonVrStaticInteractionArea)));
            CreateMap<ButtonProxy, Button>().CacheAndProcessProxyAutomatically()
                .ForMember(d => d.nonVrStaticInteractionArea, o => o.MapFrom(s => Mapper.GetFromCache(s.nonVrStaticInteractionArea)));
            CreateMap<ExternalControlDefinitionProxy, ExternalControlDefinition>().CacheAndProcessProxyAutomatically();
            CreateMap<StaticInteractionAreaProxy, StaticInteractionArea>().CacheAndProcessProxyAutomatically();
            CreateMap<InteractablePortFeederProxy, InteractablePortFeeder>().CacheAndProcessProxyAutomatically();
        }
    }
}