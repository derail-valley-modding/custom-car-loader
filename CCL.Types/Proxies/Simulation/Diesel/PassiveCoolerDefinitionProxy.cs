using CCL.Types.Proxies.Ports;
using System.Collections.Generic;

namespace CCL.Types.Proxies.Simulation.Diesel
{
    public class PassiveCoolerDefinitionProxy : SimComponentDefinitionProxy
    {
        public float coolingRate = 12500f;

        public override IEnumerable<PortDefinition> ExposedPorts => new[]
        {
            new PortDefinition(DVPortType.READONLY_OUT, DVPortValueType.HEAT_RATE, "HEAT_OUT"),
        };

        public override IEnumerable<PortReferenceDefinition> ExposedPortReferences => new[]
        {
            new PortReferenceDefinition(DVPortValueType.TEMPERATURE, "TEMPERATURE"),
            new PortReferenceDefinition(DVPortValueType.TEMPERATURE, "TARGET_TEMPERATURE"),
        };
    }
}
