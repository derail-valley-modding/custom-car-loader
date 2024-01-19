using CCL.Types.Proxies.Ports;
using System.Collections.Generic;

namespace CCL.Types.Proxies.Simulation.Diesel
{
    public class ThrottleNotchPowerProxy : SimComponentDefinitionProxy
    {
        public float[] notchPowerPercentages;

        public override IEnumerable<PortDefinition> ExposedPorts => new[]
        {
            new PortDefinition(DVPortType.READONLY_OUT, DVPortValueType.POWER, "GOAL_POWER"),
            new PortDefinition(DVPortType.READONLY_OUT, DVPortValueType.GENERIC, "NOTCH"),
            new PortDefinition(DVPortType.READONLY_OUT, DVPortValueType.GENERIC, "PREV_NOTCH"),
        };

        public override IEnumerable<PortReferenceDefinition> ExposedPortReferences => new[]
        {
            new PortReferenceDefinition(DVPortValueType.CONTROL, "THROTTLE"),
            new PortReferenceDefinition(DVPortValueType.POWER, "MAX_POWER"),
        };
    }
}
