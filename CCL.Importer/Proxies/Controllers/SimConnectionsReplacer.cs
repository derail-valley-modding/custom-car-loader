using AutoMapper;
using CCL.Types.Proxies.Ports;
using LocoSim.Definitions;
using System.ComponentModel.Composition;
using UnityEngine;

namespace CCL.Importer.Proxies.Controllers
{
    [Export(typeof(IProxyReplacer))]
    public class SimConnectionsReplacer : Profile, IProxyReplacer
    {
        public SimConnectionsReplacer()
        {
            CreateMap<SimConnectionsDefinitionProxy, SimConnectionDefinition>()
                .ForMember("executionOrder", o => o.ConvertUsing(new CacheConverter(o.DestinationMember), "executionOrder"));

            CreateMap<PortConnectionProxy, Connection>();
            CreateMap<PortReferenceConnectionProxy, PortReferenceConnection>();
        }

        public void CacheAndReplaceProxies(GameObject prefab)
        {
            prefab.StoreComponentsInChildrenInCache<SimConnectionsDefinitionProxy, SimConnectionDefinition>(_ => true);

        }

        public void MapProxies(GameObject prefab)
        {

        }

        public void ReplaceProxiesUncached(GameObject prefab)
        {
            prefab.ConvertFromCache(typeof(SimConnectionsDefinitionProxy), typeof(SimConnectionDefinition), _ => true);
        }
    }
}
