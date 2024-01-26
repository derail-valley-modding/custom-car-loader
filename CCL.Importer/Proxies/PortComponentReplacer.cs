using AutoMapper;
using CCL.Types.Proxies.Ports;
using LocoSim.Definitions;

namespace CCL.Importer.Proxies
{
    internal class PortComponentReplacer : Profile
    {
        public PortComponentReplacer()
        {
            CreateMap<CCL.Types.Proxies.Ports.PortDefinition, LocoSim.Definitions.PortDefinition>();
            CreateMap<CCL.Types.Proxies.Ports.PortReferenceDefinition, LocoSim.Definitions.PortReferenceDefinition>();

            CreateMap<ConstantPortDefinitionProxy, ConfigurablePortDefinition>().AutoCacheAndMap();
            CreateMap<ConfigurableAddDefinitionProxy, ConfigurableAddDefinition>().AutoCacheAndMap();
            CreateMap<ConfigurableMultiplierDefinitionProxy, ConfigurableMultiplierDefinition>().AutoCacheAndMap();

            CreateMap<IndependentFusesDefinitionProxy, IndependentFusesDefinition>().AutoCacheAndMap();
        }
    }
}
