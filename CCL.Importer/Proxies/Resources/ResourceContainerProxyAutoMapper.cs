using AutoMapper;
using CCL.Types.Proxies.Resources;
using LocoSim.Definitions;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace CCL.Importer.Proxies.Resources
{
    [Export(typeof(IProxyReplacer))]
    public class ResourceContainerProxyAutoMapper : Profile, IProxyReplacer
    {
        public ResourceContainerProxyAutoMapper() : base("Resource Container Proxies")
        {
            CreateMap<CoalContainerDefinitionProxy, CoalContainerDefinition>();
            CreateMap<FuelContainerDefinitionProxy, FuelContainerDefinition>();
            CreateMap<OilContainerDefinitionProxy, OilContainerDefinition>();
            CreateMap<SandContainerDefinitionProxy, SandContainerDefinition>();
            CreateMap<WaterContainerDefinitionProxy, WaterContainerDefinition>();
        }

        public void ReplaceProxies(GameObject prefab)
        {
            prefab.MapComponents<CoalContainerDefinitionProxy, CoalContainerDefinition>();
            prefab.MapComponents<FuelContainerDefinitionProxy, FuelContainerDefinition>();
            prefab.MapComponents<OilContainerDefinitionProxy, OilContainerDefinition>();
            prefab.MapComponents<SandContainerDefinitionProxy, SandContainerDefinition>();
            prefab.MapComponents<WaterContainerDefinitionProxy, WaterContainerDefinition>();
        }
    }
}
