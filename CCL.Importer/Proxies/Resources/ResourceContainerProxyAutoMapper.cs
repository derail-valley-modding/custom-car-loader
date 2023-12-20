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
    internal class ResourceContainerProxyAutoMapper : Profile, IProxyReplacer
    {
        public ResourceContainerProxyAutoMapper() : base("Resource Container Proxies")
        {
            CreateMap<WaterContainerProxy, WaterContainerDefinition>();
        }

        public void ReplaceProxies(GameObject prefab)
        {
            Mapper.MapComponents<WaterContainerProxy, WaterContainerDefinition>(prefab);
        }
    }
}
