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

            CreateMap<ConfigurablePortDefinitionProxy, ConfigurablePortDefinition>().AutoCacheAndMap();
            CreateMap<ConfigurableAddDefinitionProxy, ConfigurableAddDefinition>().AutoCacheAndMap();
            CreateMap<ConfigurableMultiplierDefinitionProxy, ConfigurableMultiplierDefinition>().AutoCacheAndMap();
            CreateMap<ConfigurableFunctionDefinitionProxy, ConfigurableFunctionDefinition>().AutoCacheAndMap();
            CreateMap<ConfigurablePortsDefinitionProxy, ConfigurablePortsDefinition>().AutoCacheAndMap()
                .ForMember(d => d.ports, o => o.MapFrom(s => s.Ports.Select(p => p.Port).ToArray()))
                .ForMember(d => d.startingValues, o => o.MapFrom(s => s.Ports.Select(p => p.StartingValue).ToArray()));
            CreateMap<MultiplePortSumDefinitionProxy, MultiplePortSumDefinition>().AutoCacheAndMap();
            CreateMap<MultiplePortDecoderEncoderDefinitionProxy, MultiplePortDecoderEncoderDefinition>().AutoCacheAndMap();
            CreateMap<MultiplePortDecoderEncoderDefinitionProxy.FloatArray, MultiplePortDecoderEncoderDefinition.FloatArray>();

            CreateMap<IndependentFusesDefinitionProxy, IndependentFusesDefinition>().AutoCacheAndMap();

            CreateMap<BroadcastPortValueConsumerProxy, BroadcastPortValueConsumer>().AutoCacheAndMap();
            CreateMap<BroadcastPortValueProviderProxy, BroadcastPortValueProvider>().AutoCacheAndMap();

            CreateMap<AnimatorPortReaderProxy, AnimatorPortReader>().AutoCacheAndMap();
            CreateMap<RotatorPortReaderProxy, RotatorPortReader>().AutoCacheAndMap();
            CreateMap<RotatorPortReaderProxy.RotationData, RotatorPortReader.RotationData>();
            CreateMap<ResourceMassPortReaderProxy, ResourceMassPortReader>().AutoCacheAndMap();

            CreateMap<WaterDetectorPortFeederProxy, WaterDetectorPortFeeder>().AutoCacheAndMap();
        }
    }
}
