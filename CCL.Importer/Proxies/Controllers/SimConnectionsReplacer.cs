using AutoMapper;
using CCL.Types.Proxies.Ports;
using LocoSim.Definitions;

namespace CCL.Importer.Proxies.Controllers
{
    public class SimConnectionsReplacer : Profile
    {
        public SimConnectionsReplacer()
        {
            CreateMap<SimConnectionsDefinitionProxy, SimConnectionDefinition>()
                .AutoCacheAndMap()
                .ForMember(d => d.executionOrder, o => o.MapFrom(s => Mapper.GetFromCache(s.executionOrder)));

            CreateMap<PortConnectionProxy, Connection>();
            CreateMap<PortReferenceConnectionProxy, PortReferenceConnection>();
        }
    }
}
