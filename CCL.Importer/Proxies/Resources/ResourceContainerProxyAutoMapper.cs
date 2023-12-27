using CCL.Types.Proxies.Resources;
using LocoSim.Definitions;
using System.ComponentModel.Composition;

namespace CCL.Importer.Proxies.Resources
{
    [ProxyMap(typeof(ResourceContainerProxy), typeof(CoalContainerDefinition), fieldToValidate: nameof(ResourceContainerProxy.type), validValue: ResourceContainerType.Coal)]
    [ProxyMap(typeof(ResourceContainerProxy), typeof(FuelContainerDefinition), fieldToValidate: nameof(ResourceContainerProxy.type), validValue: ResourceContainerType.Fuel)]
    [ProxyMap(typeof(ResourceContainerProxy), typeof(OilContainerDefinition), fieldToValidate: nameof(ResourceContainerProxy.type), validValue: ResourceContainerType.Oil)]
    [ProxyMap(typeof(ResourceContainerProxy), typeof(SandContainerDefinition), fieldToValidate: nameof(ResourceContainerProxy.type), validValue: ResourceContainerType.Sand)]
    [ProxyMap(typeof(ResourceContainerProxy), typeof(WaterContainerDefinition), fieldToValidate: nameof(ResourceContainerProxy.type), validValue: ResourceContainerType.Water)]
    public class CoalContainerProxyReplacer : ProxyReplacer
    {
    }
}
