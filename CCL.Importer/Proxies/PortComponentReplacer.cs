using AutoMapper;
using CCL.Types.Proxies.Ports;
using DV.Simulation.Cars;
using DV.Simulation.Controllers;
using DV.Simulation.Ports;
using LocoSim.Definitions;
using System.Linq;

namespace CCL.Importer.Proxies
{
    internal class PortComponentReplacer : Profile
    {
        public PortComponentReplacer()
        {
            CreateMap<CCL.Types.Proxies.Ports.PortDefinition, LocoSim.Definitions.PortDefinition>();
            CreateMap<CCL.Types.Proxies.Ports.PortReferenceDefinition, LocoSim.Definitions.PortReferenceDefinition>();
            CreateMap<CCL.Types.Proxies.Ports.FuseDefinition, LocoSim.Definitions.FuseDefinition>();

            CreateMap<ConstantPortDefinitionProxy, ConfigurablePortDefinition>().AutoCacheAndMap();
            CreateMap<ConfigurableAddDefinitionProxy, ConfigurableAddDefinition>().AutoCacheAndMap();
            CreateMap<ConfigurableMultiplierDefinitionProxy, ConfigurableMultiplierDefinition>().AutoCacheAndMap();
            CreateMap<ConfigurablePortsDefinitionProxy, ConfigurablePortsDefinition>().AutoCacheAndMap()
                .ForMember(d => d.ports, o => o.MapFrom(s => s.Ports.Select(p => p.Port).ToArray()))
                .ForMember(d => d.startingValues, o => o.MapFrom(s => s.Ports.Select(p => p.StartingValue).ToArray()));
            CreateMap<MultiplePortSumDefinitionProxy, MultiplePortSumDefinition>().AutoCacheAndMap();

            CreateMap<IndependentFusesDefinitionProxy, IndependentFusesDefinition>().AutoCacheAndMap();

            CreateMap<BroadcastPortValueConsumerProxy, BroadcastPortValueConsumer>().AutoCacheAndMap();
            CreateMap<BroadcastPortValueProviderProxy, BroadcastPortValueProvider>().AutoCacheAndMap();

            CreateMap<AnimatorPortReaderProxy, AnimatorPortReader>().AutoCacheAndMap();

            CreateMap<ResourceMassPortReaderProxy, ResourceMassPortReader>().AutoCacheAndMap();
        }
    }
}
