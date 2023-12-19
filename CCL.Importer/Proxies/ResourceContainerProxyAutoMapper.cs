using AutoMapper;
using CCL.Types.Proxies;
using LocoSim.Definitions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CCL.Importer.Proxies
{
    internal class ResourceContainerProxyAutoMapper : Profile
    {
        public ResourceContainerProxyAutoMapper() : base("Resource Container Proxies") {
            CreateMap<WaterContainerProxy, WaterContainerDefinition>();
        }
    }
}
