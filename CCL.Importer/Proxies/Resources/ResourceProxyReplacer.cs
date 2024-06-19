using AutoMapper;
using CCL.Types.Proxies.Resources;
using LocoSim.Definitions;

namespace CCL.Importer.Proxies.Resources
{
    public class ResourceProxyReplacer : Profile
    {
        public ResourceProxyReplacer()
        {
            CreateMap<ResourceContainerProxy, CoalContainerDefinition>()
                .AutoCacheAndMap(x => x.type == ResourceContainerType.Coal);
            CreateMap<ResourceContainerProxy, FuelContainerDefinition>()
                .AutoCacheAndMap(x => x.type == ResourceContainerType.Fuel);
            CreateMap<ResourceContainerProxy, OilContainerDefinition>()
                .AutoCacheAndMap(x => x.type == ResourceContainerType.Oil);
            CreateMap<ResourceContainerProxy, SandContainerDefinition>()
                .AutoCacheAndMap(x => x.type == ResourceContainerType.Sand);
            CreateMap<ResourceContainerProxy, WaterContainerDefinition>()
                .AutoCacheAndMap(x => x.type == ResourceContainerType.Water);
            CreateMap<ResourceContainerProxy, ElectricChargeContainerDefinition>()
                .AutoCacheAndMap(x => x.type == ResourceContainerType.ElectricCharge);

            CreateMap<LocoResourceReceiverProxy, LocoResourceReceiver>()
                .AutoCacheAndMap().AfterMap(LocoResourceReceiverAfter);
        }

        private void LocoResourceReceiverAfter(LocoResourceReceiverProxy _, LocoResourceReceiver real)
        {
            real.tag = "LocoResourceReceiver";
        }
    }
}
