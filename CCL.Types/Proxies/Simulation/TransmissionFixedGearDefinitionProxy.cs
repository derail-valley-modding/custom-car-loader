using CCL.Types.Proxies.Ports;
using System.Collections.Generic;

namespace CCL.Types.Proxies.Simulation
{
    public class TransmissionFixedGearDefinitionProxy : SimComponentDefinitionProxy
    {
        public float gearRatio = 5.18f;
        public float transmissionEfficiency = 1f;

        public override IEnumerable<PortDefinition> ExposedPorts => new[]
        {
            new PortDefinition(DVPortType.IN, DVPortValueType.TORQUE, "TORQUE_IN"),
            new PortDefinition(DVPortType.OUT, DVPortValueType.TORQUE, "TORQUE_OUT"),
            new PortDefinition(DVPortType.READONLY_OUT, DVPortValueType.GENERIC, "GEAR_RATIO"),
        };
    }
}