using AutoMapper;
using CCL.Types.Proxies.Ports;
using LocoSim.Definitions;

namespace CCL.Importer.Proxies
{
    internal class PortComponentReplacer : Profile
    {
        public PortComponentReplacer()
        {
            CreateMap<ConstantPortDefinitionProxy, ConfigurablePortDefinition>().AutoCacheAndMap();
            CreateMap<ConfigurableAddDefinitionProxy, ConfigurableAddDefinition>().AutoCacheAndMap();
            CreateMap<ConfigurableMultiplierDefinitionProxy, ConfigurableMultiplierDefinition>().AutoCacheAndMap();
        }
    }
}
