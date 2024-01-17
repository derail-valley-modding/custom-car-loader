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
                .CacheAndProcessProxyAutomatically()
                .ForMember("executionOrder", o => o.ConvertUsing(new CacheConverter(o.DestinationMember), "executionOrder"));

            CreateMap<PortConnectionProxy, Connection>();
            CreateMap<PortReferenceConnectionProxy, PortReferenceConnection>();
        }
    }
}
