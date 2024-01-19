using System.Collections.Generic;

namespace CCL.Types.Proxies.Ports
{
    public class ConfigurableMultiplierDefinitionProxy : SimComponentDefinitionProxy
    {
        public bool invertA;
        public bool invertB;

        public override IEnumerable<PortDefinition> ExposedPorts => new[]
        {
            new PortDefinition(DVPortType.READONLY_OUT, DVPortValueType.GENERIC, "MUL_OUT"),
        };

        public override IEnumerable<PortReferenceDefinition> ExposedPortReferences => new[]
        {
            new PortReferenceDefinition(DVPortValueType.GENERIC, "A"),
            new PortReferenceDefinition(DVPortValueType.GENERIC, "B"),
        };
    }
}