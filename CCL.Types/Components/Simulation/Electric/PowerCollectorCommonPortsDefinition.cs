using System.Collections.Generic;

using CCL.Types.Proxies.Ports;

namespace CCL.Types.Components.Simulation.Electric
{
    public abstract class PowerCollectorCommonPortsDefinition : SimComponentDefinitionProxy
    {
        public override IEnumerable<PortDefinition> ExposedPorts => new[]
        {
            new PortDefinition(DVPortType.EXTERNAL_IN, DVPortValueType.GENERIC, "WIRE_HEIGHT"),
            new PortDefinition(DVPortType.EXTERNAL_IN, DVPortValueType.GENERIC, "INITIAL_HEAD_HEIGHT"),
            new PortDefinition(DVPortType.EXTERNAL_IN, DVPortValueType.GENERIC, "HEAD_HEIGHT"),
            new PortDefinition(DVPortType.EXTERNAL_IN, DVPortValueType.VOLTS, "WIRE_VOLTAGE"),
            new PortDefinition(DVPortType.EXTERNAL_IN, DVPortValueType.STATE, "PANTOGRAPH_IN_CONTACT"),
            new PortDefinition(DVPortType.READONLY_OUT, DVPortValueType.VOLTS, "VOLTAGE")
        };
    }
}
