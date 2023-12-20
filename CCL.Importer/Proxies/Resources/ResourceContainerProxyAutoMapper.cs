using CCL.Types.Proxies.Resources;
using LocoSim.Definitions;
using System.ComponentModel.Composition;

namespace CCL.Importer.Proxies.Resources
{
    [Export(typeof(IProxyReplacer))]
    public class CoalContainerProxyReplacer : ProxyReplacer<ResourceContainerProxy, CoalContainerDefinition> {
        protected override bool CanReplace(ResourceContainerProxy sourceComponent)
        {
            return sourceComponent.type == ResourceContainerType.Coal;
        }
    }

    [Export(typeof(IProxyReplacer))]
    public class FuelContainerProxyReplacer : ProxyReplacer<ResourceContainerProxy, FuelContainerDefinition> {
        protected override bool CanReplace(ResourceContainerProxy sourceComponent)
        {
            return sourceComponent.type == ResourceContainerType.Fuel;
        }
    }

    [Export(typeof(IProxyReplacer))]
    public class OilContainerProxyReplacer : ProxyReplacer<ResourceContainerProxy, OilContainerDefinition>
    {
        protected override bool CanReplace(ResourceContainerProxy sourceComponent)
        {
            return sourceComponent.type == ResourceContainerType.Oil;
        }
    }

    [Export(typeof(IProxyReplacer))]
    public class SandContainerProxyReplacer : ProxyReplacer<ResourceContainerProxy, SandContainerDefinition> {
        protected override bool CanReplace(ResourceContainerProxy sourceComponent)
        {
            return sourceComponent.type == ResourceContainerType.Sand;
        }
    }

    [Export(typeof(IProxyReplacer))]
    public class WaterContainerProxyReplacer : ProxyReplacer<ResourceContainerProxy, WaterContainerDefinition> {
        protected override bool CanReplace(ResourceContainerProxy sourceComponent)
        {
            return sourceComponent.type == ResourceContainerType.Water;
        }
    }
}
