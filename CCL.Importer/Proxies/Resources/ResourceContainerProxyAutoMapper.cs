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
    public class CoalContainerProxyReplacer : ProxyReplacer<CoalContainerDefinitionProxy, CoalContainerDefinition> { }
    [Export(typeof(IProxyReplacer))]
    public class FuelContainerProxyReplacer : ProxyReplacer<FuelContainerDefinitionProxy, FuelContainerDefinition> { }
    [Export(typeof(IProxyReplacer))]
    public class OilContainerProxyReplacer : ProxyReplacer<OilContainerDefinitionProxy, OilContainerDefinition> { }
    [Export(typeof(IProxyReplacer))]
    public class SandContainerProxyReplacer : ProxyReplacer<SandContainerDefinitionProxy, SandContainerDefinition> { }
    [Export(typeof(IProxyReplacer))]
    public class WaterContainerProxyReplacer : ProxyReplacer<WaterContainerDefinitionProxy, WaterContainerDefinition> { }
}
