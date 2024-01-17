using AutoMapper;
using CCL.Types.Proxies.Resources;
using LocoSim.Definitions;

namespace CCL.Importer.Proxies.Resources
{
    public class ResourceContainerProxyReplacer : Profile
    {
        public ResourceContainerProxyReplacer()
        {
            CreateMap<ResourceContainerProxy, CoalContainerDefinition>()
                .CacheAndProcessProxyAutomatically(x => x.type == ResourceContainerType.Coal);
            CreateMap<ResourceContainerProxy, FuelContainerDefinition>()
                .CacheAndProcessProxyAutomatically(x => x.type == ResourceContainerType.Fuel);
            CreateMap<ResourceContainerProxy, OilContainerDefinition>()
                .CacheAndProcessProxyAutomatically(x => x.type == ResourceContainerType.Oil);
            CreateMap<ResourceContainerProxy, SandContainerDefinition>()
                .CacheAndProcessProxyAutomatically(x => x.type == ResourceContainerType.Sand);
            CreateMap<ResourceContainerProxy, WaterContainerDefinition>()
                .CacheAndProcessProxyAutomatically(x => x.type == ResourceContainerType.Water);
        }
    }
}
