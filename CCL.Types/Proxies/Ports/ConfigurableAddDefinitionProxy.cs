using System.Collections.Generic;

namespace CCL.Types.Proxies.Ports
{
    public class ConfigurableAddDefinitionProxy : SimComponentDefinitionProxy
    {
        public bool negativeA;
        public bool negativeB;

        public override IEnumerable<PortDefinition> ExposedPorts => new[]
        {
            new PortDefinition(DVPortType.READONLY_OUT, DVPortValueType.GENERIC, "ADD_OUT"),
        };

        public override IEnumerable<PortReferenceDefinition> ExposedPortReferences => new[]
        {
            new PortReferenceDefinition(DVPortValueType.GENERIC, "A"),
            new PortReferenceDefinition(DVPortValueType.GENERIC, "B"),
        };
    }
}
