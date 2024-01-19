using CCL.Types.Proxies.Ports;
using System.Collections.Generic;

namespace CCL.Types.Proxies.Simulation.Diesel
{
    public class DirectionalCoolerDefinitionProxy : SimComponentDefinitionProxy
    {
        public float coolingRate = 12500f;

        public float minCoolingSpeed = 2f;

        public float maxCoolingSpeed = 25f;

        public bool coolingInForwardDirection = true;

        public override IEnumerable<PortDefinition> ExposedPorts => new[]
        {
            new PortDefinition(DVPortType.READONLY_OUT, DVPortValueType.HEAT_RATE, "HEAT_OUT"),
        };

        public override IEnumerable<PortReferenceDefinition> ExposedPortReferences => new[]
        {
            new PortReferenceDefinition(DVPortValueType.GENERIC, "SPEED"),
            new PortReferenceDefinition(DVPortValueType.TEMPERATURE, "TEMPERATURE"),
            new PortReferenceDefinition(DVPortValueType.TEMPERATURE, "TARGET_TEMPERATURE"),
        };
    }
}